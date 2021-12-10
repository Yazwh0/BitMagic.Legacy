using BitMagic.Common;

namespace BitMagic.Cpu.Memory
{
    // we let the map do all the work for normal RAM.
    public class Ram : NormalMemory
    {
        public Ram(string name, int length) : base(name, length)
        {
        }
    }
}
