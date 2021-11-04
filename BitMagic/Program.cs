using BigMagic.Macro;
using BitMagic.Common;
using BitMagic.Compiler;
using BitMagic.Machines;
using System;
using System.Threading.Tasks;

namespace BitMagic
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var project = new Project(new CommanderX16());

            await project.Source.Load(@"D:\Documents\Source\BitMagic\test.csasm");
            project.PreProcess.Filename = @"D:\Documents\Source\BitMagic\test.csasm.pre";
            project.Code.Filename = @"D:\Documents\Source\BitMagic\test.asm";
            project.AssemblerObject.Filename = @"D:\Documents\Source\BitMagic\test.compiled.json";

            var macro = new MacroAssembler();
            await macro.ProcessFile(project);
            var compiler = new Compiler.Compiler(project);

            await compiler.Compile();
        }
    }    
}
