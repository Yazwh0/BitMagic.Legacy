using BitMagic.Common;
using BitMagic.Compiler.Cpu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BitMagic.Cpu
{
    public interface I6502 : ICpuEmulator
    {
        public new I6502Registers Registers { get; }
        public void Push(byte value);
        public byte Pop();
    }

    public class WDC65c02 : I6502
    {
        public string Name => "WDC65c02";
        public IEnumerable<ICpuOpCode> OpCodes => _opCodes;
        public I6502Registers Registers { get; } = new _6502Registers();
        IRegisters ICpuEmulator.Registers => Registers;
        public IMemory Memory { get; init; }
        public const int _stackStart = 0x100;
        public bool HasInterrupt { get; internal set; }

        public const int InterruptVector = 0xfffe;
        public const int ResetVector = 0xfffc;

        public byte LastOpCode { get; private set; } = 0;

        public int OpCodeBytes => 1;

        public double Frequency { get; }

        private readonly IReadOnlyDictionary<AccessMode, IParametersDefinition> _parameterDefinitions = new IParametersDefinition[] {
             new ParametersCommaSeparated() { AccessMode = AccessMode.ZerpPageRel, Order = 10, 
                 Left = new ParametersDefinitionSurround() {AccessMode = AccessMode.ZeroPage, ParameterSize = ParameterSize.Bit8, Order = 40 },
                 Right = new ParamatersDefinitionRelative() {AccessMode = AccessMode.Relative, ParameterSize = ParameterSize.Bit8, Order = 40 }
             },
             new ParametersDefinitionSurround() {AccessMode = AccessMode.Immediate, StartsWith = "#", ParameterSize = ParameterSize.Bit8, Order = 10 },
             new ParametersDefinitionSurround() {AccessMode = AccessMode.IndirectX, StartsWith = "(", EndsWith = ",X)", ParameterSize = ParameterSize.Bit8, Order = 20 },
             new ParametersDefinitionSurround() {AccessMode = AccessMode.IndirectY, StartsWith = "(", EndsWith = "),Y", ParameterSize = ParameterSize.Bit8, Order = 20 },
             new ParametersDefinitionSurround() {AccessMode = AccessMode.IndAbsoluteX, StartsWith = "(", EndsWith = ",X)", ParameterSize = ParameterSize.Bit16, Order = 20 },
             new ParametersDefinitionSurround() {AccessMode = AccessMode.ZeroPageIndirect, StartsWith = "(", EndsWith = ")", ParameterSize = ParameterSize.Bit8, Order = 25 },
             new ParametersDefinitionSurround() {AccessMode = AccessMode.Indirect, StartsWith = "(", EndsWith = ")", ParameterSize = ParameterSize.Bit16, Order = 25 },
             new ParametersDefinitionSurround() {AccessMode = AccessMode.ZeroPageX, EndsWith = ",X", ParameterSize = ParameterSize.Bit8, Order = 30 },
             new ParametersDefinitionSurround() {AccessMode = AccessMode.ZeroPageY, EndsWith = ",Y", ParameterSize = ParameterSize.Bit8, Order = 30 },
             new ParametersDefinitionSurround() {AccessMode = AccessMode.AbsoluteX, EndsWith = ",X", ParameterSize = ParameterSize.Bit16, Order = 30 },
             new ParametersDefinitionSurround() {AccessMode = AccessMode.AbsoluteY, EndsWith = ",Y", ParameterSize = ParameterSize.Bit16, Order = 30 },
             new ParamatersDefinitionRelative() {AccessMode = AccessMode.Relative, ParameterSize = ParameterSize.Bit8, Order = 40 },
             new ParametersDefinitionEmpty() {AccessMode = AccessMode.Implied, Order = 40 },
             new ParametersDefinitionEmpty() {AccessMode = AccessMode.Accumulator, Order = 40 },
             new ParametersDefinitionSurround() {AccessMode = AccessMode.ZeroPage, DoesntStartWith = new [] { "#" }, ParameterSize = ParameterSize.Bit8, Order = 40 },
             new ParametersDefinitionSurround() {AccessMode = AccessMode.Absolute, DoesntStartWith = new [] { "#" }, ParameterSize = ParameterSize.Bit16, Order = 40 },
        }.ToDictionary(i => i.AccessMode, i => i);

        public IReadOnlyDictionary<AccessMode, IParametersDefinition> ParameterDefinitions => _parameterDefinitions;        

        private readonly EmulatableCpuOpCode[] _opCodes = new EmulatableCpuOpCode[]
        {
            new Adc(),
            new And(),
            new Asl(),
            new Bcc(),
            new Bcs(),
            new Beq(),
            new Bit(),
            new Bmi(),
            new Bne(),
            new Bpl(),
            new Bra(),
            new Brk(),
            new Bvc(),
            new Bvs(),
            new Clc(),
            new Cld(),
            new Cli(),
            new Clv(),
            new Cmp(),
            new Cpx(),
            new Cpy(),
            new Dec(),
            new Dex(),
            new Dey(),
            new Eor(),
            new Inc(),
            new Inx(),
            new Iny(),
            new Jmp(),
            new Jsr(),
            new Lda(),
            new Ldx(),
            new Ldy(),
            new Lsr(),
            new Nop(),
            new Ora(),
            new Pha(),
            new Php(),
            new Phx(),
            new Phy(),
            new Pla(),
            new Ply(),
            new Plx(),
            new Plp(),
            new Rol(),
            new Ror(),
            new Rti(),
            new Rts(),
            new Sbc(),
            new Sec(),
            new Sed(),
            new Sei(),
            new Sta(),
            new Stz(),
            new Stx(),
            new Sty(),
            new Stp(),
            new Tax(),
            new Tay(),
            new Tsx(),
            new Txa(),
            new Txs(),
            new Tya(),
            new Trb(),
            new Tsb(),
            new Bbr0(),
            new Bbr1(),
            new Bbr2(),
            new Bbr3(),
            new Bbr4(),
            new Bbr5(),
            new Bbr6(),
            new Bbr7(),
            new Bbs0(),
            new Bbs1(),
            new Bbs2(),
            new Bbs3(),
            new Bbs4(),
            new Bbs5(),
            new Bbs6(),
            new Bbs7(),
            new Rmb0(),
            new Rmb1(),
            new Rmb2(),
            new Rmb3(),
            new Rmb4(),
            new Rmb5(),
            new Rmb6(),
            new Rmb7(),
            new Smb0(),
            new Smb1(),
            new Smb2(),
            new Smb3(),
            new Smb4(),
            new Smb5(),
            new Smb6(),
            new Smb7(),
            new Wai()
        };

        private (EmulatableCpuOpCode operation, AccessMode Mode, int Timing)?[] _operations;

        public WDC65c02(IMemory memory, double frequency)
        {
            _operations = new (EmulatableCpuOpCode operation, AccessMode Mode, int Timing)?[256];
            Memory = memory;
            Frequency = frequency;

            foreach (var op in _opCodes)
            {
                foreach (var i in op.OpCodes)
                {
                    _operations[i.OpCode] = (op, i.Mode, i.Timing);
                }
            }
        }

        public void SetInterrupt()
        {
            HasInterrupt = true;
        }

        public int HandleInterrupt(IMemory memory)
        {
            HasInterrupt = false;

            if (Registers.Flags.InterruptDisable)
                return 0;

            Push((byte)((Registers.PC & 0xff00) >> 8));
            Push((byte)(Registers.PC & 0xff));
            Push(Registers.P);
            /*
                        Push(Registers.A); // done by the kernal normally.
                        Push(Registers.X);
                        Push(Registers.Y);*/

            var interuptHandler = memory.GetByte(InterruptVector) + (memory.GetByte(InterruptVector + 1) << 8);

            Registers.PC = (ushort)interuptHandler;

            Registers.Flags.InterruptDisable = true;

            return 0;
        }

        public void Reset()
        {
            Registers.PC = (ushort)(Memory.GetByte(ResetVector) + (Memory.GetByte(ResetVector + 1) << 8));
        }

        public void SetProgramCounter(int address)
        {
            Registers.PC = (ushort)address;
        }

        public int ClockTick(IMemory memory, bool verboseOutput)
        {
            if (verboseOutput)
            {
                Console.Write($"${Registers.PC:X4}");
            }

            var opCode = memory.GetByte(Registers.PC++);

            if (_operations[opCode] == null)
            {
                opCode = 0xea; // nop
                Debug.Assert(false);
                if (verboseOutput)
                    Console.Write("?");
            }
            else if (verboseOutput)
            {
                Console.Write(" ");
            }

            LastOpCode = opCode;

            var opdef = _operations[opCode];
            if (opdef == null) throw new Exception();

            var (op, am, timing) = opdef.Value;

            if (verboseOutput)
            {
                Console.Write(op.Code);
                Console.Write(" ");
            }

            timing += op.Process(
                opCode,
                () => GetValueAtPC(am, memory, verboseOutput),
                () => GetAddressAtPC(am, memory, verboseOutput),
                memory,
                this);

            if (verboseOutput)
            {
                Console.CursorLeft = 15;
                Console.WriteLine($" Tks:{timing} -> {Registers} [{Registers.Flags}]");
                Console.ReadKey();
            }

            return timing;
        }

        public void SetValue(int address, byte value, IMemory memory)
        {
            memory.SetByte(address, value);
        }

        public (ushort address, int timing, ushort pcStep) GetAddressAtPC(AccessMode mode, IMemory memory, bool verboseOutput)
        {
            byte l;
            byte h;
            ushort toReturn;
            ushort address;
            int timing;
            int address1;

            switch (mode)
            {
                case AccessMode.ZeroPage:
                    l = memory.GetByte(Registers.PC);

                    if (verboseOutput) Console.Write($"${l:X2}");
                    return (l, 0, 1);
                case AccessMode.ZeroPageX:
                    l = memory.GetByte(Registers.PC);

                    toReturn = (ushort)((l + Registers.X) & 0xff);

                    if (verboseOutput) Console.Write($"${toReturn:X4}");
                    return (toReturn, 0, 1);
                case AccessMode.ZeroPageY:
                    l = memory.GetByte(Registers.PC);

                    toReturn = (ushort)((l + Registers.Y) & 0xff);

                    if (verboseOutput) Console.Write($"${toReturn:X4}");
                    return (toReturn, 0, 1);
                case AccessMode.IndirectX:
                    l = memory.GetByte(Registers.PC);

                    address = memory.GetByte((l + Registers.X) & 0xff);
                    address += (ushort)(memory.GetByte((l + Registers.X + 1) & 0xff) << 8);

                    if (verboseOutput) Console.Write($"${address:X4}");
                    return (address, 0, 1);
                case AccessMode.IndirectY:
                    l = memory.GetByte(Registers.PC);

                    address = memory.GetByte(l);
                    address += (ushort)(memory.GetByte(l + 1) << 8);
                    timing = (address & 0xff00) == ((address + Registers.Y) & 0xff00) ? 0 : 1;
                    toReturn = (ushort)(address + Registers.Y);

                    if (verboseOutput) Console.Write($"${toReturn:X4}");
                    return (toReturn, timing, 1);
                case AccessMode.ZeroPageIndirect:
                    l = memory.GetByte(Registers.PC);

                    address = memory.GetByte(l);
                    address += (ushort)(memory.GetByte(l + 1) << 8);

                    if (verboseOutput) Console.Write($"${address:X4}");
                    return (address, 0, 1);
                case AccessMode.Absolute:
                    l = memory.GetByte(Registers.PC);
                    h = memory.GetByte(Registers.PC + 1);
                    address = (ushort)(l + (h << 8));

                    if (verboseOutput) Console.Write($"${address:X4}");
                    return (address, 0, 2);
                case AccessMode.AbsoluteX:
                    l = memory.GetByte(Registers.PC);
                    h = memory.GetByte(Registers.PC + 1);
                    address = (ushort)(l + (h << 8));

                    timing = (address & 0xff00) == ((address + Registers.X) & 0xff00) ? 0 : 1;
                    toReturn = (ushort)(address + Registers.X);

                    if (verboseOutput) Console.Write($"${toReturn:X4}");
                    return (toReturn, timing, 2);
                case AccessMode.AbsoluteY:
                    l = memory.GetByte(Registers.PC);
                    h = memory.GetByte(Registers.PC + 1);
                    address = (ushort)(l + (h << 8));

                    timing = (address & 0xff00) == ((address + Registers.Y) & 0xff00) ? 0 : 1;
                    toReturn = (ushort)(address + Registers.Y);

                    if (verboseOutput) Console.Write($"${toReturn:X4}");
                    return (toReturn, timing, 2);
                case AccessMode.IndAbsoluteX:
                    l = memory.GetByte(Registers.PC);
                    h = memory.GetByte(Registers.PC + 1);
                    address = (ushort)(l + (h << 8));

                    address = (ushort)(memory.GetByte(address) + (memory.GetByte(address+1) << 8));
                    address += Registers.X;

                    if (verboseOutput) Console.Write($"${address:X4}");
                    return (address, 0, 2);
                case AccessMode.Indirect:
                    l = memory.GetByte(Registers.PC);
                    h = memory.GetByte(Registers.PC + 1);
                    address = (ushort)(l + (h << 8));

                    address = (ushort)(memory.GetByte(address) + (memory.GetByte(address+1) << 8));
                    if (verboseOutput) Console.Write($"${address:X4}");
                    return (address, 0, 2);
            }

            throw new Exception($"Unhandled access mode {mode}");
        }

        public (byte value, int timing, ushort pcStep) GetValueAtPC(AccessMode mode, IMemory memory, bool verboseOutput)
        {
            byte toReturn;
            switch (mode)
            {
                case AccessMode.Implied:
                    if (verboseOutput) Console.Write("#$00");
                    return (0, 0, 0);
                case AccessMode.Immediate:
                case AccessMode.Relative:
                    toReturn = memory.GetByte(Registers.PC);
                    if (verboseOutput) Console.Write($"#${toReturn:X2}");
                    return (toReturn, 0, 1);
                case AccessMode.Accumulator:
                    toReturn = Registers.A;
                    if (verboseOutput) Console.Write($"#${toReturn:X2}");

                    return (Registers.A, 0, 0);
            }

            var (address, timing, pcStep) = GetAddressAtPC(mode, memory, verboseOutput);

            return (memory.GetByte(address), timing, pcStep);
        }

        public void Push(byte value)
        {
            Memory.SetByte(_stackStart + Registers.S--, value);
        }

        public byte Pop() => Memory.GetByte(_stackStart + ++Registers.S);
    }

    public class Stp : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xdb, AccessMode.Implied, 2),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            // turn on debug mode?
            //Debug.Assert(false);

            return 0;
        }
    }

    public class Adc : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new() {
            (0x69, AccessMode.Immediate, 2),
            (0x65, AccessMode.ZeroPage, 3),
            (0x75, AccessMode.ZeroPageX, 4),
            (0x6d, AccessMode.Absolute, 4),
            (0x7d, AccessMode.AbsoluteX, 4),
            (0x79, AccessMode.AbsoluteY, 4),
            (0x61, AccessMode.IndirectX, 6),
            (0x71, AccessMode.IndirectY, 5),
            (0x72, AccessMode.ZeroPageIndirect, 5),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            var (value, timing, pcStep) = GetValueAtPC();
            var newVal = value + cpu.Registers.A + (cpu.Registers.Flags.Carry ? 1 : 0);

            cpu.Registers.Flags.Carry = newVal > 255;
            cpu.Registers.Flags.Overflow = newVal > 127;
            cpu.Registers.A = (byte)(newVal & 0xff);

            cpu.Registers.PC += pcStep;

            return timing;
        }
    }

    public class And : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new() {
            (0x29, AccessMode.Immediate, 2),
            (0x25, AccessMode.ZeroPage, 3),
            (0x35, AccessMode.ZeroPageX, 4),
            (0x2d, AccessMode.Absolute, 4),
            (0x3d, AccessMode.AbsoluteX, 4),
            (0x39, AccessMode.AbsoluteY, 4),
            (0x21, AccessMode.IndirectX, 6),
            (0x31, AccessMode.IndirectY, 5),
            (0x32, AccessMode.ZeroPageIndirect, 5),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            var (value, timing, pcStep) = GetValueAtPC();
            cpu.Registers.A = (byte)(value & cpu.Registers.A);

            cpu.Registers.PC += pcStep;

            return timing;
        }
    }

    public class Asl : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new() {
            (0x0a, AccessMode.Accumulator, 2),
            (0x06, AccessMode.ZeroPage, 5),
            (0x16, AccessMode.ZeroPageX, 6),
            (0x0e, AccessMode.Absolute, 6),
            (0x1e, AccessMode.AbsoluteX, 7),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            int address, timing, val;
            ushort pcStep;

            if (opCode != 0x0a)
            {
                (address, timing, pcStep) = GetAddressAtPc();

                val = memory.GetByte(address);
            }
            else
            {
                val = cpu.Registers.A;
                address = 0;
                timing = 0;
                pcStep = 0;
            }

            cpu.Registers.Flags.Carry = (val & 0x80) > 0;
            val = val << 1;

            byte actVal = (byte)(val & 0xff);

            if (opCode != 0x0a)
            {
                memory.SetByte(address, actVal);
                cpu.Registers.Flags.SetNv(actVal);
            }
            else
            {
                cpu.Registers.A = actVal;
            }

            cpu.Registers.PC += pcStep;
            return timing;
        }
    }

    public class Bit : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new() {
            (0x24, AccessMode.ZeroPage, 3),
            (0x2c, AccessMode.Absolute, 4),
            (0x89, AccessMode.Immediate, 2),
            (0x34, AccessMode.ZeroPageX, 4),
            (0x3c, AccessMode.AbsoluteX, 4),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            var (value, timing, pcStep) = GetValueAtPC();
            cpu.Registers.Flags.Zero = (value & cpu.Registers.A) == 0;
            cpu.Registers.Flags.Negative = (value & 128) != 0;
            cpu.Registers.Flags.Overflow = (value & 64) != 0;

            cpu.Registers.PC += pcStep;

            return timing;
        }
    }

    public abstract class BranchOpCode : EmulatableCpuOpCode
    {
        public abstract bool Condition(I6502 cpu);

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            if (!Condition(cpu))
            {
                GetValueAtPC();

                cpu.Registers.PC++;
                return 0;
            }

            var (value, timing, _) = GetValueAtPC();

            int newAddress;
            if (value > 127)
            {
                newAddress = cpu.Registers.PC - (value ^ 0xff);
            }
            else
            {
                newAddress = cpu.Registers.PC + value + 1;
            }

            timing += ((cpu.Registers.PC & 0xff00) != (newAddress & 0xff00)) ? 1 : 0;

            cpu.Registers.PC = (ushort)newAddress;

            return timing + 1;
        }
    }

    public class Bra : BranchOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x80, AccessMode.Relative, 2)
        };

        public override bool Condition(I6502 cpu) => true;
    }

    public class Bpl : BranchOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new() {
            (0x10, AccessMode.Relative, 2)
        };

        public override bool Condition(I6502 cpu) => !cpu.Registers.Flags.Negative;
    }

    public class Bmi : BranchOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new() {
            (0x30, AccessMode.Relative, 2)
        };

        public override bool Condition(I6502 cpu) => cpu.Registers.Flags.Negative;
    }

    public class Bvc : BranchOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x50, AccessMode.Relative, 2)
        };

        public override bool Condition(I6502 cpu) => !cpu.Registers.Flags.Overflow;
    }
    public class Bvs : BranchOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x70, AccessMode.Relative, 2)
        };

        public override bool Condition(I6502 cpu) => cpu.Registers.Flags.Overflow;
    }

    public class Bcc : BranchOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x90, AccessMode.Relative, 2)
        };

        public override bool Condition(I6502 cpu) => !cpu.Registers.Flags.Carry;
    }

    public class Bcs : BranchOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xb0, AccessMode.Relative, 2)
        };

        public override bool Condition(I6502 cpu) => cpu.Registers.Flags.Carry;
    }

    public class Bne : BranchOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xd0, AccessMode.Relative, 2)
        };

        public override bool Condition(I6502 cpu) => !cpu.Registers.Flags.Zero;
    }

    public class Beq : BranchOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xf0, AccessMode.Relative, 2)
        };

        public override bool Condition(I6502 cpu) => cpu.Registers.Flags.Zero;
    }

    public class Brk : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x00, AccessMode.Implied, 7)
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            //Debug.Assert(false);
            //throw new NotImplementedException();

            cpu.Registers.PC += 1; // See http://6502.org/tutorials/6502opcodes.html#BRK

            return 0;
        }
    }

    public abstract class Compare : EmulatableCpuOpCode
    {
        public abstract byte SourceVal(I6502 cpu);
        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            var (value, timing, pcStep) = GetValueAtPC();

            var cmpResult = SourceVal(cpu) - value;

            cpu.Registers.Flags.SetNv((byte)cmpResult);
            cpu.Registers.Flags.Carry = cmpResult > 0;

            cpu.Registers.PC += pcStep;

            return timing;
        }
    }

    public class Cmp : Compare
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xc9, AccessMode.Immediate, 2),
            (0xc5, AccessMode.ZeroPage, 3),
            (0xd5, AccessMode.ZeroPageX, 4),
            (0xcd, AccessMode.Absolute, 4),
            (0xdd, AccessMode.AbsoluteX, 4),
            (0xd9, AccessMode.AbsoluteY, 4),
            (0xc1, AccessMode.IndirectX, 6),
            (0xd1, AccessMode.IndirectY, 5),
            (0xd2, AccessMode.ZeroPageIndirect, 5),
        };

        public override byte SourceVal(I6502 cpu) => cpu.Registers.A;
    }

    public class Cpx : Compare
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xe0, AccessMode.Immediate, 2),
            (0xe4, AccessMode.ZeroPage, 3),
            (0xec, AccessMode.Absolute, 4),
        };

        public override byte SourceVal(I6502 cpu) => cpu.Registers.X;
    }

    public class Cpy : Compare
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xc0, AccessMode.Immediate, 2),
            (0xc4, AccessMode.ZeroPage, 3),
            (0xcc, AccessMode.Absolute, 4),
        };

        public override byte SourceVal(I6502 cpu) => cpu.Registers.Y;
    }

    public class Dec : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x3a, AccessMode.Implied, 2),
            (0xc6, AccessMode.ZeroPage, 5),
            (0xd6, AccessMode.ZeroPageX, 6),
            (0xce, AccessMode.Absolute, 6),
            (0xde, AccessMode.AbsoluteX, 7),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            byte actVal;
            ushort address;
            int timing;
            ushort pcStep;

            if (opCode == 0x3a)
            {
                actVal = (byte)(cpu.Registers.A-- & 0xff);
                pcStep = 0;
                timing = 0;
            } 
            else 
            { 
                (address, timing, pcStep) = GetAddressAtPc();

                int val = memory.GetByte(address);
                val--;
                actVal = (byte)(val & 0xff);

                memory.SetByte(address, actVal);
            }
            cpu.Registers.Flags.SetNv(actVal);

            cpu.Registers.PC += pcStep;
            return timing;
        }
    }

    public class Eor : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x49, AccessMode.Immediate, 2),
            (0x45, AccessMode.ZeroPage, 3),
            (0x55, AccessMode.ZeroPageX, 4),
            (0x4d, AccessMode.Absolute, 4),
            (0x5d, AccessMode.AbsoluteX, 4),
            (0x59, AccessMode.AbsoluteY, 4),
            (0x41, AccessMode.IndirectX, 6),
            (0x51, AccessMode.IndirectY, 5),
            (0x52, AccessMode.ZeroPageIndirect, 5),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            var (value, timing, pcStep) = GetValueAtPC();
            cpu.Registers.A = (byte)(value ^ cpu.Registers.A);

            cpu.Registers.PC += pcStep;

            return timing;
        }
    }

    public abstract class FlagInstruction : EmulatableCpuOpCode
    {
        public abstract void PerformOperation(I6502 cpu);

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            PerformOperation(cpu);

            return 0;
        }
    }

    public class Clc : FlagInstruction
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x18, AccessMode.Implied, 2)
        };

        public override void PerformOperation(I6502 cpu) => cpu.Registers.Flags.Carry = false;
    }

    public class Sec : FlagInstruction
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x38, AccessMode.Implied, 2)
        };

        public override void PerformOperation(I6502 cpu) => cpu.Registers.Flags.Carry = true;

    }

    public class Cli : FlagInstruction
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x58, AccessMode.Implied, 2)
        };

        public override void PerformOperation(I6502 cpu) => cpu.Registers.Flags.InterruptDisable = false;
    }

    public class Sei : FlagInstruction
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x78, AccessMode.Implied, 2)
        };

        public override void PerformOperation(I6502 cpu) => cpu.Registers.Flags.InterruptDisable = true;
    }

    public class Clv : FlagInstruction
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xb8, AccessMode.Implied, 2)
        };

        public override void PerformOperation(I6502 cpu) => cpu.Registers.Flags.Overflow = false;
    }

    public class Cld : FlagInstruction
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xd8, AccessMode.Implied, 2)
        };

        public override void PerformOperation(I6502 cpu) => cpu.Registers.Flags.Decimal = false;
    }

    public class Sed : FlagInstruction
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xf8, AccessMode.Implied, 2)
        };

        public override void PerformOperation(I6502 cpu) => cpu.Registers.Flags.Decimal = true;
    }

    public class Inc : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x1a, AccessMode.Implied, 2),
            (0xe6, AccessMode.ZeroPage, 5),
            (0xf6, AccessMode.ZeroPageX, 6),
            (0xee, AccessMode.Absolute, 6),
            (0xfe, AccessMode.AbsoluteX, 7),
        };
        
        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            byte actVal;
            ushort address;
            int timing;
            ushort pcStep;

            if (opCode == 0x1a)
            {
                actVal = (byte)(cpu.Registers.A++ & 0xff);
                pcStep = 0;
                timing = 0;
            }
            else
            {
                (address, timing, pcStep) = GetAddressAtPc();

                int val = memory.GetByte(address);
                val++;
                actVal = (byte)(val & 0xff);

                memory.SetByte(address, actVal);
            }

            cpu.Registers.Flags.SetNv(actVal);

            cpu.Registers.PC += pcStep;
            return timing;
        }
    }

    public class Jmp : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x4c, AccessMode.Absolute, 3),
            (0x6c, AccessMode.Indirect, 5),
            (0x7c, AccessMode.IndAbsoluteX, 6),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            var (address, timing, _) = GetAddressAtPc();

            cpu.Registers.PC = address;

            return timing;
        }
    }

    public class Jsr : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x20, AccessMode.Absolute, 6),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            var (destAddress, timing, _) = GetAddressAtPc();

            var returnAddress = (ushort)(cpu.Registers.PC + 1); // normally -1, but we've not adjusted the PC yet.

            var l = (byte)(returnAddress & 0xff);
            var h = (byte)((returnAddress & 0xff00) >> 8);

            cpu.Push(h);
            cpu.Push(l);

            cpu.Registers.PC = destAddress;

            return timing;
        }
    }

    public class Lda : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xa9, AccessMode.Immediate, 2),
            (0xa5, AccessMode.ZeroPage, 3),
            (0xb5, AccessMode.ZeroPageX, 4),
            (0xad, AccessMode.Absolute, 4),
            (0xbd, AccessMode.AbsoluteX, 4),
            (0xb9, AccessMode.AbsoluteY, 4),
            (0xa1, AccessMode.IndirectX, 6),
            (0xb1, AccessMode.IndirectY, 5),
            (0xb2, AccessMode.ZeroPageIndirect, 5)
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            var (value, timing, pcStep) = GetValueAtPC();
            cpu.Registers.A = value;

            cpu.Registers.PC += pcStep;

            return timing;
        }
    }

    public class Ldx : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xa2, AccessMode.Immediate, 2),
            (0xa6, AccessMode.ZeroPage, 3),
            (0xb6, AccessMode.ZeroPageY, 4),
            (0xae, AccessMode.Absolute, 4),
            (0xbe, AccessMode.AbsoluteY, 4),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            var (value, timing, pcStep) = GetValueAtPC();
            cpu.Registers.X = value;

            cpu.Registers.PC += pcStep;

            return timing;
        }
    }

    public class Ldy : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xa0, AccessMode.Immediate, 2),
            (0xa4, AccessMode.ZeroPage, 3),
            (0xb4, AccessMode.ZeroPageX, 4),
            (0xac, AccessMode.Absolute, 4),
            (0xbc, AccessMode.AbsoluteX, 4),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            var (value, timing, pcStep) = GetValueAtPC();
            cpu.Registers.Y = value;

            cpu.Registers.PC += pcStep;

            return timing;
        }
    }

    public class Lsr : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x4a, AccessMode.Accumulator, 2),
            (0x46, AccessMode.ZeroPage, 5),
            (0x56, AccessMode.ZeroPageX, 6),
            (0x4e, AccessMode.Absolute, 6),
            (0x5e, AccessMode.AbsoluteX, 7),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            int address, timing, val;
            ushort pcStep;

            if (opCode != 0x4a)
            {
                (address, timing, pcStep) = GetAddressAtPc();

                val = memory.GetByte(address);
            } 
            else
            {
                val = cpu.Registers.A;
                address = 0;
                timing = 0;
                pcStep = 0;
            }

            cpu.Registers.Flags.Carry = (val & 0x01) > 0;
            val = val >> 1;

            byte actVal = (byte)(val & 0xff);

            if (opCode != 0x4a)
            {
                memory.SetByte(address, actVal);
                cpu.Registers.Flags.SetNv(actVal);
            } else
            {
                cpu.Registers.A = actVal;
            }

            cpu.Registers.PC += pcStep;
            return timing;
        }
    }

    public class Nop : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xea, AccessMode.Implied, 2),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            return 0;
        }
    }

    public class Ora : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x09, AccessMode.Immediate, 2),
            (0x05, AccessMode.ZeroPage, 3),
            (0x15, AccessMode.ZeroPageX, 4),
            (0x0d, AccessMode.Absolute, 4),
            (0x1d, AccessMode.AbsoluteX, 4),
            (0x19, AccessMode.AbsoluteY, 4),
            (0x01, AccessMode.IndirectX, 6),
            (0x11, AccessMode.IndirectY, 5),
            (0x12, AccessMode.ZeroPageIndirect, 5),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            var (value, timing, pcStep) = GetValueAtPC();
            cpu.Registers.A = (byte)(value | cpu.Registers.A);

            cpu.Registers.PC += pcStep;

            return timing;
        }
    }

    public class Rol : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x2a, AccessMode.Accumulator, 2),
            (0x26, AccessMode.ZeroPage, 5),
            (0x36, AccessMode.ZeroPageX, 6),
            (0x2e, AccessMode.Absolute, 6),
            (0x3e, AccessMode.AbsoluteX, 7),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            int address, timing, val;
            ushort pcStep;

            if (opCode != 0x2a)
            {
                (address, timing, pcStep) = GetAddressAtPc();

                val = memory.GetByte(address);
            }
            else
            {
                val = cpu.Registers.A;
                address = 0;
                timing = 0;
                pcStep = 0;
            }

            var newC = (val & 0x80) > 0;
            val = val << 1;
            val += cpu.Registers.Flags.Carry ? 1 : 0;
            cpu.Registers.Flags.Carry = newC;

            byte actVal = (byte)(val & 0xff);

            if (opCode != 0x2a)
            {
                memory.SetByte(address, actVal);
                cpu.Registers.Flags.SetNv(actVal);
            }
            else
            {
                cpu.Registers.A = actVal;
            }

            cpu.Registers.PC += pcStep;
            return timing;
        }
    }

    public class Ror : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x6a, AccessMode.Accumulator, 2),
            (0x66, AccessMode.ZeroPage, 5),
            (0x76, AccessMode.ZeroPageX, 6),
            (0x6e, AccessMode.Absolute, 6),
            (0x7e, AccessMode.AbsoluteX, 7),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            int address, timing, val;
            ushort pcStep;

            if (opCode != 0x6a)
            {
                (address, timing, pcStep) = GetAddressAtPc();

                val = memory.GetByte(address);
            }
            else
            {
                val = cpu.Registers.A;
                address = 0;
                timing = 0;
                pcStep = 0;
            }

            var newC = (val & 0x01) > 0;
            val = val >> 1;
            val += cpu.Registers.Flags.Carry ? 128 : 0;
            cpu.Registers.Flags.Carry = newC;

            byte actVal = (byte)(val & 0xff);

            if (opCode != 0x6a)
            {
                memory.SetByte(address, actVal);
                cpu.Registers.Flags.SetNv(actVal);
            }
            else
            {
                cpu.Registers.A = actVal;
            }

            cpu.Registers.PC += pcStep;
            return timing;
        }
    }

    public class Rti : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x40, AccessMode.Implied, 6),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            cpu.Registers.P = cpu.Pop(); // will always have interrupt disable set to false

            var l = cpu.Pop();
            var h = cpu.Pop();

            var address = (ushort)(h * 256 + l);

            cpu.Registers.PC = address;

            return 0;
        }
    }

    public class Rts : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x60, AccessMode.Implied, 6),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            var l = cpu.Pop();
            var h = cpu.Pop();

            var address = (ushort)(h * 256 + l + 1);

            cpu.Registers.PC = address;

            return 0;
        }
    }

    public class Sbc : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xe9, AccessMode.Immediate, 2),
            (0xe5, AccessMode.ZeroPage, 3),
            (0xf5, AccessMode.ZeroPageX, 4),
            (0xed, AccessMode.Absolute, 4),
            (0xfd, AccessMode.AbsoluteX, 4),
            (0xf9, AccessMode.AbsoluteY, 4),
            (0xe1, AccessMode.IndirectX, 6),
            (0xf1, AccessMode.IndirectY, 5),
            (0xf2, AccessMode.ZeroPageIndirect, 5),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            var (value, timing, pcStep) = GetValueAtPC();
            var newVal = cpu.Registers.A - (value + (cpu.Registers.Flags.Carry ? 0 : 1));

            cpu.Registers.Flags.Carry = !(newVal < 0);
            cpu.Registers.Flags.Overflow = newVal < -128;
            cpu.Registers.A = (byte)(newVal & 0xff);

            cpu.Registers.PC += pcStep;

            return timing;
        }
    }

    public class Sta : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x85, AccessMode.ZeroPage, 3),
            (0x95, AccessMode.ZeroPageX, 4),
            (0x8d, AccessMode.Absolute, 4),
            (0x9d, AccessMode.AbsoluteX, 5),
            (0x99, AccessMode.AbsoluteY, 5),
            (0x81, AccessMode.IndirectX, 6),
            (0x91, AccessMode.IndirectY, 6),
            (0x92, AccessMode.ZeroPageIndirect, 5),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            var (address, timing, pcStep) = GetAddressAtPc();

            memory.SetByte(address, cpu.Registers.A);

            cpu.Registers.PC += pcStep;

            return timing;
        }
    }

    public class Stz : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x64, AccessMode.ZeroPage, 3),
            (0x74, AccessMode.ZeroPageX, 4),
            (0x9c, AccessMode.Absolute, 4),
            (0x9e, AccessMode.AbsoluteX, 5),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            var (address, timing, pcStep) = GetAddressAtPc();

            memory.SetByte(address, 0);

            cpu.Registers.PC += pcStep;

            return timing;
        }
    }

    public class Stx : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x86, AccessMode.ZeroPage, 3),
            (0x96, AccessMode.ZeroPageY, 4),
            (0x8e, AccessMode.Absolute, 4),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            var (address, timing, pcStep) = GetAddressAtPc();

            memory.SetByte(address, cpu.Registers.X);

            cpu.Registers.PC += pcStep;

            return timing;
        }
    }

    public class Sty : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x84, AccessMode.ZeroPage, 3),
            (0x94, AccessMode.ZeroPageX, 4),
            (0x8c, AccessMode.Absolute, 4),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            var (address, timing, pcStep) = GetAddressAtPc();

            memory.SetByte(address, cpu.Registers.Y);

            cpu.Registers.PC += pcStep;

            return timing;
        }
    }

    public class Tax : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xaa, AccessMode.Implied, 2),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            cpu.Registers.X = cpu.Registers.A;

            return 0;
        }
    }

    public class Txa : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x8a, AccessMode.Implied, 2),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            cpu.Registers.A = cpu.Registers.X;

            return 0;
        }
    }

    public class Dex : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xca, AccessMode.Implied, 2),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            cpu.Registers.X--;

            return 0;
        }
    }

    public class Inx : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xe8, AccessMode.Implied, 2),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            cpu.Registers.X++;

            return 0;
        }
    }

    public class Tay : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xa8, AccessMode.Implied, 2),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            cpu.Registers.Y = cpu.Registers.A;

            return 0;
        }
    }

    public class Tya : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x98, AccessMode.Implied, 2),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            cpu.Registers.A = cpu.Registers.Y;

            return 0;
        }
    }

    public class Dey : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x88, AccessMode.Implied, 2),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            cpu.Registers.Y--;

            return 0;
        }
    }

    public class Iny : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xc8, AccessMode.Implied, 2),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            cpu.Registers.Y++;

            return 0;
        }
    }

    public class Txs : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x9a, AccessMode.Implied, 2),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            cpu.Registers.S = cpu.Registers.X;

            return 0;
        }
    }

    public class Tsx : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xba, AccessMode.Implied, 2),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            cpu.Registers.X = cpu.Registers.S;

            return 0;
        }
    }

    public class Pha : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x48, AccessMode.Implied, 3),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            cpu.Push(cpu.Registers.A);

            return 0;
        }
    }

    public class Pla : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x68, AccessMode.Implied, 4),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            cpu.Registers.A = cpu.Pop();

            return 0;
        }
    }

    public class Php : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x08, AccessMode.Implied, 3),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            cpu.Push(cpu.Registers.P);

            return 0;
        }
    }

    public class Plp : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x28, AccessMode.Implied, 4),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            cpu.Registers.P = cpu.Pop();

            return 0;
        }
    }

    public class Plx : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xfa, AccessMode.Implied, 4),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            cpu.Registers.X = cpu.Pop();

            return 0;
        }
    }

    public class Ply : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x7a, AccessMode.Implied, 4),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            cpu.Registers.Y = cpu.Pop();

            return 0;
        }
    }

    public class Phx : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xda, AccessMode.Implied, 3),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            cpu.Push(cpu.Registers.X);

            return 0;
        }
    }

    public class Phy : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x5a, AccessMode.Implied, 3),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            cpu.Push(cpu.Registers.Y);

            return 0;
        }
    }

    public class Trb : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x14, AccessMode.ZeroPage, 5),
            (0x1c, AccessMode.Absolute, 6),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            Debug.Assert(false); // this is untested.
            var (address, timing, pcStep) = GetAddressAtPc();

            var value = memory.GetByte(address);

            cpu.Registers.Flags.Zero = (value & cpu.Registers.A) == 0;

            var newVal = (byte)(value & (cpu.Registers.A ^ 0xff));

            memory.SetByte(address, newVal);

            cpu.Registers.PC += pcStep;

            return timing;
        }
    }

    public class Tsb : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x04, AccessMode.ZeroPage, 5),
            (0x0c, AccessMode.Absolute, 6),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            Debug.Assert(false); // this is untested.
            var (address, timing, pcStep) = GetAddressAtPc();

            var value = memory.GetByte(address);

            cpu.Registers.Flags.Zero = (value & cpu.Registers.A) == 0;

            var newVal = (byte)(value | cpu.Registers.A);

            memory.SetByte(address, newVal);

            cpu.Registers.PC += pcStep;

            return timing;
        }
    }

    public abstract class BbBase : EmulatableCpuOpCode
    {
        protected abstract int Bit { get; }
        protected abstract bool BranchOn { get; }

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            Debug.Assert(false); // this is untested.
            var (zp, _, _) = GetValueAtPC();

            var zpValue = memory.GetByte(zp);

            if ((zpValue & (Bit switch
            {
                0 => 0b00000001,
                1 => 0b00000010,
                2 => 0b00000100,
                3 => 0b00001000,
                4 => 0b00010000,
                5 => 0b00100000,
                6 => 0b01000000,
                7 => 0b10000000,
                _ => throw new NotImplementedException()
            })) == 0 == BranchOn)
            {
                cpu.Registers.PC++;
                var (value, _, _) = GetValueAtPC();

                int newAddress;
                if (value > 127)
                {
                    newAddress = cpu.Registers.PC - (value ^ 0xff);
                }
                else
                {
                    newAddress = cpu.Registers.PC + value + 1;
                }

                cpu.Registers.PC = (ushort)newAddress;
            }
            else
            {
                cpu.Registers.PC += 2;
            }

            return 0;
        }
    }

    public class Bbr0 : BbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x0f, AccessMode.ZerpPageRel, 5),
        };
        protected override int Bit => 0;
        protected override bool BranchOn => false;
    }

    public class Bbr1 : BbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x1f, AccessMode.ZerpPageRel, 5),
        };
        protected override int Bit => 1;
        protected override bool BranchOn => false;
    }

    public class Bbr2 : BbBase
    { 
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x2f, AccessMode.ZerpPageRel, 5),
        };
        protected override int Bit => 2;
        protected override bool BranchOn => false;
    }

    public class Bbr3 : BbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x3f, AccessMode.ZerpPageRel, 5),
        };
        protected override int Bit => 3;
        protected override bool BranchOn => false;
    }

    public class Bbr4 : BbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x4f, AccessMode.ZerpPageRel, 5),
        };
        protected override int Bit => 4;
        protected override bool BranchOn => false;
    }

    public class Bbr5 : BbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x5f, AccessMode.ZerpPageRel, 5),
        };
        protected override int Bit => 5;
        protected override bool BranchOn => false;
    }

    public class Bbr6 : BbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x6f, AccessMode.ZerpPageRel, 5),
        };
        protected override int Bit => 6;
        protected override bool BranchOn => false;
    }

    public class Bbr7 : BbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x7f, AccessMode.ZerpPageRel, 5),
        };
        protected override int Bit => 7;
        protected override bool BranchOn => false;
    }
    public class Bbs0 : BbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x8f, AccessMode.ZerpPageRel, 5),
        };
        protected override int Bit => 0;
        protected override bool BranchOn => true;
    }

    public class Bbs1 : BbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x9f, AccessMode.ZerpPageRel, 5),
        };
        protected override int Bit => 1;
        protected override bool BranchOn => true;
    }

    public class Bbs2 : BbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xaf, AccessMode.ZerpPageRel, 5),
        };
        protected override int Bit => 2;
        protected override bool BranchOn => true;
    }

    public class Bbs3 : BbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xbf, AccessMode.ZerpPageRel, 5),
        };
        protected override int Bit => 3;
        protected override bool BranchOn => true;
    }

    public class Bbs4 : BbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xcf, AccessMode.ZerpPageRel, 5),
        };
        protected override int Bit => 4;
        protected override bool BranchOn => true;
    }

    public class Bbs5 : BbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xdf, AccessMode.ZerpPageRel, 5),
        };
        protected override int Bit => 5;
        protected override bool BranchOn => true;
    }

    public class Bbs6 : BbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xef, AccessMode.ZerpPageRel, 5),
        };
        protected override int Bit => 6;
        protected override bool BranchOn => true;
    }

    public class Bbs7 : BbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xff, AccessMode.ZerpPageRel, 5),
        };
        protected override int Bit => 7;
        protected override bool BranchOn => false;
    }

    public abstract class MbBase : EmulatableCpuOpCode
    {
        protected abstract int Bit { get; }
        protected abstract bool SetOn { get; }

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            Debug.Assert(false); // this is untested.
            var (zp, _, _) = GetValueAtPC();

            var zpValue = memory.GetByte(zp);

            if (SetOn)
            {
                zpValue = (byte)(zpValue | (Bit switch
                {
                    0 => 0b00000001,
                    1 => 0b00000010,
                    2 => 0b00000100,
                    3 => 0b00001000,
                    4 => 0b00010000,
                    5 => 0b00100000,
                    6 => 0b01000000,
                    7 => 0b10000000,
                    _ => throw new NotImplementedException()
                }));
            }
            else
            {
                zpValue = (byte)(zpValue | (Bit switch
                {
                    0 => 0b11111110,
                    1 => 0b11111101,
                    2 => 0b11111011,
                    3 => 0b11110111,
                    4 => 0b11101111,
                    5 => 0b11011111,
                    6 => 0b10111111,
                    7 => 0b01111111,
                    _ => throw new NotImplementedException()
                }));
            }

            memory.SetByte(zp, zpValue);

            cpu.Registers.PC += 1;

            return 0;
        }
    }

    public class Rmb0 : MbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x07, AccessMode.ZeroPage, 5),
        };
        protected override int Bit => 0;
        protected override bool SetOn => false;
    }

    public class Rmb1 : MbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x17, AccessMode.ZeroPage, 5),
        };
        protected override int Bit => 1;
        protected override bool SetOn => false;
    }

    public class Rmb2 : MbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x27, AccessMode.ZeroPage, 5),
        };
        protected override int Bit => 2;
        protected override bool SetOn => false;
    }

    public class Rmb3 : MbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x37, AccessMode.ZeroPage, 5),
        };
        protected override int Bit => 3;
        protected override bool SetOn => false;
    }

    public class Rmb4 : MbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x47, AccessMode.ZeroPage, 5),
        };
        protected override int Bit => 4;
        protected override bool SetOn => false;
    }

    public class Rmb5 : MbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x57, AccessMode.ZeroPage, 5),
        };
        protected override int Bit => 5;
        protected override bool SetOn => false;
    }

    public class Rmb6 : MbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x67, AccessMode.ZeroPage, 5),
        };
        protected override int Bit => 6;
        protected override bool SetOn => false;
    }

    public class Rmb7 : MbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x77, AccessMode.ZeroPage, 5),
        };
        protected override int Bit => 7;
        protected override bool SetOn => false;
    }

    public class Smb0 : MbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x87, AccessMode.ZeroPage, 5),
        };
        protected override int Bit => 0;
        protected override bool SetOn => true;
    }

    public class Smb1 : MbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x97, AccessMode.ZeroPage, 5),
        };
        protected override int Bit => 1;
        protected override bool SetOn => true;
    }

    public class Smb2 : MbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xa7, AccessMode.ZeroPage, 5),
        };
        protected override int Bit => 2;
        protected override bool SetOn => true;
    }

    public class Smb3 : MbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xb7, AccessMode.ZeroPage, 5),
        };
        protected override int Bit => 3;
        protected override bool SetOn => true;
    }

    public class Smb4 : MbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xc7, AccessMode.ZeroPage, 5),
        };
        protected override int Bit => 4;
        protected override bool SetOn => true;
    }

    public class Smb5 : MbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xd7, AccessMode.ZeroPage, 5),
        };
        protected override int Bit => 5;
        protected override bool SetOn => true;
    }

    public class Smb6 : MbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xe7, AccessMode.ZeroPage, 5),
        };
        protected override int Bit => 6;
        protected override bool SetOn => true;
    }

    public class Smb7 : MbBase
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xf7, AccessMode.ZeroPage, 5),
        };
        protected override int Bit => 7;
        protected override bool SetOn => true;
    }

    public class Wai : EmulatableCpuOpCode
    {
        internal override List<(uint OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xcb, AccessMode.Implied, 3),
        };

        public override int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu)
        {
            throw new NotImplementedException();
        }
    }

}

