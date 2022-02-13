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
        /// <param name="outputFile">output file</param>
        /// <param name="romFile">rom files</param>
        /// <param name="args">Commands to run. eg: razor compile emulate</param>
        /// <returns></returns>
        static async Task<int> Main(string razorFile = "", string preRazorFile = "", string bmasmFile = "", string asmObjectFile = "",
               string outputFile = "", string romFile= "rom.bin", string[]? args = null)
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

            var project = new Project();

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            project.Source.Filename = razorFile;
            project.PreProcess.Filename = preRazorFile;
            project.Code.Filename = bmasmFile;
            project.AssemblerObject.Filename = asmObjectFile;
            project.OutputFile.Filename = outputFile;
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

                project.Machine = new CommanderX16(project.RomFile.Contents);
            } 
            else
            {
                project.Machine = new CommanderX16(new byte[0x4000]);
            }

            project.LoadTime = stopWatch.Elapsed;

            project.Options.VerboseDebugging = ApplicationPart.Compiler;//| ApplicationPart.Emulator;

            project.CompileOptions.DisplayCode = true;
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
                await macro.ProcessFile(project);

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
                    await compiler.Compile();
                } 
                catch(CompilerException e)
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
                var debugger = new EmulatorDebugger(project);
                
            }

            Console.WriteLine("Done.");
            Console.ResetColor();
            return 0;
        }
    }    
}
