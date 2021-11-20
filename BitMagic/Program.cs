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
using BigMagic.DebugServer;

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
        /// <param name="prgFile">prg file</param>
        /// <param name="args">Commands to run. eg: razor  compile  emulate</param>
        /// <returns></returns>
        static async Task Main(string razorFile = "", string preRazorFile = "", string bmasmFile = "", string asmObjectFile = "",
               string prgFile = "", string[]? args = null)
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

            var project = new Project(new CommanderX16());

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            project.Source.Filename = razorFile;
            project.PreProcess.Filename = preRazorFile;
            project.Code.Filename = bmasmFile;
            project.AssemblerObject.Filename = asmObjectFile;
            project.ProgFile.Filename = prgFile;
            project.LoadTime = stopWatch.Elapsed;

            project.Options.VerboseDebugging = ApplicationPart.Compiler | ApplicationPart.Emulator;


            if (_args.Contains("razor"))
            {
                if (string.IsNullOrWhiteSpace(project.Source.Filename))
                {
                    Console.WriteLine("No Razor csasm file specified.");
                    return;
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
                    return;
                }

                if (project.Code.Contents == null)
                    await project.Code.Load();

                stopWatch.Restart();

                var compiler = new Compiler.Compiler(project);
                await compiler.Compile();

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
                return;
            }

            if (_args.Contains("emulate"))
            {
                if (string.IsNullOrWhiteSpace(project.ProgFile.Filename) && project.ProgFile.Contents == null)
                {
                    Console.WriteLine("No prg file specific, or no compilation has taken place. Cannot emulate.");
                    return;
                }

                if (project.ProgFile.Contents == null)
                    await project.ProgFile.Load();

                var emulator = new Emulation.Emulator(project);
                emulator.LoadPrg();
                emulator.Emulate(0x810);
            }

            if (_args.Contains("debug"))
            {
                var debugger = new EmulatorDebugger(project);
                
            }

            Console.WriteLine("Done.");
        }
    }    
}
