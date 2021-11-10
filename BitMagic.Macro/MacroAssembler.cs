using BitMagic.Common;
using BitMagic.Macro;
using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BigMagic.Macro
{
    // converts .csasm to 
    public class MacroAssembler
    {
        private readonly RazorEngine _razorEngine;
        private Project? _project = null;

        private List<string> _references = new List<string>();

        public MacroAssembler()
        {
            var config = new RazorEngineCompilationOptions();

            config.TemplateNamespace = "bitmagic.asm";
            _razorEngine = new RazorEngine();
        }

        public async Task ProcessFile(Project project)
        {
            _project = project;

            await PreProcessFile();
            await ProcessFile();
        }

        // converts .csasm file to razor friendly one.
        private async Task PreProcessFile()
        {
            if (_project == null)
                throw new ArgumentNullException(nameof(_project));

            if (_project.Source.Contents == null)
                throw new ArgumentNullException(nameof(_project.Source.Contents));

            var lines = _project.Source.Contents.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            var output = new StringBuilder();
            var userHeader = new StringBuilder();

            output.AppendLine($"@* PreProcessor Result of {_project.Source.Filename} *@");
            output.AppendLine("");
            output.AppendLine("@{");

            var commands = new HashSet<string>(_project.Machine.Cpu.OpCodes.Select(i => i.Code));

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                // emtpy line
                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    output.AppendLine("@:");
                    continue;
                }

                if (trimmed.StartsWith("using"))
                {
                    var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var name = parts[1];

                    if (name.EndsWith(';'))
                        name = name.Substring(0, name.Length - 1);

                    userHeader.AppendLine($"@using {name}");
                    continue;
                }

                if (trimmed.StartsWith("reference"))
                {
                    var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var name = parts[1];

                    if (name.EndsWith(';'))
                        name = name.Substring(0, name.Length - 1);

                    _references.Add(name);
                    continue;
                }

                // lines starting with a period, eg .byte, .word
                if (trimmed.StartsWith('.'))
                {
                    output.AppendLine($"@:{trimmed}");
                    continue;
                }

                // lines starting with a semicolon are comments
                if (trimmed.StartsWith(';'))
                {
                    output.AppendLine($"@:{trimmed}");
                    continue;
                }

                // asm
                if (commands.Any(i => trimmed.StartsWith(i, StringComparison.InvariantCultureIgnoreCase)))
                {
                    output.AppendLine($"@:{trimmed}");
                    continue;
                }
                
                output.AppendLine(line);
            }

            output.AppendLine("} @* PreProcessor End *@");
            _project.PreProcess.Contents = userHeader.ToString() + output.ToString();

            if (!string.IsNullOrWhiteSpace(_project.PreProcess.Filename))
                await _project.PreProcess.Save();
        }

        private async Task ProcessFile()
        {
            if (_project == null)
                throw new ArgumentNullException(nameof(_project));

            var toProcess = _project.PreProcess.Contents;

            try
            {
                var template = await _razorEngine.CompileAsync<RazorModel>(toProcess, a =>
                {
                    var assemblies = new List<Assembly>();

                    a.Options.TemplateNamespace = "BitMagic.Asm";

                    if (!string.IsNullOrEmpty(_project.PreProcess.Filename))
                        a.Options.TemplateFilename = Path.GetFileName(_project.PreProcess.Filename);

                    foreach (var include in _references)
                    {
                        var assembly = Assembly.Load(include);
                        if ((_project.Options.VerboseDebugging & ApplicationPart.Macro) != 0)
                            Console.WriteLine($"Adding Referenced Assembly: {include}");
                        a.AddAssemblyReference(assembly);

                        assemblies.Add(assembly);
                    }

                    if ((_project.Options.VerboseDebugging & ApplicationPart.Macro) != 0)
                    {
                        Console.WriteLine("Referenced Assemblies:");
                        foreach (var ass in a.Options.ReferencedAssemblies)
                        {
                            Console.WriteLine(ass.FullName);
                        }
                    }
                });
                
                var result = await template.RunAsync(i => { });

                _project.Code.Contents = result;
                if (!string.IsNullOrWhiteSpace(_project.Code.Filename))
                    await _project.Code.Save();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
