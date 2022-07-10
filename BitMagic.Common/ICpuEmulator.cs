using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BitMagic.Common
{
    // for compiler
    public interface ICpu
    { 
        public string Name { get; }
        IEnumerable<ICpuOpCode> OpCodes { get; }
        public int OpCodeBytes { get; }
        IReadOnlyDictionary<AccessMode, IParametersDefinition> ParameterDefinitions { get; }
    }

    public interface IParametersDefinition
    {
        AccessMode AccessMode { get; }
        (byte[]? Data, bool RequiresRecalc) Compile(string parameters, IOutputData line, ICpuOpCode opCode, IExpressionEvaluator expressionEvaluator, IVariables variables, bool final);
        (int BytesUsed, string DecompiledCode) Decompile(IEnumerable<byte> inputBytes);
        int Order { get; }
    }

    public enum ParameterSize
    {
        None,
        Bit8,
        Bit16,
        Bit32
    }

    public interface IExpressionEvaluator
    {
        public void Reset();
        public (int Result, bool RequiresRecalc) Evaluate(string expression, IVariables variables, ParameterSize size);
    }

    public interface ICpuEmulator : ICpu
    {
        void Reset();
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
        // todo: change to byte[] or similar
        uint GetOpCode(AccessMode mode);
        // number of bytes
        int OpCodeLength { get; }
        public IEnumerable<AccessMode> Modes { get; }
    }

    public interface IEmulatableCpuOpCode<TCpu> : ICpuOpCode
    {
        public abstract int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, TCpu cpu);
    }

    public enum AccessMode
    {
        Implied,
        Accumulator,     // A
        Immediate,       // #$44
        ZeroPage,        // $44
        ZeroPageX,       // $44, X
        ZeroPageY,       // $44, Y
        Absolute,        // $4400
        AbsoluteX,       // $4400, X        
        AbsoluteY,       // $4400, Y
        Indirect,        // ($4444)
        IndirectX,       // ($44, X)
        IndirectY,       // ($44), Y
        IndAbsoluteX,    // ($4444, X)
        Relative,        // #$ff for branch instruction,
        ZeroPageIndirect, // ($44)
        ZerpPageRel,     // zp, $rr
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
