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

        private Dictionary<string, Action<string, CompileState>> _lineProcessor = new Dictionary<string, Action<string, CompileState>>();
        private Action<string, CompileState>? _labelProcessor;

        private CommandParser()
        {
        }

        public static CommandParser Parser()
        {
            return new CommandParser();
        }

        public CommandParser WithParameters(string verb, Action<IDictionary<string, string>, CompileState> action, IList<string>? defaultNames = null)
        {
            _lineProcessor.Add(verb, (l, s) => ProcesParameters(l, s, action, defaultNames));
            return this;
        }

        public CommandParser WithLine(string verb, Action<string, CompileState> action) 
        {
            _lineProcessor.Add(verb, (l, s) => ProcessLine(l, s, action));
            return this;
        }
        public CommandParser WithLabel(Action<string, CompileState> action) 
        {
            _labelProcessor = action;
            return this;
        }

        public void Process(string line, CompileState state)
        {
            if (string.IsNullOrEmpty(line))
                return;

            var result = _firstWord.Match(line);

            if (!result.Success)
            {
                throw new Exception($"Cannot find verb in {line}");
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
                throw new Exception($"Unknown verb {thisVerb}");

            var map = _lineProcessor[thisVerb];

            map(toProcess, state);
        }

        private static void ProcesParameters(string line, CompileState state, Action<IDictionary<string, string>, CompileState> action, IList<string>? defaultNames)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(line))
            {
                action(parameters, state);
                return;
            }

            var defaultPos = 0;

            var thisArgs = line.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            for (var argsPos = 0; argsPos < thisArgs.Length; argsPos++)
            {
                if (string.IsNullOrWhiteSpace(thisArgs[argsPos]))
                    continue;

                if (thisArgs[argsPos].StartsWith(';'))
                    break;

                var idx = thisArgs[argsPos].IndexOf('=');

                if (idx == -1)
                {
                    if (defaultNames == null || defaultPos > defaultNames.Count)
                        throw new Exception($"Unknown parameter {thisArgs[argsPos]} on line {line}");

                    parameters.Add(defaultNames[defaultPos++], thisArgs[argsPos]);
                    continue;
                }

                parameters.Add(thisArgs[argsPos][..idx].Trim(), thisArgs[argsPos][(idx+1)..].Trim());
            }

            action(parameters, state);
        }

        private static void ProcessLine(string line, CompileState state, Action<string, CompileState> action) => action(line, state);

        private static void ProcessLabel(string line, CompileState state, Action<string, CompileState> action) => action(line, state);


/*        private static void Parse(string args,
            IList<(string verb, Action<IDictionary<string, string>> processor, IList<string> defaultParameters, bool hasParameters)> maps,
            Action<string> label)
        {
            
        }*/
    }



}
