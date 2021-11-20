using BitMagic.Common;

namespace BitMagic.Cpu.Memory
{
    public class Ram : NormalMemory
    {
        public override int Length { get; }

        public Ram(int length)
        {
            Length = length;
            Memory = new byte[length];
        }
    }
}
