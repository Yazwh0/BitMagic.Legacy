using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BitMagic.AsmTemplateEngine
{
    public interface ITemplateEngineBuilder
    {
        public ITemplateEngineBuilder WithAsmLine(Regex search);
        public ITemplateEngineBuilder WithCSharpInline(Regex search, Regex substitue);
        public ITemplateEngine Build();
    }

    public static class TemplateEngineBuilder
    {
        public static ITemplateEngineBuilder WithCSharp(Regex search, Regex substitue)
        {
            var toReturn = new TemplateEngineBuilderStep();
            toReturn._csharpLines.Add((search, substitue));
            return toReturn;
        }
        public static ITemplateEngineBuilder WithAsmLine(Regex search)
        {
            var toReturn = new TemplateEngineBuilderStep();
            toReturn._asmLines.Add(search);
            return toReturn;
        }
    }

    public class TemplateEngineBuilderStep : ITemplateEngineBuilder
    {
        internal List<Regex> _asmLines = new List<Regex>();
        internal List<(Regex Seach, Regex Subtituet)> _csharpLines = new List<(Regex Search, Regex Substitue)>();

        public ITemplateEngineBuilder WithAsmLine(Regex search)
        {
            _asmLines.Add(search);
            return this;
        }

        public ITemplateEngineBuilder WithCSharpInline(Regex search, Regex substitue)
        {
            _csharpLines.Add((search, substitue));
            return this;
        }

        public ITemplateEngine Build()
        {
            return new TemplateEngine(_asmLines, _csharpLines);
        }
    }
}
