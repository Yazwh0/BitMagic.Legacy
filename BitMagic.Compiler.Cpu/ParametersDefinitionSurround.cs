namespace BitMagic.Compiler.Cpu
{
    public class ParametersDefinitionSurround : ParametersDefinitionSingle
    {
        public string StartsWith { get; init; } = "";
        public string EndsWith { get; init; } = "";

        public string[] DoesntStartWith { get; init; } = new string[0];
        public string[] DoesntEndWith { get; init; } = new string[0];

        internal override string GetParameter(string parameters) =>
            parameters[StartsWith.Length..(parameters.Length - EndsWith.Length)];

        internal override bool Valid(string parameters)
        {
            if (!(parameters.StartsWith(StartsWith, System.StringComparison.InvariantCultureIgnoreCase) &&
                parameters.EndsWith(EndsWith, System.StringComparison.InvariantCultureIgnoreCase) &&
                !string.IsNullOrWhiteSpace(parameters)))
            {
                return false;
            }

            foreach(var s in DoesntStartWith)
            {
                if (parameters.StartsWith(s, System.StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }

            foreach (var s in DoesntEndWith)
            {
                if (parameters.EndsWith(s, System.StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }

            return true;
        }
    }
}
