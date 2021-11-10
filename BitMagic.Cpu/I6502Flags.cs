using BitMagic.Common;

namespace BitMagic.Cpu
{
    public interface I6502Flags : IFlags
    {
        public bool Negative { get; set; }
        public bool Overflow { get; set; }
        public bool Unused { get; }
        public bool Break { get; set; }
        public bool Decimal { get; set; }
        public bool InterruptDisable { get; set; }
        public bool Zero { get; set; }
        public bool Carry { get; set; }
        public void SetNv(byte value);       
    }
}

