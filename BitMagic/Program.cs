using BigMagic.Macro;
using BitMagic.Common;
using BitMagic.Compiler;
using BitMagic.Machines;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using BitMagic.Emulation;
using System.CommandLine;
using System.Linq;
using BitMagic.Compiler.Exceptions;
using System.IO;

namespace BitMagic
{
    class Program
    {

        /// <summary>
        /// X16 Compiler Emulator and Debugger.
        /// </summary>
        /// <param name="razorFile">csasm file to process</param>
        /// <param name="preRazorFile">optional debugging outputfile</param>
        /// <param name="bmasmFile">bmasm file to process or save if using a csasm source</param>
        /// <param name="asmObjectFile">option debugging json file</param>
        /// <param name="prgFile">output file</param>
        /// <param name="romFile">rom files</param>
        /// <param name="workingDirectory">Working Directory</param>
        /// <param name="showWarnings">Show C# Compiler Warnings</param>
        /// <param name="displayOutput">Write the data generated to the console</param>
        /// <param name="beautify">Beautify generated code</param>
        /// <param name="args">Commands to run. eg: razor compile emulate</param>
        /// <returns></returns>
        static async Task<int> Main(string razorFile = "", string preRazorFile = "", string bmasmFile = "", string asmObjectFile = "",
               string prgFile = "", string romFile= "rom.bin", string workingDirectory = "", bool showWarnings = false, bool displayOutput = false, bool beautify = false, string[]? args = null)
        {
            string[] _args;
            if (args == null)
                _args = new string[] { };
            else
                _args = args.Select(a => a.ToLower()).ToArray();

            var _isDebug = false;
            if (_args.Contains("debug"))
            {   // debug cant use console
                _isDebug = true;
            }
            else
            {
                Console.WriteLine($"BitMagic!");
            }

            if (!string.IsNullOrWhiteSpace(workingDirectory))
            {
                Directory.SetCurrentDirectory(workingDirectory);
            }

            var project = new Project();

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            project.Source.Filename = razorFile;
            project.PreProcess.Filename = preRazorFile;
            project.Code.Filename = bmasmFile;
            project.AssemblerObject.Filename = asmObjectFile;
            project.OutputFile.Filename = prgFile;
            project.RomFile.Filename = romFile;

            // if we're not emulating, we dont need a real rom.
            if (_args.Contains("emulate"))
            {
                await project.RomFile.Load();

                if (project.RomFile?.Contents == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"No rom file found.");
                    Console.ResetColor();
                    return -1;
                }

                var machine = MachineFactory.GetMachine(Machine.CommanderX16R38);
                project.Machine = machine;

                if (project.MachineEmulator == null)
                    throw new Exception($"Machine isn't emulatable. {machine?.Name}");

                project.MachineEmulator.SetRom(project.RomFile.Contents);
                project.MachineEmulator.Build(); // todo: turn this into a proper factory
            } 
            else
            {
                //var machine = MachineFactory.GetMachine(Machine.CommanderX16R38);
                //machine.SetRom(new byte[0x4000]);
                //machine.Build(); // todo: turn this into a proper factory
                //project.Machine = machine;
            }

            project.LoadTime = stopWatch.Elapsed;

            project.Options.VerboseDebugging = ApplicationPart.Compiler;//| ApplicationPart.Emulator;
            project.Options.Beautify = beautify;

            project.CompileOptions.DisplayCode = displayOutput;
            project.CompileOptions.DisplayVariables = true;
            project.CompileOptions.DisplaySegments= true;

            if (_args.Contains("razor"))
            {
                if (string.IsNullOrWhiteSpace(project.Source.Filename))
                {
                    Console.WriteLine("No Razor csasm file specified.");
                    Console.ResetColor();
                    return -1;
                }

                await project.Source.Load();
                stopWatch.Restart();

                var macro = new MacroAssembler();
                try
                {
                    await macro.ProcessFile(project);
                }
                catch (CompilationException e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Errors -----------------------");

                    var lines = e.GeneratedCode.Split("\n");

                    foreach (var error in e.Errors.Where(i => i.DefaultSeverity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        var errorText = error.ToString()[1..];
                        var idx = errorText.IndexOf(",");

                        Console.WriteLine(error.ToString());
                        Console.ForegroundColor = ConsoleColor.Gray;

                        if (int.TryParse(errorText[..idx], out var lineNumber))
                        {
                            lineNumber--;
                            var startLine = Math.Max(0, lineNumber - 2);
                            foreach (var l in lines[startLine..Math.Min(lines.Length, lineNumber + 3)])
                            {
                                if (startLine == lineNumber)
                                {
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.WriteLine($">> {l}");
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    Console.WriteLine($"   {l}");
                                }
                                startLine++;
                            }
                        }
                        Console.WriteLine("------------------------------");
                    }

                    if (showWarnings && e.Errors.Where(i => i.DefaultSeverity == Microsoft.CodeAnalysis.DiagnosticSeverity.Warning).Any())
                    {
                        foreach (var error in e.Errors.Where(i => i.DefaultSeverity == Microsoft.CodeAnalysis.DiagnosticSeverity.Warning))
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            var errorText = error.ToString()[1..];
                            var idx = errorText.IndexOf(",");

                            Console.WriteLine(error.ToString());
                            Console.ForegroundColor = ConsoleColor.Gray;

                            if (int.TryParse(errorText[..idx], out var lineNumber))
                            {
                                lineNumber--;
                                var startLine = Math.Max(0, lineNumber - 2);
                                foreach (var l in lines[startLine..Math.Min(lines.Length, lineNumber + 3)])
                                {
                                    if (startLine == lineNumber)
                                    {
                                        Console.ForegroundColor = ConsoleColor.White;
                                        Console.WriteLine($">> {l}");
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Gray;
                                        Console.WriteLine($"   {l}");
                                    }
                                    Console.WriteLine(l);
                                    startLine++;
                                }
                            }
                        }

                        Console.WriteLine("------------------------------");
                    }
                    Console.WriteLine("Compilation failed.");
                    Console.ResetColor();
                    return -1;
                }

                project.PreProcessTime = stopWatch.Elapsed;
            }

            if (_args.Contains("compile"))
            {
                if (string.IsNullOrWhiteSpace(project.Code.Filename) && project.Code.Contents == null)
                {
                    Console.WriteLine("No bmasm or csasm file specified. Cannot compile.");
                    Console.ResetColor();
                    return -1;
                }

                if (project.Code.Contents == null)
                    await project.Code.Load();

                stopWatch.Restart();

                var compiler = new Compiler.Compiler(project);

                try
                {
                    var warnings = await compiler.Compile();

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    foreach (var warning in warnings.Warnings)
                    {
                        Console.WriteLine(warning);
                    }

                    Console.ResetColor();

                    foreach (var result in warnings.Data.Values)
                    {
                        if (string.IsNullOrEmpty(result.FileName) || result.FileName.StartsWith(":"))
                        {
             
                            Console.WriteLine($"Segment {result.SegmentName} is {result.Length} bytes.");
                            continue;
                        }

                        using var fs = new FileStream(result.FileName, FileMode.Create);
                        result.WriteTo(fs);
                        fs.Close();

                        Console.WriteLine($"Segment {result.SegmentName} is {result.Length} bytes. Written to '{result.FileName}'.");
                    }
                }
                catch (CompilerException e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Compiler Error: {e.Message}");
                    Console.WriteLine(e.ErrorDetail);
                    Console.ResetColor();
                    return 1;
                }

                project.CompileTime = stopWatch.Elapsed;
            }

            if (!_isDebug)
            {
                Console.WriteLine($"Load         : {project.LoadTime:s\\.fff}s");
                Console.WriteLine($"PreProcessor : {project.PreProcessTime:s\\.fff}s");
                Console.WriteLine($"Compiler     : {project.CompileTime:s\\.fff}s");
            }

            if (_args.Contains("emulate") && _args.Contains("debug"))
            {
                Console.WriteLine("Cannot emulate and debug. Use only one command.");
                Console.ResetColor();
                return -1;
            }

            if (_args.Contains("emulate"))
            {
                Console.WriteLine("Using Rom: " + project.RomFile.Filename);

                if (string.IsNullOrWhiteSpace(project.OutputFile.Filename) && project.OutputFile.Contents == null)
                {
                    Console.WriteLine("No prg file specific, or no compilation has taken place. Cannot emulate.");
                    Console.ResetColor();
                    return -1;
                }

                if (project.OutputFile.Contents == null)
                    await project.OutputFile.Load();

                var emulator = new Emulation.Emulator(project);
                emulator.LoadPrg();
                emulator.Emulate();
            }

            if (_args.Contains("debug"))
            {
                Console.WriteLine("Not yet implemented.");
                Console.ResetColor();
                return -1;
            }

            Console.WriteLine("Done.");
            Console.ResetColor();
            return 0;
        }
    }    
}
