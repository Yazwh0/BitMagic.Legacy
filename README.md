# BitMagic

**Note, this should be considered 'in development'. Right now you should consider it a curiosity, not something you should use.**

BitMagic is currently an assembler template engine and compiler for the Commander X16, although in theory it would work for any 6502\65c02 based machine.

BitMagic also includes a work-in-progress emulator, and will eventually include a Debugger Adaptor to allow development from within tools like Visual Studio Code.

## Compiler

Because of the templating engine, BitMagic's compiler is deliberately simple.

It has its own extension '.bmasm', which can either be created manually and compiled, or can be created from a .csasm file. (An actual file doesn't necessarily need to be generated in the latter case.) The generation of this file makes it easier to debug the templating, as you can work on what was actually generated.

The compiler understands normal 6502 notation, and any line that doesn't start with a '.' will be considered to be 6502 asm. This can include expressions when dealing with literal values in asm. A semicolon can be used for comments.

There is a VSC add-in for basic syntax highlighting included in the code. You'll need to pack it and import it manually.

### Verbs

The following verbs are understood by the compiler:

| Verb | Parameters |Description |
| --- | --- | --- |
| .segment | name, address, filename | Switches and defines a segment. |
| .endsegment | | Switches to the default segment. |
| .scope | name | Switches to a named scope within the current segment. |
| .endscope | | Switches to a new default scope. |
| .proc | name | Switches to the named procedure. The procedure's name and address is stored as a constant. |
| .endproc | | Switches to a new default procedure. |
| .const | name, value | Sets a constant value. |
| .pad | address | Pads the current state to the target address. |
| .align | boundary | Pads the current state to the nearest boundary. (Where 'address % boundary' is zero.) |
| .byte | _values_ | Adds the bytes to the application. |
| .word | _values_ | Adds the words (little endian) to the application. |

All the verbs parameters -- apart from 'byte' and 'word' -- are optional. To specify a parameter, you can either use '_param_ = value', or constants in the order they're listed, or even a mix. Eg `.segment rom address = 0xA000`.

### Scopes

Constant values, including the addresses for procedures are scoped by Globals -> Segment -> Scope -> Procedure.

When the compiler is looking for a value, it will consider the current procedure first, before going up the tree to the scope, then segment, then globals.

You can break out of that by using colons to indicate the scope of what you're trying to find. Each layer is separated by a colon, so for fully qualified value it would be `App:SegmentName:ScopeName:ProcName:ValueName`. If the segment\scope\procedure isn't named then the 'anonymous' parts will not be considered.

The compiler can output the list of variables to make it easier to understand.

The code below shows how to get the values that are out of scope.

```asm
    .scope 

        .proc A
            .const xyz = $ff
        ;.endproc no .endproc necessary, but its advised!

    .endscope

    .scope

        .proc B
            lda App::A:xyz
            lda App:::xyz
            lda :::xyz
            lda ::A:xyz
            lda :A:xyz
            lda :xyz
        .endproc

    ; no .endscope necessary, its assumed with a .scope

    .scope example_c

        .const xyz = $f0

        .proc C
            lda App::A:xyz
            lda App:::xyz
            lda :::xyz
            lda ::A:xyz
            lda :A:xyz
            ;lda :xyz breaks, not unique
            lda xyz ; will be from this scope
        .endproc

        lda App::A:xyz
        lda App:::xyz
        lda :::xyz
        lda ::A:xyz
        lda :A:xyz
        ;lda :xyz breaks, not unique
        lda xyz ; will be from this scope

    .endscope

    lda DATA0

    .scope global_override
        .const DATA0 = $1111

        lda DATA0

        .proc override
            .const DATA0 = $2222
            lda DATA0 ; will load from $2222
        .endproc

    .endscope
```

### Expressions

For values within the assembler you can use expressions as you'd expect. ^, < and > will work.

Currently expressions can only be in the asm code, not when defining a constant. This will be addressed later.

### Compiler Notable Missing Functionality

A big current limitation is that the compiler cannot bring in source from external files. You can use a csasm file and use C# to read a text file in, however there needs to be a better way.

### Todo list

- Allow constants to use expressions, so they can be based off each other.
- Add ability to import data from an external file. One possibility is to compile and then import the 'scopes'.
- Much better error messages.

## Template Engine

BitMagic uses ASP.NETs Razor template engine to achieve what would commonly be called 'Macros' in other assembly implementations. In the same way you can use C# to generate HTML pages, we use the engine to generate .asm files.

The files that the Razor engine works on have the extension '.csasm'. (in a nod to the .cshtml file extension)

Unlike HTMLs tags which makes differentiation easy, BitMagic will examine each line of the input file. If it stars with a '.', or a three letter asm opcode, it will be flagged as part of the asm file. If not, it will be parsed as C#. You can still use C# on a line of assembler code, using the @ notation.

If you're not familiar with the Razor templating engine, its probably worth reading a few beginners guides first.

This approach will allow us to do anything we want to generate our code, and we're not limited by the assembler designers imagination.

Here is an example of some csasm code:

```csharp
    reference System.Drawing.Primitives; // reference and use system.drawing as it contains the color struct.
    using System.Drawing;

    @X16Header();   // adds a header that will start the application. The first address of code will be $810.

    .loop:          // application does nothing
    jmp loop

    // define a function that can be reused, akin to a 'macro'. 
    // It takes a color as defined in System.Drawing, changes it into 4 bit colour and writes it to the VERA.
    void SetColour(Color colour)
    {
        lda #@((colour.G & 0xf0) + (colour.B >> 4))
        sta DATA0
        lda #@(colour.R >> 4)
        sta DATA0
    }

    SetColour(Color.DarkOrchid); // $9932CC, or $93c on the X16, and so $3c, $09 when loading into VERA
    SetColour(Color.White);
```

When compiled this will create a .prg file with the following:

```txt
    $0801:  $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
    $0810:  $4C, $10, $08         JMP       loop
    $0813:  $A9, $3C              LDA       #60
    $0815:  $8D, $23, $9F         STA       DATA0
    $0818:  $A9, $09              LDA       #9
    $081A:  $8D, $23, $9F         STA       DATA0
    $081D:  $A9, $FF              LDA       #255
    $081F:  $8D, $23, $9F         STA       DATA0
    $0822:  $A9, $0F              LDA       #15
    $0824:  $8D, $23, $9F         STA       DATA0
```

The values being loaded into DATA0 match what we expect. (Note, they're displayed in decimal rather than hex.)

There are currently a handful of methods on the default model (can be accessed with the @ -- see above for an example.)

| Name | Description |
| --- | --- |
| `X16header()` | Will output the basic commands to jump to the code at $810. |
| `Bytes(IEnumerable<byte> bytes, int width = 16)` | Outputs the bytes as '.byte' code. |
| `Words(IEnumerable<short> words, int width = 16)` | Outputs the words as '.word' code. |

More helper functions will be added over time.

### Todo List

- Improve referencing, maybe via nuget somehow.
- Create a VSC plugin for basic syntax formatting.
- Add template re-use, so a template can be compiled to a .dll.

## Emulator

BitMagic also has an emulator!

Its still work in progress, so at this point its not much more than a footnote. There is plenty of work to do here. It currently supports most of the CPUs opcodes. For VERA it supports 2/4/8bpp tile modes and sprites. There is no audio, Kernel, or any IO. It can't lock at 60fps, and uses far more CPU than the official emulator.

It cannot run the official rom, so needs its own. This is build in the 'rom' folder. Its currently just enough to handle the video interrupts and a reset.

Its currently targetting 'R38' compatibility in terms of where the ROM\RAM banks swap register is kept.

The purpose of the emulator is to provide a platform for a debugger that developer IDEs can connect to via 'Debug Adaptor Protocol' so we can develop within VSC or your editor of choice. With all the things we take for granted such as breakpoints, stepping, call stacks, etc.

## How To Use

As the code isn't ready to be used as a standalone project I'll skip this part until its a bit more mature.
