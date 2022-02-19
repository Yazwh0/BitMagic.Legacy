using Newtonsoft.Json;
using System.Collections.Generic;

namespace BitMagic.Compiler
{
    internal class Segment
    {
        [JsonProperty]
        public string Name { get; set; } = "";

        [JsonProperty]
        public int StartAddress { get; set; }

        [JsonProperty]
        public Variables Variables { get; }

        [JsonProperty]
        public string? Filename { get; set; }

        [JsonProperty]
        public int Address { get; set; }

        [JsonProperty]
        public int MaxSize { get; set; }

        [JsonProperty]
        public readonly Dictionary<string, Procedure> Procedures = new Dictionary<string, Procedure>();


        public Segment(Variables globals, string name)
        {
            Name = name;
            Variables = new Variables(globals, name);
            globals.RegisterChild(Variables);
        }

        public Segment(Variables globals, bool anonymous, int startAddress, string name, string? filename = null)
        {
            Variables = new Variables(globals, name);

            globals.RegisterChild(Variables);

            StartAddress = startAddress;
            Address = startAddress;
            Name = name;
            Filename = filename;
        }

        public Procedure GetProcedure(string name, Scope scope, bool anonymous)
        {
            if (!Procedures.ContainsKey(name))
                Procedures.Add(name, new Procedure(scope, name, anonymous));

            return Procedures[name];
        }
    }
}
