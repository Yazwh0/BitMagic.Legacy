using BitMagic.Common;
using BitMagic.Cpu;
using BitMagic.Machines;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitMagic.Compiler.Exceptions;
using BitMagic.Compiler.Warnings;

namespace BitMagic.Compiler
{
    public class Compiler
    {
        private readonly Project _project;        
        private readonly Dictionary<string, ICpuOpCode> _opCodes = new Dictionary<string, ICpuOpCode>();
        private readonly CommandParser _commandParser;

        public Compiler(Project project)
        {
            _project = project;
            _commandParser = CreateParser();
        }

        public Compiler(string code)
        {
            _project = new Project();
            _project.Code.Contents = code;
            _commandParser = CreateParser();
        }

        private CommandParser CreateParser() => CommandParser.Parser()
                .WithLabel((label, state) =>
                {
                    if (label == ".:")
                        throw new Exception("Labels require a name. .: is not valid.");

                    state.Procedure.Variables.SetValue(label[1..^1], state.Segment.Address);
                })
                //.WithParameters(".scopedelimiter",  (dict, state, source) =>
                //{

                //}, new[] { "delimiter" })
                .WithParameters(".machine", (dict, state, source) =>
                {
                    var newMachine = MachineFactory.GetMachine(dict["name"]);

                    if (newMachine == null)
                        throw new MachineNotKnownException(dict["name"]);

                    if (_project.Machine != null && newMachine.Name != _project.Machine.Name && newMachine.Version != _project.Machine.Version)
                        throw new MachineAlreadySetException(_project.Machine.Name, dict["name"]);

                    if (_project.Machine == null)
                    {
                        _project.Machine = newMachine;
                    }

                    if (_project.MachineEmulator != null && !_project.MachineEmulator.Initialised)
                    {
                        _project.MachineEmulator.SetRom(new byte[0x4000]);
                        _project.MachineEmulator.Build();
                    }

                    InitFromMachine(state);

                }, new[] { "name" })
                .WithParameters(".cpu", (dict, state, source) =>
                {
                    var machine = new NoMachine();
                    var cpu = CpuFactory.GetCpu(dict["name"]);

                    if (cpu == null)
                        throw new CpuNotKnownException(dict["name"]);

                    machine.Cpu = cpu;

                    _project.Machine = machine;

                    InitFromMachine(state);

                }, new[] { "name" })
                .WithParameters(".segment", (dict, state, source) => {
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
                        foreach (var proc in segment.DefaultProcedure)
                        {
                            if (proc.Value.Data.Any())
                            {
                                throw new Exception($"Cannot modify segment start address when it already has data. {segment.Name}");
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
                    else
                    {
                        segment.Filename = ":" + segment.Name; // todo: find a better way to inform the writer that this segment isn't to be written.
                    }

                    state.Segment = segment;
                    state.Scope = state.ScopeFactory.GetScope($"Main");
                    state.Procedure = state.Segment.GetDefaultProcedure(state.Scope);

                }, new[] { "name", "address", "filename", "maxsize" })
                .WithParameters(".endsegment", (dict, state, source) => {
                    state.Segment = state.Segments["Main"];
                    state.Scope = state.ScopeFactory.GetScope($"Main");
                    state.Procedure = state.Segment.GetDefaultProcedure(state.Scope);
                })
                .WithParameters(".scope", (dict, state, source) => {

                    string name = dict.ContainsKey("name") ? dict["name"] : $"Scope_{state.AnonCounter}";
                    state.Scope = state.ScopeFactory.GetScope(name);

                    state.Procedure = state.Procedure.GetProcedure($"{name}_Proc", state.Segment.Address, state.Scope);
                    state.AnonCounter++;

                }, new[] { "name" })
                .WithParameters(".endscope", (dict, state, source) => {

                    if (state.Procedure.Name.Replace("_Proc", "") != state.Scope.Name || !state.Procedure.Anonymous)
                    {
                        state.Warnings.Add(new UnmatchedEndProcWarning(source));
                    }

                    var proc = state.Procedure.Parent;
                    if (proc == null)
                    {
                        proc = state.Segment.GetDefaultProcedure(state.Scope);
                        state.Warnings.Add(new UnmatchedEndProcWarning(source));
                    }
                    
                    state.Scope = proc.Scope;                    
                    state.Procedure = proc;

                })
                .WithParameters(".proc", (dict, state, source) => {

                    var name = dict.ContainsKey("name") ? dict["name"] : $"UnnamedProc_{state.AnonCounter++}";

                    state.Procedure = state.Procedure.GetProcedure(name, state.Segment.Address);

                }, new[] { "name" })
                .WithParameters(".endproc", (dict, state, source) => {

                    if (!state.Procedure.Variables.HasValue("endproc"))
                        state.Procedure.Variables.SetValue("endproc", state.Segment.Address);

                    if (state.Procedure.Anonymous)
                        state.Warnings.Add(new EndProcOnAnonymousWarning(source));

                    var proc = state.Procedure.Parent;
                    if (proc == null)
                    {
                        proc = state.Segment.GetDefaultProcedure(state.Scope);
                        state.Warnings.Add(new UnmatchedEndProcWarning(source));
                    }

                    state.Scope = proc.Scope;
                    state.Procedure = proc;
                })
                .WithParameters(".const", (dict, state, source) => {
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
                .WithParameters(".org", (dict, state, source) => {
                    var padto = ParseStringToValue(dict["address"]);
                    if (padto < state.Segment.Address)
                        throw new Exception($"pad with destination of ${padto:X4}, but segment address is already ${state.Segment.Address:X4}");

                    state.Segment.Address = padto;
                }, new[] { "address" })
                .WithParameters(".pad", (dict, state, source) => {
                    var size = ParseStringToValue(dict["size"]);

                    state.Segment.Address += size;
                }, new[] { "size" })
                .WithParameters(".align", (dict, state, source) => { 
                    var boundry = ParseStringToValue(dict["boundary"]);

                    if (boundry == 0)
                        return;

                    while(state.Segment.Address % boundry != 0)
                    {
                        state.Segment.Address++;
                    }
                }, new[] { "boundary" })
                .WithParameters(".importfile", (dict, state, source) => {
                    var t = CompileFile(dict["filename"], state, null, source);

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

                    state.Procedure.AddData(dataline);
                    if (_project.CompileOptions.DisplayData)
                        dataline.WriteToConsole();
                })
                .WithLine(".word", (source, state) => {
                    var dataline = new DataLine(state.Procedure, source, state.Segment.Address, DataLine.LineType.IsWord);
                    dataline.ProcessParts(false);
                    state.Segment.Address += dataline.Data.Length;

                    state.Procedure.AddData(dataline);
                    if (_project.CompileOptions.DisplayData)
                        dataline.WriteToConsole();
                });
        

        public async Task<CompileResult> Compile()
        {
            if (_project.Code.Contents == null)
                throw new ArgumentNullException(nameof(_project.Code.Contents));


            //var sb = new StringBuilder();
            //// test
            //var i = 0;
            //var j = 0;
            //foreach (var opCode in _project.Machine.Cpu.OpCodes)
            //{
            //    foreach (var m in opCode.Modes)
            //    {
            //        sb.Append($"'{opCode.GetOpCode(m):X2}\t{opCode.Code}\t");
            //        sb.AppendLine(m switch
            //        {
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
            //        //sb.AppendLine($".byte $00, ${j++:X2} ,$00");
            //        //if (m == AccessMode.Relative)
            //        //{
            //        //    sb.AppendLine($".reldest_{i++}:");
            //        //}
            //    }
            //}
            //File.WriteAllText(@"d:\documents\source\bitmagic\opcodes.asm", sb.ToString());

            var globals = new Variables("App");

            var state = new CompileState(globals, _project.OutputFile.Filename ?? "");

            await CompileFile(_project.Code.Filename ?? _project.Source.Filename ?? "", state, _project.Code.Contents);

            //PruneUnusedObjects(state);

            try
            {
                Reval(state);
            } 
            catch
            {
                DisplayVariables(globals);
                throw;
            }

            if (_project.CompileOptions.DisplaySegments)
            {
                Console.WriteLine("{0,-25} {1,-5} {2,-5} {3,-5}", "Segment", "Start", "Size", "End");
                foreach (var segment in state.Segments.Values)
                {
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

            DisplayVariables(globals);

            if (!string.IsNullOrWhiteSpace(_project.AssemblerObject.Filename))
            {
                _project.AssemblerObject.Contents = JsonConvert.SerializeObject(state.Segments, Formatting.Indented);
                await _project.AssemblerObject.Save();
            }

            var result = await GenerateDataFile(state);

            return new CompileResult(state.Warnings.Select(w => w.ToString()), result, _project);
        }
        private void InitFromMachine(CompileState state)
        {
            if (_project.Machine == null)
                throw new NullReferenceException();

            _opCodes.Clear();
            foreach (var opCode in _project.Machine.Cpu.OpCodes)
            {
                _opCodes.Add(opCode.Code.ToLower(), opCode);
            }

            foreach (var kv in _project.Machine.Variables.Values)
            {
                state.Globals.SetValue(kv.Key, kv.Value);
            }
        }

        private void DisplayVariables(Variables globals)
        {
            if (_project.CompileOptions.DisplayVariables)
            {
                Console.WriteLine("Variables:");
                foreach (var (Name, Value) in globals.GetChildVariables(globals.Namespace))
                {
                    Console.WriteLine($"{Name} = ${Value:X2}");
                }
            }
        }

        private async Task CompileFile(string fileName, CompileState state, string? contents = null, SourceFilePosition? compileSource = null)
        {
            contents ??= (await LoadFile(fileName, state, compileSource));

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

        private Task<string> LoadFile(string filename, CompileState state, SourceFilePosition? source = null)
        {
            if (!File.Exists(filename))
                throw new CompilerFileNotFound(filename);

            var path = Path.GetFullPath(filename);

            if (state.Files.Contains(path))
            {
                state.Warnings.Add(new FileAlreadyImportedWarning(source ?? new SourceFilePosition() { LineNumber = 0, Name = "Default" }, filename));

                return Task.FromResult("");
            }

            state.Files.Add(path);

            return File.ReadAllTextAsync(path);
        }

        private async Task<Dictionary<string, NamedStream>> GenerateDataFile(CompileState state)
        {
            var filenames = state.Segments.Select(i => i.Value.Filename ?? "").Distinct().ToArray();

            var toReturn = new Dictionary<string, NamedStream>();

            foreach (var filename in filenames)
            {
                var toSave = new List<byte>(0x10000);

                // todo: enforce one segment, one filename???
                var segments = state.Segments.Where(i => (i.Value.Filename ?? "") == filename).OrderBy(kv => kv.Value.StartAddress).Select(kv => kv.Value).ToArray();

                var address = segments.First().StartAddress;
                var writer = new FileWriter(segments.First().Name, filename, address);


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
                    var headerBytes = new byte[] { (byte)(address & 0xff), (byte)((address & 0xff00) >> 8) };

                    toSave.Add((byte)(address & 0xff));
                    toSave.Add((byte)((address & 0xff00) >> 8));

                    writer.SetHeader(headerBytes);
                }

                foreach (var proc in segments.SelectMany(p => p.DefaultProcedure.Values).OrderBy(p => p.StartAddress))
                {
                    proc.Write(writer);
                    //foreach (var line in proc.Data.OrderBy(p => p.Address))
                    //{
                    //    writer.Add(line.Data, line.Address);
                    //    //if (address < line.Address)
                    //    //{
                    //    //    for (var i = address; i < line.Address; i++)
                    //    //    {
                    //    //        toSave.Add(0x00);
                    //    //        address++;
                    //    //    }
                    //    //}
                    //    //else if(address > line.Address)
                    //    //{
                    //    //    throw new Exception($"Lines address ${line.Address:X4} to output is behind the position in the output ${address:X4} in proc {proc.Name}.");
                    //    //}
                    //    //toSave.AddRange(line.Data);
                    //    //address += line.Data.Length;
                    //}
                }

                var result = writer.Write();
                toReturn.Add(result.SegmentName, result);

                //if (string.IsNullOrWhiteSpace(filename))
                //{
                //    _project.OutputFile.Contents = toSave.ToArray();
                //    if (!string.IsNullOrWhiteSpace(_project.OutputFile.Filename))
                //    {
                //        await _project.OutputFile.Save();
                //        Console.WriteLine($"Written {toSave.Count} bytes to '{_project.OutputFile.Filename}'.");
                //    } else
                //    {
                //        Console.WriteLine($"Program size {toSave.Count} bytes.");
                //    }
                //} 
                //else 
                //{
                //    await File.WriteAllBytesAsync(filename, toSave.ToArray());
                //    Console.WriteLine($"Written {toSave.Count} bytes to '{filename}'.");
                //}
            }

            return toReturn;
        }

        private void PruneUnusedObjects(CompileState state)
        {
            foreach(var segment in state.Segments.Values)
            {
                foreach (var procName in segment.DefaultProcedure.Where(i => !i.Value.Data.Any()).Select(i => i.Key).ToArray())
                {
                    segment.DefaultProcedure.Remove(procName);
                }
            }

            foreach(var segmentName in state.Segments.Values.Where(i => !i.DefaultProcedure.Any()).Select(i => i.Name).ToArray())
            {
                state.Segments.Remove(segmentName);
            }
        }

        private void Reval(CompileState state)
        {
            Console.WriteLine("Revaluations:");
            foreach (var segment in state.Segments.Values)
            {
                foreach (var proc in segment.DefaultProcedure.Values)
                {
                    RevalProc(proc);
                }
            }
        }

        private void RevalProc(Procedure proc)
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

            foreach(var p in proc.Procedures)
            {
                RevalProc(p);
            }
        }

        private void ParseAsm(string[] parts, SourceFilePosition source, CompileState state)
        {
            if (_project.Machine == null)
                throw new MachineNotSetException();

            var code = parts[0].ToLower();

            if (!_opCodes.ContainsKey(code))
            {
                throw new CompilerUnknownOpcode(source, $"Unknown opcode {parts[0]}");
            }

            var opCode = _opCodes[code];

            var toAdd = new Line(opCode, source, state.Procedure, _project.Machine.Cpu, state.Evaluator, state.Segment.Address, parts[1..]);

            toAdd.ProcessParts(false);

            if (_project.CompileOptions.DisplayCode)
                toAdd.WriteToConsole();

            state.Procedure.AddData(toAdd);
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
    }
}
