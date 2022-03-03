# 4510 Instructions

## Credits and License

All text derived from the [Mega65 User Guide](https://github.com/MEGA65/mega65-user-guide). Copyright 2019-2021 by Paul Gardner-Stephen, the MEGA Museum of Electronic Games & Art e.V., and contributors.

This reference guide is made available under the GNU Free Documentation License
v1.3, or later, if desired. This means that you are free to modify, reproduce and redistribute this reference guide, subject to certain conditions. The full text of the GNU
Free Documentation License v1.3 can be found at [https://www.gnu.org/licenses/fdl-1.3.en.html](https://www.gnu.org/licenses/fdl-1.3.en.html).

## All Instructions

| |  |  |  |  |  |  |  |  |  |  |  |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| [ADC](#ADC) | [AND](#AND) | [ASL](#ASL) | [ASR](#ASR) | [ASW](#ASW) | [BBR0](#BBR0) | [BBR1](#BBR1) | [BBR2](#BBR2) | [BBR3](#BBR3) | [BBR4](#BBR4) | [BBR5](#BBR5) | [BBR6](#BBR6) |
| [BBR7](#BBR7) | [BBS0](#BBS0) | [BBS1](#BBS1) | [BBS2](#BBS2) | [BBS3](#BBS3) | [BBS4](#BBS4) | [BBS5](#BBS5) | [BBS6](#BBS6) | [BBS7](#BBS7) | [BCC](#BCC) | [BCS](#BCS) | [BEQ](#BEQ) |
| [BIT](#BIT) | [BMI](#BMI) | [BNE](#BNE) | [BPL](#BPL) | [BRA](#BRA) | [BRK](#BRK) | [BSR](#BSR) | [BVC](#BVC) | [BVS](#BVS) | [CLC](#CLC) | [CLD](#CLD) | [CLE](#CLE) |
| [CLI](#CLI) | [CLV](#CLV) | [CMP](#CMP) | [CPX](#CPX) | [CPY](#CPY) | [CPZ](#CPZ) | [DEC](#DEC) | [DEW](#DEW) | [DEX](#DEX) | [DEY](#DEY) | [DEZ](#DEZ) | [EOM](#EOM) |
| [EOR](#EOR) | [INC](#INC) | [INW](#INW) | [INX](#INX) | [INY](#INY) | [INZ](#INZ) | [JMP](#JMP) | [JSR](#JSR) | [LDA](#LDA) | [LDX](#LDX) | [LDY](#LDY) | [LDZ](#LDZ) |
| [LSR](#LSR) | [MAP](#MAP) | [NEG](#NEG) | [ORA](#ORA) | [PHA](#PHA) | [PHP](#PHP) | [PHW](#PHW) | [PHX](#PHX) | [PHY](#PHY) | [PHZ](#PHZ) | [PLA](#PLA) | [PLP](#PLP) |
| [PLX](#PLX) | [PLY](#PLY) | [PLZ](#PLZ) | [RMB0](#RMB0) | [RMB1](#RMB1) | [RMB2](#RMB2) | [RMB3](#RMB3) | [RMB4](#RMB4) | [RMB5](#RMB5) | [RMB6](#RMB6) | [RMB7](#RMB7) | [ROL](#ROL) |
| [ROR](#ROR) | [ROW](#ROW) | [RTI](#RTI) | [RTS](#RTS) | [SBC](#SBC) | [SEC](#SEC) | [SED](#SED) | [SEE](#SEE) | [SEI](#SEI) | [SMB0](#SMB0) | [SMB1](#SMB1) | [SMB2](#SMB2) |
| [SMB3](#SMB3) | [SMB4](#SMB4) | [SMB5](#SMB5) | [SMB6](#SMB6) | [SMB7](#SMB7) | [STA](#STA) | [STX](#STX) | [STY](#STY) | [STZ](#STZ) | [TAB](#TAB) | [TAX](#TAX) | [TAY](#TAY) |
| [TAZ](#TAZ) | [TBA](#TBA) | [TRB](#TRB) | [TSB](#TSB) | [TSX](#TSX) | [TSY](#TSY) | [TXA](#TXA) | [TXS](#TXS) | [TYA](#TYA) | [TYS](#TYS) | [TZA](#TZA) |

## ADC

**Add with carry**\
A <- A+M+C\
Flags: N Z C V
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Immediate | ADC #\$nn | $69 | 2 | 2<sup></sup> |
 | Base Page | ADC \$nn | $65 | 2 | 3<sup>r</sup> |
 | Base Page, X | ADC \$nn,X | $75 | 2 | 3<sup>r</sup> |
 | Absolute | ADC \$nnnn | $6D | 3 | 4<sup>r</sup> |
 | Absolute, X | ADC \$nnnn,X | $7D | 3 | 4<sup>r</sup> |
 | Absolute, Y | ADC \$nnnn,Y | $79 | 3 | 4<sup>r</sup> |
 | Indirect, X | ADC (\$nn,X) | $61 | 2 | 5<sup>r</sup> |
 | Indirect, Y | ADC (\$nn),Y | $71 | 2 | 5<sup>pr</sup> |
 | Indirect, Z | ADC (\$nn),Z | $72 | 2 | 5<sup>pr</sup> |

 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction adds the argument to the contents of the Accumulator
Register and the Carry Flag.
If the D flag is set, then the addition is performed using Binary
Coded Decimal.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.
- The V flag will be set if the result has a different sign to both of the
arguments, else it will be cleared. If the flag is set, this
indicates that a signed overflow has occurred.
- The C flag will be set if the unsigned result is >255, or >99 if the D flag is set.

## AND

**Binary AND**\
A <- A & M\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Immediate | AND #\$nn | $29 | 2 | 2<sup></sup> |
 | Base Page | AND \$nn | $25 | 2 | 3<sup>r</sup> |
 | Base Page, X | AND \$nn,X | $35 | 2 | 4<sup>pr</sup> |
 | Absolute | AND \$nnnn | $2D | 3 | 4<sup>r</sup> |
 | Absolute, X | AND \$nnnn,X | $3D | 3 | 4<sup>pr</sup> |
 | Absolute, Y | AND \$nnnn,Y | $39 | 3 | 4<sup>r</sup> |
 | Indirect, X | AND (\$nn,X) | $21 | 2 | 5<sup>pr</sup> |
 | Indirect, Y | AND (\$nn),Y | $31 | 2 | 5<sup>pr</sup> |
 | Indirect, Z | AND (\$nn),Z | $32 | 2 | 5<sup>pr</sup> |

 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instructions performs a binary AND operation of the argument with the
accumulator, and stores the result in the accumulator. Only bits that were
already set in the accumulator, and that are set in the argument will be set
in the accumulator on completion.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## ASL

**Arithmetic Shift Left**\
A <- A<<1 | M <- M<<1\
Flags: N Z C 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Accumulator | ASL A | $0A | 1 | 1<sup></sup> |
 | Base Page | ASL \$nn | $06 | 2 | 4<sup>r</sup> |
 | Base Page, X | ASL \$nn,X | $16 | 2 | 4<sup>r</sup> |
 | Absolute | ASL \$nnnn | $0E | 3 | 5<sup>r</sup> |
 | Absolute, X | ASL \$nnnn,X | $1E | 3 | 5<sup>pr</sup> |

 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction shifts either the Accumulator or contents
of the provided memory location one bit left.  Bit 0 will be
set to zero, and the bit 7 will be shifted out into the Carry Flag

Side effects:

- The N flag will be set if the result is negative, i.e., if bit 7 is set after the operation, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.
- The C flag will be set if bit 7 of the value was set, prior to being shifted.

## ASR

**Arithmetic Shift Right**\
A <- A>>1 | M <- M>>1\
Flags: N Z C 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Accumulator | ASR A | $43 | 1 | 1<sup></sup> |
 | Base Page | ASR \$nn | $44 | 2 | 4<sup>r</sup> |
 | Base Page, X | ASR \$nn,X | $54 | 2 | 5<sup>pr</sup> |

 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction shifts either the Accumulator or contents
of the provided memory location one bit right.  Bit 7 is considered
to be a sign bit, and is preserved.
The contents of bit 0 will be shifted out into the Carry Flag

Side effects:

- The N flag will be set if the result is negative, i.e., if bit 7 is set after the operation, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.
- The C flag will be set if bit 0 of the value was set, prior to being shifted.

## ASW

**Arithmetic Shift Word Left**\
M <- M<<1\
Flags: N Z C   
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Absolute | ASW \$nnnn | $CB | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction shifts a 16-bit value in memory left one bit.

For example, if location \$1234 contained \$87 and location \$1235
contained \$A9, ASW \$1234 would result in location \$1234 containing
\$0E and location \$1235 containing \$53, and the Carry Flag being set.

Side effects:

- The N flag will be set if the result is negative, i.e.,
if bit 7 of the upper byte is set after the operation, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.
- The C flag will be set if bit 7 of the upper byte was set, prior to being shifted.

## BBR0

**Branch on Bit 0 Reset**\
PC <- PC + R8\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page, Relative | BBR0 \$nn,\$rr | $0F | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction branches to the indicated address if
bit 0 is clear
in the indicated base-page memory location.

## BBR1

**Branch on Bit 1 Reset**\
PC <- PC + R8\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page, Relative | BBR1 \$nn,\$rr | $1F | 3 | 5<sup>b</sup> |

 <sup>b</sup> Add one cycle if branch is taken.

This instruction branches to the indicated address if
bit 1 is clear
in the indicated base-page memory location.

## BBR2

**Branch on Bit 2 Reset**\
PC <- PC + R8\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page, Relative | BBR2 \$nn,\$rr | $2F | 3 | 5<sup>b</sup> |

 <sup>b</sup> Add one cycle if branch is taken.

This instruction branches to the indicated address if
bit 2 is clear
in the indicated base-page memory location.

## BBR3

**Branch on Bit 3 Reset**\
PC <- PC + R8\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page, Relative | BBR3 \$nn,\$rr | $3F | 3 | 4<sup>b</sup> |

 <sup>b</sup> Add one cycle if branch is taken.

This instruction branches to the indicated address if
bit 3 is clear
in the indicated base-page memory location.

## BBR4

**Branch on Bit 4 Reset**\
PC <- PC + R8\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page, Relative | BBR4 \$nn,\$rr | $4F | 3 | 4<sup>br</sup> |

 <sup>b</sup> Add one cycle if branch is taken.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction branches to the indicated address if
bit 4 is clear
in the indicated base-page memory location.

## BBR5

**Branch on Bit 5 Reset**\
PC <- PC + R8\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page, Relative | BBR5 \$nn,\$rr | $5F | 3 | 4<sup>br</sup> |

 <sup>b</sup> Add one cycle if branch is taken.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction branches to the indicated address if
bit 5 is clear
in the indicated base-page memory location.

## BBR6

**Branch on Bit 6 Reset**\
PC <- PC + R8\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page, Relative | BBR6 \$nn,\$rr | $6F | 3 | 4<sup>br</sup> |

 <sup>b</sup> Add one cycle if branch is taken.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction branches to the indicated address if
bit 6 is clear
in the indicated base-page memory location.

## BBR7

**Branch on Bit 7 Reset**\
PC <- PC + R8\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page, Relative | BBR7 \$nn,\$rr | $7F | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction branches to the indicated address if
bit 7 is clear
in the indicated base-page memory location.

## BBS0

**Branch on Bit 0 Set**\
PC <- PC + R8\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page, Relative | BBS0 \$nn,\$rr | $8F | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction branches to the indicated address if
bit 0 is set
in the indicated base-page memory location.

## BBS1

**Branch on Bit 1 Set**\
PC <- PC + R8\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page, Relative | BBS1 \$nn,\$rr | $9F | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction branches to the indicated address if
bit 1 is set
in the indicated base-page memory location.

## BBS2

**Branch on Bit 2 Set**\
PC <- PC + R8\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page, Relative | BBS2 \$nn,\$rr | $AF | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction branches to the indicated address if
bit 2 is set
in the indicated base-page memory location.

## BBS3

**Branch on Bit 3 Set**\
PC <- PC + R8\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page, Relative | BBS3 \$nn,\$rr | $BF | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction branches to the indicated address if
bit 3 is set
in the indicated base-page memory location.

## BBS4

**Branch on Bit 4 Set**\
PC <- PC + R8\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page, Relative | BBS4 \$nn,\$rr | $CF | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction branches to the indicated address if
bit 4 is set
in the indicated base-page memory location.

## BBS5

**Branch on Bit 5 Set**\
PC <- PC + R8\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page, Relative | BBS5 \$nn,\$rr | $DF | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction branches to the indicated address if
bit 5 is set
in the indicated base-page memory location.

## BBS6

**Branch on Bit 6 Set**\
PC <- PC + R8\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page, Relative | BBS6 \$nn,\$rr | $EF | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction branches to the indicated address if
bit 6 is set
in the indicated base-page memory location.

## BBS7

**Branch on Bit 7 Set**\
PC <- PC + R8\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page, Relative | BBS7 \$nn,\$rr | $FF | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction branches to the indicated address if
bit 7 is set
in the indicated base-page memory location.

## BCC

**Branch on Carry Flag Clear**\
6502:PC <- PC + R8\
4510:PC <- PC + R8 | PC <- PC + R16\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Relative | BCC \$rr | $90 | 2 | <sup>?</sup> |
 | Relative Word | BCC \$rrrr | $93 | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction branches to the indicated address if the
Carry Flag is clear.

## BCS

**Branch on Carry Flag Set**\
6502:PC <- PC + R8\
4510:PC <- PC + R8 | PC <- PC + R16\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Relative | BCS \$rr | $B0 | 2 | <sup>?</sup> |
 | Relative Word | BCS \$rrrr | $B3 | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction branches to the indicated address if the
Carry Flag is set.

## BEQ

**Branch on Zero Flag Set**\
6502:PC <- PC + R8\
4510:PC <- PC + R8 | PC <- PC + R16\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Relative | BEQ \$rr | $F0 | 2 | <sup>?</sup> |
 | Relative Word | BEQ \$rrrr | $F3 | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction branches to the indicated address if the
Zero Flag is set.

## BIT

**Perform Bit Test**\
N <- M(7), V <- M(6), Z <- A & M\
Flags: N Z V
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Immediate | BIT #\$nn | $89 | 2 | <sup>?</sup> |
 | Base Page | BIT \$nn | $24 | 2 | 3<sup>r</sup> |
 | Base Page, X | BIT \$nn,X | $34 | 2 | 3<sup>pr</sup> |
 | Absolute | BIT \$nnnn | $2C | 3 | 4<sup>r</sup> |
 | Absolute, X | BIT \$nnnn,X | $3C | 3 | 4<sup>pr</sup> |

 <sup>?</sup> Cycles not in source documentation.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction is used to test the bits stored in a memory location.
Bits 6 and 7 of the memory location's contents are directly copied into
the Overflow Flag and Negative Flag. The Zero Flag is set or cleared
based on the result of performing the binary AND of the Accumulator Register
and the contents of the indicated memory location.

Side effects:

- The N flag will be set if the bit 7 of the memory location is set, else it will be cleared.
- The V flag will be set if the bit 6 of the memory location is set, else it will be cleared.
- The Z flag will be set if the result of A & M is zero, else it will be cleared.

## BMI

**Branch on Negative Flag Set**\
6502:PC <- PC + R8\
4510:PC <- PC + R8 | PC <- PC + R16\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Relative | BMI \$rr | $30 | 2 | 2<sup>r</sup> |
 | Relative Word | BMI \$rrrr | $33 | 3 | 3<sup>b</sup> |

 <sup>b</sup> Add one cycle if branch is taken.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction branches to the indicated address if the
Negative Flag is set.

## BNE

**Branch on Zero Flag Clear**\
6502:PC <- PC + R8\
4510:PC <- PC + R8 | PC <- PC + R16\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Relative | BNE \$rr | $D0 | 2 | <sup>?</sup> |
 | Relative Word | BNE \$rrrr | $D3 | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction branches to the indicated address if the
Zero Flag is clear.

## BPL

**Branch on Negative Flag Clear**\
6502:PC <- PC + R8\
4510:PC <- PC + R8 | PC <- PC + R16\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Relative | BPL \$rr | $10 | 2 | 2<sup>b</sup> |
 | Relative Word | BPL \$rrrr | $13 | 3 | 3<sup>b</sup> |

 <sup>b</sup> Add one cycle if branch is taken.

This instruction branches to the indicated address if the
Negative Flag is clear.

## BRA

**Branch Unconditionally**\
PC <- PC + R8 | PC <- PC + R16\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Relative | BRA \$rr | $80 | 2 | <sup>?</sup> |
 | Relative Word | BRA \$rrrr | $83 | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction branches to the indicated address.

## BRK

**Break to Interrupt**\
PC <- (\$FFFE)\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | BRK  | $00 | 1 | 7<sup></sup> |

The break command causes the microprocessor to go through an
interrupt sequence under program control.
The address of the BRK instruction + 2 is pushed to the stack
along with the status register with the Break flag set.
This allows the interrupt service routine to distinguish
between IRQ events and BRK events. For example:

```assembly
PLA           ; load status
PHA           ; restore stack
AND #$10      ; mask break flag
BNE DO_BREAK  ; -> it was a BRK
...           ; else continue with IRQ server
```

Cite from: MCS6500 Microcomputer Family Programming Manual,
January 1976, Second Edition, MOS Technology Inc., Page 144:

"The BRK is a single byte instruction and its addressing mode is Implied."

There are debates, that BRK could be seen as a two byte instruction
with the addressing mode immediate, where the operand byte is discarded.
The byte following the BRK could then be used as a call argument for
the break handler. Commodore however used the BRK, as stated in the manual,
as a single byte instruction, which breaks into the ML monitor, if present.
These builtin monitors decremented the stacked PC, so that it could
be used to return or jump directly to the code byte after the BRK.

## BSR

**Branch Sub-Routine**\
PC <- PC + R8 | PC <- PC + R16\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Relative Word | BSR \$rrrr | $63 | 3 | 3<sup>b</sup> |

 <sup>b</sup> Add one cycle if branch is taken.

This instruction branches to the indicated address, saving
the address of the caller on the stack, so that the routine
can be returned from using an RTS instruction.

This instruction is helpful for using relocatable code, as it
provides a relative-addressed alternative to JSR.

## BVC

**Branch on Overflow Flag Clear**\
6502:PC <- PC + R8\
4510:PC <- PC + R8 | PC <- PC + R16\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Relative | BVC \$rr | $50 | 2 | 2<sup>b</sup> |
 | Relative Word | BVC \$rrrr | $53 | 3 | 3<sup>b</sup> |

 <sup>b</sup> Add one cycle if branch is taken.

This instruction branches to the indicated address if the
Overflow (V) Flag is clear.

## BVS

**Branch on Overflow Flag Set**\
6502:PC <- PC + R8\
4510:PC <- PC + R8 | PC <- PC + R16\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Relative | BVS \$rr | $70 | 2 | 2<sup>b</sup> |
 | Relative Word | BVS \$rrrr | $73 | 3 | 3<sup>b</sup> |

 <sup>b</sup> Add one cycle if branch is taken.

This instruction branches to the indicated address if the
Overflow (V) Flag is set.

## CLC

**Clear Carry Flag**\
C <- 0\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | CLC  | $18 | 1 | 1<sup></sup> |

This instruction clears the Carry Flag.

Side effects:

- The C flag is cleared.

## CLD

**Clear Decimal Flag**\
D <- 0\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | CLD  | $D8 | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction clears the Decimal Flag.
Arithmetic operations will use normal binary arithmetic, instead of
Binary-Coded Decimal (BCD).

Side effects:

- The D flag is cleared.

## CLE

**Clear Extended Stack Disable Flag**\
E <- 0\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | CLE  | $02 | 1 | 1<sup></sup> |

This instruction clears the Extended Stack Disable Flag.
This causes the stack to be able to exceed 256 bytes in
length, by allowing the processor to modify the value of the
high byte of the stack address (SPH).

Side effects:

- The E flag is cleared.

## CLI

**Clear Interrupt Disable Flag**\
I <- 0\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | CLI  | $58 | 1 | 1<sup></sup> |

This instruction clears the Interrupt Disable Flag.
Interrupts will now be able to occur.

Side effects:

- The I flag is cleared.

## CLV

**Clear Overflow Flag**\
V <- 0\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | CLV  | $B8 | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction clears the Overflow Flag.

Side effects:

- The V flag is cleared.

## CMP

**Compare Accumulator**\
Flags: N Z C
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Immediate | CMP #\$nn | $C9 | 2 | <sup>?</sup> |
 | Base Page | CMP \$nn | $C5 | 2 | <sup>?</sup> |
 | Base Page, X | CMP \$nn,X | $D5 | 2 | <sup>?</sup> |
 | Absolute | CMP \$nnnn | $CD | 3 | <sup>?</sup> |
 | Absolute, X | CMP \$nnnn,X | $DD | 3 | <sup>?</sup> |
 | Absolute, Y | CMP \$nnnn,Y | $D9 | 3 | <sup>?</sup> |
 | Indirect, X | CMP (\$nn,X) | $C1 | 2 | <sup>?</sup> |
 | Indirect, Y | CMP (\$nn),Y | $D1 | 2 | <sup>?</sup> |
 | Indirect, Z | CMP (\$nn),Z | $D2 | 2 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction performs A - M, and sets the processor flags accordingly,
but does not modify the contents of the Accumulator Register.

Side effects:

- The N flag will be set if the result of A - M is negative, i.e. bit 7 is set in the result, else it will be cleared.
- The C flag will be set if the result of A - M is zero or positive, i.e., if A is not less than M, else it will be cleared.
- The Z flag will be set if the result of A - M is zero, else it will be cleared.

## CPX

**Compare X Register**\
Flags: N Z C
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Immediate | CPX #\$nn | $E0 | 2 | <sup>?</sup> |
 | Base Page | CPX \$nn | $E4 | 2 | <sup>?</sup> |
 | Absolute | CPX \$nnnn | $EC | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction performs X - M, and sets the processor flags accordingly,
but does not modify the contents of the Accumulator Register.

Side effects:

- The N flag will be set if the result of X - M is negative, i.e. bit 7 is set in the result, else it will be cleared.
- The C flag will be set if the result of X - M is zero or positive, i.e., if X is not less than M, else it will be cleared.
- The Z flag will be set if the result of X - M is zero, else it will be cleared.

## CPY

**Compare Y Register**\
Flags: N Z C
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Immediate | CPY #\$nn | $C0 | 2 | <sup>?</sup> |
 | Base Page | CPY \$nn | $C4 | 2 | <sup>?</sup> |
 | Absolute | CPY \$nnnn | $CC | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction performs Y - M, and sets the processor flags accordingly,
but does not modify the contents of the Accumulator Register.

Side effects:

- The N flag will be set if the result of Y - M is negative, i.e. bit 7 is set in the result, else it will be cleared.
- The C flag will be set if the result of Y - M is zero or positive, i.e., if Y is not less than M, else it will be cleared.
- The Z flag will be set if the result of Y - M is zero, else it will be cleared.

## CPZ

**Compare Z Register**\
Flags: N Z C
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Immediate | CPZ #\$nn | $C2 | 2 | <sup>?</sup> |
 | Base Page | CPZ \$nn | $D4 | 2 | <sup>?</sup> |
 | Absolute | CPZ \$nnnn | $DC | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction performs Z - M, and sets the processor flags accordingly,
but does not modify the contents of the Accumulator Register.

Side effects:

- The N flag will be set if the result of Z - M is negative, i.e. bit 7 is set in the result, else it will be cleared.
- The C flag will be set if the result of Z - M is zero or positive, i.e., if Z is not less than M, else it will be cleared.
- The Z flag will be set if the result of Z - M is zero, else it will be cleared.

## DEC

**Decrement Memory or Accumulator**\
A <- A - 1 | M <- M - 1\
Flags: N Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Accumulator | DEC A | $3A | 1 | 1<sup></sup> |
 | Base Page | DEC \$nn | $C6 | 2 | <sup>?</sup> |
 | Base Page, X | DEC \$nn,X | $D6 | 2 | <sup>?</sup> |
 | Absolute | DEC \$nnnn | $CE | 3 | <sup>?</sup> |
 | Absolute, X | DEC \$nnnn,X | $DE | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction decrements the Accumulator Register or indicated
memory location.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## DEW

**Decrement Memory Word**\
M16 <- M16 - 1\
Flags: N Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | DEW \$nn | $C3 | 2 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction decrements the indicated memory word in the Base Page.  The low numbered
address contains the least significant bits. For example, if memory location \$12
contains \$78 and memory location \$13 contains \$56, the instruction DEW \$12
would cause memory location to be set to \$77.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## DEX

**Decrement X Register**\
X <- X - 1\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | DEX  | $CA | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction decrements the X Register.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## DEY

**Decrement Y Register**\
Y <- Y - 1\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | DEY  | $88 | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction decrements the Y Register.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## DEZ

**Decrement Z Register**\
Z <- Z - 1\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | DEZ  | $3B | 1 | 1<sup></sup> |

This instruction decrements the Z Register.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## EOM

**End of Mapping Sequence / No-Operation**\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | EOM  | $EA | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

In contrast with the 6502, the NOP instruction on the 45GS02 performs two
additional roles when in 4502 mode.

First, indicate the end of a memory mapping sequence
caused by a MAP instruction, allowing interrupts to occur again.

Second, it instructs the processor that if the following instruction uses
Base-Page Indirect Z Indexed addressing, that the processor should use a 32-bit
pointer instead of a 16-bit 6502 style pointer.  Such 32-bit addresses are unaffected
by C64, C65 or MEGA65 memory banking.  This allows fast and easy access
to the entire address space of the MEGA65 without having to perform or be aware of any banking,
or using the DMA controller.  This addressing mode causes a two cycle penalty,
caused by the time required to read the extra two bytes of the pointer.

Side effects:

- Removes the prohibition on all interrupts caused by the the MAP instruction,
allowing Non-Maskable Interrupts to again occur, and IRQ interrupts,
if the Interrupt Disable Flag
is not set.

## EOR

**Binary Exclusive OR**\
A <- A xor M\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Immediate | EOR #\$nn | $49 | 2 | 2<sup></sup> |
 | Base Page | EOR \$nn | $45 | 2 | 3<sup>r</sup> |
 | Base Page, X | EOR \$nn,X | $55 | 2 | 3<sup>p</sup> |
 | Absolute | EOR \$nnnn | $4D | 3 | 4<sup>r</sup> |
 | Absolute, X | EOR \$nnnn,X | $5D | 3 | 4<sup>pr</sup> |
 | Absolute, Y | EOR \$nnnn,Y | $59 | 3 | 4<sup>pr</sup> |
 | Indirect, X | EOR (\$nn,X) | $41 | 2 | 5<sup>r</sup> |
 | Indirect, Y | EOR (\$nn),Y | $51 | 2 | 5<sup>pr</sup> |
 | Indirect, Z | EOR (\$nn),Z | $52 | 2 | 5<sup>pr</sup> |

 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instructions performs a binary XOR operation of the argument with the
accumulator, and stores the result in the accumulator. Only bits that were
already set in the accumulator, or that are set in the argument will be set
in the accumulator on completion, but not both.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## INC

**Increment Memory or Accumulator**\
A <- A + 1 | M <- M + 1\
Flags: N Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Accumulator | INC A | $1A | 1 | 1<sup></sup> |
 | Base Page | INC \$nn | $E6 | 2 | <sup>?</sup> |
 | Base Page, X | INC \$nn,X | $F6 | 2 | <sup>?</sup> |
 | Absolute | INC \$nnnn | $EE | 3 | <sup>?</sup> |
 | Absolute, X | INC \$nnnn,X | $FE | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction increments the Accumulator Register or indicated
memory location.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## INW

**Increment Memory Word**\
M16 <- M16 + 1\
Flags: N Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | INW \$nn | $E3 | 2 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction increments the indicated memory word in the Base Page.  The low numbered
address contains the least significant bits. For example, if memory location \$12
contains \$78 and memory location \$13 contains \$56, the instruction DEW \$12
would cause memory location to be set to \$79.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## INX

**Increment X Register**\
X <- X + 1\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | INX  | $E8 | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction increments the X Register, i.e., adds 1 to it.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## INY

**Increment Y Register**\
Y <- Y + 1\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | INY  | $C8 | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction increments the Y Register, i.e., adds 1 to it.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## INZ

**Increment Z Register**\
Z <- Y + 1\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | INZ  | $1B | 1 | 1<sup></sup> |

This instruction increments the Z Register, i.e., adds 1 to it.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## JMP

**Jump to Address**\
PC <- M2:M1\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Absolute | JMP \$nnnn | $4C | 3 | 3<sup></sup> |
 | Indirect Word | JMP (\$nnnn) | $6C | 3 | 5<sup>r</sup> |
 | Indirect Word, X | JMP (\$nnnn,X) | $7C | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction sets the Program Counter (PC) Register
to the address indicated by the instruction, causing
execution to continue from that address.

## JSR

**Jump to Sub-Routine**\
PC <- M2:M1, Stack <- PCH:PCL\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Absolute | JSR \$nnnn | $20 | 3 | 5<sup>s</sup> |
 | Indirect Word | JSR (\$nnnn) | $22 | 3 | 5<sup>r</sup> |
 | Indirect Word, X | JSR (\$nnnn,X) | $23 | 3 | 5<sup>pr</sup> |

 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.\
 <sup>s</sup> Instruction requires 2 cycles when CPU is run at 1MHz or 2MHz.

This instruction saves the address of the instruction
following the JSR instruction onto the stack, and
then sets the Program Counter (PC) Register
to the address indicated by the instruction, causing
execution to continue from that address.  Because the
return address has been saved on the stack, the RTS
instruction can be used to return from the called
sub-routine and resume execution following the JSR
instruction.

NOTE: This instruction actually pushes the address of
the last byte of the JSR instruction onto the stack.
The RTS instruction naturally is aware of this, and
increments the address on popping it from the stack,
before setting the Program Counter (PC) register.

## LDA

**Load Accumulator**\
A <- M\
Flags: N Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Immediate | LDA #\$nn | $A9 | 2 | <sup>?</sup> |
 | Base Page | LDA \$nn | $A5 | 2 | <sup>?</sup> |
 | Base Page, X | LDA \$nn,X | $B5 | 2 | <sup>?</sup> |
 | Absolute | LDA \$nnnn | $AD | 3 | <sup>?</sup> |
 | Absolute, X | LDA \$nnnn,X | $BD | 3 | <sup>?</sup> |
 | Absolute, Y | LDA \$nnnn,Y | $B9 | 3 | <sup>?</sup> |
 | Indirect, X | LDA (\$nn,X) | $A1 | 2 | <sup>?</sup> |
 | Indirect, Y | LDA (\$nn),Y | $B1 | 2 | <sup>?</sup> |
 | Indirect, Z | LDA (\$nn),Z | $B2 | 2 | <sup>?</sup> |
 | Indirect SP, Y | LDA (\$nn,SP),Y | $E2 | 2 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction loads the Accumulator Register with the indicated
value, or with the contents of the indicated location.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## LDX

**Load X Register**\
X <- M\
Flags: N Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Immediate | LDX #\$nn | $A2 | 2 | <sup>?</sup> |
 | Base Page | LDX \$nn | $A6 | 2 | <sup>?</sup> |
 | Base Page, Y | LDX \$nn,Y | $B6 | 2 | <sup>?</sup> |
 | Absolute | LDX \$nnnn | $AE | 3 | <sup>?</sup> |
 | Absolute, Y | LDX \$nnnn,Y | $BE | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction loads the X Register with the indicated
value, or with the contents of the indicated location.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## LDY

**Load Y Register**\
Y <- M\
Flags: N Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Immediate | LDY #\$nn | $A0 | 2 | <sup>?</sup> |
 | Base Page | LDY \$nn | $A4 | 2 | <sup>?</sup> |
 | Base Page, X | LDY \$nn,X | $B4 | 2 | <sup>?</sup> |
 | Absolute | LDY \$nnnn | $AC | 3 | <sup>?</sup> |
 | Absolute, X | LDY \$nnnn,X | $BC | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction loads the Y Register with the indicated
value, or with the contents of the indicated location.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## LDZ

**Load Z Register**\
Z <- M\
Flags: N Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Immediate | LDZ #\$nn | $A3 | 2 | <sup>?</sup> |
 | Absolute | LDZ \$nnnn | $AB | 3 | <sup>?</sup> |
 | Absolute, X | LDZ \$nnnn,X | $BB | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction loads the Z Register with the indicated
value, or with the contents of the indicated location.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## LSR

**Logical Shift Right**\
A <- A>>1, C <- A(0) | M <- M>>1\
Flags: N Z C 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Accumulator | LSR A | $4A | 1 | 1<sup></sup> |
 | Base Page | LSR \$nn | $46 | 2 | 4<sup>r</sup> |
 | Base Page, X | LSR \$nn,X | $56 | 2 | 3<sup>pr</sup> |
 | Absolute | LSR \$nnnn | $4E | 3 | 5<sup>r</sup> |
 | Absolute, X | LSR \$nnnn,X | $5E | 3 | 5<sup>pr</sup> |

 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction shifts either the Accumulator or contents
of the provided memory location one bit right.  Bit 7 will be
set to zero, and the bit 0 will be shifted out into the Carry Flag

Side effects:

- The N flag will be set if the result is negative, i.e., if bit 7 is set after the operation, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.
- The C flag will be set if bit 0 of the value was set, prior to being shifted.

## MAP

**Set Memory Map**\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | MAP  | $5C | 1 | 1<sup></sup> |

This instruction sets the C65 or MEGA65 style memory map, depending
on the values in the Accumulator, X, Y and Z registers.

Care should be taken to ensure that after the execution of an
MAP instruction that appropriate memory is mapped at the location
of the following instruction.  Failure to do so will result in
unpredictable results.

Further information on this instruction is available in Appendix \ref{cha:cpu}.

Side effects:

- The memory map is immediately changed to that requested.
- All interrupts, including Non-Maskable Interrupts (NMIs) are
blocked from occurring until an EOM (NOP) instruction is encountered.

## NEG

**Negate Accumulator**\
A <- -A\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Accumulator | NEG A | $42 | 1 | 1<sup></sup> |

This instruction replaces the contents of the Accumulator Register with the
twos-complement of the contents of the Accumulator Register.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## ORA

**Decrement Memory or Accumulator**\
A <- A + 1 | M <- M + 1\
Flags: N Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Immediate | ORA #\$nn | $09 | 2 | 2<sup></sup> |
 | Base Page | ORA \$nn | $05 | 2 | 3<sup>r</sup> |
 | Base Page, X | ORA \$nn,X | $15 | 2 | 3<sup>r</sup> |
 | Absolute | ORA \$nnnn | $0D | 3 | 4<sup>r</sup> |
 | Absolute, X | ORA \$nnnn,X | $1D | 3 | 4<sup>pr</sup> |
 | Absolute, Y | ORA \$nnnn,Y | $19 | 3 | 4<sup>r</sup> |
 | Indirect, X | ORA (\$nn,X) | $01 | 2 | 6<sup>pr</sup> |
 | Indirect, Y | ORA (\$nn),Y | $11 | 2 | 5<sup>pr</sup> |
 | Indirect, Z | ORA (\$nn),Z | $12 | 2 | 5<sup>pr</sup> |

 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instructions performs a binary OR operation of the argument with the
accumulator, and stores the result in the accumulator. Only bits that were
already set in the accumulator, or that are set in the argument will be set
in the accumulator on completion, or both.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## PHA

**Push Accumulator Register onto the Stack**\
STACK <- A, SP <- SP - 1\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | PHA  | $48 | 1 | 2<sup></sup> |

This instruction pushes the contents of the Accumulator Register
onto the stack, and decrements the value of the Stack Pointer by 1.

## PHP

**Push Processor Flags onto the Stack**\
STACK <- P, SP <- SP - 1\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | PHP  | $08 | 1 | 2<sup></sup> |

This instruction pushes the contents of the Processor Flags
onto the stack, and decrements the value of the Stack Pointer by 1.

## PHW

**Push Word onto the Stack**\
STACK <- M1:M2, SP <- SP - 2\
N+Z+M+M+\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Immediate Word | PHW #\$nnnn | $F4 | 3 | <sup>?</sup> |
 | Absolute | PHW \$nnnn | $FC | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction pushes either a 16-bit literal value or the memory
word indicated
onto the stack, and decrements the value of the Stack Pointer by 2.

## PHX

**Push X Register onto the Stack**\
STACK <- X, SP <- SP - 1\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | PHX  | $DA | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction pushes the contents of the X Register
onto the stack, and decrements the value of the Stack Pointer by 1.

## PHY

**Push Y Register onto the Stack**\
STACK <- Y, SP <- SP - 1\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | PHY  | $5A | 1 | 2<sup></sup> |

This instruction pushes the contents of the Y Register
onto the stack, and decrements the value of the Stack Pointer by 1.

## PHZ

**Push Z Register onto the Stack**\
STACK <- z, SP <- SP - 1\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | PHZ  | $DB | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction pushes the contents of the Z Register
onto the stack, and decrements the value of the Stack Pointer by 1.

## PLA

**Pull Accumulator Register from the Stack**\
A <- STACK, SP <- SP + 1\
Flags: N Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | PLA  | $68 | 1 | 4<sup>m</sup> |

 <sup>m</sup> Subtract non-bus cycles when at 40MHz.

This instruction replaces the contents of the Accumulator Register
with the top value from the stack, and increments the value of the
Stack Pointer by 1.

## PLP

**Pull Processor Flags from the Stack**\
A <- STACK, SP <- SP + 1\
Flags: N Z  I C D V
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | PLP  | $28 | 1 | 4<sup>m</sup> |

 <sup>m</sup> Subtract non-bus cycles when at 40MHz.

This instruction replaces the contents of the Processor Flags
with the top value from the stack, and increments the value of the
Stack Pointer by 1.

NOTE: This instruction does NOT replace the Extended Stack Disable Flag
(E Flag), or the Software Interrupt Flag (B Flag)

## PLX

**Pull X Register from the Stack**\
X <- STACK, SP <- SP + 1\
Flags: N Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | PLX  | $FA | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction replaces the contents of the X Register
with the top value from the stack, and increments the value of the
Stack Pointer by 1.

## PLY

**Pull Y Register from the Stack**\
Y <- STACK, SP <- SP + 1\
Flags: N Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | PLY  | $7A | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction replaces the contents of the Y Register
with the top value from the stack, and increments the value of the
Stack Pointer by 1.

## PLZ

**Pull Z Register from the Stack**\
Z <- STACK, SP <- SP + 1\
Flags: N Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | PLZ  | $FB | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction replaces the contents of the Z Register
with the top value from the stack, and increments the value of the
Stack Pointer by 1.

## RMB0

**Reset Bit 0 in Base Page**\
M(0) <- 0\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | RMB0 \$nn | $07 | 2 | 4<sup>br</sup> |

 <sup>b</sup> Add one cycle if branch is taken.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction clears bit zero of the indicated address.
No flags are modified, regardless of the result.

## RMB1

**Reset Bit 1 in Base Page**\
M(1) <- 0\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | RMB1 \$nn | $17 | 2 | 4<sup>br</sup> |

 <sup>b</sup> Add one cycle if branch is taken.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction clears bit 1 of the indicated address.
No flags are modified, regardless of the result.

## RMB2

**Reset Bit 2 in Base Page**\
M(2) <- 0\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | RMB2 \$nn | $27 | 2 | 4<sup>r</sup> |

 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction clears bit 2 of the indicated address.
No flags are modified, regardless of the result.

## RMB3

**Reset Bit 3 in Base Page**\
M(3) <- 0\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | RMB3 \$nn | $37 | 2 | 4<sup>r</sup> |

 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction clears bit 3 of the indicated address.
No flags are modified, regardless of the result.

## RMB4

**Reset Bit 4 in Base Page**\
M(4) <- 0\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | RMB4 \$nn | $47 | 2 | 4<sup>r</sup> |

 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction clears bit 4 of the indicated address.
No flags are modified, regardless of the result.

## RMB5

**Reset Bit 5 in Base Page**\
M(5) <- 0\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | RMB5 \$nn | $57 | 2 | 4<sup>r</sup> |

 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction clears bit 5 of the indicated address.
No flags are modified, regardless of the result.

## RMB6

**Reset Bit 6 in Base Page**\
M(6) <- 0\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | RMB6 \$nn | $67 | 2 | 5<sup>r</sup> |

 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction clears bit 6 of the indicated address.
No flags are modified, regardless of the result.

## RMB7

**Reset Bit 7 in Base Page**\
M(7) <- 0\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | RMB7 \$nn | $77 | 2 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction clears bit 7 of the indicated address.
No flags are modified, regardless of the result.

## ROL

**Rotate Left Memory or Accumulator**\
M <- M<<1, C <- M(7), M(0) <- C\
Flags: N Z C  
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Accumulator | ROL A | $2A | 1 | 1<sup></sup> |
 | Base Page | ROL \$nn | $26 | 2 | 4<sup>r</sup> |
 | Base Page, X | ROL \$nn,X | $36 | 2 | 5<sup>pr</sup> |
 | Absolute | ROL \$nnnn | $2E | 3 | 5<sup>r</sup> |
 | Absolute, X | ROL \$nnnn,X | $3E | 3 | 5<sup>pr</sup> |

 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction shifts either the Accumulator or contents
of the provided memory location one bit left.  Bit 0 will be
set to the current value of the Carry Flag, and the bit 7 will be shifted out into the Carry Flag

Side effects:

- The N flag will be set if the result is negative, i.e., if bit 7 is set after the operation, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.
- The C flag will be set if bit 7 of the value was set, prior to being shifted.

## ROR

**Rotate Right Memory or Accumulator**\
M <- M>>1, C <- M(0), M(7) <- C\
Flags: N Z C  
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Accumulator | ROR A | $6A | 1 | 1<sup></sup> |
 | Base Page | ROR \$nn | $66 | 2 | 5<sup>r</sup> |
 | Base Page, X | ROR \$nn,X | $76 | 2 | <sup>?</sup> |
 | Absolute | ROR \$nnnn | $6E | 3 | 6<sup>r</sup> |
 | Absolute, X | ROR \$nnnn,X | $7E | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction shifts either the Accumulator or contents
of the provided memory location one bit right.  Bit 7 will be
set to the current value of the Carry Flag, and the bit 0 will be shifted out into the Carry Flag

Side effects:

- The N flag will be set if the result is negative, i.e., if bit 7 is set after the operation, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.
- The C flag will be set if bit 7 of the value was set, prior to being shifted.

## ROW

**Rotate Word Left**\
M2:M1 <- M2:M1<<1, C <- M2(7), M1(0) <- C\
N+Z+C+M+M+\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Absolute | ROW \$nnnn | $EB | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction rotates the contents of the indicated memory word one bit
left.  Bit 0 of the low byte will be
set to the current value of the Carry Flag, and the bit 7 of the high byte
will be shifted out into the Carry Flag

Side effects:

- The N flag will be set if the result is negative, i.e., if bit 7 is set after the operation, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.
- The C flag will be set if bit 7 of the upper byte was set, prior to being shifted.

## RTI

**Return From Interrupt**\
P <- STACK, PC <- STACK, SP <- SP + 3\
Flags: N V C V D I
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | RTI  | $40 | 1 | 6<sup>m</sup> |

 <sup>m</sup> Subtract non-bus cycles when at 40MHz.

This instruction pops the processor flags from the stack, and then
pops the Program Counter (PC) register from the stack, allowing
an interrupted program to resume.

- The 6502 Processor Flags are restored from the stack.
- Neither the B (Software Interrupt) nor E (Extended Stack)
flags are set by this instruction.

## RTS

**Return From Subroutine**\
PC <- STACK + N, SP <- SP + 2 + N\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | RTS  | $60 | 1 | 6<sup>m</sup> |
 | Immediate | RTS #\$nn | $62 | 2 | 4<sup></sup> |

 <sup>m</sup> Subtract non-bus cycles when at 40MHz.

This instruction adds optional argument to the Stack Pointer (SP) Register, and then
pops the Program Counter (PC) register from the stack, allowing
a routine to return to its caller.

## SBC

**Subtract With Carry**\
A <- - M - 1 + C\
Flags: N Z C V
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Immediate | SBC #\$nn | $E9 | 2 | <sup>?</sup> |
 | Base Page | SBC \$nn | $E5 | 2 | <sup>?</sup> |
 | Base Page, X | SBC \$nn,X | $F5 | 2 | <sup>?</sup> |
 | Absolute | SBC \$nnnn | $ED | 3 | <sup>?</sup> |
 | Absolute, X | SBC \$nnnn,X | $FD | 3 | <sup>?</sup> |
 | Absolute, Y | SBC \$nnnn,Y | $F9 | 3 | <sup>?</sup> |
 | Indirect, X | SBC (\$nn,X) | $E1 | 2 | <sup>?</sup> |
 | Indirect, Y | SBC (\$nn),Y | $F1 | 2 | <sup>?</sup> |
 | Indirect, Z | SBC (\$nn),Z | $F2 | 2 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction performs A - M - 1 + C, and sets the processor flags accordingly.
The result is stored in the Accumulator Register.

NOTE: This instruction is affected by the status of the Decimal Flag.

Side effects:

- The N flag will be set if the result of A - M is negative, i.e. bit 7 is set in the result, else it will be cleared.
- The C flag will be set if the result of A - M is zero or positive, i.e., if A is not less than M, else it will be cleared.
- The V flag will be set if the result has a different sign to both of the
arguments, else it will be cleared. If the flag is set, this
indicates that a signed overflow has occurred.
- The Z flag will be set if the result of A - M is zero, else it will be cleared.

## SEC

**Set Carry Flag**\
C <- 1\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | SEC  | $38 | 1 | 1<sup></sup> |

This instruction sets the Carry Flag.

Side effects:

- The C flag is set.

## SED

**Set Decimal Flag**\
D <- 1\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | SED  | $F8 | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction sets the Decimal Flag.
Binary arithmetic will now use Binary-Coded Decimal (BCD) mode.

NOTE: The C64's interrupt handler does not clear the Decimal Flag,
which makes it dangerous to set the Decimal Flag without first
setting the Interrupt Disable Flag.

Side effects:

- The D flag is set.

## SEE

**Set Extended Stack Disable Flag**\
E <- 1\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | SEE  | $03 | 1 | 1<sup></sup> |

This instruction sets the Extended Stack Disable Flag.
This causes the stack to operate as on the 6502, i.e.,
limited to a single page of memory.  The page of memory
in which the stack is located can still be modified by
setting the Stack Pointer High (SPH) Register.

Side effects:

- The E flag is set.

## SEI

**Set Interrupt Disable Flag**\
I <- 1\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | SEI  | $78 | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction sets the Interrupt Disable Flag.
Normal (IRQ) interrupts will no longer be able to occur.
Non-Maskable Interrupts (NMI) will continue to occur, as their
name suggests.

Side effects:

- The I flag is set.

## SMB0

**Set Bit 0 in Base Page**\
M(0) <- 1\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | SMB0 \$nn | $87 | 2 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction sets bit zero of the indicated address.
No flags are modified, regardless of the result.

## SMB1

**Set Bit 1 in Base Page**\
M(1) <- 1\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | SMB1 \$nn | $97 | 2 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction sets bit 1 of the indicated address.
No flags are modified, regardless of the result.

## SMB2

**Set Bit 2 in Base Page**\
M(2) <- 1\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | SMB2 \$nn | $A7 | 2 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction sets bit 2 of the indicated address.
No flags are modified, regardless of the result.

## SMB3

**Set Bit 3 in Base Page**\
M(3) <- 1\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | SMB3 \$nn | $B7 | 2 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction sets bit 3 of the indicated address.
No flags are modified, regardless of the result.

## SMB4

**Set Bit 4 in Base Page**\
M(4) <- 1\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | SMB4 \$nn | $C7 | 2 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction sets bit 4 of the indicated address.
No flags are modified, regardless of the result.

## SMB5

**Set Bit 5 in Base Page**\
M(5) <- 1\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | SMB5 \$nn | $D7 | 2 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction sets bit 5 of the indicated address.
No flags are modified, regardless of the result.

## SMB6

**Set Bit 6 in Base Page**\
M(6) <- 1\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | SMB6 \$nn | $E7 | 2 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction sets bit 6 of the indicated address.
No flags are modified, regardless of the result.

## SMB7

**Set Bit 7 in Base Page**\
M(7) <- 1\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | SMB7 \$nn | $F7 | 2 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction sets bit 7 of the indicated address.
No flags are modified, regardless of the result.

## STA

**Store Accumulator**\
M <- A\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | STA \$nn | $85 | 2 | <sup>?</sup> |
 | Base Page, X | STA \$nn,X | $95 | 2 | <sup>?</sup> |
 | Absolute | STA \$nnnn | $8D | 3 | <sup>?</sup> |
 | Absolute, X | STA \$nnnn,X | $9D | 3 | <sup>?</sup> |
 | Absolute, Y | STA \$nnnn,Y | $99 | 3 | <sup>?</sup> |
 | Indirect, X | STA (\$nn,X) | $81 | 2 | <sup>?</sup> |
 | Indirect, Y | STA (\$nn),Y | $91 | 2 | <sup>?</sup> |
 | Indirect, Z | STA (\$nn),Z | $92 | 2 | <sup>?</sup> |
 | Indirect SP, Y | STA (\$nn,SP),Y | $82 | 2 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction stores the contents of the Accumulator Register
into the indicated location.

## STX

**Store X Register**\
M <- X\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | STX \$nn | $86 | 2 | <sup>?</sup> |
 | Base Page, Y | STX \$nn,Y | $96 | 2 | <sup>?</sup> |
 | Absolute | STX \$nnnn | $8E | 3 | <sup>?</sup> |
 | Absolute, Y | STX \$nnnn,Y | $9B | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction stores the contents of the X Register
into the indicated location.

## STY

**Store Y Register**\
M <- Y\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | STY \$nn | $84 | 2 | <sup>?</sup> |
 | Base Page, X | STY \$nn,X | $94 | 2 | <sup>?</sup> |
 | Absolute | STY \$nnnn | $8C | 3 | <sup>?</sup> |
 | Absolute, X | STY \$nnnn,X | $8B | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction stores the contents of the Y Register
into the indicated location.

## STZ

**Store Z Register**\
M <- Z\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | STZ \$nn | $64 | 2 | 3<sup></sup> |
 | Base Page, X | STZ \$nn,X | $74 | 2 | 3<sup></sup> |
 | Absolute | STZ \$nnnn | $9C | 3 | <sup>?</sup> |
 | Absolute, X | STZ \$nnnn,X | $9E | 3 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction stores the contents of the Z Register
into the indicated location.

## TAB

**Transfer Accumulator into Base Page Register**\
B <- A\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | TAB  | $5B | 1 | 1<sup></sup> |

This instruction sets the Base Page register to the contents
of the Accumulator Register.  This allows the relocation of
the 6502's Zero-Page into any page of memory.

## TAX

**Transfer Accumulator Register into the X Register**\
X <- A\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | TAX  | $AA | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction loads the X Register with the contents of the Accumulator Register.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## TAY

**Transfer Accumulator Register into the Y Register**\
Y <- A\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | TAY  | $A8 | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction loads the Y Register with the contents of the Accumulator Register.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## TAZ

**Transfer Accumulator Register into the Z Register**\
Z <- A\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | TAZ  | $4B | 1 | 1<sup></sup> |

This instruction loads the Z Register with the contents of the Accumulator Register.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## TBA

**Transfer Base Page Register into the Accumulator**\
A <- B\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | TBA  | $7B | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction loads the Accumulator Register with the contents
of the Base Page Register.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## TRB

**Test and Reset Bit**\
M <- M & ($NOT$ A)\
Flags: Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | TRB \$nn | $14 | 2 | 5<sup>r</sup> |
 | Absolute | TRB \$nnnn | $1C | 3 | 4<sup>r</sup> |

 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction sets performs a binary AND of the negation of the Accumulator Register
and the indicated memory location, storing the result there. That is,
any bits set in the Accumulator Register will be reset in the indicated
memory location.

It also performs a test for any bits in common between the accumulator and
indicated memory location. This can be used to construct simple shared-memory
multi-processor systems, by providing an atomic means of setting a semaphore
or acquiring a lock.

Side effects:

- The Z flag will be set if the binary AND of the Accumulator Register
and contents of the indicated memory location prior are zero, prior to the
execution of the instruction.

## TSB

**Test and Set Bit**\
M <- M | A\
Flags: Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Base Page | TSB \$nn | $04 | 2 | 3<sup>r</sup> |
 | Absolute | TSB \$nnnn | $0C | 3 | 5<sup>r</sup> |

 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction sets performs a binary OR of the Accumulator Register
and the indicated memory location, storing the result there. That is,
any bits set in the Accumulator Register will be set in the indicated
memory location.

It also performs a test for any bits in common between the accumulator and
indicated memory location. This can be used to construct simple shared-memory
multi-processor systems, by providing an atomic means of setting a semaphore
or acquiring a lock.

Side effects:

- The Z flag will be set if the binary AND of the Accumulator Register
and contents of the indicated memory location prior are zero, prior to the
execution of the instruction.

## TSX

**Transfer Stack Pointer High Register into the X Register**\
X <- SPH\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | TSX  | $BA | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction loads the X Register with the contents of the Stack Pointer High (SPL)
Register.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## TSY

**Transfer Stack Pointer High Register into the Y Register**\
Y <- SPH\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | TSY  | $0B | 1 | 1<sup></sup> |

This instruction loads the Y Register with the contents of the Stack Pointer High (SPH)
Register.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## TXA

**Transfer X Register into the Accumulator Register**\
A <- X\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | TXA  | $8A | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction loads the Accumulator Register with the contents
of the X Register.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## TXS

**Transfer X Register into Stack Pointer Low Register**\
SPL <- X\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | TXS  | $9A | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction sets the low byte of the Stack Pointer (SPL)
register to the contents of the X Register.

## TYA

**Transfer Y Register into the Accumulator Register**\
A <- Y\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | TYA  | $98 | 1 | <sup>?</sup> |

 <sup>?</sup> Cycles not in source documentation.

This instruction loads the Accumulator Register with the contents
of the Y Register.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## TYS

**Transfer Y Register into Stack Pointer High Register**\
SPH <- Y\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | TYS  | $2B | 1 | 1<sup></sup> |

This instruction sets the high byte of the Stack Pointer (SPH)
register to the contents of the Y Register.  This allows
changing the memory page where the stack is located (if the Extended Stack
Disable Flag (E) is set), or else allows setting the current Stack
Pointer to any page in memory, if the Extended Stack Disable
Flag (E) is clear.

## TZA

**Transfer Z Register into the Accumulator Register**\
A <- Z\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | TZA  | $6B | 1 | 1<sup></sup> |

This instruction loads the Accumulator Register with the contents
of the Z Register.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.
