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
        public ITemplateEngineBuilder WithUnderlying(Regex search);
        public ITemplateEngineBuilder WithCSharpInline(Regex search, Regex substitue);
        public ITemplateEngine Build();
        public ITemplateEngineBuilder RequiresTidyup(string marker);
    }

    public static class TemplateEngineBuilder
    {
        public static ITemplateEngineBuilder As(string name)
        {
            return new TemplateEngineBuilderStep(name);
        }
    }

    public class TemplateEngineBuilderStep : ITemplateEngineBuilder
    {
        internal List<Regex> _asmLines = new List<Regex>();
        internal List<(Regex Seach, Regex Subtituet)> _csharpLines = new List<(Regex Search, Regex Substitue)>();
        internal string _name;
        internal bool _requiresTidyup = false;
        internal string _tidyMarker = "";

        internal TemplateEngineBuilderStep(string name)
        {
            _name = name;
        }

        public ITemplateEngineBuilder WithUnderlying(Regex search)
        {
            _asmLines.Add(search);
            return this;
        }

        public ITemplateEngineBuilder WithCSharpInline(Regex search, Regex substitue)
        {
            _csharpLines.Add((search, substitue));
            return this;
        }

        public ITemplateEngineBuilder RequiresTidyup(string marker)
        {
            _requiresTidyup = true;
            _tidyMarker = marker;
            return this;
        }

        public ITemplateEngine Build()
        {
            return new TemplateEngine(_name, _asmLines, _csharpLines, _requiresTidyup, _tidyMarker);
        }
    }
}
