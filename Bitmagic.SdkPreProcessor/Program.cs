using BitMagic.AsmTemplateEngine;
using System.Text.RegularExpressions;

namespace BitMagic.SdkPreProcessor
{
    class Program
    {
        /// <summary>
        /// BitMagic csasm preprocessor.
        /// </summary>
        /// <param name="outputFolder">Folder to output the .cs files to</param>
        /// <param name="baseFolder">Base folder useful with recursive and wildcards</param>
        /// <param name="recursive">Also check subfolders</param>
        /// <param name="extension">Extension for the created files. Defaults to cs.</param>
        /// <param name="template">Template name that defines which lines are c# and which are for the underlying.</param>
        /// <param name="args">csasm files to process</param>
        /// <returns></returns>
        /// 
        static async Task<int> Main(string? outputFolder = null, string? baseFolder = null, bool recursive = false, 
            string extension = "cs", string template = "csasm", string[]? args = null)
        {
            Console.WriteLine("BitMagic C# Template PreProcessor");
            if (args == null)
            {
                Console.Error.WriteLine("No files specified.");
                return 1;
            }

            var engine = template.ToLower() switch { 
                "csasm" => CsasmEngine.CreateEngine(),
                "text" => CreateTextEngine(),
                _ => CsasmEngine.CreateEngine()
            };

            Console.WriteLine($"Using Template Engine : {engine.TemplateName}");

            foreach(var inputFilename in args)
            {
                var path = Path.GetDirectoryName(inputFilename);
                path = String.IsNullOrWhiteSpace(path) ? baseFolder ?? Directory.GetCurrentDirectory() : path;
                var search = Path.GetFileName(inputFilename);

                var files = Directory.GetFiles(path, search, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                foreach (var file in files)
                {
                    Console.WriteLine($"Processing: {file}");
                    var contents = await File.ReadAllTextAsync(file);

                    var newContents = engine.Process(contents);

                    var newFilename = Path.GetFileName(file) + "." + extension;
                    newFilename = Path.Combine(outputFolder ?? Path.GetDirectoryName(file) ?? throw new Exception("Cannot find file path"), newFilename);

                    if (File.Exists(newFilename))
                        File.Delete(newFilename);

                    await File.WriteAllTextAsync(newFilename, newContents);
                }
            }

            return 0;
        }

        private static ITemplateEngine CreateTextEngine() => TemplateEngineBuilder
            .As("text")
            .WithUnderlying(new Regex(@"^\s*(?<line>([\.].*))$", RegexOptions.Compiled))
            .WithCSharpInline(new Regex(@"(?<csharp>(@[^\s](?:[^\(\)]|(?<open>\()|(?<-open>\)))+(?(open)(?!))\)))", RegexOptions.Compiled), new Regex(@"(@\()(?<csharp>(.*))(\))", RegexOptions.Compiled))
            .RequiresTidyup(".")
            .Build();
    }
}
