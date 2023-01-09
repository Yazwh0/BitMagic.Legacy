using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera.Display;

[TestClass]
public class Sprites_8bbp_16x8
{
    [TestMethod]
    public async Task Normal()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R41
            .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
            .org $810
                sei

                lda #02
                sta DC_BORDER

                lda #$40        ; 1bpp bitmap
                sta L0_CONFIG
                sta L1_CONFIG
    
                ; set colour 0
                lda #$01
                sta ADDRx_H
                lda #$fa
                sta ADDRx_M
                stz ADDRx_L

                lda #$12
                sta DATA0

                jsr create_sprites
                jsr map_sprites

                lda #$41        ; Sprites
                sta DC_VIDEO       

                lda #02
                sta CTRL

                lda #0
                sta DC_HSTART
                lda #0
                sta DC_VSTART

                stz CTRL

                lda #128
                sta DC_HSCALE
                sta DC_VSCALE

                lda #01
                sta IEN
                wai
                sta ISR     ; clear interrupt and wait for second frame
                wai

                stp

          .proc create_sprites    
    
                lda #$10
                sta ADDRx_H
                stz ADDRx_M
                stz ADDRx_L

                ldy #0
                ldx #0

                .sprite_loop:

                lda template, x

                cmp #$ff
                bne no_change

                tya

                .no_change:
                sta DATA0

                inx

                cpx #8*16
                bne sprite_loop

                ldx #0

                iny
                cpy #128
                bne sprite_loop

                rts

            .align $100
            .template:
                .byte $01, $01, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00
                .byte $01, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01
                .byte $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $01, $02, $03, $04, $05, $06, $07, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $0f
                .byte $01, $02, $03, $04, $05, $06, $07, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $0f
                .byte $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $01, $01
                .byte $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $01, $01

            .endproc

            .proc map_sprites

                .const address_l = $02
                .const address_h = $03

                lda #$11
                sta ADDRx_H
                lda #$fc
                sta ADDRx_M
                stz ADDRx_L

                ldy #$0

                stz address_l
                stz address_h

                .sprite_loop:

                lda address_l
                sta DATA0       ; address

                lda address_h
                ora #$80

                sta DATA0       ; 8bpp

                clc
                lda address_l
                adc #(8*16) >> 5
                sta address_l
                lda address_h
                adc #0
                sta address_h


                ldx #04
                .step:
                lda positions   ; gets modified
                sta DATA0
    
                clc
                lda step+1
                adc #1
                sta step+1
                lda step+2
                adc #0
                sta step+2

                dex
                bne step 

                lda #$04
                sta DATA0   ; depth

                lda #$10    ; 16x8px
                sta DATA0   ; height, width, offset

                iny
                cpy #128
                bne sprite_loop

                rts

            .positions:
                .byte $00, $00, $00, $00
                .byte $20, $00, $00, $00
                .byte $40, $00, $00, $00
                .byte $60, $00, $00, $00
                .byte $80, $00, $00, $00
                .byte $a0, $00, $00, $00
                .byte $c0, $00, $00, $00
                .byte $e0, $00, $00, $00

                .byte $00, $01, $00, $00
                .byte $20, $01, $00, $00
                .byte $40, $01, $00, $00
                .byte $60, $01, $00, $00
                .byte $80, $01, $00, $00
                .byte $a0, $01, $00, $00
                .byte $c0, $01, $00, $00
                .byte $e0, $01, $00, $00
    
                .byte $00, $02, $00, $00
                .byte $20, $02, $00, $00
                .byte $40, $02, $00, $00 
                .byte $60, $02, $00, $00     ; 20

                .byte $10, $00, $08, $00
                .byte $30, $00, $08, $00
                .byte $50, $00, $08, $00
                .byte $70, $00, $08, $00
                .byte $90, $00, $08, $00
                .byte $b0, $00, $08, $00
                .byte $d0, $00, $08, $00
                .byte $f0, $00, $08, $00

                .byte $10, $01, $08, $00
                .byte $30, $01, $08, $00
                .byte $50, $01, $08, $00
                .byte $70, $01, $08, $00
                .byte $90, $01, $08, $00
                .byte $b0, $01, $08, $00
                .byte $d0, $01, $08, $00
                .byte $f0, $01, $08, $00
   
                .byte $10, $02, $08, $00
                .byte $30, $02, $08, $00
                .byte $50, $02, $08, $00 
                .byte $70, $02, $08, $00     ; 40


                .byte $00, $00, $10, $00
                .byte $20, $00, $10, $00
                .byte $40, $00, $10, $00
                .byte $60, $00, $10, $00
                .byte $80, $00, $10, $00
                .byte $a0, $00, $10, $00
                .byte $c0, $00, $10, $00
                .byte $e0, $00, $10, $00

                .byte $00, $01, $10, $00
                .byte $20, $01, $10, $00
                .byte $40, $01, $10, $00
                .byte $60, $01, $10, $00
                .byte $80, $01, $10, $00
                .byte $a0, $01, $10, $00
                .byte $c0, $01, $10, $00
                .byte $e0, $01, $10, $00
    
                .byte $00, $02, $10, $00
                .byte $20, $02, $10, $00
                .byte $40, $02, $10, $00 
                .byte $60, $02, $10, $00     ; 60

                .byte $10, $00, $18, $00
                .byte $30, $00, $18, $00
                .byte $50, $00, $18, $00
                .byte $70, $00, $18, $00
                .byte $90, $00, $18, $00
                .byte $b0, $00, $18, $00
                .byte $d0, $00, $18, $00
                .byte $f0, $00, $18, $00

                .byte $10, $01, $18, $00
                .byte $30, $01, $18, $00
                .byte $50, $01, $18, $00
                .byte $70, $01, $18, $00
                .byte $90, $01, $18, $00
                .byte $b0, $01, $18, $00
                .byte $d0, $01, $18, $00
                .byte $f0, $01, $18, $00
   
                .byte $10, $02, $18, $00
                .byte $30, $02, $18, $00
                .byte $50, $02, $18, $00 
                .byte $70, $02, $18, $00     ; 80


                .byte $00, $00, $d0, $01
                .byte $20, $00, $d0, $01
                .byte $40, $00, $d0, $01
                .byte $60, $00, $d0, $01
                .byte $80, $00, $d0, $01
                .byte $a0, $00, $d0, $01
                .byte $c0, $00, $d0, $01
                .byte $e0, $00, $d0, $01

                .byte $00, $01, $d0, $01
                .byte $20, $01, $d0, $01
                .byte $40, $01, $d0, $01
                .byte $60, $01, $d0, $01
                .byte $80, $01, $d0, $01
                .byte $a0, $01, $d0, $01
                .byte $c0, $01, $d0, $01
                .byte $e0, $01, $d0, $01
    
                .byte $00, $02, $d0, $01
                .byte $20, $02, $d0, $01
                .byte $40, $02, $d0, $01 
                .byte $60, $02, $d0, $01     ; 100

                .byte $10, $00, $d8, $01
                .byte $30, $00, $d8, $01
                .byte $50, $00, $d8, $01
                .byte $70, $00, $d8, $01
                .byte $90, $00, $d8, $01
                .byte $b0, $00, $d8, $01
                .byte $d0, $00, $d8, $01
                .byte $f0, $00, $d8, $01

                .byte $10, $01, $d8, $01
                .byte $30, $01, $d8, $01
                .byte $50, $01, $d8, $01
                .byte $70, $01, $d8, $01
                .byte $90, $01, $d8, $01
                .byte $b0, $01, $d8, $01
                .byte $d0, $01, $d8, $01
                .byte $f0, $01, $d8, $01
   
                .byte $10, $01, $10, $01     ; not on the bottom line because of vram access
                .byte $30, $01, $10, $01     ; not on the bottom line because of vram access
                .byte $50, $02, $d8, $01 
                .byte $70, $02, $d8, $01     ; 120




                .byte $00, $00, $00, $01
                .byte $08, $00, $00, $01
                .byte $10, $00, $00, $01
                .byte $18, $00, $00, $01
                .byte $20, $00, $00, $01
                .byte $28, $00, $00, $01
                .byte $30, $00, $00, $01
                .byte $38, $00, $00, $01 ; 128

            .endproc",
        emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\sprites_8bpp_16x8.png");

        emulator.CompareImage(@"Vera\Images\sprites_8bpp_16x8.png");
    }

    [TestMethod]
    public async Task Normal_HFlip()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R41
            .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
            .org $810
                sei

                lda #02
                sta DC_BORDER

                lda #$40        ; 1bpp bitmap
                sta L0_CONFIG
                sta L1_CONFIG
    
                ; set colour 0
                lda #$01
                sta ADDRx_H
                lda #$fa
                sta ADDRx_M
                stz ADDRx_L

                lda #$12
                sta DATA0

                jsr create_sprites
                jsr map_sprites

                lda #$41        ; Sprites
                sta DC_VIDEO       

                lda #02
                sta CTRL

                lda #0
                sta DC_HSTART
                lda #0
                sta DC_VSTART

                stz CTRL

                lda #128
                sta DC_HSCALE
                sta DC_VSCALE

                lda #01
                sta IEN
                wai
                sta ISR     ; clear interrupt and wait for second frame
                wai

                stp

          .proc create_sprites    
    
                lda #$10
                sta ADDRx_H
                stz ADDRx_M
                stz ADDRx_L

                ldy #0
                ldx #0

                .sprite_loop:

                lda template, x

                cmp #$ff
                bne no_change

                tya

                .no_change:
                sta DATA0

                inx

                cpx #8*16
                bne sprite_loop

                ldx #0

                iny
                cpy #128
                bne sprite_loop

                rts

            .align $100
            .template:
                .byte $01, $01, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00
                .byte $01, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01
                .byte $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $01, $02, $03, $04, $05, $06, $07, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $0f
                .byte $01, $02, $03, $04, $05, $06, $07, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $0f
                .byte $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $01, $01
                .byte $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $01, $01

            .endproc

            .proc map_sprites

                .const address_l = $02
                .const address_h = $03

                lda #$11
                sta ADDRx_H
                lda #$fc
                sta ADDRx_M
                stz ADDRx_L

                ldy #$0

                stz address_l
                stz address_h

                .sprite_loop:

                lda address_l
                sta DATA0       ; address

                lda address_h
                ora #$80

                sta DATA0       ; 8bpp

                clc
                lda address_l
                adc #(8*16) >> 5
                sta address_l
                lda address_h
                adc #0
                sta address_h


                ldx #04
                .step:
                lda positions   ; gets modified
                sta DATA0
    
                clc
                lda step+1
                adc #1
                sta step+1
                lda step+2
                adc #0
                sta step+2

                dex
                bne step 

                lda #$04+1  ; hflip
                sta DATA0   ; depth

                lda #$10    ; 16x8px
                sta DATA0   ; height, width, offset

                iny
                cpy #128
                bne sprite_loop

                rts

            .positions:
                .byte $00, $00, $00, $00
                .byte $20, $00, $00, $00
                .byte $40, $00, $00, $00
                .byte $60, $00, $00, $00
                .byte $80, $00, $00, $00
                .byte $a0, $00, $00, $00
                .byte $c0, $00, $00, $00
                .byte $e0, $00, $00, $00

                .byte $00, $01, $00, $00
                .byte $20, $01, $00, $00
                .byte $40, $01, $00, $00
                .byte $60, $01, $00, $00
                .byte $80, $01, $00, $00
                .byte $a0, $01, $00, $00
                .byte $c0, $01, $00, $00
                .byte $e0, $01, $00, $00
    
                .byte $00, $02, $00, $00
                .byte $20, $02, $00, $00
                .byte $40, $02, $00, $00 
                .byte $60, $02, $00, $00     ; 20

                .byte $10, $00, $08, $00
                .byte $30, $00, $08, $00
                .byte $50, $00, $08, $00
                .byte $70, $00, $08, $00
                .byte $90, $00, $08, $00
                .byte $b0, $00, $08, $00
                .byte $d0, $00, $08, $00
                .byte $f0, $00, $08, $00

                .byte $10, $01, $08, $00
                .byte $30, $01, $08, $00
                .byte $50, $01, $08, $00
                .byte $70, $01, $08, $00
                .byte $90, $01, $08, $00
                .byte $b0, $01, $08, $00
                .byte $d0, $01, $08, $00
                .byte $f0, $01, $08, $00
   
                .byte $10, $02, $08, $00
                .byte $30, $02, $08, $00
                .byte $50, $02, $08, $00 
                .byte $70, $02, $08, $00     ; 40


                .byte $00, $00, $10, $00
                .byte $20, $00, $10, $00
                .byte $40, $00, $10, $00
                .byte $60, $00, $10, $00
                .byte $80, $00, $10, $00
                .byte $a0, $00, $10, $00
                .byte $c0, $00, $10, $00
                .byte $e0, $00, $10, $00

                .byte $00, $01, $10, $00
                .byte $20, $01, $10, $00
                .byte $40, $01, $10, $00
                .byte $60, $01, $10, $00
                .byte $80, $01, $10, $00
                .byte $a0, $01, $10, $00
                .byte $c0, $01, $10, $00
                .byte $e0, $01, $10, $00
    
                .byte $00, $02, $10, $00
                .byte $20, $02, $10, $00
                .byte $40, $02, $10, $00 
                .byte $60, $02, $10, $00     ; 60

                .byte $10, $00, $18, $00
                .byte $30, $00, $18, $00
                .byte $50, $00, $18, $00
                .byte $70, $00, $18, $00
                .byte $90, $00, $18, $00
                .byte $b0, $00, $18, $00
                .byte $d0, $00, $18, $00
                .byte $f0, $00, $18, $00

                .byte $10, $01, $18, $00
                .byte $30, $01, $18, $00
                .byte $50, $01, $18, $00
                .byte $70, $01, $18, $00
                .byte $90, $01, $18, $00
                .byte $b0, $01, $18, $00
                .byte $d0, $01, $18, $00
                .byte $f0, $01, $18, $00
   
                .byte $10, $02, $18, $00
                .byte $30, $02, $18, $00
                .byte $50, $02, $18, $00 
                .byte $70, $02, $18, $00     ; 80


                .byte $00, $00, $d0, $01
                .byte $20, $00, $d0, $01
                .byte $40, $00, $d0, $01
                .byte $60, $00, $d0, $01
                .byte $80, $00, $d0, $01
                .byte $a0, $00, $d0, $01
                .byte $c0, $00, $d0, $01
                .byte $e0, $00, $d0, $01

                .byte $00, $01, $d0, $01
                .byte $20, $01, $d0, $01
                .byte $40, $01, $d0, $01
                .byte $60, $01, $d0, $01
                .byte $80, $01, $d0, $01
                .byte $a0, $01, $d0, $01
                .byte $c0, $01, $d0, $01
                .byte $e0, $01, $d0, $01
    
                .byte $00, $02, $d0, $01
                .byte $20, $02, $d0, $01
                .byte $40, $02, $d0, $01 
                .byte $60, $02, $d0, $01     ; 100

                .byte $10, $00, $d8, $01
                .byte $30, $00, $d8, $01
                .byte $50, $00, $d8, $01
                .byte $70, $00, $d8, $01
                .byte $90, $00, $d8, $01
                .byte $b0, $00, $d8, $01
                .byte $d0, $00, $d8, $01
                .byte $f0, $00, $d8, $01

                .byte $10, $01, $d8, $01
                .byte $30, $01, $d8, $01
                .byte $50, $01, $d8, $01
                .byte $70, $01, $d8, $01
                .byte $90, $01, $d8, $01
                .byte $b0, $01, $d8, $01
                .byte $d0, $01, $d8, $01
                .byte $f0, $01, $d8, $01
   
                .byte $10, $01, $10, $01     ; not on the bottom line because of vram access
                .byte $30, $01, $10, $01     ; not on the bottom line because of vram access
                .byte $50, $02, $d8, $01 
                .byte $70, $02, $d8, $01     ; 120




                .byte $00, $00, $00, $01
                .byte $08, $00, $00, $01
                .byte $10, $00, $00, $01
                .byte $18, $00, $00, $01
                .byte $20, $00, $00, $01
                .byte $28, $00, $00, $01
                .byte $30, $00, $00, $01
                .byte $38, $00, $00, $01 ; 128

            .endproc",
        emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\sprites_8bpp_16x8_hflip.png");

        emulator.CompareImage(@"Vera\Images\sprites_8bpp_16x8_hflip.png");
    }

    [TestMethod]
    public async Task Normal_VFlip()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R41
            .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
            .org $810
                sei

                lda #02
                sta DC_BORDER

                lda #$40        ; 1bpp bitmap
                sta L0_CONFIG
                sta L1_CONFIG
    
                ; set colour 0
                lda #$01
                sta ADDRx_H
                lda #$fa
                sta ADDRx_M
                stz ADDRx_L

                lda #$12
                sta DATA0

                jsr create_sprites
                jsr map_sprites

                lda #$41        ; Sprites
                sta DC_VIDEO       

                lda #02
                sta CTRL

                lda #0
                sta DC_HSTART
                lda #0
                sta DC_VSTART

                stz CTRL

                lda #128
                sta DC_HSCALE
                sta DC_VSCALE

                lda #01
                sta IEN
                wai
                sta ISR     ; clear interrupt and wait for second frame
                wai

                stp

          .proc create_sprites    
    
                lda #$10
                sta ADDRx_H
                stz ADDRx_M
                stz ADDRx_L

                ldy #0
                ldx #0

                .sprite_loop:

                lda template, x

                cmp #$ff
                bne no_change

                tya

                .no_change:
                sta DATA0

                inx

                cpx #8*16
                bne sprite_loop

                ldx #0

                iny
                cpy #128
                bne sprite_loop

                rts

            .align $100
            .template:
                .byte $01, $01, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00
                .byte $01, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01
                .byte $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $01, $02, $03, $04, $05, $06, $07, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $0f
                .byte $01, $02, $03, $04, $05, $06, $07, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $0f
                .byte $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $01, $01
                .byte $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $01, $01

            .endproc

            .proc map_sprites

                .const address_l = $02
                .const address_h = $03

                lda #$11
                sta ADDRx_H
                lda #$fc
                sta ADDRx_M
                stz ADDRx_L

                ldy #$0

                stz address_l
                stz address_h

                .sprite_loop:

                lda address_l
                sta DATA0       ; address

                lda address_h
                ora #$80

                sta DATA0       ; 8bpp

                clc
                lda address_l
                adc #(8*16) >> 5
                sta address_l
                lda address_h
                adc #0
                sta address_h


                ldx #04
                .step:
                lda positions   ; gets modified
                sta DATA0
    
                clc
                lda step+1
                adc #1
                sta step+1
                lda step+2
                adc #0
                sta step+2

                dex
                bne step 

                lda #$04+2  ; vflip
                sta DATA0   ; depth

                lda #$10    ; 16x8px
                sta DATA0   ; height, width, offset

                iny
                cpy #128
                bne sprite_loop

                rts

            .positions:
                .byte $00, $00, $00, $00
                .byte $20, $00, $00, $00
                .byte $40, $00, $00, $00
                .byte $60, $00, $00, $00
                .byte $80, $00, $00, $00
                .byte $a0, $00, $00, $00
                .byte $c0, $00, $00, $00
                .byte $e0, $00, $00, $00

                .byte $00, $01, $00, $00
                .byte $20, $01, $00, $00
                .byte $40, $01, $00, $00
                .byte $60, $01, $00, $00
                .byte $80, $01, $00, $00
                .byte $a0, $01, $00, $00
                .byte $c0, $01, $00, $00
                .byte $e0, $01, $00, $00
    
                .byte $00, $02, $00, $00
                .byte $20, $02, $00, $00
                .byte $40, $02, $00, $00 
                .byte $60, $02, $00, $00     ; 20

                .byte $10, $00, $08, $00
                .byte $30, $00, $08, $00
                .byte $50, $00, $08, $00
                .byte $70, $00, $08, $00
                .byte $90, $00, $08, $00
                .byte $b0, $00, $08, $00
                .byte $d0, $00, $08, $00
                .byte $f0, $00, $08, $00

                .byte $10, $01, $08, $00
                .byte $30, $01, $08, $00
                .byte $50, $01, $08, $00
                .byte $70, $01, $08, $00
                .byte $90, $01, $08, $00
                .byte $b0, $01, $08, $00
                .byte $d0, $01, $08, $00
                .byte $f0, $01, $08, $00
   
                .byte $10, $02, $08, $00
                .byte $30, $02, $08, $00
                .byte $50, $02, $08, $00 
                .byte $70, $02, $08, $00     ; 40


                .byte $00, $00, $10, $00
                .byte $20, $00, $10, $00
                .byte $40, $00, $10, $00
                .byte $60, $00, $10, $00
                .byte $80, $00, $10, $00
                .byte $a0, $00, $10, $00
                .byte $c0, $00, $10, $00
                .byte $e0, $00, $10, $00

                .byte $00, $01, $10, $00
                .byte $20, $01, $10, $00
                .byte $40, $01, $10, $00
                .byte $60, $01, $10, $00
                .byte $80, $01, $10, $00
                .byte $a0, $01, $10, $00
                .byte $c0, $01, $10, $00
                .byte $e0, $01, $10, $00
    
                .byte $00, $02, $10, $00
                .byte $20, $02, $10, $00
                .byte $40, $02, $10, $00 
                .byte $60, $02, $10, $00     ; 60

                .byte $10, $00, $18, $00
                .byte $30, $00, $18, $00
                .byte $50, $00, $18, $00
                .byte $70, $00, $18, $00
                .byte $90, $00, $18, $00
                .byte $b0, $00, $18, $00
                .byte $d0, $00, $18, $00
                .byte $f0, $00, $18, $00

                .byte $10, $01, $18, $00
                .byte $30, $01, $18, $00
                .byte $50, $01, $18, $00
                .byte $70, $01, $18, $00
                .byte $90, $01, $18, $00
                .byte $b0, $01, $18, $00
                .byte $d0, $01, $18, $00
                .byte $f0, $01, $18, $00
   
                .byte $10, $02, $18, $00
                .byte $30, $02, $18, $00
                .byte $50, $02, $18, $00 
                .byte $70, $02, $18, $00     ; 80


                .byte $00, $00, $d0, $01
                .byte $20, $00, $d0, $01
                .byte $40, $00, $d0, $01
                .byte $60, $00, $d0, $01
                .byte $80, $00, $d0, $01
                .byte $a0, $00, $d0, $01
                .byte $c0, $00, $d0, $01
                .byte $e0, $00, $d0, $01

                .byte $00, $01, $d0, $01
                .byte $20, $01, $d0, $01
                .byte $40, $01, $d0, $01
                .byte $60, $01, $d0, $01
                .byte $80, $01, $d0, $01
                .byte $a0, $01, $d0, $01
                .byte $c0, $01, $d0, $01
                .byte $e0, $01, $d0, $01
    
                .byte $00, $02, $d0, $01
                .byte $20, $02, $d0, $01
                .byte $40, $02, $d0, $01 
                .byte $60, $02, $d0, $01     ; 100

                .byte $10, $00, $d8, $01
                .byte $30, $00, $d8, $01
                .byte $50, $00, $d8, $01
                .byte $70, $00, $d8, $01
                .byte $90, $00, $d8, $01
                .byte $b0, $00, $d8, $01
                .byte $d0, $00, $d8, $01
                .byte $f0, $00, $d8, $01

                .byte $10, $01, $d8, $01
                .byte $30, $01, $d8, $01
                .byte $50, $01, $d8, $01
                .byte $70, $01, $d8, $01
                .byte $90, $01, $d8, $01
                .byte $b0, $01, $d8, $01
                .byte $d0, $01, $d8, $01
                .byte $f0, $01, $d8, $01
   
                .byte $10, $01, $10, $01     ; not on the bottom line because of vram access
                .byte $30, $01, $10, $01     ; not on the bottom line because of vram access
                .byte $50, $02, $d8, $01 
                .byte $70, $02, $d8, $01     ; 120




                .byte $00, $00, $00, $01
                .byte $08, $00, $00, $01
                .byte $10, $00, $00, $01
                .byte $18, $00, $00, $01
                .byte $20, $00, $00, $01
                .byte $28, $00, $00, $01
                .byte $30, $00, $00, $01
                .byte $38, $00, $00, $01 ; 128

            .endproc",
        emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\sprites_8bpp_16x8_vflip.png");

        emulator.CompareImage(@"Vera\Images\sprites_8bpp_16x8_vflip.png");
    }
    [TestMethod]
    public async Task Normal_HVFLip()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R41
            .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
            .org $810
                sei

                lda #02
                sta DC_BORDER

                lda #$40        ; 1bpp bitmap
                sta L0_CONFIG
                sta L1_CONFIG
    
                ; set colour 0
                lda #$01
                sta ADDRx_H
                lda #$fa
                sta ADDRx_M
                stz ADDRx_L

                lda #$12
                sta DATA0

                jsr create_sprites
                jsr map_sprites

                lda #$41        ; Sprites
                sta DC_VIDEO       

                lda #02
                sta CTRL

                lda #0
                sta DC_HSTART
                lda #0
                sta DC_VSTART

                stz CTRL

                lda #128
                sta DC_HSCALE
                sta DC_VSCALE

                lda #01
                sta IEN
                wai
                sta ISR     ; clear interrupt and wait for second frame
                wai

                stp

          .proc create_sprites    
    
                lda #$10
                sta ADDRx_H
                stz ADDRx_M
                stz ADDRx_L

                ldy #0
                ldx #0

                .sprite_loop:

                lda template, x

                cmp #$ff
                bne no_change

                tya

                .no_change:
                sta DATA0

                inx

                cpx #8*16
                bne sprite_loop

                ldx #0

                iny
                cpy #128
                bne sprite_loop

                rts

            .align $100
            .template:
                .byte $01, $01, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00
                .byte $01, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01, $00, $01
                .byte $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $01, $02, $03, $04, $05, $06, $07, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $0f
                .byte $01, $02, $03, $04, $05, $06, $07, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $0f
                .byte $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00
                .byte $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $01, $01
                .byte $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $ff, $01, $01

            .endproc

            .proc map_sprites

                .const address_l = $02
                .const address_h = $03

                lda #$11
                sta ADDRx_H
                lda #$fc
                sta ADDRx_M
                stz ADDRx_L

                ldy #$0

                stz address_l
                stz address_h

                .sprite_loop:

                lda address_l
                sta DATA0       ; address

                lda address_h
                ora #$80

                sta DATA0       ; 8bpp

                clc
                lda address_l
                adc #(8*16) >> 5
                sta address_l
                lda address_h
                adc #0
                sta address_h


                ldx #04
                .step:
                lda positions   ; gets modified
                sta DATA0
    
                clc
                lda step+1
                adc #1
                sta step+1
                lda step+2
                adc #0
                sta step+2

                dex
                bne step 

                lda #$04+3  ; hvflip
                sta DATA0   ; depth

                lda #$10    ; 16x8px
                sta DATA0   ; height, width, offset

                iny
                cpy #128
                bne sprite_loop

                rts

            .positions:
                .byte $00, $00, $00, $00
                .byte $20, $00, $00, $00
                .byte $40, $00, $00, $00
                .byte $60, $00, $00, $00
                .byte $80, $00, $00, $00
                .byte $a0, $00, $00, $00
                .byte $c0, $00, $00, $00
                .byte $e0, $00, $00, $00

                .byte $00, $01, $00, $00
                .byte $20, $01, $00, $00
                .byte $40, $01, $00, $00
                .byte $60, $01, $00, $00
                .byte $80, $01, $00, $00
                .byte $a0, $01, $00, $00
                .byte $c0, $01, $00, $00
                .byte $e0, $01, $00, $00
    
                .byte $00, $02, $00, $00
                .byte $20, $02, $00, $00
                .byte $40, $02, $00, $00 
                .byte $60, $02, $00, $00     ; 20

                .byte $10, $00, $08, $00
                .byte $30, $00, $08, $00
                .byte $50, $00, $08, $00
                .byte $70, $00, $08, $00
                .byte $90, $00, $08, $00
                .byte $b0, $00, $08, $00
                .byte $d0, $00, $08, $00
                .byte $f0, $00, $08, $00

                .byte $10, $01, $08, $00
                .byte $30, $01, $08, $00
                .byte $50, $01, $08, $00
                .byte $70, $01, $08, $00
                .byte $90, $01, $08, $00
                .byte $b0, $01, $08, $00
                .byte $d0, $01, $08, $00
                .byte $f0, $01, $08, $00
   
                .byte $10, $02, $08, $00
                .byte $30, $02, $08, $00
                .byte $50, $02, $08, $00 
                .byte $70, $02, $08, $00     ; 40


                .byte $00, $00, $10, $00
                .byte $20, $00, $10, $00
                .byte $40, $00, $10, $00
                .byte $60, $00, $10, $00
                .byte $80, $00, $10, $00
                .byte $a0, $00, $10, $00
                .byte $c0, $00, $10, $00
                .byte $e0, $00, $10, $00

                .byte $00, $01, $10, $00
                .byte $20, $01, $10, $00
                .byte $40, $01, $10, $00
                .byte $60, $01, $10, $00
                .byte $80, $01, $10, $00
                .byte $a0, $01, $10, $00
                .byte $c0, $01, $10, $00
                .byte $e0, $01, $10, $00
    
                .byte $00, $02, $10, $00
                .byte $20, $02, $10, $00
                .byte $40, $02, $10, $00 
                .byte $60, $02, $10, $00     ; 60

                .byte $10, $00, $18, $00
                .byte $30, $00, $18, $00
                .byte $50, $00, $18, $00
                .byte $70, $00, $18, $00
                .byte $90, $00, $18, $00
                .byte $b0, $00, $18, $00
                .byte $d0, $00, $18, $00
                .byte $f0, $00, $18, $00

                .byte $10, $01, $18, $00
                .byte $30, $01, $18, $00
                .byte $50, $01, $18, $00
                .byte $70, $01, $18, $00
                .byte $90, $01, $18, $00
                .byte $b0, $01, $18, $00
                .byte $d0, $01, $18, $00
                .byte $f0, $01, $18, $00
   
                .byte $10, $02, $18, $00
                .byte $30, $02, $18, $00
                .byte $50, $02, $18, $00 
                .byte $70, $02, $18, $00     ; 80


                .byte $00, $00, $d0, $01
                .byte $20, $00, $d0, $01
                .byte $40, $00, $d0, $01
                .byte $60, $00, $d0, $01
                .byte $80, $00, $d0, $01
                .byte $a0, $00, $d0, $01
                .byte $c0, $00, $d0, $01
                .byte $e0, $00, $d0, $01

                .byte $00, $01, $d0, $01
                .byte $20, $01, $d0, $01
                .byte $40, $01, $d0, $01
                .byte $60, $01, $d0, $01
                .byte $80, $01, $d0, $01
                .byte $a0, $01, $d0, $01
                .byte $c0, $01, $d0, $01
                .byte $e0, $01, $d0, $01
    
                .byte $00, $02, $d0, $01
                .byte $20, $02, $d0, $01
                .byte $40, $02, $d0, $01 
                .byte $60, $02, $d0, $01     ; 100

                .byte $10, $00, $d8, $01
                .byte $30, $00, $d8, $01
                .byte $50, $00, $d8, $01
                .byte $70, $00, $d8, $01
                .byte $90, $00, $d8, $01
                .byte $b0, $00, $d8, $01
                .byte $d0, $00, $d8, $01
                .byte $f0, $00, $d8, $01

                .byte $10, $01, $d8, $01
                .byte $30, $01, $d8, $01
                .byte $50, $01, $d8, $01
                .byte $70, $01, $d8, $01
                .byte $90, $01, $d8, $01
                .byte $b0, $01, $d8, $01
                .byte $d0, $01, $d8, $01
                .byte $f0, $01, $d8, $01
   
                .byte $10, $01, $10, $01     ; not on the bottom line because of vram access
                .byte $30, $01, $10, $01     ; not on the bottom line because of vram access
                .byte $50, $02, $d8, $01 
                .byte $70, $02, $d8, $01     ; 120




                .byte $00, $00, $00, $01
                .byte $08, $00, $00, $01
                .byte $10, $00, $00, $01
                .byte $18, $00, $00, $01
                .byte $20, $00, $00, $01
                .byte $28, $00, $00, $01
                .byte $30, $00, $00, $01
                .byte $38, $00, $00, $01 ; 128

            .endproc",
        emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\sprites_8bpp_16x8_hvflip.png");

        emulator.CompareImage(@"Vera\Images\sprites_8bpp_16x8_hvflip.png");
    }
}
