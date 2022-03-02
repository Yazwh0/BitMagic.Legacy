# 45GS02 Instructions

## Credits and License

All text derived from the [Mega65 User Guide](https://github.com/MEGA65/mega65-user-guide). Copyright 2019-2021 by Paul Gardner-Stephen, the MEGA Museum of Electronic Games & Art e.V., and contributors.

This reference guide is made available under the GNU Free Documentation License
v1.3, or later, if desired. This means that you are free to modify, reproduce and redistribute this reference guide, subject to certain conditions. The full text of the GNU
Free Documentation License v1.3 can be found at [https://www.gnu.org/licenses/fdl-1.3.en.html](https://www.gnu.org/licenses/fdl-1.3.en.html).

## All Instructions

| |  |  |  |  |  |  |  |  |  |  |  |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| [ADC](#ADC) | [ADCQ](#ADCQ) | [AND](#AND) | [ANDQ](#ANDQ) | [ASLQ](#ASLQ) | [ASRQ](#ASRQ) | [BITQ](#BITQ) | [CMP](#CMP) | [CMPQ](#CMPQ) | [DEQ](#DEQ) | [EOR](#EOR) | [EORQ](#EORQ) |
| [INQ](#INQ) | [LDA](#LDA) | [LDQ](#LDQ) | [LSRQ](#LSRQ) | [ORA](#ORA) | [ORQ](#ORQ) | [RESQ](#RESQ) | [ROLQ](#ROLQ) | [RORQ](#RORQ) | [RSVQ](#RSVQ) | [SBC](#SBC) | [SBCQ](#SBCQ) |
| [STA](#STA) | [STQ](#STQ) |

## ADC

**Add with carry**\
A <- A+M+C\
Flags: N Z C V
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Indirect Quad, Z | ADC [\$nn],Z | $EA 72 | 3 | 7<sup>ipr</sup> |

 <sup>i</sup> Add one cycle if clock speed is at 40 MHz.\
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

## ADCQ

**Add with carry**\
Q <- Q+M+C\
Flags: N Z C V
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page | ADCQ \$nn | $42 42 65 | 4 | 8<sup>r</sup> |
 | Absolute | ADCQ \$nnnn | $42 42 6D | 5 | 9<sup>r</sup> |
 | Indirect | ADCQ (\$nn) | $42 42 72 | 4 | 10<sup>ipr</sup> |
 | Indirect Quad | ADCQ [\$nn] | $42 42 EA 72 | 5 | 13<sup>ipr</sup> |

 <sup>i</sup> Add one cycle if clock speed is at 40 MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction adds the argument to the contents of the 32-bit Q pseudo-register
Register and the Carry Flag.
If the D flag is set, then the operation is undefined, and is subject to change.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.
- The V flag will be set if the result has a different sign to both of the
arguments, else it will be cleared. If the flag is set, this
indicates that a signed overflow has occurred.
- The C flag will be set if the unsigned result is >255 if the D flag is clear.

## AND

**Binary AND**\
A <- A & M\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Indirect Quad, Z | AND [\$nn],Z | $EA 32 | 3 | 7<sup>ipr</sup> |

 <sup>i</sup> Add one cycle if clock speed is at 40 MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instructions performs a binary AND operation of the argument with the
accumulator, and stores the result in the accumulator. Only bits that were
already set in the accumulator, and that are set in the argument will be set
in the accumulator on completion.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## ANDQ

**Binary AND**\
Q <- Q & M\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page | ANDQ \$nn | $42 42 25 | 4 | 8<sup>r</sup> |
 | Absolute | ANDQ \$nnnn | $42 42 2D | 5 | 9<sup>r</sup> |
 | Indirect | ANDQ (\$nn) | $42 42 32 | 4 | 10<sup>ipr</sup> |
 | Indirect Quad | ANDQ [\$nn] | $42 42 EA 32 | 5 | 13<sup>ipr</sup> |

 <sup>i</sup> Add one cycle if clock speed is at 40 MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instructions performs a binary AND operation of the argument with the
Q pseudo register, and stores the result in the accumulator. Only bits that were
already set in the Q pseudo register, and that are set in the argument will be set
in the Q pseudo register on completion.

Note that the indicated memory location is treated as the
first byte of a 32-bit little-endian value.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## ASLQ

**Arithmetic Shift Left**\
Q <- Q<<1 | M <- M<<1\
Flags: N Z C 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | ASLQ  | $42 42 0A | 3 | 3<sup></sup> |
 | Zero Page | ASLQ \$nn | $42 42 06 | 4 | 12<sup>dmr</sup> |
 | Zero Page, X | ASLQ \$nn,X | $42 42 16 | 4 | 12<sup>dmpr</sup> |
 | Absolute | ASLQ \$nnnn | $42 42 0E | 5 | 13<sup>dmr</sup> |
 | Absolute, X | ASLQ \$nnnn,X | $42 42 1E | 5 | 13<sup>dmpr</sup> |

 <sup>d</sup> Subtract one cycle when CPU is at 3.5MHz.\
 <sup>m</sup> Subtract non-bus cycles when at 40MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction shifts either the Q pseudo-register or contents
of the provided memory location and following three one bit left, treating
them as holding a little-endian 32-bit value.  Bit 0 will be
set to zero, and the bit 31 will be shifted out into the Carry Flag

Side effects:

- The N flag will be set if the result is negative, i.e., if bit 31 is set after the operation, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.
- The C flag will be set if bit 31 of the value was set, prior to being shifted.

## ASRQ

**Arithmetic Shift Right**\
Q <- Q>>1 | M <- M>>1\
Flags: N Z C 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | ASRQ  | $42 42 43 | 3 | 3<sup></sup> |
 | Zero Page | ASRQ \$nn | $42 42 44 | 4 | 12<sup>dmr</sup> |
 | Zero Page, X | ASRQ \$nn,X | $42 42 54 | 4 | 12<sup>dmpr</sup> |

 <sup>d</sup> Subtract one cycle when CPU is at 3.5MHz.\
 <sup>m</sup> Subtract non-bus cycles when at 40MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction shifts either the Q pseudo-register or contents
of the provided memory location and following three one bit right, treating
them as holding a little-endian 32-bit value.  Bit 31
is considered to be a sign bit, and is preserved.
The content of bit 0 will be shifted out into the Carry Flag

Side effects:

- The N flag will be set if the result is negative, i.e., if bit 31 is set after the operation, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.
- The C flag will be set if bit 0 of the value was set, prior to being shifted.

## BITQ

**Perform Bit Test**\
N <- M(31), V <- M(30), Z <- Q & M\
Flags: N Z V
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page | BITQ \$nn | $42 42 24 | 4 | 8<sup>r</sup> |
 | Absolute | BITQ \$nnnn | $42 42 2C | 5 | 9<sup>r</sup> |

 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction is used to test the bits stored in a memory location
and following three, treating
them as holding a little-endian 32-bit value.
Bits 30 and 31 of the memory location's contents are directly copied into
the Overflow Flag and Negative Flag. The Zero Flag is set or cleared
based on the result of performing the binary AND of the Q Register
and the contents of the indicated memory location.

Side effects:

- The N flag will be set if the bit 31 of the memory location is set, else it will be cleared.
- The V flag will be set if the bit 30 of the memory location is set, else it will be cleared.
- The Z flag will be set if the result of Q & M is zero, else it will be cleared.

## CMP

**Compare Accumulator**\
Flags: N Z C
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Indirect Quad, Z | CMP [\$nn],Z | $EA D2 | 3 | 7<sup>ipr</sup> |

 <sup>i</sup> Add one cycle if clock speed is at 40 MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction performs A - M, and sets the processor flags accordingly,
but does not modify the contents of the Accumulator Register.

Side effects:

- The N flag will be set if the result of A - M is negative, i.e. bit 7 is set in the result, else it will be cleared.
- The C flag will be set if the result of A - M is zero or positive, i.e., if A is not less than M, else it will be cleared.
- The Z flag will be set if the result of A - M is zero, else it will be cleared.

## CMPQ

**Compare Q Pseudo Register**\
Flags: N Z C
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page | CMPQ \$nn | $42 42 C5 | 4 | 8<sup>r</sup> |
 | Absolute | CMPQ \$nnnn | $42 42 CD | 5 | 9<sup>r</sup> |
 | Indirect | CMPQ (\$nn) | $42 42 D2 | 4 | 10<sup>ipr</sup> |
 | Indirect Quad | CMPQ [\$nn] | $42 42 EA D2 | 5 | 13<sup>ipr</sup> |

 <sup>i</sup> Add one cycle if clock speed is at 40 MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction performs Q - M, and sets the processor flags accordingly,
but does not modify the contents of the Q Register. The memory location is
treated as the address of a little-endian 32-bit value.

Side effects:

- The N flag will be set if the result of A - M is negative, i.e. bit 31 is set in the result, else it will be cleared.
- The C flag will be set if the result of A - M is zero or positive, i.e., if A is not less than M, else it will be cleared.
- The Z flag will be set if the result of A - M is zero, else it will be cleared.

## DEQ

**Decrement Memory or Q**\
Q <- Q - 1 | M <- M - 1\
Flags: N Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | DEQ  | $42 42 3A | 3 | 3<sup>s</sup> |
 | Zero Page | DEQ \$nn | $42 42 C6 | 4 | 12<sup>dmr</sup> |
 | Zero Page, X | DEQ \$nn,X | $42 42 D6 | 4 | 12<sup>dmpr</sup> |
 | Absolute | DEQ \$nnnn | $42 42 CE | 5 | 13<sup>dmr</sup> |
 | Absolute, X | DEQ \$nnnn,X | $42 42 DE | 5 | 13<sup>dmpr</sup> |

 <sup>d</sup> Subtract one cycle when CPU is at 3.5MHz.\
 <sup>m</sup> Subtract non-bus cycles when at 40MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.\
 <sup>s</sup> Instruction requires 2 cycles when CPU is run at 1MHz or 2MHz.

This instruction decrements the Accumulator Register or indicated
memory location.

Note that the indicated memory location is treated as the
first byte of a 32-bit little-endian value.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## EOR

**Binary Exclusive OR**\
A <- A xor M\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Indirect Quad, Z | EOR [\$nn],Z | $EA 52 | 3 | 7<sup>ipr</sup> |

 <sup>i</sup> Add one cycle if clock speed is at 40 MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instructions performs a binary XOR operation of the argument with the
accumulator, and stores the result in the accumulator. Only bits that were
already set in the accumulator, or that are set in the argument will be set
in the accumulator on completion, but not both.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## EORQ

**Binary Exclusive OR**\
Q <- Q xor M\
Flags: N Z
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page | EORQ \$nn | $42 42 45 | 4 | 8<sup>r</sup> |
 | Absolute | EORQ \$nnnn | $42 42 4D | 5 | 9<sup>r</sup> |
 | Indirect | EORQ (\$nn) | $42 42 52 | 4 | 10<sup>ipr</sup> |
 | Indirect Quad | EORQ [\$nn] | $42 42 EA 52 | 5 | 13<sup>ipr</sup> |

 <sup>i</sup> Add one cycle if clock speed is at 40 MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instructions performs a binary XOR operation of the argument with the
Q pseudo register, and stores the result in the Q pseudo register. Only bits that were
already set in the Q pseudo register, or that are set in the argument will be set
in the accumulator on completion, but not bits that were set in both.

Side effects:

- The N flag will be set if the result is negative, i.e., bit 31 is set, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## INQ

**Increment Memory or Accumulator**\
Q <- Q + 1 | M <- M + 1\
Flags: N Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | INQ  | $42 42 1A | 3 | 3<sup>s</sup> |
 | Zero Page | INQ \$nn | $42 42 E6 | 4 | 13<sup>dmr</sup> |
 | Zero Page, X | INQ \$nn,X | $42 42 F6 | 4 | 13<sup>dmpr</sup> |
 | Absolute | INQ \$nnnn | $42 42 EE | 5 | 14<sup>dmr</sup> |
 | Absolute, X | INQ \$nnnn,X | $42 42 FE | 5 | 14<sup>dpr</sup> |

 <sup>d</sup> Subtract one cycle when CPU is at 3.5MHz.\
 <sup>m</sup> Subtract non-bus cycles when at 40MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.\
 <sup>s</sup> Instruction requires 2 cycles when CPU is run at 1MHz or 2MHz.

This instruction increments the Q pseudo register or indicated
memory location.

Note that the indicated memory location is treated as the
first byte of a 32-bit little-endian value.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## LDA

**Load Accumulator**\
A <- M\
Flags: N Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Indirect Quad, Z | LDA [\$nn],Z | $EA B2 | 3 | 7<sup>ipr</sup> |

 <sup>i</sup> Add one cycle if clock speed is at 40 MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction loads the Accumulator Register with the indicated
value, or with the contents of the indicated location.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## LDQ

**Load Q Pseudo Register**\
Q <- M\
Flags: N Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page | LDQ \$nn | $42 42 A5 | 4 | 8<sup>r</sup> |
 | Absolute | LDQ \$nnnn | $42 42 AD | 5 | 9<sup>r</sup> |
 | Indirect, Z | LDQ (\$nn),Z | $42 42 B2 | 4 | 10<sup>ipr</sup> |
 | Indirect Quad, Z | LDQ [\$nn],Z | $42 42 EA B2 | 5 | 13<sup>ipr</sup> |

 <sup>i</sup> Add one cycle if clock speed is at 40 MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction loads the Q pseudo register with the indicated
value, or with the contents of the indicated location.
As the Q register is an alias for A, X, Y and Z used together,
this operation will set those four registers. A contains the
least significant bits, X the next least significant, then Y,
and Z contains the most significant bits.

Side effects:

- The N flag will be set if the result is negative, i.e., bit 31 is set, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## LSRQ

**Logical Shift Right**\
Q <- Q>>1, C <- A(0) | M <- M>>1\
Flags: N Z C 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | LSRQ  | $42 42 4A | 3 | 3<sup>s</sup> |
 | Zero Page | LSRQ \$nn | $42 42 46 | 4 | 12<sup>dmr</sup> |
 | Zero Page, X | LSRQ \$nn,X | $42 42 56 | 4 | 12<sup>dmpr</sup> |
 | Absolute | LSRQ \$nnnn | $42 42 4E | 5 | 13<sup>dmr</sup> |
 | Absolute, X | LSRQ \$nnnn,X | $42 42 5E | 5 | 13<sup>dmpr</sup> |

 <sup>d</sup> Subtract one cycle when CPU is at 3.5MHz.\
 <sup>m</sup> Subtract non-bus cycles when at 40MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.\
 <sup>s</sup> Instruction requires 2 cycles when CPU is run at 1MHz or 2MHz.

This instruction shifts either the Q pseudo register or contents
of the provided memory location one bit right.  Bit 31 will be
set to zero, and the bit 0 will be shifted out into the Carry Flag.

Note that the memory address is treated as the first address of a
little-endian encoded 32-bit value.

Side effects:

- The N flag will be set if the result is negative, i.e., if bit 31 is set after the operation, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.
- The C flag will be set if bit 0 of the value was set, prior to being shifted.

## ORA

**Decrement Memory or Accumulator**\
A <- A + 1 | M <- M + 1\
Flags: N Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Indirect Quad, Z | ORA [\$nn],Z | $EA 12 | 3 | 7<sup>ipr</sup> |

 <sup>i</sup> Add one cycle if clock speed is at 40 MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instructions performs a binary OR operation of the argument with the
accumulator, and stores the result in the accumulator. Only bits that were
already set in the accumulator, or that are set in the argument will be set
in the accumulator on completion, or both.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## ORQ

**Decrement Memory or Q**\
Q <- Q + 1 | M <- M + 1\
Flags: N Z 
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page | ORQ \$nn | $42 42 05 | 4 | 8<sup>r</sup> |
 | Absolute | ORQ \$nnnn | $42 42 0D | 5 | 9<sup>r</sup> |
 | Indirect | ORQ (\$nn) | $42 42 12 | 4 | 10<sup>pr</sup> |
 | Indirect Quad | ORQ [\$nn] | $42 42 EA 12 | 5 | 13<sup>pr</sup> |

 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instructions performs a binary OR operation of the argument with the
Q pseudo register, and stores the result in the Q pseudo register. Only bits that were
already set in the Q pseudo register, or that are set in the argument, or both, will be set
in the Q pseudo register on completion.

Note that this operation treats the memory address as the first address of a 32-bit
little-endian value. That is, the memory address and the three following will be used.

Side effects:

- The N flag will be set if the result is negative, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.

## RESQ

**Reserved extended opcode**\
UNDEFINED\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page, X | RESQ \$nn,X | $42 42 15 | 4 | 8<sup>pr</sup> |
 | Zero Page, X | RESQ \$nn,X | $42 42 34 | 4 | 8<sup>pr</sup> |
 | Zero Page, X | RESQ \$nn,X | $42 42 35 | 4 | 8<sup>pr</sup> |
 | Zero Page, X | RESQ \$nn,X | $42 42 55 | 4 | 8<sup>pr</sup> |
 | Zero Page, X | RESQ \$nn,X | $42 42 75 | 4 | 8<sup>pr</sup> |
 | Absolute, X | RESQ \$nnnn,X | $42 42 1D | 5 | 9<sup>pr</sup> |
 | Absolute, X | RESQ \$nnnn,X | $42 42 3C | 5 | 9<sup>pr</sup> |
 | Absolute, X | RESQ \$nnnn,X | $42 42 3D | 5 | 10<sup>pr</sup> |
 | Absolute, X | RESQ \$nnnn,X | $42 42 5D | 5 | 9<sup>pr</sup> |
 | Absolute, X | RESQ \$nnnn,X | $42 42 7D | 5 | 10<sup>pr</sup> |
 | Absolute, Y | RESQ \$nnnn,Y | $42 42 19 | 5 | 9<sup>pr</sup> |
 | Absolute, Y | RESQ \$nnnn,Y | $42 42 39 | 5 | 10<sup>pr</sup> |
 | Absolute, Y | RESQ \$nnnn,Y | $42 42 59 | 5 | 9<sup>pr</sup> |
 | Absolute, Y | RESQ \$nnnn,Y | $42 42 79 | 5 | 10<sup>pr</sup> |
 | Indirect, X | RESQ (\$nn,X) | $42 42 01 | 4 | 10<sup>ipr</sup> |
 | Indirect, X | RESQ (\$nn,X) | $42 42 21 | 4 | 10<sup>ir</sup> |
 | Indirect, X | RESQ (\$nn,X) | $42 42 41 | 4 | 10<sup>ipr</sup> |
 | Indirect, X | RESQ (\$nn,X) | $42 42 61 | 4 | 10<sup>ir</sup> |
 | Indirect, Y | RESQ (\$nn),Y | $42 42 11 | 4 | 10<sup>ipr</sup> |
 | Indirect, Y | RESQ (\$nn),Y | $42 42 31 | 4 | 10<sup>ipr</sup> |
 | Indirect, Y | RESQ (\$nn),Y | $42 42 51 | 4 | 10<sup>ipr</sup> |
 | Indirect, Y | RESQ (\$nn),Y | $42 42 71 | 4 | 10<sup>ipr</sup> |

 <sup>i</sup> Add one cycle if clock speed is at 40 MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

???
These extended opcodes are reserved, and their function
is undefined and subject to change in future revisions
of the 45GS02.  They should therefore not be used in
any program.

## ROLQ

**Rotate Left Memory or Q**\
M <- M<<1, C <- M(31), M(0) <- C\
Flags: N Z C  
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | ROLQ  | $42 42 2A | 3 | 3<sup></sup> |
 | Zero Page | ROLQ \$nn | $42 42 26 | 4 | 12<sup>dmr</sup> |
 | Zero Page, X | ROLQ \$nn,X | $42 42 36 | 4 | 12<sup>dmpr</sup> |
 | Absolute | ROLQ \$nnnn | $42 42 2E | 5 | 13<sup>dmr</sup> |
 | Absolute, X | ROLQ \$nnnn,X | $42 42 3E | 5 | 13<sup>dmpr</sup> |

 <sup>d</sup> Subtract one cycle when CPU is at 3.5MHz.\
 <sup>m</sup> Subtract non-bus cycles when at 40MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction shifts either the Accumulator or contents
of the provided memory location one bit left.  Bit 0 will be
set to the current value of the Carry Flag, and the bit 31 will be shifted out into the Carry Flag.

NOTE: The memory address is treated as the first address of a little-endian encoded 32-bit value.

Side effects:

- The N flag will be set if the result is negative, i.e., if bit 31 is set after the operation, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.
- The C flag will be set if bit 31 of the value was set, prior to being shifted.

## RORQ

**Rotate Right Memory or Accumulator**\
M <- M>>1, C <- M(0), M(31) <- C\
Flags: N Z C  
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Implied | RORQ  | $42 42 6A | 3 | 3<sup></sup> |
 | Zero Page | RORQ \$nn | $42 42 66 | 4 | 12<sup>dmr</sup> |
 | Zero Page, X | RORQ \$nn,X | $42 42 76 | 4 | 12<sup>dmpr</sup> |
 | Absolute | RORQ \$nnnn | $42 42 6E | 5 | 13<sup>dmr</sup> |
 | Absolute, X | RORQ \$nnnn,X | $42 42 7E | 5 | 13<sup>dmpr</sup> |

 <sup>d</sup> Subtract one cycle when CPU is at 3.5MHz.\
 <sup>m</sup> Subtract non-bus cycles when at 40MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction shifts either the Q pseudo register or contents
of the provided memory location one bit right.  Bit 31 will be
set to the current value of the Carry Flag, and the bit 0 will be shifted out into the Carry Flag

Note that the address is treated as the first address of a little-endian 32-bit value.

Side effects:

- The N flag will be set if the result is negative, i.e., if bit 31 is set after the operation, else it will be cleared.
- The Z flag will be set if the result is zero, else it will be cleared.
- The C flag will be set if bit 31 of the value was set, prior to being shifted.

## RSVQ

**Reserved extended opcode**\
UNDEFINED\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page, X | RSVQ \$nn,X | $42 42 95 | 4 | 8<sup>p</sup> |
 | Zero Page, X | RSVQ \$nn,X | $42 42 D5 | 4 | 8<sup>pr</sup> |
 | Zero Page, X | RSVQ \$nn,X | $42 42 F5 | 4 | 8<sup>pr</sup> |
 | Absolute, X | RSVQ \$nnnn,X | $42 42 9D | 5 | 9<sup>p</sup> |
 | Absolute, X | RSVQ \$nnnn,X | $42 42 DD | 5 | 9<sup>pr</sup> |
 | Absolute, X | RSVQ \$nnnn,X | $42 42 FD | 5 | 9<sup>pr</sup> |
 | Absolute, Y | RSVQ \$nnnn,Y | $42 42 99 | 5 | 9<sup>p</sup> |
 | Absolute, Y | RSVQ \$nnnn,Y | $42 42 D9 | 5 | 9<sup>pr</sup> |
 | Absolute, Y | RSVQ \$nnnn,Y | $42 42 F9 | 5 | 8<sup>pr</sup> |
 | Indirect, X | RSVQ (\$nn,X) | $42 42 81 | 4 | 10<sup>ip</sup> |
 | Indirect, X | RSVQ (\$nn,X) | $42 42 A1 | 4 | 10<sup>ipr</sup> |
 | Indirect, X | RSVQ (\$nn,X) | $42 42 C1 | 4 | 10<sup>ipr</sup> |
 | Indirect, X | RSVQ (\$nn,X) | $42 42 E1 | 4 | 10<sup>ipr</sup> |
 | Indirect, Y | RSVQ (\$nn),Y | $42 42 91 | 4 | 10<sup>ip</sup> |
 | Indirect, Y | RSVQ (\$nn),Y | $42 42 D1 | 4 | 10<sup>ipr</sup> |
 | Indirect, Y | RSVQ (\$nn),Y | $42 42 F1 | 4 | 10<sup>ipr</sup> |
 | Indirect SP, Y | RSVQ (\$nn,SP),Y | $42 42 82 | 4 | 10<sup>ip</sup> |

 <sup>i</sup> Add one cycle if clock speed is at 40 MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

???
These extended opcodes are reserved, and their function
is undefined and subject to change in future revisions
of the 45GS02.  They should therefore not be used in
any program.

## SBC

**Subtract With Carry**\
A <- - M - 1 + C\
Flags: N Z C V
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Indirect Quad, Z | SBC [\$nn],Z | $EA F2 | 3 | <sup>?</sup> |

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

## SBCQ

**Subtract With Carry**\
Q <- - M - 1 + C\
Flags: N Z C V
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page | SBCQ \$nn | $42 42 E5 | 4 | 8<sup>r</sup> |
 | Absolute | SBCQ \$nnnn | $42 42 ED | 5 | 9<sup>r</sup> |
 | Indirect | SBCQ (\$nn) | $42 42 F2 | 4 | 10<sup>ipr</sup> |
 | Indirect Quad | SBCQ [\$nn] | $42 42 EA F2 | 5 | 13<sup>ipr</sup> |

 <sup>i</sup> Add one cycle if clock speed is at 40 MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.\
 <sup>r</sup> Add one cycle if clock speed is at 40 MHz.

This instruction performs Q - M - 1 + C, and sets the processor flags accordingly.
The result is stored in the Q pseudi register.

NOTE: that the indicated memory location is treated as the
first byte of a 32-bit little-endian value.

NOTE: The decimal (D) flag must be clear. Operation is reserved when D flag is set.

Side effects:

- The N flag will be set if the result of A - M is negative, i.e. bit 31 is set in the result, else it will be cleared.
- The C flag will be set if the result of A - M is zero or positive, i.e., if A is not less than M, else it will be cleared.
- The V flag will be set if the result has a different sign to both of the
arguments, else it will be cleared. If the flag is set, this
indicates that a signed overflow has occurred.
- The Z flag will be set if the result of A - M is zero, else it will be cleared.

## STA

**Store Accumulator**\
M <- A\
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Indirect Quad, Z | STA [\$nn],Z | $EA 92 | 3 | 8<sup>ip</sup> |

 <sup>i</sup> Add one cycle if clock speed is at 40 MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.

This instruction stores the contents of the Accumulator Register
into the indicated location.

## STQ

**Store Q**\
M <- Q = M+0 <- A, ... , M+3 <- Z \
Flags: -
 | Mode | Syntax | Hex | Len | Cycles |
 | --- | --- | --- | --- | --- |
 | Zero Page | STQ \$nn | $42 42 85 | 4 | 8<sup></sup> |
 | Absolute | STQ \$nnnn | $42 42 8D | 5 | 9<sup></sup> |
 | Indirect | STQ (\$nn) | $42 42 92 | 4 | 10<sup>ip</sup> |
 | Indirect Quad | STQ [\$nn] | $42 42 EA 92 | 5 | 13<sup>ip</sup> |

 <sup>i</sup> Add one cycle if clock speed is at 40 MHz.\
 <sup>p</sup> Add one cycle if indexing crosses a page boundary.

This instruction stores the contents of the Q pseudo register
into the indicated location.

As Q is composed of A, X, Y and Z, this means that these four
registers will be written to the indicated memory location through
to the indicated memory location plus 3, respectively.
