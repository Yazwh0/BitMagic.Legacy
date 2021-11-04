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
        public List<Line> Data { get; set; } = new List<Line>();

        public Procedure(Scope scope, string name, bool anonymous)
        {
            Variables = anonymous ? scope.Variables : new Variables(scope.Variables);
            Name = name;
        }

        public void AddLine(Line line)
        {
            Data.Add(line);
        }
    }
}
