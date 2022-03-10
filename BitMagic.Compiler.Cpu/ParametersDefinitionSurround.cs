namespace BitMagic.Compiler.Cpu
{
    public class ParametersDefinitionSurround : ParametersDefinitionSingle
    {
        public string StartsWith { get; init; } = "";
        public string EndsWith { get; init; } = "";

        internal override string GetParameter(string parameters) =>
            parameters[StartsWith.Length..(parameters.Length - EndsWith.Length)];

        internal override bool Valid(string parameters) => 
            parameters.StartsWith(StartsWith, System.StringComparison.InvariantCultureIgnoreCase) &&
            parameters.EndsWith(EndsWith, System.StringComparison.InvariantCultureIgnoreCase) &&
            !string.IsNullOrWhiteSpace(parameters);
    }
}
