using BitMagic.Common;

namespace BitMagic.Cpu
{
    public interface I6502Flags : IFlags
    {
        bool Negative { get; set; }
        bool Overflow { get; set; }
        bool Unused { get; }
        bool Break { get; set; }
        bool Decimal { get; set; }
        bool InterruptDisable { get; set; }
        bool Zero { get; set; }
        bool Carry { get; set; }
        void SetNv(byte value);       
    }
}

