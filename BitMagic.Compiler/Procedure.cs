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
            Name = name;
            Variables = anonymous ? scope.Variables : new Variables(scope.Variables, name);
            
            if (!anonymous)
                scope.Variables.RegisterChild(Variables);
        }

        public void AddLine(ILine line)
        {
            Data.Add(line);
        }
    }
}
