using Newtonsoft.Json;
using System.Collections.Generic;

namespace BitMagic.Compiler
{
    internal class Scope
    {
        [JsonProperty]
        public string Name { get; }
        [JsonProperty]
        public Variables Variables { get; }
        [JsonProperty]
        public readonly Dictionary<string, Procedure> Procedures = new Dictionary<string, Procedure>();

        public Scope(Segment segment, string name, bool anonymous)
        {
            Variables = anonymous ? segment.Variables : new Variables(segment.Variables);
            Name = name;
        }
        public Procedure GetProcedure(string name, bool anonymous)
        {
            if (!Procedures.ContainsKey(name))
                Procedures.Add(name, new Procedure(this, name, anonymous));

            return Procedures[name];
        }
    }
}
