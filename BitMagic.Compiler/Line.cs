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

        private readonly ICpu _cpu;
        private readonly IExpressionEvaluator _expressionEvaluator;           

        internal Line(ICpuOpCode opCode, SourceFilePosition source, Procedure proc, ICpu cpu, IExpressionEvaluator expressionEvaluator, int address, string[] parts)
        {
            _cpu = cpu;
            _expressionEvaluator = expressionEvaluator;
            _procedure = proc;
            _opCode = opCode;
            _toParse = string.Concat(parts).Replace(" ", "");
            _original = _toParse;
            Params = _toParse;
            Address = address;
            Source = source;
        }

        private IEnumerable<byte> IntToByteArray(uint i)
        {
            byte toReturn = (byte)((i & 0xff000000) >> 24);

            if (toReturn != 0)
                yield return toReturn;

            toReturn = (byte)((i & 0xff0000) >> 16);

            if (toReturn != 0)
                yield return toReturn;

            toReturn = (byte)((i & 0xff00) >> 8);

            if (toReturn != 0)
                yield return toReturn;

            yield return (byte)(i & 0xff);
        }

        public void ProcessParts(bool finalParse)
        {
            //(byte[]? Data, bool RequiresReacalt, AccessMode AccessMode)? testResult = null;

            foreach (var i in _opCode.Modes.Where(i => _cpu.ParameterDefinitions.ContainsKey(i)).Select(i => _cpu.ParameterDefinitions[i]).OrderBy(i => i.Order))
            {
                try
                {
                    var compileResult = i.Compile(Params, this, _opCode, _expressionEvaluator, _procedure.Variables, finalParse);
                    if (compileResult.Data != null)
                    {
                        RequiresReval = compileResult.RequiresRecalc;

                        if (finalParse && RequiresReval)
                            throw new Exception($"Unknown label within '{_toParse}'");

                        Data = IntToByteArray(_opCode.GetOpCode(i.AccessMode)).Union(compileResult.Data).ToArray();
                        return;
                    }
                } 
                catch(ExpressionEvaluatorSyntaxErrorException ex)
                {
                    throw new UnknownSymbolException(this, ex.Message);
                }
            }

            throw new Exception($"Cannot compile line {_original}");
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

    public class UnknownSymbolException : CompilerException
    {
        public ILine Line { get; }

        public UnknownSymbolException(ILine line, string message) : base(message)
        {
            Line = line;
        }

        public override string ErrorDetail => Line.Source.ToString();
    }

}
