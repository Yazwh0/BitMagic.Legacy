

namespace BitMagic.Common
{
    public interface IMachine
    {
        public ICpu Cpu { get; }
        public string Name { get; }
    }
}
