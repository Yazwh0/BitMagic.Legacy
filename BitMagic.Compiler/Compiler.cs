﻿using BitMagic.Common;
using BitMagic.Cpu;
using CodingSeb.ExpressionEvaluator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Compiler
{
    public class Compiler
    {
        private readonly Project _project;
        private int _anonCounter = 0;
        private Dictionary<string, Segment> _segments = new Dictionary<string, Segment>();
        private Dictionary<string, ICpuOpCode> _opCodes = new Dictionary<string, ICpuOpCode>();

        public Compiler(Project project)
        {
            _project = project;
        }

        public async Task Compile()
        {
            if (_project.Code.Contents == null)
                throw new ArgumentNullException(nameof(_project.Code.Contents));

            foreach(var opCode in _project.Machine.Cpu.OpCodes)
            {
                _opCodes.Add(opCode.Code.ToLower(), opCode);
            }

            _anonCounter = 0;
            var globals = new Variables();

            // all segments must be greated from the start via config?
            // maybe should be in state?
            _segments.Clear();
            _segments.Add(".Default", new Segment(globals, true, 0x801, ".Default"));

            var lines = _project.Code.Contents.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.TrimEntries);

            var state = GetInitialState();

            var previousLines = new StringBuilder();

            foreach (var line in lines)
            {
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

                var parts = line.Split(new[] { ' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);

                if (parts[0].StartsWith('.'))
                {
                    previousLines.Clear();
                    ParseCommand(parts, line, state);
                }
                else
                {
                    previousLines.AppendLine(line);
                    ParseAsm(parts, previousLines.ToString(), state);
                    previousLines.Clear();
                }
            }

            PruneUnusedObjects();

            Reval();

            if (!string.IsNullOrWhiteSpace(_project.AssemblerObject.Filename))
            {
                _project.AssemblerObject.Contents = JsonConvert.SerializeObject(_segments, Formatting.Indented);
                await _project.AssemblerObject.Save();
            }

            await GenerateDataFile();
        }

        private async Task GenerateDataFile()
        {
            // todo -- add padding between segments for the prog file generation and save!
            var toSave = new List<byte>(0x10000);

            var address = _segments.First().Value.StartAddress;
            toSave.Add((byte)(address & 0xff));
            toSave.Add((byte)((address & 0xff00) >> 8));

            foreach (var segment in _segments.Values)
            {
                foreach (var scope in segment.Scopes.Values)
                {
                    foreach (var proc in scope.Procedures.Values)
                    {
                        foreach (var line in proc.Data)
                        {
                            if (address != line.Address)
                            {
                                for(var i = address; i < line.Address; i++)
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

            _project.ProgFile.Contents = toSave.ToArray();
            if (!string.IsNullOrWhiteSpace(_project.ProgFile.Filename))
            {
                await _project.ProgFile.Save();
            }
        }

        private void PruneUnusedObjects()
        {
            foreach(var segment in _segments.Values)
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
        }

        private CompileState GetInitialState()
        {
            var segment = GetSegment(".Default"); // use '.' so this cant be generated by the user
            var scope = segment.GetScope($".Default_{_anonCounter++}", true);
            var procedure = scope.GetProcedure($".Default_{_anonCounter++}", true);

            return new CompileState(segment, scope, procedure);
        }

        private void Reval()
        {
            Console.WriteLine("Revaluations:");
            foreach (var segment in _segments.Values)
            {
                foreach(var scope in segment.Scopes.Values)
                {
                    foreach(var proc in scope.Procedures.Values)
                    {
                        foreach (var line in proc.Data.Where(l => l.RequiresReval))
                        {
                            line.ProcessParts(true);
                            line.WriteToConsole();
                        }
                    }
                }
            }
        }

        private void ParseAsm(string[] parts, string line, CompileState state)
        {
            var code = parts[0].ToLower();

            if (!_opCodes.ContainsKey(code))
            {
                throw new Exception($"Unknown opcode {parts[0]}");
            }

            var opCode = _opCodes[code];

            var toAdd = new Line(opCode, line, state.Procedure, state.Segment.Address, parts[1..]);

            toAdd.ProcessParts(false);
            toAdd.WriteToConsole();

            state.Procedure.AddLine(toAdd);
            state.Segment.Address += toAdd.Data.Length;
        }

        private void ParseCommand(string[] parts, string line, CompileState state)
        {
            if (parts[0].EndsWith(':'))
            {
                if (parts[0] == ".:")
                    throw new Exception("Labels require a name. .: is not valid.");

                state.Procedure.Variables.SetValue(parts[0].Substring(1, parts[0].Length - 2), state.Segment.Address);
                return;
            }

            string name;
            switch (parts[0].Substring(1).ToLower())
            {
                case "segment":
                    HasParameter(parts, 2, line);
                    state.Segment = GetSegment(parts[1]);
                    state.Scope = state.Segment.GetScope($".Default_{_anonCounter++}", true);
                    state.Procedure = state.Scope.GetProcedure($".Default_{_anonCounter++}", true);
                    return;

                case "scope":
                    if (parts.Length == 1)
                    {
                        name = $".Anonymous_{_anonCounter++}";
                    } 
                    else
                    {
                        HasParameter(parts, 2, line);
                        name = parts[1];
                    }

                    state.Scope = state.Segment.GetScope(name, false);
                    state.Procedure = state.Scope.GetProcedure($".Default_{_anonCounter++}", true);
                    return;
                case "endscope":
                    state.Scope = state.Segment.GetScope($".Default_{_anonCounter++}", true);
                    return;

                case "proc":
                    if (parts.Length == 1)
                    {
                        name = $".Anonymous_{_anonCounter++}";
                    }
                    else
                    {
                        HasParameter(parts, 2, line);
                        name = parts[1];
                    }

                    state.Procedure = state.Scope.GetProcedure(name, false);
                    state.Scope.Variables.SetValue(name, state.Segment.Address);
                    return;
                case "endproc":
                    state.Procedure = state.Scope.GetProcedure($".Default_{_anonCounter++}", true);
                    return;

                case "const": // .var foo = $12 -or- .var foo $12
                    HasParameter(parts, 3, line);
                    string value;
                    if (parts[2] == "=")
                    {
                        HasParameter(parts, 4, line);
                        value = parts[3];
                    }
                    else
                    {
                        value = parts[2];
                    }
                    state.Procedure.Variables.SetValue(parts[1], ParseStringToValue(value));
                    return;

                case "padto":
                    var padto = ParseStringToValue(parts[1]);
                    if (padto < state.Segment.Address)
                        throw new Exception($"padto with destination of {padto:X4}, but segment address is already {state.Segment.Address}");

                    state.Segment.Address = padto;
                    return;
                case "byte":
                case "word":
                    var dataline = new DataLine(state.Procedure, line, state.Segment.Address);
                    dataline.ProcessParts(false);
                    state.Segment.Address += dataline.Data.Length;

                    state.Procedure.AddLine(dataline);
                    dataline.WriteToConsole();

                    return;
                default:
                    throw new Exception($"Unknown command {parts[0]}");
            }

            throw new Exception($"Unknown command {parts[0]}");
        }

        private int ParseStringToValue(string inp)
        {
            if (inp.StartsWith('$'))
                return Convert.ToInt32(inp.Substring(1), 16);

            if (int.TryParse(inp, out var result))
                return result;

            throw new Exception($"Cannot parse {inp} into an int");

        }

        private void HasParameter(string[] parts, int size, string sourceLine)
        {
            if (parts.Length < size || string.IsNullOrEmpty(parts[size - 1]))
                throw new Exception($"Syntax error {sourceLine}, expecting {size} parts");

        }

        private Segment GetSegment(string name)
        {
            if (_segments.ContainsKey(name))
                return _segments[name];

            throw new Exception($"Unknown segment {name}");
        }
    }
}
