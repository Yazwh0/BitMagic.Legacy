using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera.Display;

[TestClass]
public class Tiles_8Bpp
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
                    sta L0_TILEBASE ; 8x8, tiles are at $00000

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

                    lda #$73
                    sta L0_CONFIG ; 128x64 tiles, 8bpp

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
                .byte $00, $01, $02, $03, $04, $05, $06, $07
                .byte $08, $09, $0a, $0b, $0c, $0d, $0e, $0f
                .byte $10, $11, $12, $13, $14, $15, $16, $17
                .byte $18, $19, $1a, $1b, $1c, $1d, $1e, $1f
                .byte $20, $21, $22, $23, $24, $25, $26, $27
                .byte $28, $29, $2a, $2b, $2c, $2d, $2e, $2f
                .byte $30, $31, $32, $33, $34, $35, $36, $37
                .byte $38, $39, $3a, $3b, $3c, $3d, $3e, $3f
                ;2
                .byte $40, $41, $42, $43, $44, $45, $46, $47
                .byte $48, $49, $4a, $4b, $4c, $4d, $4e, $4f
                .byte $50, $51, $52, $53, $54, $55, $56, $57
                .byte $58, $59, $5a, $5b, $5c, $5d, $5e, $5f
                .byte $60, $61, $62, $63, $64, $65, $66, $67
                .byte $68, $69, $6a, $6b, $6c, $6d, $6e, $6f
                .byte $70, $71, $72, $73, $74, $75, $76, $77
                .byte $78, $79, $7a, $7b, $7c, $7d, $7e, $7f
                ;3
                .byte $80, $81, $82, $83, $84, $85, $86, $87
                .byte $88, $89, $8a, $8b, $8c, $8d, $8e, $8f
                .byte $90, $91, $92, $93, $94, $95, $96, $97
                .byte $98, $99, $9a, $9b, $9c, $9d, $9e, $9f
                .byte $a0, $a1, $a2, $a3, $a4, $a5, $a6, $a7
                .byte $a8, $a9, $aa, $ab, $ac, $ad, $ae, $af
                .byte $b0, $b1, $b2, $b3, $b4, $b5, $b6, $b7
                .byte $b8, $b9, $ba, $bb, $bc, $bd, $be, $bf
                ;4
                .byte $c0, $c1, $c2, $c3, $c4, $c5, $c6, $c7
                .byte $c8, $c9, $ca, $cb, $cc, $cd, $ce, $cf
                .byte $d0, $d1, $d2, $d3, $d4, $d5, $d6, $d7
                .byte $d8, $d9, $da, $db, $dc, $dd, $de, $df
                .byte $e0, $e1, $e2, $e3, $e4, $e5, $e6, $e7
                .byte $e8, $e9, $ea, $eb, $ec, $ed, $ee, $ef
                .byte $f0, $f1, $f2, $f3, $f4, $f5, $f6, $f7
                .byte $f8, $f9, $fa, $fb, $fc, $fd, $fe, $ff
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_8bpp_l0_8x8_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_8bpp_l0_8x8_normal.png");
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
                    sta L0_TILEBASE ; 8x8, tiles are at $00000

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

                    lda #$73
                    sta L0_CONFIG ; 128x64 tiles, 8bpp

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
                .byte $00, $01, $02, $03, $04, $05, $06, $07
                .byte $08, $09, $0a, $0b, $0c, $0d, $0e, $0f
                .byte $10, $11, $12, $13, $14, $15, $16, $17
                .byte $18, $19, $1a, $1b, $1c, $1d, $1e, $1f
                .byte $20, $21, $22, $23, $24, $25, $26, $27
                .byte $28, $29, $2a, $2b, $2c, $2d, $2e, $2f
                .byte $30, $31, $32, $33, $34, $35, $36, $37
                .byte $38, $39, $3a, $3b, $3c, $3d, $3e, $3f
                ;2
                .byte $40, $41, $42, $43, $44, $45, $46, $47
                .byte $48, $49, $4a, $4b, $4c, $4d, $4e, $4f
                .byte $50, $51, $52, $53, $54, $55, $56, $57
                .byte $58, $59, $5a, $5b, $5c, $5d, $5e, $5f
                .byte $60, $61, $62, $63, $64, $65, $66, $67
                .byte $68, $69, $6a, $6b, $6c, $6d, $6e, $6f
                .byte $70, $71, $72, $73, $74, $75, $76, $77
                .byte $78, $79, $7a, $7b, $7c, $7d, $7e, $7f
                ;3
                .byte $80, $81, $82, $83, $84, $85, $86, $87
                .byte $88, $89, $8a, $8b, $8c, $8d, $8e, $8f
                .byte $90, $91, $92, $93, $94, $95, $96, $97
                .byte $98, $99, $9a, $9b, $9c, $9d, $9e, $9f
                .byte $a0, $a1, $a2, $a3, $a4, $a5, $a6, $a7
                .byte $a8, $a9, $aa, $ab, $ac, $ad, $ae, $af
                .byte $b0, $b1, $b2, $b3, $b4, $b5, $b6, $b7
                .byte $b8, $b9, $ba, $bb, $bc, $bd, $be, $bf
                ;4
                .byte $c0, $c1, $c2, $c3, $c4, $c5, $c6, $c7
                .byte $c8, $c9, $ca, $cb, $cc, $cd, $ce, $cf
                .byte $d0, $d1, $d2, $d3, $d4, $d5, $d6, $d7
                .byte $d8, $d9, $da, $db, $dc, $dd, $de, $df
                .byte $e0, $e1, $e2, $e3, $e4, $e5, $e6, $e7
                .byte $e8, $e9, $ea, $eb, $ec, $ed, $ee, $ef
                .byte $f0, $f1, $f2, $f3, $f4, $f5, $f6, $f7
                .byte $f8, $f9, $fa, $fb, $fc, $fd, $fe, $ff
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_8bpp_l0_8x8_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_8bpp_l0_8x8_shifted.png");
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
                    
                    lda #00
                    sta L1_TILEBASE ; 8x8, tiles are at $00000

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

                    lda #$73
                    sta L1_CONFIG ; 128x64 tiles, 8bpp

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
                ; 8x8
                ;1
                .byte $00, $01, $02, $03, $04, $05, $06, $07
                .byte $08, $09, $0a, $0b, $0c, $0d, $0e, $0f
                .byte $10, $11, $12, $13, $14, $15, $16, $17
                .byte $18, $19, $1a, $1b, $1c, $1d, $1e, $1f
                .byte $20, $21, $22, $23, $24, $25, $26, $27
                .byte $28, $29, $2a, $2b, $2c, $2d, $2e, $2f
                .byte $30, $31, $32, $33, $34, $35, $36, $37
                .byte $38, $39, $3a, $3b, $3c, $3d, $3e, $3f
                ;2
                .byte $40, $41, $42, $43, $44, $45, $46, $47
                .byte $48, $49, $4a, $4b, $4c, $4d, $4e, $4f
                .byte $50, $51, $52, $53, $54, $55, $56, $57
                .byte $58, $59, $5a, $5b, $5c, $5d, $5e, $5f
                .byte $60, $61, $62, $63, $64, $65, $66, $67
                .byte $68, $69, $6a, $6b, $6c, $6d, $6e, $6f
                .byte $70, $71, $72, $73, $74, $75, $76, $77
                .byte $78, $79, $7a, $7b, $7c, $7d, $7e, $7f
                ;3
                .byte $80, $81, $82, $83, $84, $85, $86, $87
                .byte $88, $89, $8a, $8b, $8c, $8d, $8e, $8f
                .byte $90, $91, $92, $93, $94, $95, $96, $97
                .byte $98, $99, $9a, $9b, $9c, $9d, $9e, $9f
                .byte $a0, $a1, $a2, $a3, $a4, $a5, $a6, $a7
                .byte $a8, $a9, $aa, $ab, $ac, $ad, $ae, $af
                .byte $b0, $b1, $b2, $b3, $b4, $b5, $b6, $b7
                .byte $b8, $b9, $ba, $bb, $bc, $bd, $be, $bf
                ;4
                .byte $c0, $c1, $c2, $c3, $c4, $c5, $c6, $c7
                .byte $c8, $c9, $ca, $cb, $cc, $cd, $ce, $cf
                .byte $d0, $d1, $d2, $d3, $d4, $d5, $d6, $d7
                .byte $d8, $d9, $da, $db, $dc, $dd, $de, $df
                .byte $e0, $e1, $e2, $e3, $e4, $e5, $e6, $e7
                .byte $e8, $e9, $ea, $eb, $ec, $ed, $ee, $ef
                .byte $f0, $f1, $f2, $f3, $f4, $f5, $f6, $f7
                .byte $f8, $f9, $fa, $fb, $fc, $fd, $fe, $ff
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_8bpp_l1_8x8_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_8bpp_l1_8x8_normal.png");
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
                    
                    lda #00
                    sta L1_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 8bpp test tile
                    ldx #0
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

                    lda #$73
                    sta L1_CONFIG ; 128x64 tiles, 8bpp

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
                ; 8x8
                ;1
                .byte $00, $01, $02, $03, $04, $05, $06, $07
                .byte $08, $09, $0a, $0b, $0c, $0d, $0e, $0f
                .byte $10, $11, $12, $13, $14, $15, $16, $17
                .byte $18, $19, $1a, $1b, $1c, $1d, $1e, $1f
                .byte $20, $21, $22, $23, $24, $25, $26, $27
                .byte $28, $29, $2a, $2b, $2c, $2d, $2e, $2f
                .byte $30, $31, $32, $33, $34, $35, $36, $37
                .byte $38, $39, $3a, $3b, $3c, $3d, $3e, $3f
                ;2
                .byte $40, $41, $42, $43, $44, $45, $46, $47
                .byte $48, $49, $4a, $4b, $4c, $4d, $4e, $4f
                .byte $50, $51, $52, $53, $54, $55, $56, $57
                .byte $58, $59, $5a, $5b, $5c, $5d, $5e, $5f
                .byte $60, $61, $62, $63, $64, $65, $66, $67
                .byte $68, $69, $6a, $6b, $6c, $6d, $6e, $6f
                .byte $70, $71, $72, $73, $74, $75, $76, $77
                .byte $78, $79, $7a, $7b, $7c, $7d, $7e, $7f
                ;3
                .byte $80, $81, $82, $83, $84, $85, $86, $87
                .byte $88, $89, $8a, $8b, $8c, $8d, $8e, $8f
                .byte $90, $91, $92, $93, $94, $95, $96, $97
                .byte $98, $99, $9a, $9b, $9c, $9d, $9e, $9f
                .byte $a0, $a1, $a2, $a3, $a4, $a5, $a6, $a7
                .byte $a8, $a9, $aa, $ab, $ac, $ad, $ae, $af
                .byte $b0, $b1, $b2, $b3, $b4, $b5, $b6, $b7
                .byte $b8, $b9, $ba, $bb, $bc, $bd, $be, $bf
                ;4
                .byte $c0, $c1, $c2, $c3, $c4, $c5, $c6, $c7
                .byte $c8, $c9, $ca, $cb, $cc, $cd, $ce, $cf
                .byte $d0, $d1, $d2, $d3, $d4, $d5, $d6, $d7
                .byte $d8, $d9, $da, $db, $dc, $dd, $de, $df
                .byte $e0, $e1, $e2, $e3, $e4, $e5, $e6, $e7
                .byte $e8, $e9, $ea, $eb, $ec, $ed, $ee, $ef
                .byte $f0, $f1, $f2, $f3, $f4, $f5, $f6, $f7
                .byte $f8, $f9, $fa, $fb, $fc, $fd, $fe, $ff
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_8bpp_l1_8x8_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_8bpp_l1_8x8_shifted.png");
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
                    sta L0_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 8bpp test tile
                    ldx #0
                .load_loop:
                    lda tile_data, x
                    sta DATA0
                    inx
                    bne load_loop

                    ldx #0
                .load_loop_2:
                    lda tile_data_2, x
                    sta DATA0
                    inx
                    bne load_loop_2

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

                    lda #$73
                    sta L0_CONFIG ; 128x64 tiles, 8bpp

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
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08

                .byte $00, $01, $02, $03, $04, $05, $06, $07
                .byte $08, $09, $0a, $0b, $0c, $0d, $0e, $0f
                .byte $10, $11, $12, $13, $14, $15, $16, $17
                .byte $18, $19, $1a, $1b, $1c, $1d, $1e, $1f
                .byte $20, $21, $22, $23, $24, $25, $26, $27
                .byte $28, $29, $2a, $2b, $2c, $2d, $2e, $2f
                .byte $30, $31, $32, $33, $34, $35, $36, $37
                .byte $38, $39, $3a, $3b, $3c, $3d, $3e, $3f
                ;2
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08

                .byte $40, $41, $42, $43, $44, $45, $46, $47
                .byte $48, $49, $4a, $4b, $4c, $4d, $4e, $4f
                .byte $50, $51, $52, $53, $54, $55, $56, $57
                .byte $58, $59, $5a, $5b, $5c, $5d, $5e, $5f
                .byte $60, $61, $62, $63, $64, $65, $66, $67
                .byte $68, $69, $6a, $6b, $6c, $6d, $6e, $6f
                .byte $70, $71, $72, $73, $74, $75, $76, $77
                .byte $78, $79, $7a, $7b, $7c, $7d, $7e, $7f
.tile_data_2:
                ;3
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08

                .byte $80, $81, $82, $83, $84, $85, $86, $87
                .byte $88, $89, $8a, $8b, $8c, $8d, $8e, $8f
                .byte $90, $91, $92, $93, $94, $95, $96, $97
                .byte $98, $99, $9a, $9b, $9c, $9d, $9e, $9f
                .byte $a0, $a1, $a2, $a3, $a4, $a5, $a6, $a7
                .byte $a8, $a9, $aa, $ab, $ac, $ad, $ae, $af
                .byte $b0, $b1, $b2, $b3, $b4, $b5, $b6, $b7
                .byte $b8, $b9, $ba, $bb, $bc, $bd, $be, $bf
                ;4
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08

                .byte $c0, $c1, $c2, $c3, $c4, $c5, $c6, $c7
                .byte $c8, $c9, $ca, $cb, $cc, $cd, $ce, $cf
                .byte $d0, $d1, $d2, $d3, $d4, $d5, $d6, $d7
                .byte $d8, $d9, $da, $db, $dc, $dd, $de, $df
                .byte $e0, $e1, $e2, $e3, $e4, $e5, $e6, $e7
                .byte $e8, $e9, $ea, $eb, $ec, $ed, $ee, $ef
                .byte $f0, $f1, $f2, $f3, $f4, $f5, $f6, $f7
                .byte $f8, $f9, $fa, $fb, $fc, $fd, $fe, $ff
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_8bpp_l0_8x16_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_8bpp_l0_8x16_normal.png");
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
                    sta L0_TILEBASE ; 8x16, tiles are at $00000

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
                    bne load_loop

                    ldx #0
                .load_loop_2:
                    lda tile_data_2, x
                    sta DATA0
                    inx
                    bne load_loop_2

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

                    lda #$73
                    sta L0_CONFIG ; 128x64 tiles, 8bpp

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
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08

                .byte $00, $01, $02, $03, $04, $05, $06, $07
                .byte $08, $09, $0a, $0b, $0c, $0d, $0e, $0f
                .byte $10, $11, $12, $13, $14, $15, $16, $17
                .byte $18, $19, $1a, $1b, $1c, $1d, $1e, $1f
                .byte $20, $21, $22, $23, $24, $25, $26, $27
                .byte $28, $29, $2a, $2b, $2c, $2d, $2e, $2f
                .byte $30, $31, $32, $33, $34, $35, $36, $37
                .byte $38, $39, $3a, $3b, $3c, $3d, $3e, $3f
                ;2
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08

                .byte $40, $41, $42, $43, $44, $45, $46, $47
                .byte $48, $49, $4a, $4b, $4c, $4d, $4e, $4f
                .byte $50, $51, $52, $53, $54, $55, $56, $57
                .byte $58, $59, $5a, $5b, $5c, $5d, $5e, $5f
                .byte $60, $61, $62, $63, $64, $65, $66, $67
                .byte $68, $69, $6a, $6b, $6c, $6d, $6e, $6f
                .byte $70, $71, $72, $73, $74, $75, $76, $77
                .byte $78, $79, $7a, $7b, $7c, $7d, $7e, $7f
.tile_data_2:
                ;3
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08

                .byte $80, $81, $82, $83, $84, $85, $86, $87
                .byte $88, $89, $8a, $8b, $8c, $8d, $8e, $8f
                .byte $90, $91, $92, $93, $94, $95, $96, $97
                .byte $98, $99, $9a, $9b, $9c, $9d, $9e, $9f
                .byte $a0, $a1, $a2, $a3, $a4, $a5, $a6, $a7
                .byte $a8, $a9, $aa, $ab, $ac, $ad, $ae, $af
                .byte $b0, $b1, $b2, $b3, $b4, $b5, $b6, $b7
                .byte $b8, $b9, $ba, $bb, $bc, $bd, $be, $bf
                ;4
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08

                .byte $c0, $c1, $c2, $c3, $c4, $c5, $c6, $c7
                .byte $c8, $c9, $ca, $cb, $cc, $cd, $ce, $cf
                .byte $d0, $d1, $d2, $d3, $d4, $d5, $d6, $d7
                .byte $d8, $d9, $da, $db, $dc, $dd, $de, $df
                .byte $e0, $e1, $e2, $e3, $e4, $e5, $e6, $e7
                .byte $e8, $e9, $ea, $eb, $ec, $ed, $ee, $ef
                .byte $f0, $f1, $f2, $f3, $f4, $f5, $f6, $f7
                .byte $f8, $f9, $fa, $fb, $fc, $fd, $fe, $ff
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_8bpp_l0_8x16_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_8bpp_l0_8x16_shifted.png");
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
                    sta L1_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 8bpp test tile
                    ldx #0
                .load_loop:
                    lda tile_data, x
                    sta DATA0
                    inx
                    bne load_loop

                    ldx #0
                .load_loop_2:
                    lda tile_data_2, x
                    sta DATA0
                    inx
                    bne load_loop_2

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

                    lda #$73
                    sta L1_CONFIG ; 128x64 tiles, 8bpp

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
; 8x16
               ;1
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08

                .byte $00, $01, $02, $03, $04, $05, $06, $07
                .byte $08, $09, $0a, $0b, $0c, $0d, $0e, $0f
                .byte $10, $11, $12, $13, $14, $15, $16, $17
                .byte $18, $19, $1a, $1b, $1c, $1d, $1e, $1f
                .byte $20, $21, $22, $23, $24, $25, $26, $27
                .byte $28, $29, $2a, $2b, $2c, $2d, $2e, $2f
                .byte $30, $31, $32, $33, $34, $35, $36, $37
                .byte $38, $39, $3a, $3b, $3c, $3d, $3e, $3f
                ;2
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08

                .byte $40, $41, $42, $43, $44, $45, $46, $47
                .byte $48, $49, $4a, $4b, $4c, $4d, $4e, $4f
                .byte $50, $51, $52, $53, $54, $55, $56, $57
                .byte $58, $59, $5a, $5b, $5c, $5d, $5e, $5f
                .byte $60, $61, $62, $63, $64, $65, $66, $67
                .byte $68, $69, $6a, $6b, $6c, $6d, $6e, $6f
                .byte $70, $71, $72, $73, $74, $75, $76, $77
                .byte $78, $79, $7a, $7b, $7c, $7d, $7e, $7f
.tile_data_2:
                ;3
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08

                .byte $80, $81, $82, $83, $84, $85, $86, $87
                .byte $88, $89, $8a, $8b, $8c, $8d, $8e, $8f
                .byte $90, $91, $92, $93, $94, $95, $96, $97
                .byte $98, $99, $9a, $9b, $9c, $9d, $9e, $9f
                .byte $a0, $a1, $a2, $a3, $a4, $a5, $a6, $a7
                .byte $a8, $a9, $aa, $ab, $ac, $ad, $ae, $af
                .byte $b0, $b1, $b2, $b3, $b4, $b5, $b6, $b7
                .byte $b8, $b9, $ba, $bb, $bc, $bd, $be, $bf
                ;4
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08

                .byte $c0, $c1, $c2, $c3, $c4, $c5, $c6, $c7
                .byte $c8, $c9, $ca, $cb, $cc, $cd, $ce, $cf
                .byte $d0, $d1, $d2, $d3, $d4, $d5, $d6, $d7
                .byte $d8, $d9, $da, $db, $dc, $dd, $de, $df
                .byte $e0, $e1, $e2, $e3, $e4, $e5, $e6, $e7
                .byte $e8, $e9, $ea, $eb, $ec, $ed, $ee, $ef
                .byte $f0, $f1, $f2, $f3, $f4, $f5, $f6, $f7
                .byte $f8, $f9, $fa, $fb, $fc, $fd, $fe, $ff
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_8bpp_l1_8x16_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_8bpp_l1_8x16_normal.png");
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
                    sta L1_TILEBASE ; 8x16, tiles are at $00000

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
                    bne load_loop

                    ldx #0
                .load_loop_2:
                    lda tile_data_2, x
                    sta DATA0
                    inx
                    bne load_loop_2

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

                    lda #$73
                    sta L1_CONFIG ; 128x64 tiles, 8bpp

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
; 8x16
               ;1
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08

                .byte $00, $01, $02, $03, $04, $05, $06, $07
                .byte $08, $09, $0a, $0b, $0c, $0d, $0e, $0f
                .byte $10, $11, $12, $13, $14, $15, $16, $17
                .byte $18, $19, $1a, $1b, $1c, $1d, $1e, $1f
                .byte $20, $21, $22, $23, $24, $25, $26, $27
                .byte $28, $29, $2a, $2b, $2c, $2d, $2e, $2f
                .byte $30, $31, $32, $33, $34, $35, $36, $37
                .byte $38, $39, $3a, $3b, $3c, $3d, $3e, $3f
                ;2
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08

                .byte $40, $41, $42, $43, $44, $45, $46, $47
                .byte $48, $49, $4a, $4b, $4c, $4d, $4e, $4f
                .byte $50, $51, $52, $53, $54, $55, $56, $57
                .byte $58, $59, $5a, $5b, $5c, $5d, $5e, $5f
                .byte $60, $61, $62, $63, $64, $65, $66, $67
                .byte $68, $69, $6a, $6b, $6c, $6d, $6e, $6f
                .byte $70, $71, $72, $73, $74, $75, $76, $77
                .byte $78, $79, $7a, $7b, $7c, $7d, $7e, $7f
.tile_data_2:
                ;3
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08

                .byte $80, $81, $82, $83, $84, $85, $86, $87
                .byte $88, $89, $8a, $8b, $8c, $8d, $8e, $8f
                .byte $90, $91, $92, $93, $94, $95, $96, $97
                .byte $98, $99, $9a, $9b, $9c, $9d, $9e, $9f
                .byte $a0, $a1, $a2, $a3, $a4, $a5, $a6, $a7
                .byte $a8, $a9, $aa, $ab, $ac, $ad, $ae, $af
                .byte $b0, $b1, $b2, $b3, $b4, $b5, $b6, $b7
                .byte $b8, $b9, $ba, $bb, $bc, $bd, $be, $bf
                ;4
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $01, $01, $02, $02, $03, $03, $04, $04
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08

                .byte $c0, $c1, $c2, $c3, $c4, $c5, $c6, $c7
                .byte $c8, $c9, $ca, $cb, $cc, $cd, $ce, $cf
                .byte $d0, $d1, $d2, $d3, $d4, $d5, $d6, $d7
                .byte $d8, $d9, $da, $db, $dc, $dd, $de, $df
                .byte $e0, $e1, $e2, $e3, $e4, $e5, $e6, $e7
                .byte $e8, $e9, $ea, $eb, $ec, $ed, $ee, $ef
                .byte $f0, $f1, $f2, $f3, $f4, $f5, $f6, $f7
                .byte $f8, $f9, $fa, $fb, $fc, $fd, $fe, $ff
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_8bpp_l1_8x16_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_8bpp_l1_8x16_shifted.png");
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
                    sta L0_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 8bpp test tile
                    ldx #0
                .load_loop:
                    lda tile_data, x
                    sta DATA0
                    inx
                    bne load_loop

                    ldx #0
                .load_loop_2:
                    lda tile_data_2, x
                    sta DATA0
                    inx
                    bne load_loop_2

                    ldx #0
                .load_loop_3:
                    lda tile_data_3, x
                    sta DATA0
                    inx
                    bne load_loop_3

                    ldx #0
                .load_loop_4:
                    lda tile_data_4, x
                    sta DATA0
                    inx
                    bne load_loop_4

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

                    lda #$73
                    sta L0_CONFIG ; 128x64 tiles, 8bpp

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
                ;1
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $00, $01, $02, $03, $04, $05, $06, $07, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $10, $11, $12, $13, $14, $15, $16, $17, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $18, $19, $1a, $1b, $1c, $1d, $1e, $1f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $20, $21, $22, $23, $24, $25, $26, $27, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $28, $29, $2a, $2b, $2c, $2d, $2e, $2f, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $30, $31, $32, $33, $34, $35, $36, $37, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $38, $39, $3a, $3b, $3c, $3d, $3e, $3f, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_2:
                ;2
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $40, $41, $42, $43, $44, $45, $46, $47, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $48, $49, $4a, $4b, $4c, $4d, $4e, $4f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $50, $51, $52, $53, $54, $55, $56, $57, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $58, $59, $5a, $5b, $5c, $5d, $5e, $5f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $60, $61, $62, $63, $64, $65, $66, $67, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $68, $69, $6a, $6b, $6c, $6d, $6e, $6f, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $70, $71, $72, $73, $74, $75, $76, $77, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $78, $79, $7a, $7b, $7c, $7d, $7e, $7f, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_3:
                ;3
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $80, $81, $82, $83, $84, $85, $86, $87, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $88, $89, $8a, $8b, $8c, $8d, $8e, $8f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $90, $91, $92, $93, $94, $95, $96, $97, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $98, $99, $9a, $9b, $9c, $9d, $9e, $9f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $a0, $a1, $a2, $a3, $a4, $a5, $a6, $a7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $a8, $a9, $aa, $ab, $ac, $ad, $ae, $af, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $b0, $b1, $b2, $b3, $b4, $b5, $b6, $b7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $b8, $b9, $ba, $bb, $bc, $bd, $be, $bf, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_4:
                ;4
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $c0, $c1, $c2, $c3, $c4, $c5, $c6, $c7, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $c8, $c9, $ca, $cb, $cc, $cd, $ce, $cf, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $d0, $d1, $d2, $d3, $d4, $d5, $d6, $d7, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $d8, $d9, $da, $db, $dc, $dd, $de, $df, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $e0, $e1, $e2, $e3, $e4, $e5, $e6, $e7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $e8, $e9, $ea, $eb, $ec, $ed, $ee, $ef, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $f0, $f1, $f2, $f3, $f4, $f5, $f6, $f7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $f8, $f9, $fa, $fb, $fc, $fd, $fe, $ff, $00, $00, $00, $00, $00, $00, $00, $00

                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_8bpp_l0_16x16_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_8bpp_l0_16x16_normal.png");
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
    
                    ; write a 4bpp test tile
                    ldx #0
                .load_loop:
                    lda tile_data, x
                    sta DATA0
                    inx
                    bne load_loop

                    ldx #0
                .load_loop_2:
                    lda tile_data_2, x
                    sta DATA0
                    inx
                    bne load_loop_2

                    ldx #0
                .load_loop_3:
                    lda tile_data_3, x
                    sta DATA0
                    inx
                    bne load_loop_3

                    ldx #0
                .load_loop_4:
                    lda tile_data_4, x
                    sta DATA0
                    inx
                    bne load_loop_4

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

                    lda #$73
                    sta L0_CONFIG ; 128x64 tiles, 8bpp

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
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $00, $01, $02, $03, $04, $05, $06, $07, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $10, $11, $12, $13, $14, $15, $16, $17, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $18, $19, $1a, $1b, $1c, $1d, $1e, $1f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $20, $21, $22, $23, $24, $25, $26, $27, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $28, $29, $2a, $2b, $2c, $2d, $2e, $2f, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $30, $31, $32, $33, $34, $35, $36, $37, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $38, $39, $3a, $3b, $3c, $3d, $3e, $3f, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_2:
                ;2
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $40, $41, $42, $43, $44, $45, $46, $47, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $48, $49, $4a, $4b, $4c, $4d, $4e, $4f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $50, $51, $52, $53, $54, $55, $56, $57, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $58, $59, $5a, $5b, $5c, $5d, $5e, $5f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $60, $61, $62, $63, $64, $65, $66, $67, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $68, $69, $6a, $6b, $6c, $6d, $6e, $6f, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $70, $71, $72, $73, $74, $75, $76, $77, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $78, $79, $7a, $7b, $7c, $7d, $7e, $7f, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_3:
                ;3
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $80, $81, $82, $83, $84, $85, $86, $87, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $88, $89, $8a, $8b, $8c, $8d, $8e, $8f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $90, $91, $92, $93, $94, $95, $96, $97, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $98, $99, $9a, $9b, $9c, $9d, $9e, $9f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $a0, $a1, $a2, $a3, $a4, $a5, $a6, $a7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $a8, $a9, $aa, $ab, $ac, $ad, $ae, $af, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $b0, $b1, $b2, $b3, $b4, $b5, $b6, $b7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $b8, $b9, $ba, $bb, $bc, $bd, $be, $bf, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_4:
                ;4
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $c0, $c1, $c2, $c3, $c4, $c5, $c6, $c7, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $c8, $c9, $ca, $cb, $cc, $cd, $ce, $cf, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $d0, $d1, $d2, $d3, $d4, $d5, $d6, $d7, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $d8, $d9, $da, $db, $dc, $dd, $de, $df, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $e0, $e1, $e2, $e3, $e4, $e5, $e6, $e7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $e8, $e9, $ea, $eb, $ec, $ed, $ee, $ef, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $f0, $f1, $f2, $f3, $f4, $f5, $f6, $f7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $f8, $f9, $fa, $fb, $fc, $fd, $fe, $ff, $00, $00, $00, $00, $00, $00, $00, $00
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_8bpp_l0_16x16_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_8bpp_l0_16x16_shifted.png");
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
                    sta L1_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 8bpp test tile
                    ldx #0
                .load_loop:
                    lda tile_data, x
                    sta DATA0
                    inx
                    bne load_loop

                    ldx #0
                .load_loop_2:
                    lda tile_data_2, x
                    sta DATA0
                    inx
                    bne load_loop_2

                    ldx #0
                .load_loop_3:
                    lda tile_data_3, x
                    sta DATA0
                    inx
                    bne load_loop_3

                    ldx #0
                .load_loop_4:
                    lda tile_data_4, x
                    sta DATA0
                    inx
                    bne load_loop_4

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

                    lda #$73
                    sta L1_CONFIG ; 128x64 tiles, 8bpp

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
                ;1
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $00, $01, $02, $03, $04, $05, $06, $07, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $10, $11, $12, $13, $14, $15, $16, $17, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $18, $19, $1a, $1b, $1c, $1d, $1e, $1f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $20, $21, $22, $23, $24, $25, $26, $27, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $28, $29, $2a, $2b, $2c, $2d, $2e, $2f, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $30, $31, $32, $33, $34, $35, $36, $37, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $38, $39, $3a, $3b, $3c, $3d, $3e, $3f, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_2:
                ;2
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $40, $41, $42, $43, $44, $45, $46, $47, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $48, $49, $4a, $4b, $4c, $4d, $4e, $4f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $50, $51, $52, $53, $54, $55, $56, $57, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $58, $59, $5a, $5b, $5c, $5d, $5e, $5f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $60, $61, $62, $63, $64, $65, $66, $67, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $68, $69, $6a, $6b, $6c, $6d, $6e, $6f, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $70, $71, $72, $73, $74, $75, $76, $77, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $78, $79, $7a, $7b, $7c, $7d, $7e, $7f, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_3:
                ;3
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $80, $81, $82, $83, $84, $85, $86, $87, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $88, $89, $8a, $8b, $8c, $8d, $8e, $8f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $90, $91, $92, $93, $94, $95, $96, $97, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $98, $99, $9a, $9b, $9c, $9d, $9e, $9f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $a0, $a1, $a2, $a3, $a4, $a5, $a6, $a7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $a8, $a9, $aa, $ab, $ac, $ad, $ae, $af, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $b0, $b1, $b2, $b3, $b4, $b5, $b6, $b7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $b8, $b9, $ba, $bb, $bc, $bd, $be, $bf, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_4:
                ;4
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $c0, $c1, $c2, $c3, $c4, $c5, $c6, $c7, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $c8, $c9, $ca, $cb, $cc, $cd, $ce, $cf, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $d0, $d1, $d2, $d3, $d4, $d5, $d6, $d7, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $d8, $d9, $da, $db, $dc, $dd, $de, $df, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $e0, $e1, $e2, $e3, $e4, $e5, $e6, $e7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $e8, $e9, $ea, $eb, $ec, $ed, $ee, $ef, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $f0, $f1, $f2, $f3, $f4, $f5, $f6, $f7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $f8, $f9, $fa, $fb, $fc, $fd, $fe, $ff, $00, $00, $00, $00, $00, $00, $00, $00

                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_8bpp_l1_16x16_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_8bpp_l1_16x16_normal.png");
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
                    sta L1_TILEBASE ; 8x16, tiles are at $00000

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
                    bne load_loop

                    ldx #0
                .load_loop_2:
                    lda tile_data_2, x
                    sta DATA0
                    inx
                    bne load_loop_2

                    ldx #0
                .load_loop_3:
                    lda tile_data_3, x
                    sta DATA0
                    inx
                    bne load_loop_3

                    ldx #0
                .load_loop_4:
                    lda tile_data_4, x
                    sta DATA0
                    inx
                    bne load_loop_4

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

                    lda #$73
                    sta L1_CONFIG ; 128x64 tiles, 8bpp

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
               ;1
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $00, $01, $02, $03, $04, $05, $06, $07, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $10, $11, $12, $13, $14, $15, $16, $17, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $18, $19, $1a, $1b, $1c, $1d, $1e, $1f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $20, $21, $22, $23, $24, $25, $26, $27, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $28, $29, $2a, $2b, $2c, $2d, $2e, $2f, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $30, $31, $32, $33, $34, $35, $36, $37, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $38, $39, $3a, $3b, $3c, $3d, $3e, $3f, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_2:
                ;2
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $40, $41, $42, $43, $44, $45, $46, $47, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $48, $49, $4a, $4b, $4c, $4d, $4e, $4f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $50, $51, $52, $53, $54, $55, $56, $57, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $58, $59, $5a, $5b, $5c, $5d, $5e, $5f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $60, $61, $62, $63, $64, $65, $66, $67, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $68, $69, $6a, $6b, $6c, $6d, $6e, $6f, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $70, $71, $72, $73, $74, $75, $76, $77, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $78, $79, $7a, $7b, $7c, $7d, $7e, $7f, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_3:
                ;3
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $80, $81, $82, $83, $84, $85, $86, $87, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $88, $89, $8a, $8b, $8c, $8d, $8e, $8f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $90, $91, $92, $93, $94, $95, $96, $97, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $98, $99, $9a, $9b, $9c, $9d, $9e, $9f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $a0, $a1, $a2, $a3, $a4, $a5, $a6, $a7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $a8, $a9, $aa, $ab, $ac, $ad, $ae, $af, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $b0, $b1, $b2, $b3, $b4, $b5, $b6, $b7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $b8, $b9, $ba, $bb, $bc, $bd, $be, $bf, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_4:
                ;4
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $c0, $c1, $c2, $c3, $c4, $c5, $c6, $c7, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $c8, $c9, $ca, $cb, $cc, $cd, $ce, $cf, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $d0, $d1, $d2, $d3, $d4, $d5, $d6, $d7, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $d8, $d9, $da, $db, $dc, $dd, $de, $df, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $e0, $e1, $e2, $e3, $e4, $e5, $e6, $e7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $e8, $e9, $ea, $eb, $ec, $ed, $ee, $ef, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $f0, $f1, $f2, $f3, $f4, $f5, $f6, $f7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $f8, $f9, $fa, $fb, $fc, $fd, $fe, $ff, $00, $00, $00, $00, $00, $00, $00, $00
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_8bpp_l1_16x16_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_8bpp_l1_16x16_shifted.png");
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
                    
                    lda #02
                    sta L0_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 8bpp test tile
                    ldx #0
                .load_loop:
                    lda tile_data, x
                    sta DATA0
                    inx
                    bne load_loop

                    ldx #0
                .load_loop_2:
                    lda tile_data_2, x
                    sta DATA0
                    inx
                    bne load_loop_2

                    ldx #0
                .load_loop_3:
                    lda tile_data_3, x
                    sta DATA0
                    inx
                    bne load_loop_3

                    ldx #0
                .load_loop_4:
                    lda tile_data_4, x
                    sta DATA0
                    inx
                    bne load_loop_4

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

                    lda #$73
                    sta L0_CONFIG ; 128x64 tiles, 8bpp

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
                ;1
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $00, $01, $02, $03, $04, $05, $06, $07, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $10, $11, $12, $13, $14, $15, $16, $17, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $18, $19, $1a, $1b, $1c, $1d, $1e, $1f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $20, $21, $22, $23, $24, $25, $26, $27, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $28, $29, $2a, $2b, $2c, $2d, $2e, $2f, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $30, $31, $32, $33, $34, $35, $36, $37, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $38, $39, $3a, $3b, $3c, $3d, $3e, $3f, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_2:
                ;2
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $40, $41, $42, $43, $44, $45, $46, $47, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $48, $49, $4a, $4b, $4c, $4d, $4e, $4f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $50, $51, $52, $53, $54, $55, $56, $57, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $58, $59, $5a, $5b, $5c, $5d, $5e, $5f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $60, $61, $62, $63, $64, $65, $66, $67, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $68, $69, $6a, $6b, $6c, $6d, $6e, $6f, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $70, $71, $72, $73, $74, $75, $76, $77, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $78, $79, $7a, $7b, $7c, $7d, $7e, $7f, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_3:
                ;3
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $80, $81, $82, $83, $84, $85, $86, $87, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $88, $89, $8a, $8b, $8c, $8d, $8e, $8f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $90, $91, $92, $93, $94, $95, $96, $97, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $98, $99, $9a, $9b, $9c, $9d, $9e, $9f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $a0, $a1, $a2, $a3, $a4, $a5, $a6, $a7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $a8, $a9, $aa, $ab, $ac, $ad, $ae, $af, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $b0, $b1, $b2, $b3, $b4, $b5, $b6, $b7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $b8, $b9, $ba, $bb, $bc, $bd, $be, $bf, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_4:
                ;4
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $c0, $c1, $c2, $c3, $c4, $c5, $c6, $c7, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $c8, $c9, $ca, $cb, $cc, $cd, $ce, $cf, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $d0, $d1, $d2, $d3, $d4, $d5, $d6, $d7, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $d8, $d9, $da, $db, $dc, $dd, $de, $df, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $e0, $e1, $e2, $e3, $e4, $e5, $e6, $e7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $e8, $e9, $ea, $eb, $ec, $ed, $ee, $ef, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $f0, $f1, $f2, $f3, $f4, $f5, $f6, $f7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $f8, $f9, $fa, $fb, $fc, $fd, $fe, $ff, $00, $00, $00, $00, $00, $00, $00, $00

                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_8bpp_l0_16x8_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_8bpp_l0_16x8_normal.png");
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
                    
                    lda #02
                    sta L0_TILEBASE ; 8x16, tiles are at $00000

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
                    bne load_loop

                    ldx #0
                .load_loop_2:
                    lda tile_data_2, x
                    sta DATA0
                    inx
                    bne load_loop_2

                    ldx #0
                .load_loop_3:
                    lda tile_data_3, x
                    sta DATA0
                    inx
                    bne load_loop_3

                    ldx #0
                .load_loop_4:
                    lda tile_data_4, x
                    sta DATA0
                    inx
                    bne load_loop_4

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

                    lda #$73
                    sta L0_CONFIG ; 128x64 tiles, 8bpp

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
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $00, $01, $02, $03, $04, $05, $06, $07, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $10, $11, $12, $13, $14, $15, $16, $17, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $18, $19, $1a, $1b, $1c, $1d, $1e, $1f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $20, $21, $22, $23, $24, $25, $26, $27, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $28, $29, $2a, $2b, $2c, $2d, $2e, $2f, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $30, $31, $32, $33, $34, $35, $36, $37, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $38, $39, $3a, $3b, $3c, $3d, $3e, $3f, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_2:
                ;2
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $40, $41, $42, $43, $44, $45, $46, $47, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $48, $49, $4a, $4b, $4c, $4d, $4e, $4f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $50, $51, $52, $53, $54, $55, $56, $57, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $58, $59, $5a, $5b, $5c, $5d, $5e, $5f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $60, $61, $62, $63, $64, $65, $66, $67, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $68, $69, $6a, $6b, $6c, $6d, $6e, $6f, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $70, $71, $72, $73, $74, $75, $76, $77, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $78, $79, $7a, $7b, $7c, $7d, $7e, $7f, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_3:
                ;3
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $80, $81, $82, $83, $84, $85, $86, $87, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $88, $89, $8a, $8b, $8c, $8d, $8e, $8f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $90, $91, $92, $93, $94, $95, $96, $97, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $98, $99, $9a, $9b, $9c, $9d, $9e, $9f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $a0, $a1, $a2, $a3, $a4, $a5, $a6, $a7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $a8, $a9, $aa, $ab, $ac, $ad, $ae, $af, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $b0, $b1, $b2, $b3, $b4, $b5, $b6, $b7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $b8, $b9, $ba, $bb, $bc, $bd, $be, $bf, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_4:
                ;4
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $c0, $c1, $c2, $c3, $c4, $c5, $c6, $c7, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $c8, $c9, $ca, $cb, $cc, $cd, $ce, $cf, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $d0, $d1, $d2, $d3, $d4, $d5, $d6, $d7, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $d8, $d9, $da, $db, $dc, $dd, $de, $df, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $e0, $e1, $e2, $e3, $e4, $e5, $e6, $e7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $e8, $e9, $ea, $eb, $ec, $ed, $ee, $ef, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $f0, $f1, $f2, $f3, $f4, $f5, $f6, $f7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $f8, $f9, $fa, $fb, $fc, $fd, $fe, $ff, $00, $00, $00, $00, $00, $00, $00, $00
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_8bpp_l0_16x8_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_8bpp_l0_16x8_shifted.png");
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
                    
                    lda #02
                    sta L1_TILEBASE ; 8x8, tiles are at $00000

                    ; Tile definition
                    lda #$10
                    sta ADDRx_H
                    lda #$00
                    sta ADDRx_M
                    lda #$00
                    sta ADDRx_L
    
                    ; write a 8bpp test tile
                    ldx #0
                .load_loop:
                    lda tile_data, x
                    sta DATA0
                    inx
                    bne load_loop

                    ldx #0
                .load_loop_2:
                    lda tile_data_2, x
                    sta DATA0
                    inx
                    bne load_loop_2

                    ldx #0
                .load_loop_3:
                    lda tile_data_3, x
                    sta DATA0
                    inx
                    bne load_loop_3

                    ldx #0
                .load_loop_4:
                    lda tile_data_4, x
                    sta DATA0
                    inx
                    bne load_loop_4

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

                    lda #$73
                    sta L1_CONFIG ; 128x64 tiles, 8bpp

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
                ;1
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $00, $01, $02, $03, $04, $05, $06, $07, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $10, $11, $12, $13, $14, $15, $16, $17, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $18, $19, $1a, $1b, $1c, $1d, $1e, $1f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $20, $21, $22, $23, $24, $25, $26, $27, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $28, $29, $2a, $2b, $2c, $2d, $2e, $2f, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $30, $31, $32, $33, $34, $35, $36, $37, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $38, $39, $3a, $3b, $3c, $3d, $3e, $3f, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_2:
                ;2
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $40, $41, $42, $43, $44, $45, $46, $47, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $48, $49, $4a, $4b, $4c, $4d, $4e, $4f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $50, $51, $52, $53, $54, $55, $56, $57, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $58, $59, $5a, $5b, $5c, $5d, $5e, $5f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $60, $61, $62, $63, $64, $65, $66, $67, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $68, $69, $6a, $6b, $6c, $6d, $6e, $6f, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $70, $71, $72, $73, $74, $75, $76, $77, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $78, $79, $7a, $7b, $7c, $7d, $7e, $7f, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_3:
                ;3
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $80, $81, $82, $83, $84, $85, $86, $87, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $88, $89, $8a, $8b, $8c, $8d, $8e, $8f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $90, $91, $92, $93, $94, $95, $96, $97, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $98, $99, $9a, $9b, $9c, $9d, $9e, $9f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $a0, $a1, $a2, $a3, $a4, $a5, $a6, $a7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $a8, $a9, $aa, $ab, $ac, $ad, $ae, $af, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $b0, $b1, $b2, $b3, $b4, $b5, $b6, $b7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $b8, $b9, $ba, $bb, $bc, $bd, $be, $bf, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_4:
                ;4
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $c0, $c1, $c2, $c3, $c4, $c5, $c6, $c7, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $c8, $c9, $ca, $cb, $cc, $cd, $ce, $cf, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $d0, $d1, $d2, $d3, $d4, $d5, $d6, $d7, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $d8, $d9, $da, $db, $dc, $dd, $de, $df, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $e0, $e1, $e2, $e3, $e4, $e5, $e6, $e7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $e8, $e9, $ea, $eb, $ec, $ed, $ee, $ef, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $f0, $f1, $f2, $f3, $f4, $f5, $f6, $f7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $f8, $f9, $fa, $fb, $fc, $fd, $fe, $ff, $00, $00, $00, $00, $00, $00, $00, $00

                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_8bpp_l1_16x8_normal.png");
        emulator.CompareImage(@"Vera\Images\tile_8bpp_l1_16x8_normal.png");
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
                    
                    lda #02
                    sta L1_TILEBASE ; 8x16, tiles are at $00000

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
                    bne load_loop

                    ldx #0
                .load_loop_2:
                    lda tile_data_2, x
                    sta DATA0
                    inx
                    bne load_loop_2

                    ldx #0
                .load_loop_3:
                    lda tile_data_3, x
                    sta DATA0
                    inx
                    bne load_loop_3

                    ldx #0
                .load_loop_4:
                    lda tile_data_4, x
                    sta DATA0
                    inx
                    bne load_loop_4

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

                    lda #$73
                    sta L1_CONFIG ; 128x64 tiles, 8bpp

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
               ;1
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $00, $01, $02, $03, $04, $05, $06, $07, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $10, $11, $12, $13, $14, $15, $16, $17, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $18, $19, $1a, $1b, $1c, $1d, $1e, $1f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $20, $21, $22, $23, $24, $25, $26, $27, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $28, $29, $2a, $2b, $2c, $2d, $2e, $2f, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $30, $31, $32, $33, $34, $35, $36, $37, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $38, $39, $3a, $3b, $3c, $3d, $3e, $3f, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_2:
                ;2
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $40, $41, $42, $43, $44, $45, $46, $47, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $48, $49, $4a, $4b, $4c, $4d, $4e, $4f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $50, $51, $52, $53, $54, $55, $56, $57, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $58, $59, $5a, $5b, $5c, $5d, $5e, $5f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $60, $61, $62, $63, $64, $65, $66, $67, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $68, $69, $6a, $6b, $6c, $6d, $6e, $6f, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $70, $71, $72, $73, $74, $75, $76, $77, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $78, $79, $7a, $7b, $7c, $7d, $7e, $7f, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_3:
                ;3
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $80, $81, $82, $83, $84, $85, $86, $87, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $88, $89, $8a, $8b, $8c, $8d, $8e, $8f, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $90, $91, $92, $93, $94, $95, $96, $97, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $98, $99, $9a, $9b, $9c, $9d, $9e, $9f, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $a0, $a1, $a2, $a3, $a4, $a5, $a6, $a7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $a8, $a9, $aa, $ab, $ac, $ad, $ae, $af, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $b0, $b1, $b2, $b3, $b4, $b5, $b6, $b7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $b8, $b9, $ba, $bb, $bc, $bd, $be, $bf, $00, $00, $00, $00, $00, $00, $00, $00
.tile_data_4:
                ;4
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $01, $01, $02, $02, $03, $03, $04, $04, $05, $05, $06, $06, $07, $07, $08, $08
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00
                .byte $05, $05, $06, $06, $07, $07, $08, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $00

                .byte $c0, $c1, $c2, $c3, $c4, $c5, $c6, $c7, $00, $01, $00, $02, $00, $03, $00, $04
                .byte $c8, $c9, $ca, $cb, $cc, $cd, $ce, $cf, $05, $00, $06, $00, $07, $00, $08, $00
                .byte $d0, $d1, $d2, $d3, $d4, $d5, $d6, $d7, $00, $09, $00, $0a, $00, $0b, $00, $0c
                .byte $d8, $d9, $da, $db, $dc, $dd, $de, $df, $0d, $00, $0e, $00, $0f, $00, $0f, $00
                .byte $e0, $e1, $e2, $e3, $e4, $e5, $e6, $e7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $e8, $e9, $ea, $eb, $ec, $ed, $ee, $ef, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $f0, $f1, $f2, $f3, $f4, $f5, $f6, $f7, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $f8, $f9, $fa, $fb, $fc, $fd, $fe, $ff, $00, $00, $00, $00, $00, $00, $00, $00
                ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\tile_8bpp_l1_16x8_shifted.png");
        emulator.CompareImage(@"Vera\Images\tile_8bpp_l1_16x8_shifted.png");
    }
}