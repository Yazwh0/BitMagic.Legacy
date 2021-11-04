using BitMagic.Common;

namespace BitMagic.Cpu.Memory
{
    public class Ram : IMemory
    {
        public int Length { get; }

        private readonly byte[] _memory;

        public Ram(int length)
        {
            Length = length;
            _memory = new byte[length];
        }

        public byte GetByte(int address) => _memory[address];
        public void SetByte(int address, byte value)
        {
            _memory[address] = value;
        }
        public byte GetDebugByte(int address) => GetByte(address);
        public void SetDebugByte(int address, byte value) => SetByte(address, value);
    }
}
