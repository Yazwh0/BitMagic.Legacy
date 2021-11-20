using BitMagic.Common;

namespace BitMagic.Cpu.Memory
{
    public class Rom: NormalMemory
    {
        public override int Length { get; }

        public Rom(int length, byte[]? image = null)
        {
            Length = length;
            Memory = image ?? new byte[length];
        }

        public override void SetByte(int address, byte value)
        {
        }
    }
}
