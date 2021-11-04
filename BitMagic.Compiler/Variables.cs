using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BitMagic.Compiler
{
    internal class Variables
    {
        [JsonProperty]
        private readonly Dictionary<string, int> _variables = new Dictionary<string, int>();

        private readonly Variables? _parent;

        public Variables(Variables parent)
        {
            _parent = parent;
        }

        public Variables()
        {
            _parent = null;
        }

        public bool TryGetValue(string name, out int result)
        {
            if (_variables.ContainsKey(name))
            {
                result = _variables[name];
                return true;
            }

            if (_parent != null)
            {
                return _parent.TryGetValue(name, out result);
            }

            result = 0;
            return false;
        }

        public void SetValue(string name, int value)
        {
            if (_variables.ContainsKey("name"))
                throw new Exception($"Variable already defined {name}");

            _variables[name] = value;
        }
    }
}
