using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

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

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_2bpp_l0_8x8_normal.png");
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

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_2bpp_l1_8x8_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_2bpp_l1_8x8_normal.png");
    }
}
