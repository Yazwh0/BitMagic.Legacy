using Newtonsoft.Json;
using System.Collections.Generic;

namespace BitMagic.Compiler
{
    internal class ScopeFactory
    {
        private readonly Dictionary<string, Scope> _scopes = new Dictionary<string, Scope>();
        private readonly Variables _global;
        public ScopeFactory(Variables global)
        {
            _global = global;
        }

        public Scope GetScope(string name)
        {
            if (!_scopes.ContainsKey(name))
                _scopes.Add(name, new Scope(name, _global));

            return _scopes[name];
        }
    }

    internal class Scope
    {
        [JsonProperty]
        public string Name { get; }
        [JsonProperty]
        public Variables Variables { get; }

        public Scope(string name, Variables globals)
        {
            Name = name;
            Variables = new Variables(globals, name);
        }
    }
}
