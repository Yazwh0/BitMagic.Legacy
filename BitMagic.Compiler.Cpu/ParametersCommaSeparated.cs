using BitMagic.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Compiler.Cpu
{
    public class ParametersCommaSeparated : IParametersDefinition
    {
        public AccessMode AccessMode { get; init; }

        public int Order { get; init; }

        public ParametersDefinitionSingle? Left { get; init; }
        public ParametersDefinitionSingle? Right { get; init; }

        public (byte[]? Data, bool RequiresRecalc) Compile(string parameters, IOutputData line, ICpuOpCode opCode, IExpressionEvaluator expressionEvaluator, IVariables variables, bool final)
        {
            if (string.IsNullOrWhiteSpace(parameters))
                return (null, false);

            if (Left == null || Right == null)
                return (null, false);

            var split = parameters.Split(',');
            
            if (split.Length != 2)
                return (null, false);

            if (!Left.Valid(split[0]) || !Right.Valid(split[1]))
                return (null, false);

            var left = Left.Compile(split[0], line, opCode, expressionEvaluator, variables, final);
            var right = Right.Compile(split[1], line, opCode, expressionEvaluator, variables, final);

            if (left.Data == null || right.Data == null)
                return (null, false);

            var requiresRecalc = left.RequiresRecalc || right.RequiresRecalc;
            var data = left.Data.Concat(right.Data).ToArray();

            return (data, requiresRecalc);
        }

        public (int BytesUsed, string DecompiledCode) Decompile(IEnumerable<byte> inputBytes)
        {
            throw new NotImplementedException();
        }
    }
}
