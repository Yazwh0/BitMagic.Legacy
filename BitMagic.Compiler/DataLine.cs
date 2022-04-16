using BitMagic.Common;
using CodingSeb.ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BitMagic.Compiler
{
    public class DataLine : IOutputData
    {
        public byte[] Data { get; private set; } = new byte[] { };
/*        public string OriginalText { get; }
        public int LineNumber { get; }*/
        public int Address { get; }
        public bool RequiresReval { get; private set; }
        public List<string> RequiresRevalNames { get; } = new List<string>();
        private Procedure _procedure { get; }
        private LineType _lineType { get; }
        public SourceFilePosition Source { get; }

        internal DataLine(Procedure proc, SourceFilePosition source, int address, LineType type)
        {
            Source = source;
//            OriginalText = originalText;
            Address = address;
            _procedure = proc;
            _lineType = type;
//            LineNumber = lineNumber;
        }

        internal enum LineType
        {
            IsByte,
            IsWord
        }

        public void ProcessParts(bool finalParse)
        {
            var data = new List<byte>();

            var toProcess = Source.Source.Trim().ToLower();

            var idx = toProcess.IndexOf(';');
            if (idx != -1)
                toProcess = toProcess.Substring(0, idx);

            idx = toProcess.IndexOf('.');

            if (idx == -1)
            {
                throw new Exception("Cannot find data on the line");
            }

            toProcess = toProcess.Substring(idx + 5).Trim();

            RequiresRevalNames.Clear();
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
                    throw new Exception($"Expected int? value back, actually have {r.GetType().Name} for {r}");

                if (_lineType == LineType.IsByte)
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
            if (_procedure.Variables.TryGetValue(e.Name, Source.LineNumber, out var result))
            {
                e.Value = result;
                RequiresReval = false;
            }
            else
            {
                RequiresRevalNames.Add(e.Name);
                RequiresReval = true;
                e.Value = 0xaaaa; // random two byte number
            }
        }

        public void WriteToConsole()
        {
            Console.WriteLine($"${Address:X4}:\t{string.Join(", ", Data.Select(a => $"${a:X2}")),-22}");
        }
    }
}
