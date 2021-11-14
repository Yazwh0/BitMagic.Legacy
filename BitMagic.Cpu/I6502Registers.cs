using BitMagic.Common;

namespace BitMagic.Cpu
{
    public interface I6502Registers : IRegisters
    {
        ushort PC { get; set; }
        byte S { get; set; }
        byte P { get; set; }
        byte A { get; set; }
        byte X { get; set; }
        byte Y { get; set; }
        new I6502Flags Flags { get; }
    }
}

