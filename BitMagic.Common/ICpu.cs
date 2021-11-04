using System.Collections.Generic;

namespace BitMagic.Common
{
    public interface ICpu
    {
        IEnumerable<ICpuOpCode> OpCodes { get; }
    }

    public interface ICpuOpCode
    {
        public string Code { get; } // not unique
        public byte GetOpCode(AccessMode mode);
        public IEnumerable<AccessMode> Modes { get; }
    }

    public enum AccessMode
    {
        Implied,
        Accumulator,  // A
        Immediate,    // #$44
        ZeroPage,     // $44
        ZeroPageX,    // $44, X
        ZeroPageY,    // $44, Y
        Absolute,     // $4400
        AbsoluteX,    // $4400, X        
        AbsoluteY,    // $4400, Y
        Indirect,     // ($4444)
        IndirectX,    // ($44, X)
        IndirectY,    // ($44), Y
        IndAbsoluteX, // ($4444, X)
        Relative,     // #$ff for branch instruction
    }
}
