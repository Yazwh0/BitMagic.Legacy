using BitMagic.AsmTemplate;
using BitMagic.AsmTemplateEngine;
using BitMagic.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BigMagic.Macro
{
    // converts .csasm to 
    public class MacroAssembler
    {
        private Project? _project = null;

        private List<string> _references = new List<string>();
        private List<string> _assemblyFilenames = new List<string>();

        public MacroAssembler()
        {
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

            output.AppendLine("using System;");
            output.AppendLine("using System.Linq;");
            output.AppendLine("using System.Collections;");
            output.AppendLine("using System.Collections.Generic;");
            output.AppendLine("using System.Threading.Tasks;");
            output.AppendLine("using BM = BitMagic.AsmTemplate.BitMagicHelper;");
            output.AppendLine($"// PreProcessor Result of {_project.Source.Filename}");
            output.AppendLine("namespace BitMagic.App");
            output.AppendLine("{");
            output.AppendLine("public class Template : BitMagic.AsmTemplate.ITemplateRunner");
            output.AppendLine("{");
            output.AppendLine("\tpublic async Task Execute()");
            output.AppendLine("\t{");

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                // emtpy line
                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    continue;
                }

                if (trimmed.StartsWith("using"))
                {
                    userHeader.AppendLine(trimmed);
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

                if (trimmed.StartsWith("assembly"))
                {
                    var name = trimmed.Substring("assembly ".Length);

                    var idx = name.IndexOf(';');

                    if (idx >= 0)
                        name = name.Substring(0, idx);
                    
                    _assemblyFilenames.Add(name);
                    continue;
                }

                output.AppendLine(line);
            }

            output.AppendLine("\t}");
            output.AppendLine("}");
            output.AppendLine("}");

            var engine = CsasmEngine.CreateEngine();

            _project.PreProcess.Contents = engine.Process(userHeader.ToString() + output.ToString());

            if (!string.IsNullOrWhiteSpace(_project.PreProcess.Filename))
                await _project.PreProcess.Save();
        }

        private async Task ProcessFile()
        {
            if (_project == null)
                throw new ArgumentNullException(nameof(_project));

            var toProcess = _project.PreProcess.Contents;

            if (toProcess == null)
                throw new ArgumentNullException(nameof(toProcess));

            var syntaxTree = CSharpSyntaxTree.ParseText(toProcess);

            var assemblies = new List<Assembly>();

            assemblies.AddRange(new[] {
                    typeof(object).Assembly,
                    Assembly.Load(new AssemblyName("Microsoft.CSharp")),
                    Assembly.Load(new AssemblyName("System.Runtime")),
                    typeof(System.Collections.IList).Assembly,
                    typeof(System.Collections.Generic.IEnumerable<>).Assembly,
                    Assembly.Load(new AssemblyName("System.Linq")),
                    Assembly.Load(new AssemblyName("System.Linq.Expressions")),
                    Assembly.Load(new AssemblyName("netstandard")),
                    typeof(BitMagicHelper).Assembly
            });

            foreach(var assemblyFilename in _assemblyFilenames)
            {
                var assemblyInclude = Assembly.LoadFrom(assemblyFilename);
                if ((_project.Options.VerboseDebugging & ApplicationPart.Macro) != 0)
                    Console.WriteLine($"Adding File Assembly: {assemblyInclude.FullName}");

                assemblies.Add(assemblyInclude);
            }

            foreach (var include in _references)
            {
                var assemblyInclude = Assembly.Load(include);
                if ((_project.Options.VerboseDebugging & ApplicationPart.Macro) != 0)
                    Console.WriteLine($"Adding Referenced Assembly: {include}");

                assemblies.Add(assemblyInclude);
            }

            CSharpCompilation compilation = CSharpCompilation.Create(
                !string.IsNullOrEmpty(_project.PreProcess.Filename) ? Path.GetFileName(_project.PreProcess.Filename) : Path.GetFileName(_project.Source.Filename),
                new[] { syntaxTree },
                assemblies.Select(ass => { return MetadataReference.CreateFromFile(ass.Location); }),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            MemoryStream memoryStream = new MemoryStream();

            EmitResult emitResult = compilation.Emit(memoryStream);

            if (!emitResult.Success)
            {
                var exception = new CompilationException()
                {
                    Errors = emitResult.Diagnostics.ToList(),
                    GeneratedCode = toProcess
                };

                throw exception;
            }

            memoryStream.Position = 0;

            // save dll only if the PreProcess file is being written as this means we're debugging.
            if (!string.IsNullOrWhiteSpace(_project.PreProcess.Filename))
            {
                using (FileStream fileStream = new FileStream(
                    path: Path.Combine(Path.GetDirectoryName(_project.PreProcess.Filename) ?? throw new Exception(), Path.GetFileNameWithoutExtension(_project.PreProcess.Filename) + ".dll"),
                    mode: FileMode.OpenOrCreate,
                    access: FileAccess.Write,
                    share: FileShare.None,
                    bufferSize: 4096,
                    useAsync: true))
                {
                    await memoryStream.CopyToAsync(fileStream);
                }
            }

            Template.StartProject();

            var assembly = Assembly.Load(memoryStream.ToArray());

            var runner = Activator.CreateInstance(assembly.GetType($"BitMagic.App.Template") ?? throw new Exception("BitMagic.App.Template not in compiled dll.")) as ITemplateRunner;

            if (runner == null)
                throw new Exception("Temlpate is not a ITemplateRunner");

            await runner.Execute();

            _project.Code.Contents = Template.ToString;
                if (!string.IsNullOrWhiteSpace(_project.Code.Filename))
                    await _project.Code.Save();
        }
    }

    internal class CompilationException : Exception {
        public List<Diagnostic> Errors { get; set; } = new List<Diagnostic>();

        public string GeneratedCode { get; set; } = "";

        public override string Message
        {
            get
            {
                string errors = string.Join("\n", this.Errors.Where(w => w.IsWarningAsError || w.Severity == DiagnosticSeverity.Error));
                return "Unable to compile template: " + errors;
            }
        }
    }

}
