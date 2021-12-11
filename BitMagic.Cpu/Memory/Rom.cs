using BitMagic.Common;

namespace BitMagic.Cpu.Memory
{
    public class Rom : NormalMemory
    {
        private byte[]? _data;

        public Rom(string name, int length, byte[]? data) : base(name, length)
        {
            _data = data;
        }

        public override void Init(IMemory memory, int startAddress)
        {
            base.Init(memory, startAddress);

            for(var i = 0; i < Length; i++)
            {
                memory.WriteNotification[startAddress + i] = BlockWrite;
                if (_data != null)
                    memory.Memory[startAddress + i] = _data[i];
            }
        }

        private void BlockWrite(int address, byte value)
        {
        }
    }
}
