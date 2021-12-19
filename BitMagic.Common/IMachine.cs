

using System.Collections.Generic;

namespace BitMagic.Common
{
    public interface IMachine
    {
        public IMemory Memory { get; }
        public ICpu Cpu { get; }
        public string Name { get; }
        public IDisplay Display { get; }
        public IVariables Variables { get; }
    }

    public interface IVariables
    {
        IReadOnlyDictionary<string, int> Values { get; }
    }
}
