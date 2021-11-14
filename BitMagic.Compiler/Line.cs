using BitMagic.Common;
using CodingSeb.ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Compiler
{
    public interface ILine
    {
        byte[] Data { get; }
        string OriginalText { get; }
        int Address { get; }
        bool RequiresReval { get; }
        void ProcessParts(bool finalParse);
        void WriteToConsole();
    }

    public class DataLine : ILine
    {
        public byte[] Data { get; private set; } = new byte[] { };
        public string OriginalText { get; }
        public int Address { get; }
        public bool RequiresReval { get; private set; }
        private Procedure _procedure { get; }

        internal DataLine(Procedure proc, string originalText, int address)
        {
            OriginalText = originalText;
            Address = address;
            _procedure = proc;
        }

        private enum LineType
        {
            IsByte,
            IsWord
        }

        public void ProcessParts(bool finalParse)
        {
            var a = 0;
            var data = new List<byte>();

            var toProcess = OriginalText.Trim().ToLower();
            var lineType = toProcess.StartsWith(".byte") ? LineType.IsByte : LineType.IsWord;

            toProcess = toProcess.Substring(5);

            var idx = toProcess.IndexOf(';');
            if (idx != -1)
                toProcess = toProcess.Substring(0, idx);

            Line._evaluator.PreEvaluateVariable += _evaluator_PreEvaluateVariable;
            var rawResult = Line._evaluator.Evaluate($"Array({toProcess})");
            Line._evaluator.PreEvaluateVariable -= _evaluator_PreEvaluateVariable;

            var result = rawResult as object[];

            if (result == null)
                throw new Exception($"Expected object[] back, actually have {rawResult.GetType().Name}");

            foreach (var r in result) 
            {
                var i = r as int?;
                
                if (i == null)
                    throw new Exception($"Expected value back, actually have {r.GetType().Name} for {r}");

                if (lineType == LineType.IsByte)
                {
                    data.Add((byte)(i & 0xff));
                } 
                else
                {
                    var us = (ushort)i;

                    data.Add((byte)(us & 0xff));
                    data.Add((byte)((us & 0xff00) >> 8));
                }
            }

            Data = data.ToArray();
        }

        private void _evaluator_PreEvaluateVariable(object? sender, VariablePreEvaluationEventArg e)
        {
            if (_procedure.Variables.TryGetValue(e.Name, out var result))
            {
                e.Value = result;
                RequiresReval = false;
            }
            else
            {
                RequiresReval = true;
                e.Value = 0xaaaa; // random two byte number
            }
        }

        public void WriteToConsole()
        {
            Console.WriteLine($"${Address:X4}:\t{string.Join(", ", Data.Select(a => $"${a:X2}")),-22}");
        }

    }

    public class Line : ILine
    {
        public readonly static ExpressionEvaluator _evaluator = new();

        public byte[] Data { get; internal set; } = new byte[] { };
        public string OriginalText { get; }
        private ICpuOpCode _opCode;
        public bool RequiresReval { get; internal set; }
        private Procedure _procedure { get; }
        private string _toParse { get; set; }
        public string Params { get; }
        public int Address { get; }

        internal Line(ICpuOpCode opCode, string line, Procedure proc, int address, string[] parts)
        {
            OriginalText = line;
            _procedure = proc;
            _opCode = opCode;
            _toParse = string.Concat(parts).Replace(" ", "");
            Params = _toParse;
            Address = address;
        }

        public void ProcessParts(bool finalParse)
        {
            // take the input and remove all spaces. trim down the initial # and ( X.Y) if necessary.
            // can then parse the expression.

            if (_toParse == "")
            {
                AccessMode? code = _opCode.Modes.FirstOrDefault(o => o is AccessMode.Implied or AccessMode.Accumulator);
                if (code != null)
                {
                    Data = new[] { _opCode.GetOpCode(code.Value) };
                    return;
                } 
                else
                {
                    throw new Exception($"OpCode expects parameters, none found. {_opCode.Code}");
                }
            }

            var idx = _toParse.IndexOf(';');
            if (idx != -1)
                _toParse = _toParse.Substring(idx);

            var isValue = _toParse.StartsWith('#');

            if (isValue)
                _toParse = _toParse.Substring(1);

            var isIndiect = _toParse.StartsWith('(') && _toParse.EndsWith(')');
            if (isIndiect)
            {
                _toParse = _toParse.Substring(1, _toParse.Length - 2);
            }
            var isX = _toParse.EndsWith(",X");
            var isY = _toParse.EndsWith(",Y");

            if (isY || isX)
            {
                _toParse = _toParse.Substring(0, _toParse.Length - 2);
            }

            _evaluator.PreEvaluateVariable += _evaluator_PreEvaluateVariable;
            var result = (int)_evaluator.Evaluate(_toParse);
            _evaluator.PreEvaluateVariable -= _evaluator_PreEvaluateVariable;

            var singleByte = (result & 0xff) == result;
            var isRelative = _opCode.Modes.Count() == 1 && _opCode.Modes.FirstOrDefault() == AccessMode.Relative;

            var thisMode = (isValue, isIndiect, isX, isY, singleByte, isRelative) switch
            {
                (true, false, false, false, true, false) => AccessMode.Immediate,

                (false, false, false, false, true, false) => AccessMode.ZeroPage,
                (false, false, true, false, true, false) => AccessMode.ZeroPageX,
                (false, false, false, true, true, false) => AccessMode.ZeroPageY,

                (false, false, false, false, false, false) => AccessMode.Absolute,
                (false, false, true, false, false, false) => AccessMode.AbsoluteX,
                (false, false, false, true, false, false) => AccessMode.AbsoluteY,

                (false, true, false, false, false, false) => AccessMode.Indirect,
                (false, true, true, false, true, false) => AccessMode.ZeroPageX,
                (false, true, false, true, true, false) => AccessMode.ZeroPageY,

                (false, true, true, false, false, false) => AccessMode.IndAbsoluteX,
                (false, false, false, false, false, true) => AccessMode.Relative,

                _ => throw new Exception($"Unhandled mode select for {_toParse}")
            };

            if (thisMode == AccessMode.Relative)
            {
                var offset = result - Address - 2; // -2 for the branch command
                Data = new[] { _opCode.GetOpCode(thisMode), (byte)offset };
            }
            else if (singleByte)
            {
                Data = new[] { _opCode.GetOpCode(thisMode), (byte)result };
            } 
            else 
            {
                Data = new[] { _opCode.GetOpCode(thisMode), (byte)(result & 0xff), (byte)(result >> 8 & 0xff)};
            }

            if (finalParse && RequiresReval)
                throw new Exception($"Unknown label within '{_toParse}'");
        }

        private void _evaluator_PreEvaluateVariable(object? sender, VariablePreEvaluationEventArg e)
        {
            if (_procedure.Variables.TryGetValue(e.Name, out var result))
            {
                e.Value = result;
                RequiresReval = false;
            }
            else
            {                
                RequiresReval = true;
                e.Value = 0xabcd; // random two byte number
            }
        }

        public void WriteToConsole()
        {
            Console.Write($"${Address:X4}:{(RequiresReval ? "* " : "  ")}{string.Join(", ", Data.Select(a => $"${a:X2}")),-22}");
            Console.WriteLine($"{_opCode.Code}\t{Params}");
        }
    }
}
