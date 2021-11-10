using Newtonsoft.Json;
using System.Collections.Generic;

namespace BitMagic.Compiler
{
    internal class Procedure
    {
        [JsonProperty]
        public string Name { get; }
        [JsonProperty]
        public Variables Variables { get; }
        [JsonProperty]
        public List<ILine> Data { get; set; } = new List<ILine>();

        public Procedure(Scope scope, string name, bool anonymous)
        {
            Variables = anonymous ? scope.Variables : new Variables(scope.Variables);
            Name = name;
        }

        public void AddLine(ILine line)
        {
            Data.Add(line);
        }
    }
}
