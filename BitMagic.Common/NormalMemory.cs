namespace BitMagic.Common
{
    public abstract class NormalMemory : IMemory
    {
        public abstract int Length { get; }
        public byte[] Memory { get; protected set; }

        public NormalMemory()
        {
            Memory = new byte[Length];
        }

        public virtual byte GetByte(int address) => Memory[address];
        public virtual void SetByte(int address, byte value) => Memory[address] = value;
        public virtual byte PeekByte(int Address) => GetByte(Address);
    }
}
