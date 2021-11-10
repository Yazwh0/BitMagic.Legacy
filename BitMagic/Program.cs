using BigMagic.Macro;
using BitMagic.Common;
using BitMagic.Compiler;
using BitMagic.Machines;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BitMagic
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine($"BitMagic!");

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var project = new Project(new CommanderX16());

            await project.Source.Load(@"D:\Documents\Source\BitMagic\test.csasm");
            project.PreProcess.Filename = @"D:\Documents\Source\BitMagic\test.csasm.pre";
            project.Code.Filename = @"D:\Documents\Source\BitMagic\test.asm";
            project.AssemblerObject.Filename = @"D:\Documents\Source\BitMagic\test.compiled.json";
            project.ProgFile.Filename = @"D:\Documents\Source\BitMagic\test.prg";

            project.Options.VerboseDebugging = ApplicationPart.Compiler & ApplicationPart.Emulator;

            project.LoadTime = stopWatch.Elapsed;
            stopWatch.Restart();

            var macro = new MacroAssembler();
            await macro.ProcessFile(project);

            project.PreProcessTime = stopWatch.Elapsed;
            stopWatch.Restart();

            var compiler = new Compiler.Compiler(project);
            await compiler.Compile();

            project.CompileTime = stopWatch.Elapsed;

            Console.WriteLine($"Load         : {project.LoadTime:s\\.fff}");
            Console.WriteLine($"PreProcessor : {project.PreProcessTime:s\\.fff}");
            Console.WriteLine($"Compiler     : {project.CompileTime:s\\.fff}");
        }
    }    
}
