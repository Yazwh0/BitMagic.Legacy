

namespace BitMagic.Common
{
    public interface IMachine
    {
        public IMemory Memory { get; }
        public ICpu Cpu { get; }
        public string Name { get; }
    }
}
