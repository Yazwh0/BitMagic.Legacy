using System.Collections.Generic;

namespace BitMagic.Common
{
    public interface ICpu
    {
        IEnumerable<ICpuOpCode> OpCodes { get; }
        IRegisters Registers { get; }
        void SetProgramCounter(int address);
        int ClockTick(IMemory memory, bool debugOutput);
        double Frequency { get; }
        void SetInterrupt();
        bool HasInterrupt { get; }
        int HandleInterrupt(IMemory memory);
    }

    public interface ICpuOpCode
    {
        string Code { get; } // not unique
        byte GetOpCode(AccessMode mode);
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
        int NumFlags { get; }
        bool GetFlag(int index);
        void SetFlag(int index, bool value);
        string GetFlagName(int index);
        byte Register { get; set; }
    }

    public interface IRegisters
    {
        int NumRegisters { get; }
        byte GetRegister(int index);
        void SetRegister(int index, byte value);
        string GetRegisterName(int index);
        IFlags Flags {get;}
    }
}
