using BitMagic.Common;
using BitMagic.Cpu;
using CodingSeb.ExpressionEvaluator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Compiler
{
    public abstract class CompilerException : Exception
    {
        public int LineNumber { get; set; }
        public abstract string ErrorDetail { get; }

        public CompilerException(string message) : base(message)
        {
        }
    }

    public class CompilerLineException : CompilerException
    {
        public ILine Line { get; }

        public CompilerLineException(ILine line, string message) : base(message)
        {
            Line = line;
            LineNumber = line.LineNumber;
        }

        public override string ErrorDetail => Line.OriginalText.Trim();
    }

    public class CompilerVerbException : CompilerException
    {
        public string Line { get; }

        public CompilerVerbException(string line, int lineNumber, string message) : base(message)
        {
            Line = line;
            LineNumber = lineNumber;
        }

        public override string ErrorDetail => Line;
    }

    public class Compiler
    {
        private readonly Project _project;        
        private readonly Dictionary<string, ICpuOpCode> _opCodes = new Dictionary<string, ICpuOpCode>();
        private readonly CommandParser _commandParser;

        public Compiler(Project project)
        {
            _project = project;

            _commandParser = CommandParser.Parser()
                .WithLabel((label, state) =>
                {
                    if (label == ".:")
                        throw new Exception("Labels require a name. .: is not valid.");

                    state.Procedure.Variables.SetValue(label[1..^1], state.Segment.Address);
                })
/*                .WithParameters(".definesegment", (dict, state) => {


                }, new[] { "name", "address", "filename" })*/
                .WithParameters(".segment", (dict, state) => {
                    Segment segment;

                    if (state.Segments.ContainsKey(dict["name"]))
                    {
                        segment = state.Segments[dict["name"]];
                    }
                    else
                    {
                        segment = new Segment(state.Globals, dict["name"]);
                        state.Segments.Add(dict["name"], segment);
                    }

                    if (dict.ContainsKey("address"))
                    {
                        foreach (var scope in segment.Scopes)
                        {
                            foreach (var proc in scope.Value.Procedures)
                            {
                                if (proc.Value.Data.Any())
                                {
                                    throw new Exception($"Cannot modify segment start address when it already has data. {segment.Name}");
                                }
                            }
                        }
                    }

                    if (dict.ContainsKey("address"))
                    {
                        segment.Address = ParseStringToValue(dict["address"]);
                        segment.StartAddress = segment.Address;
                    }

                    if (dict.ContainsKey("filename"))
                    {
                        var filename = dict["filename"];

                        if (filename.StartsWith('"') && filename.EndsWith('"'))
                            filename = filename[1..^1];

                        segment.Filename = filename;
                    }

                    state.Segment = segment;
                    state.Scope = state.Segment.GetScope($".Default_{state.AnonCounter++}", true);
                    state.Procedure = state.Scope.GetProcedure($".Default_{state.AnonCounter++}", true);
                }, new[] { "name", "address", "filename" })
                .WithParameters(".endsegment", (dict, state) => {
                    state.Segment = state.Segments[".Default"];
                    state.Scope = state.Segment.GetScope($".Default_{state.AnonCounter++}", true);
                    state.Procedure = state.Scope.GetProcedure($".Default_{state.AnonCounter++}", true);
                })
                .WithParameters(".scope", (dict, state) => {
                    string name = dict.ContainsKey("name") ? dict["name"] : $"UnnamedScope_{state.AnonCounter++}";
                    state.Scope = state.Segment.GetScope(name, false);
                    state.Procedure = state.Scope.GetProcedure($".Default_{state.AnonCounter++}", true);
                }, new[] { "name" })
                .WithParameters(".endscope", (dict, state) => { 
                    state.Scope = state.Segment.GetScope($".Default_{state.AnonCounter++}", true); 
                })
                .WithParameters(".proc", (dict, state) => {
                    var name = dict.ContainsKey("name") ? dict["name"] : $"UnnamedProc_{state.AnonCounter++}";
                    state.Procedure = state.Scope.GetProcedure(name, false);
                    state.Scope.Variables.SetValue(name, state.Segment.Address);
                }, new[] { "name" })
                .WithParameters(".endproc", (dict, state) => {
                    state.Procedure = state.Scope.GetProcedure($".Default_{state.AnonCounter++}", true);
                })
                .WithParameters(".const", (dict, state) => {
                    // .const foo $ff
                    if (dict.ContainsKey("name") && dict.ContainsKey("value"))
                    {
                        state.Procedure.Variables.SetValue(dict["name"], ParseStringToValue(dict["value"]));
                        return;
                    }

                    foreach(var kv in dict)
                    {
                        state.Procedure.Variables.SetValue(kv.Key, ParseStringToValue(kv.Value));
                    }
                }, new[] { "name", "value" })
                .WithParameters(".pad", (dict, state) => {
                    var padto = ParseStringToValue(dict["address"]);
                    if (padto < state.Segment.Address)
                        throw new Exception($"pad with destination of ${padto:X4}, but segment address is already ${state.Segment.Address:X4}");

                    state.Segment.Address = padto;
                }, new[] { "address" })
                .WithParameters(".align", (dict, state) => { 
                    var boundry = ParseStringToValue(dict["boundary"]);

                    if (boundry == 0)
                        return;

                    while(state.Segment.Address % boundry != 0)
                    {
                        state.Segment.Address++;
                    }
                }
                , new[] { "boundry" })
                .WithLine(".byte", (paramLine, lineNumber, state) => {
                    var dataline = new DataLine(state.Procedure, lineNumber, paramLine, state.Segment.Address, DataLine.LineType.IsByte);
                    dataline.ProcessParts(false);
                    state.Segment.Address += dataline.Data.Length;

                    state.Procedure.AddLine(dataline);
                    dataline.WriteToConsole();
                })
                .WithLine(".word", (paramLine, lineNumber, state) => {
                    var dataline = new DataLine(state.Procedure, lineNumber, paramLine, state.Segment.Address, DataLine.LineType.IsWord);
                    dataline.ProcessParts(false);
                    state.Segment.Address += dataline.Data.Length;

                    state.Procedure.AddLine(dataline);
                    dataline.WriteToConsole();
                });
        }

        public async Task Compile()
        {
            if (_project.Code.Contents == null)
                throw new ArgumentNullException(nameof(_project.Code.Contents));

            if (_project.Machine == null)
                throw new ArgumentNullException(nameof(_project.Machine));

            foreach(var opCode in _project.Machine.Cpu.OpCodes)
            {
                _opCodes.Add(opCode.Code.ToLower(), opCode);
            }

            var globals = new Variables(_project.Machine.Variables, "App");

            var lines = _project.Code.Contents.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.TrimEntries);

            var state = new CompileState(globals);

            var previousLines = new StringBuilder();
            int lineNumber = 0;

            foreach (var line in lines)
            {
                lineNumber++;

                if (string.IsNullOrWhiteSpace(line))
                {
                    previousLines.AppendLine();
                    continue;
                }

                if (line.StartsWith(';'))
                {
                    previousLines.AppendLine(line);
                    continue;
                }

                if (line.StartsWith('.'))
                {
                    previousLines.Clear();
                    ParseCommand(line, lineNumber, state);
                }
                else
                {
                    var idx = line.IndexOf(';');
                    
                    var parts = (idx == -1 ? line : line[..idx]).Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    previousLines.AppendLine(line);
                    ParseAsm(parts, previousLines.ToString(), lineNumber, state);
                    previousLines.Clear();
                }
            }

            PruneUnusedObjects(state);

            Reval(state);

            if (_project.CompileOptions.DisplayVariables)
            {
                Console.WriteLine("Variables:");
                foreach(var (Name, Value) in globals.GetChildVariables(globals.Namespace))
                {
                    Console.WriteLine($"{Name} = ${Value:X2}");
                }
            }

            if (!string.IsNullOrWhiteSpace(_project.AssemblerObject.Filename))
            {
                _project.AssemblerObject.Contents = JsonConvert.SerializeObject(state.Segments, Formatting.Indented);
                await _project.AssemblerObject.Save();
            }

            await GenerateDataFile(state);
        }

        private async Task GenerateDataFile(CompileState state)
        {
            var filenames = state.Segments.Select(i => i.Value.Filename).Distinct().ToArray();

            foreach (var filename in filenames)
            {                
                var toSave = new List<byte>(0x10000);
                var segments = state.Segments.Where(i => i.Value.Filename == filename).OrderBy(kv => kv.Value.StartAddress).Select(kv => kv.Value).ToArray();

                var address = segments.First().StartAddress;

                // segments with filenames
                bool header;

                // main output
                if (string.IsNullOrWhiteSpace(filename))
                {
                    header = string.IsNullOrWhiteSpace(_project.OutputFile.Filename) || _project.OutputFile.Filename.EndsWith(".prg", StringComparison.OrdinalIgnoreCase);
                }
                else
                {   // segment with filename
                    header = !string.IsNullOrWhiteSpace(filename) && filename.EndsWith(".prg", StringComparison.OrdinalIgnoreCase);
                }

                if (header)
                {
                    toSave.Add((byte)(address & 0xff));
                    toSave.Add((byte)((address & 0xff00) >> 8));
                }

                foreach (var segment in segments)
                {
                    foreach (var scope in segment.Scopes.Values)
                    {
                        foreach (var proc in scope.Procedures.Values)
                        {
                            foreach (var line in proc.Data)
                            {
                                if (address != line.Address)
                                {
                                    for (var i = address; i < line.Address; i++)
                                    {
                                        toSave.Add(0x00);
                                        address++;
                                    }
                                }
                                toSave.AddRange(line.Data);
                                address += line.Data.Length;
                            }
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(filename))
                {
                    _project.OutputFile.Contents = toSave.ToArray();
                    if (!string.IsNullOrWhiteSpace(_project.OutputFile.Filename))
                    {
                        await _project.OutputFile.Save();
                        Console.WriteLine($"Written {toSave.Count} bytes to '{_project.OutputFile.Filename}'.");
                    } else
                    {
                        Console.WriteLine($"Program size {toSave.Count} bytes.");
                    }
                } 
                else 
                {
                    await File.WriteAllBytesAsync(filename, toSave.ToArray());
                    Console.WriteLine($"Written {toSave.Count} bytes to '{filename}'.");
                }
            }
        }

        private void PruneUnusedObjects(CompileState state)
        {
            foreach(var segment in state.Segments.Values)
            {
                foreach (var scope in segment.Scopes.Values)
                {
                    foreach (var procName in scope.Procedures.Where(i => !i.Value.Data.Any()).Select(i => i.Key).ToArray())
                    {
                        scope.Procedures.Remove(procName);
                    }
                }

                foreach(var scopeName in segment.Scopes.Where(i => !i.Value.Procedures.Any()).Select(i => i.Key).ToArray())
                {
                    segment.Scopes.Remove(scopeName);
                }
            }

            foreach(var segmentName in state.Segments.Values.Where(i => !i.Scopes.Any()).Select(i => i.Name).ToArray())
            {
                state.Segments.Remove(segmentName);
            }
        }

        private void Reval(CompileState state)
        {
            Console.WriteLine("Revaluations:");
            foreach (var segment in state.Segments.Values)
            {
                foreach(var scope in segment.Scopes.Values)
                {
                    foreach(var proc in scope.Procedures.Values)
                    {
                        foreach (var line in proc.Data.Where(l => l.RequiresReval))
                        {
                            line.ProcessParts(true);
                            line.WriteToConsole();

                            if (line.RequiresReval)
                            {
                                throw new CompilerLineException(line, $"Unknown name {string.Join(", ", line.RequiresRevalNames.Select(i => $"'{i}'"))}");
                            }
                        }
                    }
                }
            }
        }

        private void ParseAsm(string[] parts, string line, int lineNumber, CompileState state)
        {
            var code = parts[0].ToLower();

            if (!_opCodes.ContainsKey(code))
            {
                throw new Exception($"Unknown opcode {parts[0]}");
            }

            var opCode = _opCodes[code];

            var toAdd = new Line(opCode, line, lineNumber, state.Procedure, state.Segment.Address, parts[1..]);

            toAdd.ProcessParts(false);
            toAdd.WriteToConsole();

            state.Procedure.AddLine(toAdd);
            state.Segment.Address += toAdd.Data.Length;
        }

        private void ParseCommand(string line, int lineNumber, CompileState state) => _commandParser.Process(line, lineNumber, state);        

        private int ParseStringToValue(string inp)
        {
            if (inp.StartsWith('$'))
                return Convert.ToInt32(inp[1..], 16);

            if (inp.StartsWith('%'))
                return Convert.ToInt32(inp[1..], 2);

            if (int.TryParse(inp, out var result))
                return result;

            throw new Exception($"Cannot parse {inp} into an int");
        }

/*        private Segment GetSegment(string name)
        {
            if (_segments.ContainsKey(name))
                return _segments[name];

            throw new Exception($"Unknown segment {name}");
        }*/
    }
}
