using System.Text;
using System.Text.RegularExpressions;

namespace BitMagic.AsmTemplateEngine
{
    public interface ITemplateEngine
    {
        public string TemplateName { get; }
        public string Process(string input);
    }

    public class TemplateEngine : ITemplateEngine
    {
        private Regex[] _lineParsers = Array.Empty<Regex>();
        private (Regex Search, Regex Substitute)[] _inLineCSharp = Array.Empty<(Regex, Regex)>();
        public string TemplateName { get; }
        public bool RequiresTidyup { get; }
        public string TidyMarker { get; }

        internal TemplateEngine(string name, IEnumerable<Regex> lineParsers, IEnumerable<(Regex Search, Regex Substitute)> inLineParsers,
            bool requiresTidyup = false, string tidyMarker = "")
        {
            TemplateName = name;
            _lineParsers = lineParsers.ToArray();
            _inLineCSharp = inLineParsers.ToArray();
            RequiresTidyup = requiresTidyup;
            TidyMarker = tidyMarker;
        }

        public string Process(string input)
        {
            var lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var sb = new StringBuilder();

            foreach(var line in lines)
            {
                bool matched = false;
                foreach(var r in _lineParsers)
                {
                    var result = r.Match(line);

                    if (result.Success)
                    {
                        var match = result.Groups["line"];

                        if (match.Success)
                        {
                            sb.AppendLine(ProcessAsmLine(match.Value));
                        }
                        // perform change
                        matched = true;
                    } 
                }

                if (!matched)
                {
                    sb.AppendLine(line);
                }

            }

            return sb.ToString();
        }

        public string ProcessAsmLine(string input)
        {
            var output = input;

            foreach(var r in _inLineCSharp)
            {
                output = r.Search.Replace(output, 
                    m => r.Substitute.Replace(m.Value, @"{${csharp}}")
                    );
            }

            //output = output.Replace("\"", "\\\"");

            if (RequiresTidyup)
            {
                output = output.Trim();
                if (!string.IsNullOrEmpty(TidyMarker))
                {
                    var idx = output.IndexOf(TidyMarker);
                    if (idx != -1)
                        output = output[(idx + 1)..];
                }
            }
            else
            {
                if (output == ".")
                    output = "";
            }

            return $"BitMagic.AsmTemplate.Template.WriteLiteral($@\"{output}\");";
        }
    }
}