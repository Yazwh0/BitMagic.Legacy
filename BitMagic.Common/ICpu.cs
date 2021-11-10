using System.Collections.Generic;

namespace BitMagic.Common
{
    public interface ICpu
    {
        IEnumerable<ICpuOpCode> OpCodes { get; }
        public IRegisters Registers { get; }
        public void SetProgramCounter(int address);
        public int ClockTick(IMemory memory);
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

    public interface IFlags
    {
        public int NumFlags { get; }
        public bool GetFlag(int index);
        public void SetFlag(int index, bool value);
        public string GetFlagName(int index);
        public byte Register { get; set; }
    }

    public interface IRegisters
    {
        public int NumRegisters { get; }
        public byte GetRegister(int index);
        public void SetRegister(int index, byte value);
        public string GetRegisterName(int index);
        public IFlags Flags {get;}
    }
}
