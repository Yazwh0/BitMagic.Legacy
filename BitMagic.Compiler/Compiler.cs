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
        }

        public override string ErrorDetail => Line.Source.ToString();
    }

    public abstract class CompilerSourceException : CompilerException
    {
        public SourceFilePosition SourceFile { get; }

        public CompilerSourceException(SourceFilePosition source, string message) : base(message)
        {
            SourceFile = source;
        }

        public override string ErrorDetail => SourceFile.ToString();
    }

    public class CompilerVerbException : CompilerSourceException
    {
        public CompilerVerbException(SourceFilePosition source, string message) : base(source, message)
        {
        }
    }

    public class CompilerFileNotFound : CompilerException
    {
        public string Filename { get; }

        public CompilerFileNotFound(string filename) : base ("File not found.")
        {
            Filename = filename;
        }

        public override string ErrorDetail => $"'{Filename}'";
    }

    public class CompilerUnknownOpcode : CompilerSourceException
    {
        public CompilerUnknownOpcode(SourceFilePosition source, string message) : base(source, message)
        {
        }
    }

    public class CompilerSegmentTooLarge : CompilerException
    {
        internal Segment Segment { get; }

        internal CompilerSegmentTooLarge(Segment segment) : base("Segment too large.")
        {
            Segment = segment;
        }

        public override string ErrorDetail => $"Maxsize is ${Segment.MaxSize:X4}, but the segment is ${Segment.Address - Segment.StartAddress:X4}";
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

                    if (dict.ContainsKey("maxsize"))
                    {
                        segment.MaxSize = ParseStringToValue(dict["maxsize"]);
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
                }, new[] { "name", "address", "filename", "maxsize" })
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
                .WithParameters(".org", (dict, state) => {
                    var padto = ParseStringToValue(dict["address"]);
                    if (padto < state.Segment.Address)
                        throw new Exception($"pad with destination of ${padto:X4}, but segment address is already ${state.Segment.Address:X4}");

                    state.Segment.Address = padto;
                }, new[] { "address" })
                .WithParameters(".pad", (dict, state) => {
                    var size = ParseStringToValue(dict["size"]);

                    state.Segment.Address += size;
                }, new[] { "size" })
                .WithParameters(".align", (dict, state) => { 
                    var boundry = ParseStringToValue(dict["boundary"]);

                    if (boundry == 0)
                        return;

                    while(state.Segment.Address % boundry != 0)
                    {
                        state.Segment.Address++;
                    }
                }, new[] { "boundary" })
                .WithParameters(".importfile", (dict, state) => {
                    var t = CompileFile(dict["filename"], state);

                    try
                    {
                        t.Wait();
                    } 
                    catch(Exception e)
                    {
                        throw e.InnerException ?? e;
                    }

                }, new[] { "filename" })
                .WithLine(".byte", (source, state) => {
                    var dataline = new DataLine(state.Procedure, source, state.Segment.Address, DataLine.LineType.IsByte);
                    dataline.ProcessParts(false);
                    state.Segment.Address += dataline.Data.Length;

                    state.Procedure.AddLine(dataline);
                    dataline.WriteToConsole();
                })
                .WithLine(".word", (source, state) => {
                    var dataline = new DataLine(state.Procedure, source, state.Segment.Address, DataLine.LineType.IsWord);
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

            //var sb = new StringBuilder();
            //// test
            //var i = 0;
            //var j = 0;
            //foreach(var opCode in _project.Machine.Cpu.OpCodes)
            //{
            //    foreach (var m in opCode.Modes)
            //    {
            //        sb.Append($"{opCode.Code}\t");
            //        sb.AppendLine(m switch {
            //            AccessMode.Implied => "",
            //            AccessMode.Accumulator => "",     // A
            //            AccessMode.Immediate => "#$44",       // #$44
            //            AccessMode.ZeroPage => "$44",        // $44
            //            AccessMode.ZeroPageX => "$44, X",       // $44, X
            //            AccessMode.ZeroPageY => "$44, Y",       // $44, Y
            //            AccessMode.Absolute => "$4433",        // $4400
            //            AccessMode.AbsoluteX => "$4433, X",       // $4400, X        
            //            AccessMode.AbsoluteY => "$4433, Y",       // $4400, Y
            //            AccessMode.Indirect => "($4433)",        // ($4444)
            //            AccessMode.IndirectX => "($44, X)",       // ($44, X)
            //            AccessMode.IndirectY => "($44), Y",       // ($44), Y
            //            AccessMode.IndAbsoluteX => "($4433, X)",    // ($4444, X)
            //            AccessMode.Relative => $"reldest_{i}",        // #$ff for branch instruction,
            //            AccessMode.ZeroPageIndirect => "($44)",// ($44)})
            //            _ => throw new Exception()
            //        });
            //        sb.AppendLine($".byte $00, ${j++:X2} ,$00");
            //        if (m == AccessMode.Relative)
            //        {
            //            sb.AppendLine($".reldest_{i++}:");
            //        }
            //    }
            //}
            //File.WriteAllText(@"d:\documents\source\bitmagic\test.asm", sb.ToString());


            var globals = new Variables(_project.Machine.Variables, "App");

            var state = new CompileState(globals);

            await CompileFile(_project.Code.Filename ?? _project.Source.Filename ?? "", state, _project.Code.Contents);

            PruneUnusedObjects(state);

            Reval(state);

            if (_project.CompileOptions.DisplaySegments)
            {
                foreach (var segment in state.Segments.Values)
                {
                    Console.WriteLine("{0,-25} {1,-5} {2,-5} {3,-5}", "Segment", "Start", "Size", "End");
                    Console.WriteLine($"{segment.Name,-25} ${segment.StartAddress:X4} ${segment.Address - segment.StartAddress:X4} ${segment.Address:X4}");
                }
            }

            foreach (var segment in state.Segments.Values)
            {
                if (segment.MaxSize != 0)
                {
                    if (segment.Address - segment.StartAddress > segment.MaxSize)
                    {
                        throw new CompilerSegmentTooLarge(segment);
                    }
                }
            }

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

        private async Task CompileFile(string fileName, CompileState state, string? contents = null)
        {
            contents ??= (await LoadFile(fileName, state));

            var lines = contents.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.TrimEntries);

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

                var idx = line.IndexOf(';');

                var thisLine = (idx == -1 ? line : line[..idx]);

                if (thisLine.StartsWith('.'))
                {
                    previousLines.Clear();
                    var source = new SourceFilePosition { LineNumber = lineNumber, Source = thisLine, Name = _project.Code.Filename ?? _project.Source.Filename ?? "" };
                    ParseCommand(source, state);
                }
                else
                {
                    var parts = thisLine.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    previousLines.AppendLine(line);
                    var source = new SourceFilePosition { LineNumber = lineNumber, Source = previousLines.ToString(), Name = _project.Code.Filename ?? _project.Source.Filename ?? "" };
                    ParseAsm(parts, source, state);
                    previousLines.Clear();
                }
            }
        }

        private Task<string> LoadFile(string filename, CompileState state)
        {
            if (!File.Exists(filename))
                throw new CompilerFileNotFound(filename);

            var path = Path.GetFullPath(filename);

            if (state.Files.Contains(path))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Warning: {filename} has allready been imported. This instance will be ignored.");
                Console.ResetColor();
                return Task.FromResult("");
            }

            state.Files.Add(path);

            return File.ReadAllTextAsync(path);
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

        private void ParseAsm(string[] parts, SourceFilePosition source, CompileState state)
        {
            var code = parts[0].ToLower();

            if (!_opCodes.ContainsKey(code))
            {
                throw new CompilerUnknownOpcode(source, $"Unknown opcode {parts[0]}");
            }

            var opCode = _opCodes[code];

            var toAdd = new Line(opCode, source, state.Procedure, state.Segment.Address, parts[1..]);

            toAdd.ProcessParts(false);
            toAdd.WriteToConsole();

            state.Procedure.AddLine(toAdd);
            state.Segment.Address += toAdd.Data.Length;
        }

        private void ParseCommand(SourceFilePosition source, CompileState state) => _commandParser.Process(source, state);        

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
