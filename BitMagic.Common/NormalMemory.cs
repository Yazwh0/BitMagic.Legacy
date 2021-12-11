namespace BitMagic.Common
{
    public abstract class NormalMemory: IMemoryBlock
    {
        public int Length { get; }
        public string Name { get; }
        public IMemory? Memory { get; internal set; }
        public int StartAddress { get; internal set; }

        public NormalMemory(string name, int length)
        {
            Length = length;
            Name = name;
        }

        public virtual void Init(IMemory memory, int startAddress)
        {
            Memory = memory;
            StartAddress = startAddress;
        }
    }
}
