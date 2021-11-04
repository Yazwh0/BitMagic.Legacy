using BitMagic.Common;

namespace BitMagic.Cpu.Memory
{
    public class Rom: IMemory
    {
        public int Length { get; }

        private readonly byte[] _memory;

        public Rom(int length, byte[]? image = null)
        {
            Length = length;
            _memory = image ?? new byte[length];
        }

        public byte GetByte(int address) => _memory[address];
        public void SetByte(int address, byte value)
        {
        }

        public byte GetDebugByte(int address) => GetByte(address);
        public void SetDebugByte(int address, byte value) => SetByte(address, value);
    }
}
