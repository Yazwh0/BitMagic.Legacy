﻿using Newtonsoft.Json;
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

        public CompileState(Variables globals)
        {
            Globals = globals;
            var segment = new Segment(Globals, true, 0x801, ".Default"); // use '.' so this cant be generated by the user
            var scope = segment.GetScope($".Default_{AnonCounter++}", true);
            var procedure = scope.GetProcedure($".Default_{AnonCounter++}", true);
            Segments.Add(segment.Name, segment);
            Segment = segment;
            Scope = scope;
            Procedure = procedure;
        }
    }
}
