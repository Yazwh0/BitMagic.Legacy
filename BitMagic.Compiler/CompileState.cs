using BitMagic.Common;
using BitMagic.Compiler.Warnings;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BitMagic.Compiler
{
    internal class CompileState
    {
        [JsonProperty]
        public Dictionary<string, Segment> Segments { get; set; } = new Dictionary<string, Segment>();

        [JsonProperty]
        internal Segment Segment { get; set; }
        [JsonProperty]
        internal Scope Scope { get; set; }
        [JsonProperty]
        internal Procedure Procedure { get; set; }
        [JsonProperty]
        internal Variables Globals { get; }
        [JsonProperty]
        internal int AnonCounter { get; set; }
        [JsonProperty]
        public List<string> Files { get; set; } = new List<string>();

        [JsonProperty]
        internal ScopeFactory ScopeFactory { get; set; }
        internal IExpressionEvaluator Evaluator { get; set; }

        internal readonly Queue<Scope> _scopeQueue = new Queue<Scope>();

        public List<CompilerWarning> Warnings = new List<CompilerWarning>();
        public CompileState(Variables globals, string defaultFileName)
        {
            Globals = globals;
            ScopeFactory = new ScopeFactory(Globals);
            Evaluator = new ExpressionEvaluator(this);

            var segment = new Segment(Globals, true, 0x801, "Main", defaultFileName);
            var scope = ScopeFactory.GetScope($"Main");
            var procedure = segment.GetDefaultProcedure(scope);

            Segments.Add(segment.Name, segment);
            Segment = segment;
            Scope = scope;
            Procedure = procedure;
        }
    }
}
