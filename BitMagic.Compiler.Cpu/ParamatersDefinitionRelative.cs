using BitMagic.Common;

namespace BitMagic.Compiler.Cpu
{
    public class ParamatersDefinitionRelative : ParametersDefinitionSurround
    {
        public int Offset { get; init; } = -1;

        public override (byte[]? Data, bool RequiresRecalc) Compile(string parameters, IOutputData line, ICpuOpCode opCode, IExpressionEvaluator expressionEvaluator, IVariables variables, bool final)
        {
            if (!Valid(parameters))
                return (null, false);

            if (string.IsNullOrWhiteSpace(parameters))
                return (null, false);

            var toParse = GetParameter(parameters);

            var (Result, RequiresRecalc) = expressionEvaluator.Evaluate(toParse, variables, ParameterSize);

            var offset = Result - line.Address - opCode.OpCodeLength + Offset;

            if ((ParameterSize == ParameterSize.Bit8 && (offset < sbyte.MinValue || offset > sbyte.MaxValue)) ||
                (ParameterSize == ParameterSize.Bit16 && (offset < short.MinValue || offset > short.MaxValue)) ||
                (ParameterSize == ParameterSize.Bit32 && (offset < int.MinValue || offset > int.MaxValue)))
            {
                // todo: throw proper exception?
                if (final)
                    return (null, false);

                offset = 0;
                RequiresRecalc = true;
            }

            return ParameterSize switch
            {
                ParameterSize.Bit8 => (new byte[] { (byte)offset }, RequiresRecalc),
                ParameterSize.Bit16 => (new byte[] { (byte)(offset & 0xff), (byte)((offset >> 8) & 0xff) }, RequiresRecalc),
                ParameterSize.Bit32 => (new byte[] { (byte)(offset & 0xff), (byte)((offset >> 8) & 0xff), (byte)((offset >> 16) & 0xff), (byte)((offset >> 24) & 0xff) }, RequiresRecalc),
                _ => throw new System.Exception($"Unknown ParametersSize {ParameterSize}")
            };
        }

        public new (int BytesUsed, string DecompiledCode) Decompile(IEnumerable<byte> inputBytes)
        {
            throw new System.NotImplementedException();
        }
    }
}
