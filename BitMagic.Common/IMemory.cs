using System;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Common
{
    public interface IMemory
    {
        int Length { get; }
        byte GetByte(int address);
        byte PeekByte(int address);
        void SetByte(int address, byte value);
        public Memory<byte> MemoryStruct { get; }
        public byte[] Memory { get; }
        public Func<int, byte>[] ReadNotification { get; }
        public Action<int, byte>[] WriteNotification { get; }
    }

    public interface IMemoryBlock
    {
        int Length { get; }
        void Init(IMemory memory, int startAddress);
        string Name { get; }
    }
}
