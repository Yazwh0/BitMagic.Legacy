using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mega65Parser
{
    public class Instruction
    {
        public string Code { get; set; } = "";
        public int OpCode { get; set; }
        public string Parameters { get; set; } = "";
        public int Cycles { get; set; }
        public List<char> CycleNotes { get; set; } = new List<char>();
        public string CycleNotesAsString => string.Concat(CycleNotes);
    }

    public class CodeDescription
    {
        public string Code { get; set; } = "";
        public string Description { get; set; } = "";
        public string Flags { get; set; } = "";
        public string ExplanationText { get; set; } = "";
        public List<string> Explanations { get; set; } = new List<string>(1);
    }

    public class ParameterDescription
    {
        public string Parameter { get; set; } = "";
        public int ByteCount { get; set; }
        public int Order { get; set; }
        public string Name { get; set; } = "";
    }

    public class Parser
    {
        private readonly string _basePath;

        public List<Instruction> Instructions = new List<Instruction>();
        public Dictionary<string, CodeDescription> Explanation = new Dictionary<string, CodeDescription>();
        public Dictionary<string, ParameterDescription> ParametersOrder = new Dictionary<string, ParameterDescription>();
        public Dictionary<char, string> CycleNotes;

        public string? ChipName { get; set; }

        public Parser(string basePath)
        {
            _basePath = basePath;
            ParameterDescriptions(new ParameterDescription[] {
                    new ParameterDescription{ Parameter = @"", Name = "Implied",  ByteCount = 0, Order = -1},
                    new ParameterDescription{ Parameter = @"A", Name = "Accumulator",  ByteCount = 0, Order = 0},
                    new ParameterDescription{ Parameter = @"#\$nn", Name = "Immediate",  ByteCount = 1, Order = 10},
                    new ParameterDescription{ Parameter = @"\$nn", Name = "Zero Page", ByteCount = 1, Order = 20},
                    new ParameterDescription{ Parameter = @"\$nn,X", Name = "Zero Page, X", ByteCount = 1, Order = 30},
                    new ParameterDescription{ Parameter = @"\$nn,Y", Name = "Zero Page, Y", ByteCount = 1, Order = 35},
                    new ParameterDescription{ Parameter = @"\$nnnn", Name = "Absolute", ByteCount = 2, Order = 40},
                    new ParameterDescription{ Parameter = @"\$nnnn,X", Name = "Absolute, X", ByteCount = 2, Order = 50},
                    new ParameterDescription{ Parameter = @"\$nnnn,Y", Name = "Absolute, Y", ByteCount = 2, Order = 60},
                    new ParameterDescription{ Parameter = @"(\$nn,X)", Name = "Indirect, X", ByteCount = 1, Order = 70},
                    new ParameterDescription{ Parameter = @"(\$nn),Y", Name = "Indirect, Y", ByteCount = 1, Order = 80},
                    new ParameterDescription{ Parameter = @"(\$nn),Z", Name = "Indirect, Z", ByteCount = 1, Order = 90},
                    new ParameterDescription{ Parameter = @"\$nn,\$rr", Name = "Zero Page, Relative", ByteCount = 2, Order = 90},
                    new ParameterDescription{ Parameter = @"\$rr", Name = "Relative", ByteCount = 1, Order = 100},
                    new ParameterDescription{ Parameter = @"\$rrrr", Name = "Relative Word", ByteCount = 2, Order = 110},
                    new ParameterDescription{ Parameter = @"(\$rrrr)", Name = "Indirect Word", ByteCount = 2, Order = 120},
                    new ParameterDescription{ Parameter = @"(\$nnnn)", Name = "Indirect Word", ByteCount = 2, Order = 62},
                    new ParameterDescription{ Parameter = @"(\$nnnn,X)", Name = "Indirect Word, X", ByteCount = 2, Order = 63},
                    new ParameterDescription{ Parameter = @"(\$nn,SP),Y", Name = "Indirect SP, Y", ByteCount = 1, Order = 150},
                    new ParameterDescription{ Parameter = @"#\$nnnn", Name = "Immediate Word",  ByteCount = 2, Order = 15},
            });
            CycleNotes = new Dictionary<char, string> {
                { '?', "Cycles not in source documentation." },
                { 'p', "Add one cycle if indexing crosses a page boundary." },
                { 'r', "Add one cycle if clock speed is at 40 MHz." },
                { 's', "Instruction requires 2 cycles when CPU is run at 1MHz or 2MHz." },
                { 'b', "Add one cycle if branch is taken." },
                { 'm', "Subtract non-bus cycles when at 40MHz."}
            };
        }
        

        private void ParameterDescriptions(IEnumerable<ParameterDescription> toProcess)
        {
            foreach(var i in toProcess)
            {
                ParametersOrder.Add(i.Parameter, i);
            }
        }

        public void Parse(string chipName)
        {
            LoadOpCodes(chipName);
            LoadCycles(chipName);
            LoadDescription();
            ChipName = chipName;
        }   

        private void LoadCycles(string chipName)
        {
            var allLines = File.ReadAllLines(Path.Join(_basePath, chipName + ".cycles"));

            foreach(var line in allLines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    break;

                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);


                if (int.TryParse(parts[0], System.Globalization.NumberStyles.HexNumber, null, out var opCode))
                {
                    var instruction = Instructions.First(i => i.OpCode == opCode);

                    if ("0123456789".Contains(parts[parts.Length - 1][0]))
                    {
                        instruction.Cycles = int.Parse($"{parts[parts.Length - 1][0]}");
                        var notes = parts[parts.Length - 1][1..];
                        foreach(var c in notes)
                        {
                            if ("^${}".Contains(c))
                                continue;

                            instruction.CycleNotes.Add(c);
                        }
                    }
                }
            }

            foreach (var i in Instructions.Where(i => i.Cycles == 0))
            {
                i.CycleNotes.Add('?');
            }

        }

        private void LoadOpCodes(string chipName)
        {
            var allLines = File.ReadAllLines(Path.Join(_basePath, chipName + ".opc"));

            foreach(var line in allLines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                
                var parts = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                var instruction = new Instruction { Code = parts[1], Parameters = parts.Length > 2 ? parts[2].Replace("$", @"\$") : "", OpCode = int.Parse(parts[0], System.Globalization.NumberStyles.HexNumber) };
                Instructions.Add(instruction);
            }
        }

        private void LoadDescription()
        {
            var section = new Regex(@"\\(?<name>(.*)){(?<value>(.*))}", RegexOptions.Compiled);

            var sb = new StringBuilder();
            bool itemise = false;
            bool lastLineEmpty = false;
            string toWrite = "";

            foreach(var code in Instructions.Select(i => i.Code).Distinct())
            {
                var allText = File.ReadAllText(Path.Combine(_basePath, "inst." + code));

                allText = allText.Replace(@"$\leftarrow$", "<-");
                allText = allText.Replace(@"$\rightarrow$", "->");
                allText = allText.Replace(@"$AND$", "&");
                allText = allText.Replace(@"$OR$", "|");
                allText = allText.Replace(@"$or$", "|");
                allText = allText.Replace(@"$xor$", "xor");
                allText = allText.Replace(@"$XOR$", "xor");
                allText = allText.Replace(@"$>>$", ">>");
                allText = allText.Replace(@"$<<$", "<<");
                allText = allText.Replace(@"$>$", ">");
                allText = allText.Replace(@"$<$", "<");
                allText = allText.Replace(@"$+$", "+");
                allText = allText.Replace(@"$-$", "-");
                allText = allText.ReplaceLineEndings();

                var description = new CodeDescription();

                var allLines = allText.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                description.Code = code;
                description.Description = allLines[0];

                if (code == "TZA")
                {
                    var a = 0;
                }

                var linePos = 0;
                while(true)
                {
                    linePos++;
                    if (string.IsNullOrWhiteSpace(allLines[linePos]))
                        break;

                    if (allLines[linePos].Contains(":"))
                    {
                        description.Explanations.Add(allLines[linePos]);
                        continue;
                    }
                    description.Explanations.Add(allLines[linePos]);
                    break;
                }

                linePos++;
                if (allLines[linePos].Length >= 2 && allLines[linePos][1] == '+')
                {
                    description.Flags = allLines[linePos++].Replace("+", " ").Trim().Replace("M", "");
                }

                if (string.IsNullOrWhiteSpace(description.Flags))
                    description.Flags = "-";

                sb.Clear();

                int trimPos = allLines.Length - 1;
                for (var pos = trimPos; pos >= 0; pos--)
                {
                    if (string.IsNullOrWhiteSpace(allLines[pos]))
                    {
                        trimPos = pos;
                        continue;
                    }
                    break;
                }

                while(string.IsNullOrWhiteSpace(allLines[linePos]))
                {
                    linePos++;
                }

                for(var pos = linePos; pos < trimPos; pos++)
                {
                    if (allLines[pos].StartsWith(@"\"))
                    {
                        var matches = section.Match(allLines[pos]);
                        if (matches.Success)
                        {
                            var name = matches.Groups["name"].Value;
                            var value = matches.Groups["value"].Value;

                            if (name == "begin")
                            {
                                switch (value)
                                {
                                    case "verbatim":
                                        sb.AppendLine("```assembly");
                                        lastLineEmpty = false;
                                        continue;

                                    case "itemize":
                                        if (!lastLineEmpty)
                                            sb.AppendLine();

                                        lastLineEmpty = true;
                                        continue;

                                    default:
                                        Console.WriteLine($"Unhandled {allLines[pos]}");
                                        sb.AppendLine(allLines[pos]);
                                        continue;
                                }
                            }
                            if (name == "end")
                            {
                                switch (value)
                                {
                                    case "verbatim":
                                        sb.AppendLine("```");
                                        lastLineEmpty = false;

                                        continue;
                                    case "itemize":
                                        continue;

                                    default:
                                        Console.WriteLine($"Unhandled {allLines[pos]}");
                                        sb.AppendLine(allLines[pos]);
                                        continue;
                                }
                            }
                            if (name == "subsubsection*")
                            {
                                if (!lastLineEmpty)
                                    sb.AppendLine();

                                sb.AppendLine($"{value}:");
                                lastLineEmpty = false;
                                continue;
                            }

                            Console.WriteLine($"Skipping {allLines[pos]}");
                            continue;
                        } 
                        else
                        {
                            if (allLines[pos].StartsWith(@"\item"))
                            {
                                toWrite = $"- {allLines[pos].Substring(5).Trim()}";
                            }
                            else
                            {
                                toWrite = allLines[pos];
                            }
                        }
                    }
                    else
                    {
                        toWrite = allLines[pos];
                        if (itemise)
                        {
                            toWrite = "- " + toWrite;
                        }
                    }

                    if (!lastLineEmpty || !string.IsNullOrWhiteSpace(toWrite))
                        sb.AppendLine(toWrite.Trim());

                    lastLineEmpty = string.IsNullOrWhiteSpace(toWrite);
                }

                description.ExplanationText = sb.ToString().Trim();

                Explanation.Add(code, description);
            }
        }
    }
}
