namespace BitMagic.Compiler.Cpu
{
    public class ParametersDefinitionEmpty : ParametersDefinitionSingle
    {
        internal override string GetParameter(string parameters) => parameters;

        internal override bool Valid(string parameters) => string.IsNullOrWhiteSpace(parameters);
    }
}
