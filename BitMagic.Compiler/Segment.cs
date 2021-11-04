﻿using Newtonsoft.Json;
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
        public readonly Dictionary<string, Scope> Scopes = new Dictionary<string, Scope>();
        public int Address { get; set; }

        public Segment(Variables globals, bool anonymous, int startAddress, string name)
        {
            Variables = anonymous ? globals : new Variables(globals);
            StartAddress = startAddress;
            Address = startAddress;
            Name = name;
        }

        public Scope GetScope(string name, bool anonymous)
        {
            if (!Scopes.ContainsKey(name))
                Scopes.Add(name, new Scope(this, name, anonymous));

            return Scopes[name];
        }
    }
}
