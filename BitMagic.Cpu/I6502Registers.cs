using BitMagic.Common;

namespace BitMagic.Cpu
{
    public interface I6502Registers : IRegisters
    {
        public ushort PC { get; set; }
        public byte S { get; set; }
        public byte P { get; set; }
        public byte A { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public new I6502Flags Flags { get; }
    }
}

