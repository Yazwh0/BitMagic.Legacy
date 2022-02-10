using BitMagic.Common;
using CodingSeb.ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Compiler
{

    public class Line : ILine
    {
        public readonly static ExpressionEvaluator _evaluator = new();

        public byte[] Data { get; internal set; } = new byte[] { };
        private ICpuOpCode _opCode;
        public bool RequiresReval { get; internal set; }
        public List<string> RequiresRevalNames { get; } = new List<string>();
        private Procedure _procedure { get; }
        private string _toParse { get; set; }
        private string _original { get; set; }
        public SourceFilePosition Source { get; }
        public string Params { get; }
        public int Address { get; }

        internal Line(ICpuOpCode opCode, SourceFilePosition source, Procedure proc, int address, string[] parts)
        {
            _procedure = proc;
            _opCode = opCode;
            _toParse = string.Concat(parts).Replace(" ", "");
            _original = _toParse;
            Params = _toParse;
            Address = address;
            Source = source;
        }

        public void ProcessParts(bool finalParse)
        {
            // take the input and remove all spaces. trim down the initial # and ( X.Y) if necessary.
            // can then parse the expression.
            _toParse = _original;
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

            var isY = _toParse.EndsWith(",Y", StringComparison.CurrentCultureIgnoreCase);
            if (isY)
            {
                _toParse = _toParse.Substring(0, _toParse.Length - 2);
            }

            var isIndiect = _toParse.StartsWith('(') && _toParse.EndsWith(')');
            if (isIndiect)
            {
                _toParse = _toParse.Substring(1, _toParse.Length - 2);
            }
            var isX = _toParse.EndsWith(",X", StringComparison.CurrentCultureIgnoreCase);
            var isNowY = _toParse.EndsWith(",Y", StringComparison.CurrentCultureIgnoreCase);
            isY = isY || isNowY;

            if (isNowY || isX)
            {
                _toParse = _toParse.Substring(0, _toParse.Length - 2);
            }

            RequiresRevalNames.Clear();
            _evaluator.PreEvaluateVariable += _evaluator_PreEvaluateVariable;
            var result = (int)_evaluator.Evaluate(_toParse);
            _evaluator.PreEvaluateVariable -= _evaluator_PreEvaluateVariable;

            var singleByte = isValue ? (result > -128 && result < 256) : (result & 0xff) == result;
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
                (false, true, true, false, true, false) => AccessMode.IndirectX,
                (false, true, false, true, true, false) => AccessMode.IndirectY,

                (false, true, true, false, false, false) => AccessMode.IndAbsoluteX,
                (false, false, false, false, false, true) => AccessMode.Relative,
                (false, true, false, false, true, false) => AccessMode.ZeroPageIndirect,

                _ => throw new Exception($"Unhandled mode select for {_original}")
            };

            if (thisMode == AccessMode.Relative)
            {
                var offset = result - Address - 2; // -2 for the branch command
                Data = new[] { _opCode.GetOpCode(thisMode), (byte)offset };
                if ((offset > 128 || offset < -127) && (!RequiresReval || finalParse)) 
                    throw new CompilerBranchToFarException(this, $"Branch too far. Trying to branch {offset} bytes.");
            }
            else if (singleByte)
            {
                Data = new[] { _opCode.GetOpCode(thisMode), (byte)result };
            } 
            else 
            {
                Data = new[] { _opCode.GetOpCode(thisMode), (byte)(result & 0xff), (byte)(result >> 8 & 0xff)};
            }

            // should be handled by the caller.
            // if (finalParse && RequiresReval)
            //    throw new Exception($"Unknown label within '{_toParse}'");
        }

        private void _evaluator_PreEvaluateVariable(object? sender, VariablePreEvaluationEventArg e)
        {
            if (_procedure.Variables.TryGetValue(e.Name, 0, out var result))
            {
                e.Value = result;
                RequiresReval = false;
            }
            else
            {
                RequiresRevalNames.Add(e.Name);
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

    public class CompilerBranchToFarException : CompilerException
    {
        public ILine Line { get; }

        public CompilerBranchToFarException(ILine line, string message) : base(message)
        {
            Line = line;
        }

        public override string ErrorDetail => Line.Source.ToString();
    }
}
