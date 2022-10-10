using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class Tiles_1Bpp
{
    [TestMethod]
    public async Task Normal_Layer0()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L0_MAPBASE

                    stz L0_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    ; Tile map details - top left
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    lda #01;

                    ldy #60
                .line_loop:
                    ldx #128

                .step_loop:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    inc

                    dex
                    bne step_loop

                    dey
                    bne line_loop

                    ldy #4
                .line_loop2:
                    ldx #128

                .step_loop2:
                    stz DATA0   ; tile number
                    stz DATA0   ; colour

                    inc

                    dex
                    bne step_loop2

                    dey
                    bne line_loop2

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$60        
                    sta L0_CONFIG

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    stx DC_HSCALE

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l0_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l0_normal.png");
    }

    [TestMethod]
    public async Task Normal_Layer1()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L1_MAPBASE

                    stz L1_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    ; Tile map details - top left
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    lda #01;

                    ldy #60
                .line_loop:
                    ldx #128

                .step_loop:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    inc

                    dex
                    bne step_loop

                    dey
                    bne line_loop

                    ldy #4
                .line_loop2:
                    ldx #128

                .step_loop2:
                    stz DATA0   ; tile number
                    stz DATA0   ; colour

                    inc

                    dex
                    bne step_loop2

                    dey
                    bne line_loop2

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$60        
                    sta L1_CONFIG

                    lda #$21
                    sta DC_VIDEO ; enable layer 1

                    ldx #128
                    stx DC_VSCALE  
                    stx DC_HSCALE

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l1_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l1_normal.png");
    }

    [TestMethod]
    public async Task Normal_200VScale_Layer0()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L0_MAPBASE

                    stz L0_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    ; Tile map details - top left
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    lda #01;

                    ldy #60
                .line_loop:
                    ldx #128

                .step_loop:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    inc

                    dex
                    bne step_loop

                    dey
                    bne line_loop

                    ldy #4
                .line_loop2:
                    ldx #128

                .step_loop2:
                    stz DATA0   ; tile number
                    stz DATA0   ; colour

                    inc

                    dex
                    bne step_loop2

                    dey
                    bne line_loop2

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$60        
                    sta L0_CONFIG

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #200
                    stx DC_VSCALE  
                    ldx #128
                    stx DC_HSCALE

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l0_200vscale.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l0_200vscale.png");
    }

    [TestMethod]
    public async Task Normal_200VScale_Layer1()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L1_MAPBASE

                    stz L1_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    ; Tile map details - top left
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    lda #01;

                    ldy #60
                .line_loop:
                    ldx #128

                .step_loop:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    inc

                    dex
                    bne step_loop

                    dey
                    bne line_loop

                    ldy #4
                .line_loop2:
                    ldx #128

                .step_loop2:
                    stz DATA0   ; tile number
                    stz DATA0   ; colour

                    inc

                    dex
                    bne step_loop2

                    dey
                    bne line_loop2

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$60        
                    sta L1_CONFIG

                    lda #$21
                    sta DC_VIDEO ; enable layer 1

                    ldx #200
                    stx DC_VSCALE  
                    ldx #128
                    stx DC_HSCALE

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp",
                emulator);

        emulator.CompareImage(@"Vera\Images\tile_1bpp_l1_200vscale.png");
    }


    [TestMethod]
    public async Task Normal_192HScale_Layer0()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                 .machine CommanderX16R40
                .org $801

                .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
                .org $810
                    ;jmp ($fffc)
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L0_MAPBASE

                    stz L0_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ; blank lines
                    ldx #03         ; 3 blank lines
                    jsr blank_line

                    jsr test_data   ; 5 lines

                    ; blank lines
                    ldx #10         ; 10 blank lines
                    jsr blank_line
        
                    jsr test_data   ; 5 lines
    
                    ; blank lines
                    ldx #37         ; 10 blank lines
                    jsr blank_line

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$70        
                    sta L0_CONFIG ; 128x64 tiles

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    ldx #$c0
                    stx DC_HSCALE

                    lda #136
                    sta IRQLINE_L

                    lda #02
                    sta IEN

                    sta CTRL    ; set to 2, so we can change DC_HSTART

                    ldy #02

                .loop:
                    wai
                    lda ISR
                    and #$02
                    beq loop
                    sta ISR

                    lda #32
                    sta DC_HSTART

                    lda #136+56
                    sta IRQLINE_L

                .second_wait:
                    wai
                    lda ISR
                    and #$02
                    beq second_wait
                    sta ISR

                    stz DC_HSTART
    
                    lda #136
                    sta IRQLINE_L

                    dey
                    beq done

                    jmp loop
                .done:
                    stp

                .proc fill_vera
                .next_char:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    dex
                    bne next_char

                    rts
                .endproc

                .proc blank_line

                .new_line:
                    ldy #00
                .next_char:
                    stz DATA0
                    stz DATA0
                    iny
                    bne next_char
                    dex
                    bne new_line
                    rts
                .endproc

                .proc test_data
                    ; blue tiles
                    lda #$e6;
                    ldx #$50-2         ; width
                    jsr fill_vera

                    ; red tiles
                    lda #$a2;
                    ldx #$b0+2        ; width
                    jsr fill_vera
    
                    ; blue tiles
                    lda #$e6;
                    ldx #$50-1         ; width
                    jsr fill_vera

                    ; red tiles
                    lda #$a2;
                    ldx #$b0+1         ; width
                    jsr fill_vera

                    ; blue tiles
                    lda #$d5;
                    ldx #$50            ; width
                    jsr fill_vera

                    ; red tiles
                    lda #$a2;
                    ldx #$b0            ; width
                    jsr fill_vera
    
                    ; blue tiles
                    lda #$e6;
                    ldx #$50+1         ; width
                    jsr fill_vera

                    ; red tiles
                    lda #$a2;
                    ldx #$b0-1         ; width
                    jsr fill_vera
    
                    ; blue tiles
                    lda #$e6;
                    ldx #$50+2        ; width
                    jsr fill_vera

                    ; red tiles
                    lda #$a2;
                    ldx #$b0-2         ; width
                    jsr fill_vera

                    rts
                .endproc",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l0_192hscale.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l0_192hscale.png");
    }

    [TestMethod]
    public async Task Normal_192HScale_Layer1()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                 .machine CommanderX16R40
                .org $801

                .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
                .org $810
                    ;jmp ($fffc)
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L1_MAPBASE

                    stz L1_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ; blank lines
                    ldx #03         ; 3 blank lines
                    jsr blank_line

                    jsr test_data   ; 5 lines

                    ; blank lines
                    ldx #10         ; 10 blank lines
                    jsr blank_line
        
                    jsr test_data   ; 5 lines
    
                    ; blank lines
                    ldx #37         ; 10 blank lines
                    jsr blank_line

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$70        
                    sta L1_CONFIG ; 128x64 tiles

                    lda #$21
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    ldx #$c0
                    stx DC_HSCALE

                    lda #136
                    sta IRQLINE_L

                    lda #02
                    sta IEN

                    sta CTRL    ; set to 2, so we can change DC_HSTART

                    ldy #02

                .loop:
                    wai
                    lda ISR
                    and #$02
                    beq loop
                    sta ISR

                    lda #32
                    sta DC_HSTART

                    lda #136+56
                    sta IRQLINE_L

                .second_wait:
                    wai
                    lda ISR
                    and #$02
                    beq second_wait
                    sta ISR

                    stz DC_HSTART
    
                    lda #136
                    sta IRQLINE_L

                    dey
                    beq done

                    jmp loop
                .done:
                    stp

                .proc fill_vera
                .next_char:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    dex
                    bne next_char

                    rts
                .endproc

                .proc blank_line

                .new_line:
                    ldy #00
                .next_char:
                    stz DATA0
                    stz DATA0
                    iny
                    bne next_char
                    dex
                    bne new_line
                    rts
                .endproc

                .proc test_data
                    ; blue tiles
                    lda #$e6;
                    ldx #$50-2         ; width
                    jsr fill_vera

                    ; red tiles
                    lda #$a2;
                    ldx #$b0+2        ; width
                    jsr fill_vera
    
                    ; blue tiles
                    lda #$e6;
                    ldx #$50-1         ; width
                    jsr fill_vera

                    ; red tiles
                    lda #$a2;
                    ldx #$b0+1         ; width
                    jsr fill_vera

                    ; blue tiles
                    lda #$d5;
                    ldx #$50            ; width
                    jsr fill_vera

                    ; red tiles
                    lda #$a2;
                    ldx #$b0            ; width
                    jsr fill_vera
    
                    ; blue tiles
                    lda #$e6;
                    ldx #$50+1         ; width
                    jsr fill_vera

                    ; red tiles
                    lda #$a2;
                    ldx #$b0-1         ; width
                    jsr fill_vera
    
                    ; blue tiles
                    lda #$e6;
                    ldx #$50+2        ; width
                    jsr fill_vera

                    ; red tiles
                    lda #$a2;
                    ldx #$b0-2         ; width
                    jsr fill_vera

                    rts
                .endproc",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l1_192hscale.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l1_192hscale.png");
    }

    [TestMethod]
    public async Task Normal_8x8_Layer0()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                     .machine CommanderX16R40
                .org $801

                .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
                .org $810
                    ;jmp ($fffc)
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L0_MAPBASE

                    stz L0_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ldy #16
                    lda #00

                    .yloop:
                    ldx #16
                    .xloop:
                    stz DATA0
                    sta DATA0
                    inc
                    dex
                    bne xloop
                    pha
                    ldx #$f0
                    lda 0
                    jsr fill_vera
                    pla
                    dey
                    bne yloop

                    ldx #14
                    jsr blank_line

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$70  
                    sta L0_CONFIG ; 128x64 tiles

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #64
                    stx DC_VSCALE  
                    stx DC_HSCALE

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp 

                .proc fill_vera
                .next_char:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    dex
                    bne next_char

                    rts
                .endproc

                .proc blank_line

                .new_line:
                    ldy #00
                .next_char:
                    stz DATA0
                    stz DATA0
                    iny
                    bne next_char
                    dex
                    bne new_line
                    rts
                .endproc
                ",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l0_8x8_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l0_8x8_normal.png");
    }

    [TestMethod]
    public async Task Normal_8x8_Layer1()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $801

                .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
                .org $810
                    ;jmp ($fffc)
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L1_MAPBASE

                    stz L1_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ldy #16
                    lda #00

                    .yloop:
                    ldx #16
                    .xloop:
                    stz DATA0
                    sta DATA0
                    inc
                    dex
                    bne xloop
                    pha
                    ldx #$f0
                    lda 0
                    jsr fill_vera
                    pla
                    dey
                    bne yloop

                    ldx #14
                    jsr blank_line

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$70
                    sta L1_CONFIG ; 128x64 tiles 

                    lda #$21
                    sta DC_VIDEO ; enable layer 0

                    ldx #64
                    stx DC_VSCALE  
                    stx DC_HSCALE

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp 

                .proc fill_vera
                .next_char:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    dex
                    bne next_char

                    rts
                .endproc

                .proc blank_line

                .new_line:
                    ldy #00
                .next_char:
                    stz DATA0
                    stz DATA0
                    iny
                    bne next_char
                    dex
                    bne new_line
                    rts
                .endproc
                ",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l1_8x8_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l1_8x8_normal.png");
    }

    [TestMethod]
    public async Task Normal_8x16_Layer0()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $801

                .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
                .org $810
                    ;jmp ($fffc)
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L0_MAPBASE

                    lda #$02
                    sta L0_TILEBASE ; 16x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$55
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ldy #16
                    lda #00

                    .yloop:
                    ldx #16
                    .xloop:
                    stz DATA0
                    sta DATA0
                    inc
                    dex
                    bne xloop
                    pha
                    ldx #$f0
                    lda 0
                    jsr fill_vera
                    pla
                    dey
                    bne yloop

                    ldx #14
                    jsr blank_line

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$70
                    sta L0_CONFIG ; 128x64 tiles

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    stx DC_HSCALE

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp 

                .proc fill_vera
                .next_char:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    dex
                    bne next_char

                    rts
                .endproc

                .proc blank_line

                .new_line:
                    ldy #00
                .next_char:
                    stz DATA0
                    stz DATA0
                    iny
                    bne next_char
                    dex
                    bne new_line
                    rts
                .endproc
                ",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l0_8x16_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l0_8x16_normal.png");
    }

    [TestMethod]
    public async Task Normal_8x16_Layer1()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $801

                .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
                .org $810
                    ;jmp ($fffc)
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L1_MAPBASE

                    lda #$02
                    sta L1_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$55
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ldy #16
                    lda #00

                    .yloop:
                    ldx #16
                    .xloop:
                    stz DATA0
                    sta DATA0
                    inc
                    dex
                    bne xloop
                    pha
                    ldx #$f0
                    lda 0
                    jsr fill_vera
                    pla
                    dey
                    bne yloop

                    ldx #14
                    jsr blank_line

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$70
                    sta L1_CONFIG ; 128x64 tiles

                    lda #$21
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    stx DC_HSCALE

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp 

                .proc fill_vera
                .next_char:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    dex
                    bne next_char

                    rts
                .endproc

                .proc blank_line

                .new_line:
                    ldy #00
                .next_char:
                    stz DATA0
                    stz DATA0
                    iny
                    bne next_char
                    dex
                    bne new_line
                    rts
                .endproc
                ",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l1_8x16_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l1_8x16_normal.png");
    }

    [TestMethod]
    public async Task Normal_16x8_Layer0()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $801

                .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
                .org $810
                    ;jmp ($fffc)
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L0_MAPBASE

                    lda #$01
                    sta L0_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0    
                    lda #$aa
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$55
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ldy #16
                    lda #00

                    .yloop:
                    ldx #16
                    .xloop:
                    stz DATA0
                    sta DATA0
                    inc
                    dex
                    bne xloop
                    pha
                    ldx #$f0
                    lda 0
                    jsr fill_vera
                    pla
                    dey
                    bne yloop

                    ldx #14
                    jsr blank_line

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$70
                    sta L0_CONFIG ; 128x64 tiles

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    stx DC_HSCALE

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp 

                .proc fill_vera
                .next_char:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    dex
                    bne next_char

                    rts
                .endproc

                .proc blank_line

                .new_line:
                    ldy #00
                .next_char:
                    stz DATA0
                    stz DATA0
                    iny
                    bne next_char
                    dex
                    bne new_line
                    rts
                .endproc
                ",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l0_16x8_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l0_16x8_normal.png");
    }

    [TestMethod]
    public async Task Normal_16x8_Layer1()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $801

                .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
                .org $810
                    ;jmp ($fffc)
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L1_MAPBASE

                    lda #$01
                    sta L1_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0    
                    lda #$aa
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$55
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ldy #16
                    lda #00

                    .yloop:
                    ldx #16
                    .xloop:
                    stz DATA0
                    sta DATA0
                    inc
                    dex
                    bne xloop
                    pha
                    ldx #$f0
                    lda 0
                    jsr fill_vera
                    pla
                    dey
                    bne yloop

                    ldx #14
                    jsr blank_line

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$70
                    sta L1_CONFIG ; 128x64 tiles

                    lda #$21
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    stx DC_HSCALE

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp 

                .proc fill_vera
                .next_char:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    dex
                    bne next_char

                    rts
                .endproc

                .proc blank_line

                .new_line:
                    ldy #00
                .next_char:
                    stz DATA0
                    stz DATA0
                    iny
                    bne next_char
                    dex
                    bne new_line
                    rts
                .endproc
                ",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l1_16x8_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l1_16x8_normal.png");
    }

    [TestMethod]
    public async Task Normal_16x16_Layer0()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $801

                .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
                .org $810
                    ;jmp ($fffc)
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L0_MAPBASE

                    lda #$03
                    sta L0_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0    
                    lda #$aa
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$55
                    sta DATA0


                    lda #$aa
                    sta DATA0
                    lda #$f0
                    sta DATA0    

                    lda #$55
                    sta DATA0
                    lda #$f0
                    sta DATA0

                    lda #$aa
                    sta DATA0
                    lda #$f0
                    sta DATA0    

                    lda #$55
                    sta DATA0
                    lda #$f0
                    sta DATA0

                    lda #$aa
                    sta DATA0
                    lda #$0f
                    sta DATA0    

                    lda #$55
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    lda #$aa
                    sta DATA0
                    lda #$0f
                    sta DATA0    

                    lda #$55
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ldy #16
                    lda #00

                    .yloop:
                    ldx #16
                    .xloop:
                    stz DATA0
                    sta DATA0
                    inc
                    dex
                    bne xloop
                    pha
                    ldx #$f0
                    lda 0
                    jsr fill_vera
                    pla
                    dey
                    bne yloop

                    ldx #14
                    jsr blank_line

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$70
                    sta L0_CONFIG ; 128x64 tiles

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    stx DC_HSCALE

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp 

                .proc fill_vera
                .next_char:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    dex
                    bne next_char

                    rts
                .endproc

                .proc blank_line

                .new_line:
                    ldy #00
                .next_char:
                    stz DATA0
                    stz DATA0
                    iny
                    bne next_char
                    dex
                    bne new_line
                    rts
                .endproc
                ",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l0_16x16_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l0_16x16_normal.png");
    }

    [TestMethod]
    public async Task Normal_16x16_Layer1()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $801

                .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
                .org $810
                    ;jmp ($fffc)
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L1_MAPBASE

                    lda #$03
                    sta L1_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0    
                    lda #$aa
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$55
                    sta DATA0


                    lda #$aa
                    sta DATA0
                    lda #$f0
                    sta DATA0    

                    lda #$55
                    sta DATA0
                    lda #$f0
                    sta DATA0

                    lda #$aa
                    sta DATA0
                    lda #$f0
                    sta DATA0    

                    lda #$55
                    sta DATA0
                    lda #$f0
                    sta DATA0

                    lda #$aa
                    sta DATA0
                    lda #$0f
                    sta DATA0    

                    lda #$55
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    lda #$aa
                    sta DATA0
                    lda #$0f
                    sta DATA0    

                    lda #$55
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ldy #16
                    lda #00

                    .yloop:
                    ldx #16
                    .xloop:
                    stz DATA0
                    sta DATA0
                    inc
                    dex
                    bne xloop
                    pha
                    ldx #$f0
                    lda 0
                    jsr fill_vera
                    pla
                    dey
                    bne yloop

                    ldx #14
                    jsr blank_line

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$70
                    sta L1_CONFIG ; 128x64 tiles

                    lda #$21
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    stx DC_HSCALE

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp 

                .proc fill_vera
                .next_char:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    dex
                    bne next_char

                    rts
                .endproc

                .proc blank_line

                .new_line:
                    ldy #00
                .next_char:
                    stz DATA0
                    stz DATA0
                    iny
                    bne next_char
                    dex
                    bne new_line
                    rts
                .endproc
                ",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l1_16x16_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l1_16x16_normal.png");
    }

    [TestMethod]
    public async Task Shifted_8x8_Layer0()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                     .machine CommanderX16R40
                .org $801

                .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
                .org $810
                    ;jmp ($fffc)
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L0_MAPBASE

                    stz L0_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ldy #16
                    lda #00

                    .yloop:
                    ldx #16
                    .xloop:
                    stz DATA0
                    sta DATA0
                    inc
                    dex
                    bne xloop
                    pha
                    ldx #$f0
                    lda 0
                    jsr fill_vera
                    pla
                    dey
                    bne yloop

                    ldx #14
                    jsr blank_line

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$70
                    sta L0_CONFIG ; 128x64 tiles

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #64
                    stx DC_VSCALE  
                    stx DC_HSCALE

                    lda #09
                    sta L0_HSCROLL_L
                    lda #10
                    sta L0_VSCROLL_L

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp 

                .proc fill_vera
                .next_char:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    dex
                    bne next_char

                    rts
                .endproc

                .proc blank_line

                .new_line:
                    ldy #00
                .next_char:
                    stz DATA0
                    stz DATA0
                    iny
                    bne next_char
                    dex
                    bne new_line
                    rts
                .endproc
                ",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l0_8x8_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l0_8x8_shifted.png");
    }

    [TestMethod]
    public async Task Shifted_8x8_Layer1()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $801

                .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
                .org $810
                    ;jmp ($fffc)
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L1_MAPBASE

                    stz L1_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ldy #16
                    lda #00

                    .yloop:
                    ldx #16
                    .xloop:
                    stz DATA0
                    sta DATA0
                    inc
                    dex
                    bne xloop
                    pha
                    ldx #$f0
                    lda 0
                    jsr fill_vera
                    pla
                    dey
                    bne yloop

                    ldx #14
                    jsr blank_line

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$70
                    sta L1_CONFIG ; 128x64 tiles

                    lda #$21
                    sta DC_VIDEO ; enable layer 0

                    ldx #64
                    stx DC_VSCALE  
                    stx DC_HSCALE

                    lda #09
                    sta L1_HSCROLL_L
                    lda #10
                    sta L1_VSCROLL_L

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp 

                .proc fill_vera
                .next_char:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    dex
                    bne next_char

                    rts
                .endproc

                .proc blank_line

                .new_line:
                    ldy #00
                .next_char:
                    stz DATA0
                    stz DATA0
                    iny
                    bne next_char
                    dex
                    bne new_line
                    rts
                .endproc
                ",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l1_8x8_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l1_8x8_shifted.png");
    }

    [TestMethod]
    public async Task Shifted_8x16_Layer0()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $801

                .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
                .org $810
                    ;jmp ($fffc)
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L0_MAPBASE

                    lda #$02
                    sta L0_TILEBASE ; 16x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$55
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ldy #16
                    lda #00

                    .yloop:
                    ldx #16
                    .xloop:
                    stz DATA0
                    sta DATA0
                    inc
                    dex
                    bne xloop
                    pha
                    ldx #$f0
                    lda 0
                    jsr fill_vera
                    pla
                    dey
                    bne yloop

                    ldx #14
                    jsr blank_line

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$70
                    sta L0_CONFIG ; 128x64 tiles

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    stx DC_HSCALE

                    lda #09
                    sta L0_HSCROLL_L
                    lda #10
                    sta L0_VSCROLL_L

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp 

                .proc fill_vera
                .next_char:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    dex
                    bne next_char

                    rts
                .endproc

                .proc blank_line

                .new_line:
                    ldy #00
                .next_char:
                    stz DATA0
                    stz DATA0
                    iny
                    bne next_char
                    dex
                    bne new_line
                    rts
                .endproc
                ",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l0_8x16_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l0_8x16_shifted.png");
    }

    [TestMethod]
    public async Task Shifted_8x16_Layer1()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $801

                .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
                .org $810
                    ;jmp ($fffc)
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L1_MAPBASE

                    lda #$02
                    sta L1_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$55
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ldy #16
                    lda #00

                    .yloop:
                    ldx #16
                    .xloop:
                    stz DATA0
                    sta DATA0
                    inc
                    dex
                    bne xloop
                    pha
                    ldx #$f0
                    lda 0
                    jsr fill_vera
                    pla
                    dey
                    bne yloop

                    ldx #14
                    jsr blank_line

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$70
                    sta L1_CONFIG ; 128x64 tiles

                    lda #$21
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    stx DC_HSCALE

                    lda #09
                    sta L1_HSCROLL_L
                    lda #10
                    sta L1_VSCROLL_L

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp 

                .proc fill_vera
                .next_char:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    dex
                    bne next_char

                    rts
                .endproc

                .proc blank_line

                .new_line:
                    ldy #00
                .next_char:
                    stz DATA0
                    stz DATA0
                    iny
                    bne next_char
                    dex
                    bne new_line
                    rts
                .endproc
                ",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l1_8x16_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l1_8x16_shifted.png");
    }

    [TestMethod]
    public async Task Shifted_16x8_Layer0()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $801

                .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
                .org $810
                    ;jmp ($fffc)
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L0_MAPBASE

                    lda #$01
                    sta L0_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0    
                    lda #$aa
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$55
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ldy #16
                    lda #00

                    .yloop:
                    ldx #16
                    .xloop:
                    stz DATA0
                    sta DATA0
                    inc
                    dex
                    bne xloop
                    pha
                    ldx #$f0
                    lda 0
                    jsr fill_vera
                    pla
                    dey
                    bne yloop

                    ldx #14
                    jsr blank_line

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$70
                    sta L0_CONFIG ; 128x64 tiles

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    stx DC_HSCALE

                    lda #09
                    sta L0_HSCROLL_L
                    lda #10
                    sta L0_VSCROLL_L

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp 

                .proc fill_vera
                .next_char:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    dex
                    bne next_char

                    rts
                .endproc

                .proc blank_line

                .new_line:
                    ldy #00
                .next_char:
                    stz DATA0
                    stz DATA0
                    iny
                    bne next_char
                    dex
                    bne new_line
                    rts
                .endproc
                ",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l0_16x8_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l0_16x8_shifted.png");
    }

    [TestMethod]
    public async Task Shifted_16x8_Layer1()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $801

                .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
                .org $810
                    ;jmp ($fffc)
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L1_MAPBASE

                    lda #$01
                    sta L1_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0    
                    lda #$aa
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$55
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ldy #16
                    lda #00

                    .yloop:
                    ldx #16
                    .xloop:
                    stz DATA0
                    sta DATA0
                    inc
                    dex
                    bne xloop
                    pha
                    ldx #$f0
                    lda 0
                    jsr fill_vera
                    pla
                    dey
                    bne yloop

                    ldx #14
                    jsr blank_line

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$70
                    sta L1_CONFIG ; 128x64 tiles

                    lda #$21
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    stx DC_HSCALE

                    lda #09
                    sta L1_HSCROLL_L
                    lda #10
                    sta L1_VSCROLL_L

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp 

                .proc fill_vera
                .next_char:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    dex
                    bne next_char

                    rts
                .endproc

                .proc blank_line

                .new_line:
                    ldy #00
                .next_char:
                    stz DATA0
                    stz DATA0
                    iny
                    bne next_char
                    dex
                    bne new_line
                    rts
                .endproc
                ",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l1_16x8_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l1_16x8_shifted.png");
    }


    [TestMethod]
    public async Task Shifted_16x16_Layer0()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $801

                .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
                .org $810
                    ;jmp ($fffc)
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L0_MAPBASE

                    lda #$03
                    sta L0_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0    
                    lda #$aa
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$55
                    sta DATA0


                    lda #$aa
                    sta DATA0
                    lda #$f0
                    sta DATA0    

                    lda #$55
                    sta DATA0
                    lda #$f0
                    sta DATA0

                    lda #$aa
                    sta DATA0
                    lda #$f0
                    sta DATA0    

                    lda #$55
                    sta DATA0
                    lda #$f0
                    sta DATA0

                    lda #$aa
                    sta DATA0
                    lda #$0f
                    sta DATA0    

                    lda #$55
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    lda #$aa
                    sta DATA0
                    lda #$0f
                    sta DATA0    

                    lda #$55
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ldy #16
                    lda #00

                    .yloop:
                    ldx #16
                    .xloop:
                    stz DATA0
                    sta DATA0
                    inc
                    dex
                    bne xloop
                    pha
                    ldx #$f0
                    lda 0
                    jsr fill_vera
                    pla
                    dey
                    bne yloop

                    ldx #14
                    jsr blank_line

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$70
                    sta L0_CONFIG ; 128x64 tiles

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    stx DC_HSCALE

                    lda #09
                    sta L0_HSCROLL_L
                    lda #10
                    sta L0_VSCROLL_L

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp 

                .proc fill_vera
                .next_char:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    dex
                    bne next_char

                    rts
                .endproc

                .proc blank_line

                .new_line:
                    ldy #00
                .next_char:
                    stz DATA0
                    stz DATA0
                    iny
                    bne next_char
                    dex
                    bne new_line
                    rts
                .endproc
                ",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l0_16x16_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l0_16x16_shifted.png");
    }

    [TestMethod]
    public async Task Shifted_16x16_Layer1()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $801

                .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
                .org $810
                    ;jmp ($fffc)
                    sei

                    lda #02
                    sta DC_BORDER

                    lda #$a0        ; map is at $14000
                    sta L1_MAPBASE

                    lda #$03
                    sta L1_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a tile
                    lda #$f0
                    sta DATA0    
                    lda #$aa
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$f0
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$aa
                    sta DATA0

                    lda #$0f
                    sta DATA0
                    lda #$55
                    sta DATA0


                    lda #$aa
                    sta DATA0
                    lda #$f0
                    sta DATA0    

                    lda #$55
                    sta DATA0
                    lda #$f0
                    sta DATA0

                    lda #$aa
                    sta DATA0
                    lda #$f0
                    sta DATA0    

                    lda #$55
                    sta DATA0
                    lda #$f0
                    sta DATA0

                    lda #$aa
                    sta DATA0
                    lda #$0f
                    sta DATA0    

                    lda #$55
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    lda #$aa
                    sta DATA0
                    lda #$0f
                    sta DATA0    

                    lda #$55
                    sta DATA0
                    lda #$0f
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ldy #16
                    lda #00

                    .yloop:
                    ldx #16
                    .xloop:
                    stz DATA0
                    sta DATA0
                    inc
                    dex
                    bne xloop
                    pha
                    ldx #$f0
                    lda 0
                    jsr fill_vera
                    pla
                    dey
                    bne yloop

                    ldx #14
                    jsr blank_line

                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$70
                    sta L1_CONFIG ; 128x64 tiles

                    lda #$21
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    stx DC_HSCALE

                    lda #09
                    sta L1_HSCROLL_L
                    lda #10
                    sta L1_VSCROLL_L

                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai

                    stp 

                .proc fill_vera
                .next_char:
                    stz DATA0   ; tile number
                    sta DATA0   ; colour

                    dex
                    bne next_char

                    rts
                .endproc

                .proc blank_line

                .new_line:
                    ldy #00
                .next_char:
                    stz DATA0
                    stz DATA0
                    iny
                    bne next_char
                    dex
                    bne new_line
                    rts
                .endproc
                ",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_1bpp_l1_16x16_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_1bpp_l1_16x16_shifted.png");
    }
}