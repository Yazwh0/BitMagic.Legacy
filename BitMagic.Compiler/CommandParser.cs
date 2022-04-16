using BitMagic.Common;
using BitMagic.Compiler.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BitMagic.Compiler
{
    internal class CommandParser
    {
        private Regex _firstWord = new Regex("^\\s*(?<result>([.][\\w\\-:]+))(?<line>(.*))$");

        private Dictionary<string, Action<SourceFilePosition, CompileState, string>> _lineProcessor = new Dictionary<string, Action<SourceFilePosition, CompileState, string>>();
        private Action<string, CompileState>? _labelProcessor;

        private CommandParser()
        {
        }

        public static CommandParser Parser()
        {
            return new CommandParser();
        }

        public CommandParser WithParameters(string verb, Action<IDictionary<string, string>, CompileState, SourceFilePosition> action, IList<string>? defaultNames = null)
        {
            _lineProcessor.Add(verb, (p, s, r) => ProcesParameters(r ,p, s, action, defaultNames));
            return this;
        }

        public CommandParser WithLine(string verb, Action<SourceFilePosition, CompileState> action) 
        {
            _lineProcessor.Add(verb, (p, s, r) => ProcessLine(p, s, action));
            return this;
        }

        public CommandParser WithLabel(Action<string, CompileState> action) 
        {
            _labelProcessor = action;
            return this;
        }

        public void Process(SourceFilePosition source, CompileState state)
        {
            if (string.IsNullOrEmpty(source.Source))
                return;

            var result = _firstWord.Match(source.Source);

            if (!result.Success)
            {
                throw new CompilerVerbException(source, $"Cannot find verb on line.");
            }

            var thisVerb = result.Groups["result"].Value;
            var toProcess = result.Groups["line"].Value;

            if (thisVerb.EndsWith(':'))
            {
                if (_labelProcessor == null)
                    throw new Exception("Label processor is null");

                _labelProcessor(thisVerb, state);
                return;
            }

            if (!_lineProcessor.ContainsKey(thisVerb))
                throw new CompilerVerbException(source, $"Unknown verb '{thisVerb.Substring(1)}'");

            var map = _lineProcessor[thisVerb];

            map(source, state, toProcess);
        }

        private static void ProcesParameters(string rawParams, SourceFilePosition source, CompileState state, Action<IDictionary<string, string>, CompileState, SourceFilePosition> action, IList<string>? defaultNames)
        {
            var parameters = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(rawParams))
            {
                action(parameters, state, source);
                return;
            }

            var defaultPos = 0;

            var thisArgs = rawParams.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            for (var argsPos = 0; argsPos < thisArgs.Length; argsPos++)
            {
                if (string.IsNullOrWhiteSpace(thisArgs[argsPos]))
                    continue;

                if (thisArgs[argsPos].StartsWith(';'))
                    break;

                var idx = thisArgs[argsPos].IndexOf('=');

                if (idx == -1)
                {
                    if (defaultNames == null || defaultPos >= defaultNames.Count)
                        throw new Exception($"Unknown parameter {thisArgs[argsPos]} at {source.ToString()}");

                    parameters.Add(defaultNames[defaultPos++], thisArgs[argsPos]);
                    continue;
                }

                parameters.Add(thisArgs[argsPos][..idx].Trim(), thisArgs[argsPos][(idx+1)..].Trim());
            }

            action(parameters, state, source);
        }

        private static void ProcessLine(SourceFilePosition source, CompileState state, Action<SourceFilePosition, CompileState> action) => action(source, state);

        private static void ProcessLabel(SourceFilePosition source, CompileState state, Action<SourceFilePosition, CompileState> action) => action(source, state);


/*        private static void Parse(string args,
            IList<(string verb, Action<IDictionary<string, string>> processor, IList<string> defaultParameters, bool hasParameters)> maps,
            Action<string> label)
        {
            
        }*/
    }



}
