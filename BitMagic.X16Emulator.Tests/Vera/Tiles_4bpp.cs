using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera.Display;

[TestClass]
public class Tiles_4Bpp
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
                    
                    lda #00
                    sta L0_TILEBASE ; 16x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 4bpp test tile
                    ldx #0
                .load_loop:
                    lda tile_data, x
                    sta DATA0
                    inx
                    cpx #(4*8*4)
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

                    lda #$72
                    sta L0_CONFIG ; 128x64 tiles, 4bpp

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
                ; 8x8
                ;1
                .byte $11, $11, $11, $17
                .byte $10, $00, $00, $76
                .byte $10, $00, $07, $65
                .byte $10, $00, $76, $54
                .byte $10, $07, $65, $43
                .byte $10, $76, $54, $32
                .byte $17, $65, $43, $21
                .byte $76, $11, $11, $10

                ;2
                .byte $11, $11, $11, $1b
                .byte $10, $00, $00, $ba
                .byte $10, $00, $0b, $a9
                .byte $10, $00, $ba, $98
                .byte $10, $0b, $a9, $87
                .byte $10, $ba, $98, $76
                .byte $1b, $a9, $87, $65
                .byte $ba, $98, $76, $54
                ;3
                .byte $11, $11, $11, $1f
                .byte $10, $00, $00, $fe
                .byte $10, $00, $0f, $ed
                .byte $10, $00, $fe, $dc
                .byte $10, $0f, $ed, $cb
                .byte $10, $fe, $dc, $ba
                .byte $1f, $ed, $cb, $a9
                .byte $fe, $dc, $ba, $98
                ;4
                .byte $11, $11, $11, $11
                .byte $10, $00, $00, $01
                .byte $10, $00, $00, $01
                .byte $10, $00, $00, $01
                .byte $10, $00, $00, $01
                .byte $10, $00, $00, $01
                .byte $10, $00, $00, $01
                .byte $11, $11, $11, $11
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_4bpp_l0_8x8_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_4bpp_l0_8x8_normal.png");
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
                    
                    lda #00
                    sta L0_TILEBASE ; 16x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 4bpp test tile
                    ldx #0
                .load_loop:
                    lda tile_data, x
                    sta DATA0
                    inx
                    cpx #(4*8*4)
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

                    lda #$72
                    sta L0_CONFIG ; 128x64 tiles, 4bpp

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
                ; 8x8
                ;1
                .byte $11, $11, $11, $17
                .byte $10, $00, $00, $76
                .byte $10, $00, $07, $65
                .byte $10, $00, $76, $54
                .byte $10, $07, $65, $43
                .byte $10, $76, $54, $32
                .byte $17, $65, $43, $21
                .byte $76, $11, $11, $10

                ;2
                .byte $11, $11, $11, $1b
                .byte $10, $00, $00, $ba
                .byte $10, $00, $0b, $a9
                .byte $10, $00, $ba, $98
                .byte $10, $0b, $a9, $87
                .byte $10, $ba, $98, $76
                .byte $1b, $a9, $87, $65
                .byte $ba, $98, $76, $54
                ;3
                .byte $11, $11, $11, $1f
                .byte $10, $00, $00, $fe
                .byte $10, $00, $0f, $ed
                .byte $10, $00, $fe, $dc
                .byte $10, $0f, $ed, $cb
                .byte $10, $fe, $dc, $ba
                .byte $1f, $ed, $cb, $a9
                .byte $fe, $dc, $ba, $98
                ;4
                .byte $11, $11, $11, $11
                .byte $10, $00, $00, $01
                .byte $10, $00, $00, $01
                .byte $10, $00, $00, $01
                .byte $10, $00, $00, $01
                .byte $10, $00, $00, $01
                .byte $10, $00, $00, $01
                .byte $11, $11, $11, $11
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_4bpp_l0_8x8_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_4bpp_l0_8x8_shifted.png");
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
                    ldx #0
                .load_loop:
                    lda tile_data, x
                    sta DATA0
                    inx
                ;    cpx #(4*16*4)
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

                    lda #$72
                    sta L0_CONFIG ; 128x64 tiles, 4bpp

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
                ; 8x16
                ;1
                .byte $11, $11, $11, $11
                .byte $10, $00, $00, $01
                .byte $10, $00, $00, $01
                .byte $10, $00, $00, $01
                .byte $10, $50, $00, $01
                .byte $10, $54, $00, $01
                .byte $10, $54, $30, $01
                .byte $10, $54, $32, $01
                .byte $10, $54, $32, $01
                .byte $1a, $54, $30, $a1
                .byte $1b, $54, $00, $b1
                .byte $1c, $50, $00, $c1
                .byte $1d, $00, $00, $d1
                .byte $1e, $00, $00, $e1
                .byte $1f, $00, $00, $f1
                .byte $11, $11, $11, $11
                ;2
                .byte $22, $22, $22, $22
                .byte $20, $00, $00, $02
                .byte $20, $00, $00, $02
                .byte $20, $00, $00, $02
                .byte $20, $70, $00, $02
                .byte $20, $76, $00, $02
                .byte $20, $76, $50, $02
                .byte $20, $76, $54, $02
                .byte $20, $76, $54, $02
                .byte $2a, $76, $50, $a2
                .byte $2b, $76, $00, $b2
                .byte $2c, $70, $00, $c2
                .byte $2d, $00, $00, $d2
                .byte $2e, $00, $00, $e2
                .byte $2f, $00, $00, $f2
                .byte $22, $22, $22, $22
                ;3
                .byte $33, $33, $33, $33
                .byte $30, $00, $00, $03
                .byte $30, $00, $00, $03
                .byte $30, $00, $00, $03
                .byte $30, $90, $00, $03
                .byte $30, $98, $00, $03
                .byte $30, $98, $70, $03
                .byte $30, $98, $76, $03
                .byte $30, $98, $76, $03
                .byte $3a, $98, $70, $a3
                .byte $3b, $98, $00, $b3
                .byte $3c, $90, $00, $c3
                .byte $3d, $00, $00, $d3
                .byte $3e, $00, $00, $e3
                .byte $3f, $00, $00, $f3
                .byte $33, $33, $33, $33
                ;4
                .byte $44, $44, $44, $44
                .byte $40, $00, $00, $04
                .byte $40, $00, $00, $04
                .byte $40, $00, $00, $04
                .byte $40, $b0, $00, $04
                .byte $40, $ba, $00, $04
                .byte $40, $ba, $90, $04
                .byte $40, $ba, $98, $04
                .byte $40, $ba, $98, $04
                .byte $4a, $ba, $90, $a4
                .byte $4b, $ba, $00, $b4
                .byte $4c, $b0, $00, $c4
                .byte $4d, $00, $00, $d4
                .byte $4e, $00, $00, $e4
                .byte $4f, $00, $00, $f4
                .byte $44, $44, $44, $44

                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_4bpp_l0_8x16_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_4bpp_l0_8x16_normal.png");
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
                    ldx #0
                .load_loop:
                    lda tile_data, x
                    sta DATA0
                    inx
                ;    cpx #(4*16*4)
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

                    lda #$72
                    sta L0_CONFIG ; 128x64 tiles, 4bpp

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
                ; 8x16
                ;1
                .byte $11, $11, $11, $11
                .byte $10, $00, $00, $01
                .byte $10, $00, $00, $01
                .byte $10, $00, $00, $01
                .byte $10, $50, $00, $01
                .byte $10, $54, $00, $01
                .byte $10, $54, $30, $01
                .byte $10, $54, $32, $01
                .byte $10, $54, $32, $01
                .byte $1a, $54, $30, $a1
                .byte $1b, $54, $00, $b1
                .byte $1c, $50, $00, $c1
                .byte $1d, $00, $00, $d1
                .byte $1e, $00, $00, $e1
                .byte $1f, $00, $00, $f1
                .byte $11, $11, $11, $11
                ;2
                .byte $22, $22, $22, $22
                .byte $20, $00, $00, $02
                .byte $20, $00, $00, $02
                .byte $20, $00, $00, $02
                .byte $20, $70, $00, $02
                .byte $20, $76, $00, $02
                .byte $20, $76, $50, $02
                .byte $20, $76, $54, $02
                .byte $20, $76, $54, $02
                .byte $2a, $76, $50, $a2
                .byte $2b, $76, $00, $b2
                .byte $2c, $70, $00, $c2
                .byte $2d, $00, $00, $d2
                .byte $2e, $00, $00, $e2
                .byte $2f, $00, $00, $f2
                .byte $22, $22, $22, $22
                ;3
                .byte $33, $33, $33, $33
                .byte $30, $00, $00, $03
                .byte $30, $00, $00, $03
                .byte $30, $00, $00, $03
                .byte $30, $90, $00, $03
                .byte $30, $98, $00, $03
                .byte $30, $98, $70, $03
                .byte $30, $98, $76, $03
                .byte $30, $98, $76, $03
                .byte $3a, $98, $70, $a3
                .byte $3b, $98, $00, $b3
                .byte $3c, $90, $00, $c3
                .byte $3d, $00, $00, $d3
                .byte $3e, $00, $00, $e3
                .byte $3f, $00, $00, $f3
                .byte $33, $33, $33, $33
                ;4
                .byte $44, $44, $44, $44
                .byte $40, $00, $00, $04
                .byte $40, $00, $00, $04
                .byte $40, $00, $00, $04
                .byte $40, $b0, $00, $04
                .byte $40, $ba, $00, $04
                .byte $40, $ba, $90, $04
                .byte $40, $ba, $98, $04
                .byte $40, $ba, $98, $04
                .byte $4a, $ba, $90, $a4
                .byte $4b, $ba, $00, $b4
                .byte $4c, $b0, $00, $c4
                .byte $4d, $00, $00, $d4
                .byte $4e, $00, $00, $e4
                .byte $4f, $00, $00, $f4
                .byte $44, $44, $44, $44
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_4bpp_l0_8x16_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_4bpp_l0_8x16_shifted.png");
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
                    sta L0_TILEBASE ; 8x16, tiles are at $00000

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
                ;    cpx #(4*16*4)
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

                    lda #$72
                    sta L0_CONFIG ; 128x64 tiles, 4bpp

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
                ; 16x8
                ;1
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                ;2
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                .byte $10, $00, $00, $00, $35, $55, $22, $21
                .byte $10, $00, $00, $02, $55, $55, $22, $21
                .byte $10, $00, $00, $00, $35, $55, $22, $21
                .byte $10, $00, $00, $02, $55, $55, $22, $21
                .byte $10, $00, $00, $00, $35, $55, $22, $21
                .byte $10, $00, $00, $02, $55, $55, $22, $21
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                ;3
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                .byte $10, $00, $00, $00, $e0, $00, $00, $21
                .byte $10, $00, $00, $0f, $00, $00, $00, $21
                .byte $10, $00, $00, $00, $e0, $00, $00, $21
                .byte $10, $00, $00, $0f, $00, $00, $00, $21
                .byte $10, $00, $00, $00, $e0, $00, $00, $21
                .byte $10, $00, $00, $0f, $00, $00, $00, $21
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                ;4
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $11, $11, $11, $11, $11, $11, $11, $11

                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_4bpp_l0_16x8_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_4bpp_l0_16x8_normal.png");
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
                    sta L0_TILEBASE ; 8x16, tiles are at $00000

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
                ;    cpx #(4*16*4)
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

                    lda #$72
                    sta L0_CONFIG ; 128x64 tiles, 4bpp

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
                ; 16x8
                ;1
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                ;2
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                .byte $10, $00, $00, $00, $35, $55, $22, $21
                .byte $10, $00, $00, $02, $55, $55, $22, $21
                .byte $10, $00, $00, $00, $35, $55, $22, $21
                .byte $10, $00, $00, $02, $55, $55, $22, $21
                .byte $10, $00, $00, $00, $35, $55, $22, $21
                .byte $10, $00, $00, $02, $55, $55, $22, $21
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                ;3
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                .byte $10, $00, $00, $00, $e0, $00, $00, $21
                .byte $10, $00, $00, $0f, $00, $00, $00, $21
                .byte $10, $00, $00, $00, $e0, $00, $00, $21
                .byte $10, $00, $00, $0f, $00, $00, $00, $21
                .byte $10, $00, $00, $00, $e0, $00, $00, $21
                .byte $10, $00, $00, $0f, $00, $00, $00, $21
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                ;4
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_4bpp_l0_16x8_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_4bpp_l0_16x8_shifted.png");
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
                    sta L0_TILEBASE ; 8x16, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 4bpp test tile - do it twice
                    ldx #0
                .load_loop:
                    lda tile_data, x
                    sta DATA0
                    inx
                    bne load_loop

                    ldx #0
                .load_loop2:
                    lda tile_data2, x
                    sta DATA0
                    inx
                    bne load_loop2

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

                    lda #$72
                    sta L0_CONFIG ; 128x64 tiles, 4bpp

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
                ;1
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $11, $10, $00, $00, $22, $22, $22, $21
                .byte $11, $10, $00, $00, $22, $22, $22, $21
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                ;2
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                .byte $10, $00, $00, $00, $35, $55, $22, $21
                .byte $10, $00, $00, $02, $55, $55, $22, $21
                .byte $10, $00, $00, $00, $35, $55, $22, $21
                .byte $10, $00, $00, $02, $55, $55, $22, $21
                .byte $10, $00, $00, $00, $35, $55, $22, $21
                .byte $10, $00, $00, $02, $55, $55, $22, $21
                .byte $10, $00, $00, $00, $35, $55, $22, $21
                .byte $10, $00, $00, $02, $55, $55, $22, $21
                .byte $10, $00, $00, $00, $35, $55, $22, $21
                .byte $10, $00, $00, $02, $55, $55, $22, $21
                .byte $10, $00, $00, $00, $35, $55, $22, $21
                .byte $10, $00, $00, $02, $55, $55, $22, $21
                .byte $11, $10, $00, $00, $35, $55, $22, $21
                .byte $11, $10, $00, $02, $55, $55, $22, $21
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                .tile_data2:
                ;3
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                .byte $10, $00, $00, $00, $e0, $00, $00, $21
                .byte $10, $00, $00, $0f, $00, $00, $00, $21
                .byte $10, $00, $00, $00, $e0, $00, $00, $21
                .byte $10, $00, $00, $0f, $00, $00, $00, $21
                .byte $10, $00, $00, $00, $e0, $00, $00, $21
                .byte $10, $00, $00, $0f, $00, $00, $00, $21
                .byte $10, $00, $00, $00, $e0, $00, $00, $21
                .byte $10, $00, $00, $0f, $00, $00, $00, $21
                .byte $10, $00, $00, $00, $e0, $00, $00, $21
                .byte $10, $00, $00, $0f, $00, $00, $00, $21
                .byte $10, $00, $00, $00, $e0, $00, $00, $21
                .byte $10, $00, $00, $0f, $00, $00, $00, $21
                .byte $11, $10, $00, $00, $e0, $00, $00, $21
                .byte $11, $10, $00, $0f, $00, $00, $00, $21
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                ;4
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $11, $11, $11, $11, $11, $11, $11, $11


                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_4bpp_l0_16x16_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_4bpp_l0_16x16_normal.png");
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
                    sta L0_TILEBASE ; 8x16, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 4bpp test tile -- do it twice
                    ldx #0
                .load_loop:
                    lda tile_data, x
                    sta DATA0
                    inx
                    bne load_loop

                    ldx #0
                .load_loop2:
                    lda tile_data2, x
                    sta DATA0
                    inx
                    bne load_loop2

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

                    lda #$72
                    sta L0_CONFIG ; 128x64 tiles, 4bpp

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
                ; 16x8
                ;1
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $10, $00, $00, $00, $22, $22, $22, $21
                .byte $11, $10, $00, $00, $22, $22, $22, $21
                .byte $11, $10, $00, $00, $22, $22, $22, $21
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                ;2
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                .byte $10, $00, $00, $00, $35, $55, $22, $21
                .byte $10, $00, $00, $02, $55, $55, $22, $21
                .byte $10, $00, $00, $00, $35, $55, $22, $21
                .byte $10, $00, $00, $02, $55, $55, $22, $21
                .byte $10, $00, $00, $00, $35, $55, $22, $21
                .byte $10, $00, $00, $02, $55, $55, $22, $21
                .byte $10, $00, $00, $00, $35, $55, $22, $21
                .byte $10, $00, $00, $02, $55, $55, $22, $21
                .byte $10, $00, $00, $00, $35, $55, $22, $21
                .byte $10, $00, $00, $02, $55, $55, $22, $21
                .byte $10, $00, $00, $00, $35, $55, $22, $21
                .byte $10, $00, $00, $02, $55, $55, $22, $21
                .byte $11, $10, $00, $00, $35, $55, $22, $21
                .byte $11, $10, $00, $02, $55, $55, $22, $21
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                .tile_data2:
                ;3
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                .byte $10, $00, $00, $00, $e0, $00, $00, $21
                .byte $10, $00, $00, $0f, $00, $00, $00, $21
                .byte $10, $00, $00, $00, $e0, $00, $00, $21
                .byte $10, $00, $00, $0f, $00, $00, $00, $21
                .byte $10, $00, $00, $00, $e0, $00, $00, $21
                .byte $10, $00, $00, $0f, $00, $00, $00, $21
                .byte $10, $00, $00, $00, $e0, $00, $00, $21
                .byte $10, $00, $00, $0f, $00, $00, $00, $21
                .byte $10, $00, $00, $00, $e0, $00, $00, $21
                .byte $10, $00, $00, $0f, $00, $00, $00, $21
                .byte $10, $00, $00, $00, $e0, $00, $00, $21
                .byte $10, $00, $00, $0f, $00, $00, $00, $21
                .byte $11, $10, $00, $00, $e0, $00, $00, $21
                .byte $11, $10, $00, $0f, $00, $00, $00, $21
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                ;4
                .byte $11, $11, $11, $11, $11, $11, $11, $11
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $12, $34, $56, $78, $9a, $bc, $de, $f1
                .byte $11, $11, $11, $11, $11, $11, $11, $11

                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_4bpp_l0_16x16_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_4bpp_l0_16x16_shifted.png");
    }
}
