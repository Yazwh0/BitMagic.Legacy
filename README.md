# BitMagic

BitMagic is currently an assembler template engine and compiler for the Commander X16, although in theory it would work for any 6502\65c02 based machine.

BitMagic also includes a work in progress emulator, and will eventually include a Debugger Adaptor to allow development from within tools like Visual Studio Code.

## Compiler

Todo.

## Template Engine

BitMagic uses ASP.NETs Razor template engine to achieve what would commonly be called 'Macros' in other assembly implementations. In the same way you can use C# to generate HTML pages, we use the engine to generate .asm files.

The files that the Razor engine works on have the extension '.csasm'. (in a nod to the .cshtml file extension)

Unlike HTMLs tags which makes differentiation easy, BitMagic will examine each line of the input file. If it stars with a '.', or a three letter asm opcode, it will be flagged as part of the asm file. If not, it will be parsed as C#. You can still use C# on a line of assembler code, using the @ notation.

If you're not familiar with the Razor templating engine, its probably worth reading a few beginners guides first.

Here is an example of some csasm code:

    reference System.Drawing.Primitives; // reference and use system.drawing as it contains the color struct.
    using System.Drawing;git

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

When compiled this will create a .prg file with the following:

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

The values being loaded into DATA0 match what we expect. (Note, they're displayed in decimal rather than hex.)
