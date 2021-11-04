using BitMagic.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Cpu._6502
{

    public class WDC65c02 : ICpu
    {
        public IEnumerable<ICpuOpCode> OpCodes => _opCodes;

        private ICpuOpCode[] _opCodes = new ICpuOpCode[]
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
            new Pla(),
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
            new Stx(),
            new Sty(),
            new Tax(),
            new Tay(),
            new Tsx(),
            new Txa(),
            new Txs(),
            new Tya()
        };

    }

    public class Adc : CpuOpCode
    {
        public override string Code => "ADC";

        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new () {
            (0x69, AccessMode.Immediate, 2),
            (0x65, AccessMode.ZeroPage, 3),
            (0x75, AccessMode.ZeroPageX, 4),
            (0x6d, AccessMode.Absolute, 4),
            (0x7d, AccessMode.AbsoluteX, 4),
            (0x79, AccessMode.AbsoluteY, 4),
            (0x61, AccessMode.IndirectX, 6),
            (0x71, AccessMode.IndirectY, 5),
        };
    }

    public class And : CpuOpCode
    {
        public override string Code => "AND";

        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new () {
            (0x29, AccessMode.Immediate, 2),
            (0x25, AccessMode.ZeroPage, 3),
            (0x35, AccessMode.ZeroPageX, 4),
            (0x2d, AccessMode.Absolute, 4),
            (0x3d, AccessMode.AbsoluteX, 4),
            (0x39, AccessMode.AbsoluteY, 4),
            (0x21, AccessMode.IndirectX, 6),
            (0x31, AccessMode.IndirectY, 5),
        };
    }

    public class Asl : CpuOpCode
    {
        public override string Code => "ASL";

        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new () {
            (0x0a, AccessMode.Accumulator, 2),
            (0x06, AccessMode.ZeroPage, 5),
            (0x16, AccessMode.ZeroPageX, 6),
            (0x0e, AccessMode.Absolute, 6),
            (0x1e, AccessMode.AbsoluteX, 7),
        };
    }
    public class Bit : CpuOpCode
    {
        public override string Code => "BIT";

        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new () {
            (0x24, AccessMode.ZeroPage, 3),
            (0x2c, AccessMode.Absolute, 4),
        };
    }

    public class Bpl : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new () {
            (0x10, AccessMode.Relative, 2)
        };
    }

    public class Bmi : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new() {
            (0x30, AccessMode.Relative, 2)
        };
    }
    public class Bvc : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x50, AccessMode.Relative, 2)
        };
    }
    public class Bvs : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x70, AccessMode.Relative, 2)
        };
    }
    public class Bcc : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x90, AccessMode.Relative, 2)
        };
    }
    public class Bcs : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xb0, AccessMode.Relative, 2)
        };
    }
    public class Bne : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xd0, AccessMode.Relative, 2)
        };
    }
    public class Beq : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xf0, AccessMode.Relative, 2)
        };
    }
    public class Brk : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x00, AccessMode.Implied, 7)
        };
    }
    public class Cmp : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xc9, AccessMode.Immediate, 2),
            (0xc5, AccessMode.ZeroPage, 3),
            (0xd5, AccessMode.ZeroPageX, 4),
            (0xcd, AccessMode.Absolute, 4),
            (0xdd, AccessMode.AbsoluteX, 4),
            (0xd9, AccessMode.AbsoluteY, 4),
            (0xc1, AccessMode.IndirectX, 6),
            (0xd1, AccessMode.IndirectY, 5),
        };
    }

    public class Cpx : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xc9, AccessMode.Immediate, 2),
            (0xc5, AccessMode.ZeroPage, 3),
            (0xcd, AccessMode.Absolute, 4),
        };
    }
    public class Cpy : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xc9, AccessMode.Immediate, 2),
            (0xc5, AccessMode.ZeroPage, 3),
            (0xcd, AccessMode.Absolute, 4),
        };
    }

    public class Dec : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xc6, AccessMode.ZeroPage, 5),
            (0xd6, AccessMode.ZeroPageX, 6),
            (0xce, AccessMode.Absolute, 6),
            (0xde, AccessMode.AbsoluteX, 7),
        };
    }
    public class Eor : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x49, AccessMode.Immediate, 2),
            (0x45, AccessMode.ZeroPage, 3),
            (0x55, AccessMode.ZeroPageX, 4),
            (0x4d, AccessMode.Absolute, 4),
            (0x5d, AccessMode.AbsoluteX, 4),
            (0x59, AccessMode.AbsoluteY, 4),
            (0x41, AccessMode.IndirectX, 6),
            (0x51, AccessMode.IndirectY, 5),
        };
    }
    public class Clc : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x18, AccessMode.Implied, 2)
        };
    }
    public class Sec : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x38, AccessMode.Implied, 2)
        };
    }
    public class Cli : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x58, AccessMode.Implied, 2)
        };
    }
    public class Sei : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x78, AccessMode.Implied, 2)
        };
    }
    public class Clv : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xb8, AccessMode.Implied, 2)
        };
    }
    public class Cld : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xd8, AccessMode.Implied, 2)
        };
    }
    public class Sed : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xf8, AccessMode.Implied, 2)
        };
    }
    public class Inc : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xe6, AccessMode.ZeroPage, 5),
            (0xf6, AccessMode.ZeroPageX, 6),
            (0xee, AccessMode.Absolute, 6),
            (0xfe, AccessMode.AbsoluteX, 7),
        };
    }

    public class Jmp : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x4c, AccessMode.Absolute, 6),
            (0x6c, AccessMode.Indirect, 5),
        };
    }

    public class Jsr : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x20, AccessMode.Absolute, 6),
        };
    }
    public class Lda : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xa9, AccessMode.Immediate, 2),
            (0xa5, AccessMode.ZeroPage, 3),
            (0xb5, AccessMode.ZeroPageX, 4),
            (0xad, AccessMode.Absolute, 4),
            (0xbd, AccessMode.AbsoluteX, 4),
            (0xb9, AccessMode.AbsoluteY, 4),
            (0xa1, AccessMode.IndirectX, 6),
            (0xb1, AccessMode.IndirectY, 5),
        };
    }
    public class Ldx : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xa2, AccessMode.Immediate, 2),
            (0xe6, AccessMode.ZeroPage, 3),
            (0xb6, AccessMode.ZeroPageY, 4),
            (0xae, AccessMode.Absolute, 4),
            (0xbe, AccessMode.AbsoluteY, 4),
        };
    }

    public class Ldy : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xa0, AccessMode.Immediate, 2),
            (0xe4, AccessMode.ZeroPage, 3),
            (0xb4, AccessMode.ZeroPageX, 4),
            (0xac, AccessMode.Absolute, 4),
            (0xbc, AccessMode.AbsoluteX, 4),
        };
    }

    public class Lsr : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x4a, AccessMode.Accumulator, 2),
            (0x46, AccessMode.ZeroPage, 5),
            (0x56, AccessMode.ZeroPageX, 6),
            (0x4e, AccessMode.Absolute, 6),
            (0x5e, AccessMode.AbsoluteX, 7),
        };
    }

    public class Nop : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xea, AccessMode.Implied, 2),
        };
    }
    public class Ora : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x09, AccessMode.Immediate, 2),
            (0x05, AccessMode.ZeroPage, 3),
            (0x15, AccessMode.ZeroPageX, 4),
            (0x0d, AccessMode.Absolute, 4),
            (0x1d, AccessMode.AbsoluteX, 4),
            (0x19, AccessMode.AbsoluteY, 4),
            (0x01, AccessMode.IndirectX, 6),
            (0x11, AccessMode.IndirectY, 5),
        };
    }
    public class Rol : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x2a, AccessMode.Accumulator, 2),
            (0x26, AccessMode.ZeroPage, 5),
            (0x36, AccessMode.ZeroPageX, 6),
            (0x2e, AccessMode.Absolute, 6),
            (0x3e, AccessMode.AbsoluteX, 7),
        };
    }
    public class Ror : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x6a, AccessMode.Accumulator, 2),
            (0x66, AccessMode.ZeroPage, 5),
            (0x76, AccessMode.ZeroPageX, 6),
            (0x6e, AccessMode.Absolute, 6),
            (0x7e, AccessMode.AbsoluteX, 7),
        };
    }
    public class Rti : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x40, AccessMode.Implied, 6),
        };
    }
    public class Rts : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x60, AccessMode.Implied, 6),
        };
    }
    public class Sbc : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xe9, AccessMode.Immediate, 2),
            (0xe5, AccessMode.ZeroPage, 3),
            (0xf5, AccessMode.ZeroPageX, 4),
            (0xed, AccessMode.Absolute, 4),
            (0xfd, AccessMode.AbsoluteX, 4),
            (0xf9, AccessMode.AbsoluteY, 4),
            (0xe1, AccessMode.IndirectX, 6),
            (0xf1, AccessMode.IndirectY, 5),
        };
    }
    public class Sta : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x85, AccessMode.ZeroPage, 3),
            (0x95, AccessMode.ZeroPageX, 4),
            (0x8d, AccessMode.Absolute, 4),
            (0x9d, AccessMode.AbsoluteX, 4),
            (0x99, AccessMode.AbsoluteY, 4),
            (0x81, AccessMode.IndirectX, 6),
            (0x91, AccessMode.IndirectY, 5),
        };
    }
    public class Stx : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x86, AccessMode.ZeroPage, 3),
            (0x96, AccessMode.ZeroPageY, 4),
            (0x8e, AccessMode.Absolute, 4),
        };
    }
    public class Sty : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x84, AccessMode.ZeroPage, 3),
            (0x94, AccessMode.ZeroPageX, 4),
            (0x8c, AccessMode.Absolute, 4),
        };
    }
    public class Tax : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xaa, AccessMode.Implied, 2),
        };
    }
    public class Txa : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x8a, AccessMode.Implied, 2),
        };
    }
    public class Dex : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xca, AccessMode.Implied, 2),
        };
    }
    public class Inx : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xe8, AccessMode.Implied, 2),
        };
    }
    public class Tay : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xa8, AccessMode.Implied, 2),
        };
    }
    public class Tya : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x98, AccessMode.Implied, 2),
        };
    }
    public class Dey : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x88, AccessMode.Implied, 2),
        };
    }
    public class Iny : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xc8, AccessMode.Implied, 2),
        };
    }
    public class Txs : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x9a, AccessMode.Implied, 2),
        };
    }
    public class Tsx : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0xba, AccessMode.Implied, 2),
        };
    }
    public class Pha : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x48, AccessMode.Implied, 2),
        };
    }
    public class Pla : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x68, AccessMode.Implied, 2),
        };
    }
    public class Php : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x08, AccessMode.Implied, 2),
        };
    }
    public class Plp : CpuOpCode
    {
        internal override List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes => new()
        {
            (0x28, AccessMode.Implied, 2),
        };
    }
}

