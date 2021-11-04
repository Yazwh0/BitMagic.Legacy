using Newtonsoft.Json;

namespace BitMagic.Compiler
{
    internal class CompileState
    {
        [JsonProperty]
        internal Segment Segment { get; set; }
        [JsonProperty]
        internal Scope Scope { get; set; }
        [JsonProperty]
        internal Procedure Procedure { get; set; }

        public CompileState(Segment segment, Scope scope, Procedure procedure)
        {
            Segment = segment;
            Scope = scope;
            Procedure = procedure;
        }
    }
}
