using BitMagic.Common;
using BitMagic.Compiler;
using System;
using System.Collections.Generic;

namespace BitMagic.Compiler.Cpu
{
    public abstract class ParametersDefinitionSingle : IParametersDefinition
    {
        public AccessMode AccessMode { get; init; }
        public ParameterSize ParameterSize { get; init; } = ParameterSize.None;

        public int Order { get; init; }
        
        public virtual (byte[]? Data, bool RequiresRecalc) Compile(string parameters, IOutputData line, ICpuOpCode opCode, IExpressionEvaluator expressionEvaluator, IVariables variables, bool final)
        {
            if (!Valid(parameters))
                return (null, false);

            if (string.IsNullOrWhiteSpace(parameters))
                return (Array.Empty<byte>(), false);

            var toParse = GetParameter(parameters);            

            var (Result, RequiresRecalc) = expressionEvaluator.Evaluate(toParse, variables, ParameterSize);

            if ((ParameterSize == ParameterSize.Bit8 && (Result < sbyte.MinValue || Result > byte.MaxValue)) ||
                (ParameterSize == ParameterSize.Bit16 && (Result < short.MinValue || Result > ushort.MaxValue)) ||
                (ParameterSize == ParameterSize.Bit32 && (Result < int.MinValue || Result > int.MaxValue)))
            {
                return (null, false);
            }

            return ParameterSize switch
            {
                ParameterSize.Bit8 => (new byte[] { (byte)Result }, RequiresRecalc),
                ParameterSize.Bit16 => (new byte[] { (byte)(Result & 0xff), (byte)((Result >> 8) & 0xff) }, RequiresRecalc),
                ParameterSize.Bit32 => (new byte[] { (byte)(Result & 0xff), (byte)((Result >> 8) & 0xff), (byte)((Result >> 16) & 0xff), (byte)((Result >> 24) & 0xff) }, RequiresRecalc),
                _ => throw new System.Exception($"Unknown ParametersSize {ParameterSize}")
            };
        }

        public (int BytesUsed, string DecompiledCode) Decompile(IEnumerable<byte> inputBytes)
        {
            throw new System.NotImplementedException();
        }

        internal abstract string GetParameter(string parameters);
        internal abstract bool Valid(string parameters);
    }
}
