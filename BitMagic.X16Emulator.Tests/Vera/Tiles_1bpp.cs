using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class Tiles_1Bpp
{
    [TestMethod]
    public async Task Normal_Layer0()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;

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

        emulator.A = 0x03;

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

        emulator.A = 0x03;

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

        emulator.A = 0x03;

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
}