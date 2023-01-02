using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera.Display;

[TestClass]
public class Tiles_2Bpp
{
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
    
                    ; write a 2bpp test tile
                    lda #$00
                    sta DATA0
                    lda #$01
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$04
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$40
                    sta DATA0

                    ; 0, 0, 3, 3, 0, 1, 2, 3
                    lda #$01
                    sta DATA0
                    lda #$00
                    sta DATA0       
                    lda #$04
                    sta DATA0
                    lda #$2a
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$2f
                    sta DATA0
                    lda #$40
                    sta DATA0
                    lda #$2f
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ; clear whole page
                    ldx #2
                    stx $03
                    ldx #0
                    ldy #60
                    lda #0
                    .clear_loop:
                    stz DATA0
                    sta DATA0
                    eor #4              ; flip
                    dex
                    bne clear_loop
                    eor #8              ; flip

                    phx
                    ldx $03
                    dex
                    bne no_reset
                    ldx #2
                    clc
                    adc #$10
                    .no_reset:
                    stx $03

                    plx
                    dey
                    bne clear_loop

    
                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$71
                    sta L0_CONFIG ; 128x64 tiles, 2bpp

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    ldx #128
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

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_2bpp_l0_8x8_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_2bpp_l0_8x8_normal.png");
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
    
                    ; write a 2bpp test tile
                    lda #$00
                    sta DATA0
                    lda #$01
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$04
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$40
                    sta DATA0

                    ; 0, 0, 3, 3, 0, 1, 2, 3
                    lda #$01
                    sta DATA0
                    lda #$00
                    sta DATA0       
                    lda #$04
                    sta DATA0
                    lda #$2a
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$2f
                    sta DATA0
                    lda #$40
                    sta DATA0
                    lda #$2f
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ; clear whole page
                    ldx #2
                    stx $03
                    ldx #0
                    ldy #60
                    lda #0
                    .clear_loop:
                    stz DATA0
                    sta DATA0
                    eor #4              ; flip
                    dex
                    bne clear_loop
                    eor #8              ; flip

                    phx
                    ldx $03
                    dex
                    bne no_reset
                    ldx #2
                    clc
                    adc #$10
                    .no_reset:
                    stx $03

                    plx
                    dey
                    bne clear_loop

    
                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$71
                    sta L1_CONFIG ; 128x64 tiles, 2bpp

                    lda #$21
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    ldx #128
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

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_2bpp_l1_8x8_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_2bpp_l1_8x8_normal.png");
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
    
                    ; write a 2bpp test tile
                    lda #$00
                    sta DATA0
                    lda #$01
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$04
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$40
                    sta DATA0

                    ; 0, 0, 3, 3, 0, 1, 2, 3
                    lda #$01
                    sta DATA0
                    lda #$00
                    sta DATA0       
                    lda #$04
                    sta DATA0
                    lda #$2a
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$2f
                    sta DATA0
                    lda #$40
                    sta DATA0
                    lda #$2f
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ; clear whole page
                    ldx #2
                    stx $03
                    ldx #0
                    ldy #60
                    lda #0
                    .clear_loop:
                    stz DATA0
                    sta DATA0
                    eor #4              ; flip
                    dex
                    bne clear_loop
                    eor #8              ; flip

                    phx
                    ldx $03
                    dex
                    bne no_reset
                    ldx #2
                    clc
                    adc #$10
                    .no_reset:
                    stx $03

                    plx
                    dey
                    bne clear_loop

    
                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$71
                    sta L0_CONFIG ; 128x64 tiles, 2bpp

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    ldx #128
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

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_2bpp_l0_8x8_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_2bpp_l0_8x8_shifted.png");
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
    
                    ; write a 2bpp test tile
                    lda #$00
                    sta DATA0
                    lda #$01
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$04
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$40
                    sta DATA0

                    ; 0, 0, 3, 3, 0, 1, 2, 3
                    lda #$01
                    sta DATA0
                    lda #$00
                    sta DATA0       
                    lda #$04
                    sta DATA0
                    lda #$2a
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$2f
                    sta DATA0
                    lda #$40
                    sta DATA0
                    lda #$2f
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ; clear whole page
                    ldx #2
                    stx $03
                    ldx #0
                    ldy #60
                    lda #0
                    .clear_loop:
                    stz DATA0
                    sta DATA0
                    eor #4              ; flip
                    dex
                    bne clear_loop
                    eor #8              ; flip

                    phx
                    ldx $03
                    dex
                    bne no_reset
                    ldx #2
                    clc
                    adc #$10
                    .no_reset:
                    stx $03

                    plx
                    dey
                    bne clear_loop

    
                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$71
                    sta L1_CONFIG ; 128x64 tiles, 2bpp

                    lda #$21
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    ldx #128
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

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_2bpp_l1_8x8_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_2bpp_l1_8x8_shifted.png");
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
                    
                    lda #02
                    sta L0_TILEBASE ; 16x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 2bpp test tile
                    lda #$00
                    sta DATA0
                    lda #$01
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$04
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$40
                    sta DATA0

                    lda #$01
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$04
                    sta DATA0
                    lda #$2a
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$2f
                    sta DATA0
                    lda #$40
                    sta DATA0
                    lda #$2f
                    sta DATA0


                    lda #$55
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$ff
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0

                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ; clear whole page
                    ldx #2
                    stx $03
                    ldx #0
                    ldy #60
                    lda #0
                    .clear_loop:
                    stz DATA0
                    sta DATA0
                    eor #4              ; flip
                    dex
                    bne clear_loop
                    eor #8              ; flip

                    phx
                    ldx $03
                    dex
                    bne no_reset
                    ldx #2
                    clc
                    adc #$10
                    .no_reset:
                    stx $03

                    plx
                    dey
                    bne clear_loop

    
                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$71
                    sta L0_CONFIG ; 128x64 tiles, 2bpp

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    ldx #128
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

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_2bpp_l0_8x16_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_2bpp_l0_8x16_normal.png");
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
                    
                    lda #02
                    sta L0_TILEBASE ; 16x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 2bpp test tile
                    lda #$00
                    sta DATA0
                    lda #$01
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$04
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$40
                    sta DATA0

                    lda #$01
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$04
                    sta DATA0
                    lda #$2a
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$2f
                    sta DATA0
                    lda #$40
                    sta DATA0
                    lda #$2f
                    sta DATA0


                    lda #$55
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$ff
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0

                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ; clear whole page
                    ldx #2
                    stx $03
                    ldx #0
                    ldy #60
                    lda #0
                    .clear_loop:
                    stz DATA0
                    sta DATA0
                    eor #4              ; flip
                    dex
                    bne clear_loop
                    eor #8              ; flip

                    phx
                    ldx $03
                    dex
                    bne no_reset
                    ldx #2
                    clc
                    adc #$10
                    .no_reset:
                    stx $03

                    plx
                    dey
                    bne clear_loop

    
                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$71
                    sta L0_CONFIG ; 128x64 tiles, 2bpp

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    ldx #128
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

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_2bpp_l0_8x16_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_2bpp_l0_8x16_shifted.png");
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
                    
                    lda #02
                    sta L1_TILEBASE ; 16x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 2bpp test tile
                    lda #$00
                    sta DATA0
                    lda #$01
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$04
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$40
                    sta DATA0

                    lda #$01
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$04
                    sta DATA0
                    lda #$2a
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$2f
                    sta DATA0
                    lda #$40
                    sta DATA0
                    lda #$2f
                    sta DATA0


                    lda #$55
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$ff
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0

                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ; clear whole page
                    ldx #2
                    stx $03
                    ldx #0
                    ldy #60
                    lda #0
                    .clear_loop:
                    stz DATA0
                    sta DATA0
                    eor #4              ; flip
                    dex
                    bne clear_loop
                    eor #8              ; flip

                    phx
                    ldx $03
                    dex
                    bne no_reset
                    ldx #2
                    clc
                    adc #$10
                    .no_reset:
                    stx $03

                    plx
                    dey
                    bne clear_loop

    
                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$71
                    sta L1_CONFIG ; 128x64 tiles, 2bpp

                    lda #$21
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    ldx #128
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

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_2bpp_l1_8x16_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_2bpp_l1_8x16_normal.png");
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
                    
                    lda #02
                    sta L1_TILEBASE ; 16x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 2bpp test tile
                    lda #$00
                    sta DATA0
                    lda #$01
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$04
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$40
                    sta DATA0

                    lda #$01
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$04
                    sta DATA0
                    lda #$2a
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$2f
                    sta DATA0
                    lda #$40
                    sta DATA0
                    lda #$2f
                    sta DATA0


                    lda #$55
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$aa
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$ff
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0

                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0
                    lda #$5a
                    sta DATA0
                    lda #$f0
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ; clear whole page
                    ldx #2
                    stx $03
                    ldx #0
                    ldy #60
                    lda #0
                    .clear_loop:
                    stz DATA0
                    sta DATA0
                    eor #4              ; flip
                    dex
                    bne clear_loop
                    eor #8              ; flip

                    phx
                    ldx $03
                    dex
                    bne no_reset
                    ldx #2
                    clc
                    adc #$10
                    .no_reset:
                    stx $03

                    plx
                    dey
                    bne clear_loop

    
                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$71
                    sta L1_CONFIG ; 128x64 tiles, 2bpp

                    lda #$21
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    ldx #128
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

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_2bpp_l1_8x16_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_2bpp_l1_8x16_shifted.png");
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

                    lda #01
                    sta L0_TILEBASE ; 16x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 2bpp test tile
                    lda #$15
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0
    
                    lda #$4b
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$21
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$01
                    sta DATA0
                    lda #$20
                    sta DATA0
                    lda #$00
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$12
                    sta DATA0
                    lda #$00
                    sta DATA0
    
                    lda #$4b
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$01
                    sta DATA0
                    lda #$20
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$12
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ; clear whole page
                    ldx #2
                    stx $03
                    ldx #0
                    ldy #60
                    lda #0
                    .clear_loop:
                    stz DATA0
                    sta DATA0
                    eor #4              ; flip
                    dex
                    bne clear_loop
                    eor #8              ; flip

                    phx
                    ldx $03
                    dex
                    bne no_reset
                    ldx #2
                    clc
                    adc #$10
                    .no_reset:
                    stx $03

                    plx
                    dey
                    bne clear_loop

    
                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$71
                    sta L0_CONFIG ; 128x64 tiles, 2bpp

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    ldx #128
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

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_2bpp_l0_16x8_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_2bpp_l0_16x8_normal.png");
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

                    lda #01
                    sta L1_TILEBASE ; 16x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                     ; write a 2bpp test tile
                    lda #$15
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0
    
                    lda #$4b
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$21
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$01
                    sta DATA0
                    lda #$20
                    sta DATA0
                    lda #$00
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$12
                    sta DATA0
                    lda #$00
                    sta DATA0
    
                    lda #$4b
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$01
                    sta DATA0
                    lda #$20
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$12
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ; clear whole page
                    ldx #2
                    stx $03
                    ldx #0
                    ldy #60
                    lda #0
                    .clear_loop:
                    stz DATA0
                    sta DATA0
                    eor #4              ; flip
                    dex
                    bne clear_loop
                    eor #8              ; flip

                    phx
                    ldx $03
                    dex
                    bne no_reset
                    ldx #2
                    clc
                    adc #$10
                    .no_reset:
                    stx $03

                    plx
                    dey
                    bne clear_loop

    
                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$71
                    sta L1_CONFIG ; 128x64 tiles, 2bpp

                    lda #$21
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    ldx #128
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

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_2bpp_l1_16x8_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_2bpp_l1_16x8_normal.png");
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

                    lda #01
                    sta L0_TILEBASE ; 16x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 2bpp test tile
                    lda #$15
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0
    
                    lda #$4b
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$21
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$01
                    sta DATA0
                    lda #$20
                    sta DATA0
                    lda #$00
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$12
                    sta DATA0
                    lda #$00
                    sta DATA0
    
                    lda #$4b
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$01
                    sta DATA0
                    lda #$20
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$12
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ; clear whole page
                    ldx #2
                    stx $03
                    ldx #0
                    ldy #60
                    lda #0
                    .clear_loop:
                    stz DATA0
                    sta DATA0
                    eor #4              ; flip
                    dex
                    bne clear_loop
                    eor #8              ; flip

                    phx
                    ldx $03
                    dex
                    bne no_reset
                    ldx #2
                    clc
                    adc #$10
                    .no_reset:
                    stx $03

                    plx
                    dey
                    bne clear_loop

    
                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$71
                    sta L0_CONFIG ; 128x64 tiles, 2bpp

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    ldx #128
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

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_2bpp_l0_16x8_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_2bpp_l0_16x8_shifted.png");
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

                    lda #01
                    sta L1_TILEBASE ; 16x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                     ; write a 2bpp test tile
                    lda #$15
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$55
                    sta DATA0
                    lda #$55
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0
    
                    lda #$4b
                    sta DATA0
                    lda #$10
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$21
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$01
                    sta DATA0
                    lda #$20
                    sta DATA0
                    lda #$00
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$12
                    sta DATA0
                    lda #$00
                    sta DATA0
    
                    lda #$4b
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$01
                    sta DATA0
                    lda #$20
                    sta DATA0

                    lda #$4b
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$00
                    sta DATA0
                    lda #$12
                    sta DATA0

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ; clear whole page
                    ldx #2
                    stx $03
                    ldx #0
                    ldy #60
                    lda #0
                    .clear_loop:
                    stz DATA0
                    sta DATA0
                    eor #4              ; flip
                    dex
                    bne clear_loop
                    eor #8              ; flip

                    phx
                    ldx $03
                    dex
                    bne no_reset
                    ldx #2
                    clc
                    adc #$10
                    .no_reset:
                    stx $03

                    plx
                    dey
                    bne clear_loop

    
                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$71
                    sta L1_CONFIG ; 128x64 tiles, 2bpp

                    lda #$21
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    ldx #128
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

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_2bpp_l1_16x8_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_2bpp_l1_16x8_shifted.png");
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

                    lda #03
                    sta L0_TILEBASE ; 16x16, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
                      
                    ; write a 2bpp test tile
                    ldx #0
                .load_loop:
                    lda tile_data, x
                    sta DATA0
                    inx
                    cpx #(4*16)
                    bne load_loop

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ; clear whole page
                    ldx #2
                    stx $03
                    ldx #0
                    ldy #60
                    lda #0
                    .clear_loop:
                    stz DATA0
                    sta DATA0
                    eor #4              ; flip
                    dex
                    bne clear_loop
                    eor #8              ; flip

                    phx
                    ldx $03
                    dex
                    bne no_reset
                    ldx #2
                    clc
                    adc #$10
                    .no_reset:
                    stx $03

                    plx
                    dey
                    bne clear_loop

    
                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$71
                    sta L0_CONFIG ; 128x64 tiles, 2bpp

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    ldx #128
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
                .align $100
                .tile_data:
                ; 16x16
                .byte $ff, $ff, $ff, $ff
                .byte $c5, $55, $50, $0c
                .byte $c5, $55, $00, $30
                .byte $c5, $50, $00, $c0
                .byte $c5, $00, $03, $00
                .byte $c0, $00, $0c, $00
                .byte $ca, $aa, $30, $00
                .byte $ca, $aa, $c0, $00
                .byte $ca, $03, $00, $00
                .byte $c0, $0c, $00, $00
                .byte $c0, $30, $00, $00
                .byte $c0, $c0, $00, $00
                .byte $c3, $00, $00, $a5
                .byte $cc, $00, $0a, $55
                .byte $f0, $00, $a5, $55
                .byte $c0, $00, $a5, $55
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_2bpp_l0_16x16_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_2bpp_l0_16x16_normal.png");
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

                    lda #03
                    sta L1_TILEBASE ; 16x16, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 2bpp test tile
                    ldx #0
                .load_loop:
                    lda tile_data, x
                    sta DATA0
                    inx
                    cpx #(4*16)
                    bne load_loop

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ; clear whole page
                    ldx #2
                    stx $03
                    ldx #0
                    ldy #60
                    lda #0
                    .clear_loop:
                    stz DATA0
                    sta DATA0
                    eor #4              ; flip
                    dex
                    bne clear_loop
                    eor #8              ; flip

                    phx
                    ldx $03
                    dex
                    bne no_reset
                    ldx #2
                    clc
                    adc #$10
                    .no_reset:
                    stx $03

                    plx
                    dey
                    bne clear_loop

    
                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$71
                    sta L1_CONFIG ; 128x64 tiles, 2bpp

                    lda #$21
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    ldx #128
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
                .align $100
                .tile_data:
                ; 16x16
                .byte $ff, $ff, $ff, $ff
                .byte $c5, $55, $50, $0c
                .byte $c5, $55, $00, $30
                .byte $c5, $50, $00, $c0
                .byte $c5, $00, $03, $00
                .byte $c0, $00, $0c, $00
                .byte $ca, $aa, $30, $00
                .byte $ca, $aa, $c0, $00
                .byte $ca, $03, $00, $00
                .byte $c0, $0c, $00, $00
                .byte $c0, $30, $00, $00
                .byte $c0, $c0, $00, $00
                .byte $c3, $00, $00, $a5
                .byte $cc, $00, $0a, $55
                .byte $f0, $00, $a5, $55
                .byte $c0, $00, $a5, $55
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_2bpp_l1_16x16_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_2bpp_l1_16x16_normal.png");
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

                    lda #03
                    sta L0_TILEBASE ; 16x16, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 2bpp test tile
                    ldx #0
                .load_loop:
                    lda tile_data, x
                    sta DATA0
                    inx
                    cpx #(4*16)
                    bne load_loop

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ; clear whole page
                    ldx #2
                    stx $03
                    ldx #0
                    ldy #60
                    lda #0
                    .clear_loop:
                    stz DATA0
                    sta DATA0
                    eor #4              ; flip
                    dex
                    bne clear_loop
                    eor #8              ; flip

                    phx
                    ldx $03
                    dex
                    bne no_reset
                    ldx #2
                    clc
                    adc #$10
                    .no_reset:
                    stx $03

                    plx
                    dey
                    bne clear_loop

    
                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$71
                    sta L0_CONFIG ; 128x64 tiles, 2bpp

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    ldx #128
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
                .align $100
                .tile_data:
                ; 16x16
                .byte $ff, $ff, $ff, $ff
                .byte $c5, $55, $50, $0c
                .byte $c5, $55, $00, $30
                .byte $c5, $50, $00, $c0
                .byte $c5, $00, $03, $00
                .byte $c0, $00, $0c, $00
                .byte $ca, $aa, $30, $00
                .byte $ca, $aa, $c0, $00
                .byte $ca, $03, $00, $00
                .byte $c0, $0c, $00, $00
                .byte $c0, $30, $00, $00
                .byte $c0, $c0, $00, $00
                .byte $c3, $00, $00, $a5
                .byte $cc, $00, $0a, $55
                .byte $f0, $00, $a5, $55
                .byte $c0, $00, $a5, $55
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_2bpp_l0_16x16_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_2bpp_l0_16x16_shifted.png");
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

                    lda #03
                    sta L1_TILEBASE ; 16x16, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 2bpp test tile
                    ldx #0
                .load_loop:
                    lda tile_data, x
                    sta DATA0
                    inx
                    cpx #(4*16)
                    bne load_loop

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ; clear whole page
                    ldx #2
                    stx $03
                    ldx #0
                    ldy #60
                    lda #0
                    .clear_loop:
                    stz DATA0
                    sta DATA0
                    eor #4              ; flip
                    dex
                    bne clear_loop
                    eor #8              ; flip

                    phx
                    ldx $03
                    dex
                    bne no_reset
                    ldx #2
                    clc
                    adc #$10
                    .no_reset:
                    stx $03

                    plx
                    dey
                    bne clear_loop

    
                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$71
                    sta L1_CONFIG ; 128x64 tiles, 2bpp

                    lda #$21
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    ldx #128
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
                .align $100
                .tile_data:
                ; 16x16
                .byte $ff, $ff, $ff, $ff
                .byte $c5, $55, $50, $0c
                .byte $c5, $55, $00, $30
                .byte $c5, $50, $00, $c0
                .byte $c5, $00, $03, $00
                .byte $c0, $00, $0c, $00
                .byte $ca, $aa, $30, $00
                .byte $ca, $aa, $c0, $00
                .byte $ca, $03, $00, $00
                .byte $c0, $0c, $00, $00
                .byte $c0, $30, $00, $00
                .byte $c0, $c0, $00, $00
                .byte $c3, $00, $00, $a5
                .byte $cc, $00, $0a, $55
                .byte $f0, $00, $a5, $55
                .byte $c0, $00, $a5, $55
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_2bpp_l1_16x16_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_2bpp_l1_16x16_shifted.png");
    }

    [TestMethod]
    public async Task Multiple_16x16_Layer0()
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

                    lda #03
                    sta L0_TILEBASE ; 16x16, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 2bpp test tile
                .load_loop:
                    lda tile_data, x
                    sta DATA0
                    inx
                    bne load_loop

                    ; Tile map details
                    lda #$11
                    sta ADDRx_H
                    lda #$40
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L

                    ; clear whole page
                    ldx #2
                    stx $03
                    ldx #0
                    ldy #60
                    lda #0
                    stz $04
                .clear_loop:
                    pha
                    lda $04
                    clc
                    inc
                    and #$07
                    sta $04
                    clc
                    ror
                    sta DATA0
                    pla
                    sta DATA0
                    eor #4              ; flip
                    dex
                    bne clear_loop
                    eor #8              ; flip

                    phx
                    ldx $03
                    dex
                    bne no_reset
                    ldx #2
                    clc
                    adc #$10
                    .no_reset:
                    stx $03

                    plx
                    dey
                    bne clear_loop

    
                    ; background colour
                    lda #$11
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    lda #$12
                    sta DATA0

                    lda #$71
                    sta L0_CONFIG ; 128x64 tiles, 2bpp

                    lda #$11
                    sta DC_VIDEO ; enable layer 0

                    ldx #128
                    stx DC_VSCALE  
                    ldx #128
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
                .align $100
                .tile_data:
                ; 16x16
                ;1
                .byte $55, $55, $55, $55
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $50, $00, $01
                .byte $40, $55, $00, $01
                .byte $40, $55, $50, $01
                .byte $40, $55, $55, $01
                .byte $40, $55, $55, $01
                .byte $40, $55, $50, $01
                .byte $40, $55, $00, $01
                .byte $40, $50, $00, $01
                .byte $40, $00, $05, $01
                .byte $40, $00, $55, $01
                .byte $40, $05, $55, $01
                .byte $55, $55, $55, $55
                ; 2
                .byte $55, $55, $55, $55
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $a0, $00, $01
                .byte $40, $aa, $00, $01
                .byte $40, $aa, $a0, $01
                .byte $40, $aa, $aa, $01
                .byte $40, $aa, $aa, $01
                .byte $40, $aa, $a0, $01
                .byte $40, $aa, $00, $01
                .byte $40, $a0, $0a, $01
                .byte $40, $00, $aa, $01
                .byte $40, $0a, $aa, $01
                .byte $40, $aa, $aa, $01
                .byte $55, $55, $55, $55
                ; 3
                .byte $55, $55, $55, $55
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $f0, $00, $01
                .byte $40, $ff, $00, $01
                .byte $40, $ff, $f0, $01
                .byte $40, $ff, $ff, $01
                .byte $40, $ff, $ff, $01
                .byte $40, $ff, $f0, $01
                .byte $40, $ff, $00, $01
                .byte $40, $f0, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $55, $55, $55, $55
                ; 4
                .byte $55, $55, $55, $55
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $40, $00, $00, $01
                .byte $55, $55, $55, $55
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_2bpp_l0_16x16_multiple.png");
        emulator.CompareImage(@"Vera\Images\tile_2bpp_l0_16x16_multiple.png");
    }
}
