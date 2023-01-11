using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera.Display;

[TestClass]
public class Bitmap_1Bpp
{
    [TestMethod]
    public async Task Box_Normal_Layer0()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$11        ; enable layer 0
                        sta DC_VIDEO

                        lda #$04        ; bitmap, 1bpp
                        sta L0_CONFIG

                        lda #$01        ; 640 wide
                        sta L0_TILEBASE 

                        ; set colour 0
                        lda #$01
                        sta ADDRx_H
                        lda #$fa
                        sta ADDRx_M
                        stz ADDRx_L

                        lda #$12
                        sta DATA0

                        ; Draw box
                        lda #$10
                        sta ADDRx_H
                        stz ADDRx_M
                        stz ADDRx_L

                        jsr blankline
                        jsr fullline

                        ldy #$00
                    .loop_1:
                        jsr middleline
                        dey
                        bne loop_1
    
                        ldy #$dc
                    .loop_2:
                        jsr middleline
                        dey
                        bne loop_2

                        jsr fullline
                        jsr blankline

                        jsr addnoise

                        ldx #128
                        stx DC_VSCALE  
                        stx DC_HSCALE

                        lda #01
                        sta IEN
                        wai
                        sta ISR     ; clear interrupt and wait for second frame
                        wai

                        stp

                    .proc blankline
                        ldx #$50

                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        rts
                    .endproc

                    .proc fullline
                        lda #$7f
                        sta DATA0
                        ldx #$4e
                        lda #$ff
                    .loop:
                        sta DATA0
                        dex
                        bne loop

                        lda #$fe
                        sta DATA0

                        rts
                    .endproc

                    .proc middleline
                        lda #$40
                        sta DATA0
                        ldx #$4e
                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        lda #$02
                        sta DATA0

                        rts
                    .endproc

                    .proc addnoise
                        ldx #$50

                    .loop:
                        stx DATA0
                        dex
                        bne loop

                        rts
                    .endproc
                    ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_1bpp_l0_box_normal.png");
        emulator.CompareImage(@"Vera\Images\bitmap_1bpp_l0_box_normal.png");
    }

    [TestMethod]
    public async Task Box_Normal_PaletteOffset_Layer0()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$11        ; enable layer 0
                        sta DC_VIDEO

                        lda #$04        ; bitmap, 1bpp
                        sta L0_CONFIG

                        lda #$01        ; 640 wide
                        sta L0_TILEBASE 

                        ; set colour 0
                        lda #$01
                        sta ADDRx_H
                        lda #$fa
                        sta ADDRx_M
                        stz ADDRx_L

                        lda #$12
                        sta DATA0

                        ; Draw box
                        lda #$10
                        sta ADDRx_H
                        stz ADDRx_M
                        stz ADDRx_L

                        jsr blankline
                        jsr fullline

                        ldy #$00
                    .loop_1:
                        jsr middleline
                        dey
                        bne loop_1
    
                        ldy #$dc
                    .loop_2:
                        jsr middleline
                        dey
                        bne loop_2

                        jsr fullline
                        jsr blankline

                        jsr addnoise

                        ldx #128
                        stx DC_VSCALE  
                        stx DC_HSCALE

                        lda #$07
                        sta L0_HSCROLL_H

                        lda #01
                        sta IEN
                        wai
                        sta ISR     ; clear interrupt and wait for second frame
                        wai

                        stp

                    .proc blankline
                        ldx #$50

                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        rts
                    .endproc

                    .proc fullline
                        lda #$7f
                        sta DATA0
                        ldx #$4e
                        lda #$ff
                    .loop:
                        sta DATA0
                        dex
                        bne loop

                        lda #$fe
                        sta DATA0

                        rts
                    .endproc

                    .proc middleline
                        lda #$40
                        sta DATA0
                        ldx #$4e
                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        lda #$02
                        sta DATA0

                        rts
                    .endproc

                    .proc addnoise
                        ldx #$50

                    .loop:
                        stx DATA0
                        dex
                        bne loop

                        rts
                    .endproc
                    ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_1bpp_l0_box_paletteoffset_normal.png");
        emulator.CompareImage(@"Vera\Images\bitmap_1bpp_l0_box_paletteoffset_normal.png");
    }

    [TestMethod]
    public async Task Box_Normal_AddressChange_Layer0()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$11        ; enable layer 0
                        sta DC_VIDEO

                        lda #$04        ; bitmap, 1bpp
                        sta L0_CONFIG

                        lda #$09        ; 640 wide - starts at $1000
                        sta L0_TILEBASE 

                        ; set colour 0
                        lda #$01
                        sta ADDRx_H
                        lda #$fa
                        sta ADDRx_M
                        stz ADDRx_L

                        lda #$12
                        sta DATA0

                        ; Draw box
                        lda #$10
                        sta ADDRx_H
                        sta ADDRx_M
                        stz ADDRx_L

                        jsr blankline
                        jsr fullline

                        ldy #$00
                    .loop_1:
                        jsr middleline
                        dey
                        bne loop_1
    
                        ldy #$dc
                    .loop_2:
                        jsr middleline
                        dey
                        bne loop_2

                        jsr fullline
                        jsr blankline

                        jsr addnoise

                        ldx #128
                        stx DC_VSCALE  
                        stx DC_HSCALE

                        lda #01
                        sta IEN
                        wai
                        sta ISR     ; clear interrupt and wait for second frame
                        wai

                        stp

                    .proc blankline
                        ldx #$50

                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        rts
                    .endproc

                    .proc fullline
                        lda #$7f
                        sta DATA0
                        ldx #$4e
                        lda #$ff
                    .loop:
                        sta DATA0
                        dex
                        bne loop

                        lda #$fe
                        sta DATA0

                        rts
                    .endproc

                    .proc middleline
                        lda #$40
                        sta DATA0
                        ldx #$4e
                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        lda #$02
                        sta DATA0

                        rts
                    .endproc

                    .proc addnoise
                        ldx #$50

                    .loop:
                        stx DATA0
                        dex
                        bne loop

                        rts
                    .endproc
                    ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_1bpp_l0_box_normal_addresschange.png");
        emulator.CompareImage(@"Vera\Images\bitmap_1bpp_l0_box_normal_addresschange.png");
    }

    [TestMethod]
    public async Task Box_ScaledDown_Layer0()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$11        ; enable layer 0
                        sta DC_VIDEO

                        lda #$04        ; bitmap, 1bpp
                        sta L0_CONFIG

                        lda #$01        ; 640 wide
                        sta L0_TILEBASE 

                        ; set colour 0
                        lda #$01
                        sta ADDRx_H
                        lda #$fa
                        sta ADDRx_M
                        stz ADDRx_L

                        lda #$12
                        sta DATA0

                        ; Draw box
                        lda #$10
                        sta ADDRx_H
                        stz ADDRx_M
                        stz ADDRx_L

                        jsr blankline
                        jsr fullline

                        ldy #$00
                    .loop_1:
                        jsr middleline
                        dey
                        bne loop_1
    
                        ldy #$dc
                    .loop_2:
                        jsr middleline
                        dey
                        bne loop_2

                        jsr fullline
                        jsr blankline

                        jsr addnoise

                        ldx #64
                        stx DC_VSCALE  
                        stx DC_HSCALE

                        lda #01
                        sta IEN
                        wai
                        sta ISR     ; clear interrupt and wait for second frame
                        wai

                        stp

                    .proc blankline
                        ldx #$50

                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        rts
                    .endproc

                    .proc fullline
                        lda #$7f
                        sta DATA0
                        ldx #$4e
                        lda #$ff
                    .loop:
                        sta DATA0
                        dex
                        bne loop

                        lda #$fe
                        sta DATA0

                        rts
                    .endproc

                    .proc middleline
                        lda #$40
                        sta DATA0
                        ldx #$4e
                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        lda #$02
                        sta DATA0

                        rts
                    .endproc

                    .proc addnoise
                        ldx #$50

                    .loop:
                        stx DATA0
                        dex
                        bne loop

                        rts
                    .endproc
                    ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_1bpp_l0_box_scaleddown.png");
        emulator.CompareImage(@"Vera\Images\bitmap_1bpp_l0_box_scaleddown.png");
    }

    [TestMethod]
    public async Task Box_ScaledUp_Layer0()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$11        ; enable layer 0
                        sta DC_VIDEO

                        lda #$04        ; bitmap, 1bpp
                        sta L0_CONFIG

                        lda #$01        ; 640 wide
                        sta L0_TILEBASE 

                        ; set colour 0
                        lda #$01
                        sta ADDRx_H
                        lda #$fa
                        sta ADDRx_M
                        stz ADDRx_L

                        lda #$12
                        sta DATA0

                        ; Draw box
                        lda #$10
                        sta ADDRx_H
                        stz ADDRx_M
                        stz ADDRx_L

                        jsr blankline
                        jsr fullline

                        ldy #$00
                    .loop_1:
                        jsr middleline
                        dey
                        bne loop_1
    
                        ldy #$dc
                    .loop_2:
                        jsr middleline
                        dey
                        bne loop_2

                        jsr fullline
                        jsr blankline

                        jsr addnoise

                        ldx #$ff
                        stx DC_VSCALE  
                        stx DC_HSCALE

                        lda #01
                        sta IEN
                        wai
                        sta ISR     ; clear interrupt and wait for second frame
                        wai

                        stp

                    .proc blankline
                        ldx #$50

                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        rts
                    .endproc

                    .proc fullline
                        lda #$7f
                        sta DATA0
                        ldx #$4e
                        lda #$ff
                    .loop:
                        sta DATA0
                        dex
                        bne loop

                        lda #$fe
                        sta DATA0

                        rts
                    .endproc

                    .proc middleline
                        lda #$40
                        sta DATA0
                        ldx #$4e
                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        lda #$02
                        sta DATA0

                        rts
                    .endproc

                    .proc addnoise
                        ldx #$50

                    .loop:
                        stx DATA0
                        dex
                        bne loop

                        rts
                    .endproc
                    ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_1bpp_l0_box_scaledup.png");
        emulator.CompareImage(@"Vera\Images\bitmap_1bpp_l0_box_scaledup.png");
    }

    [TestMethod]
    public async Task Image_Normal_Layer0()
    {
        var emulator = new Emulator();

        emulator.LoadImage(@"Vera\Images\testimage_1bpp.png", ImageHelper.ColourDepthImage.Depth_1bpp, 0);

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$11        ; enable layer 0
                        sta DC_VIDEO

                        lda #$04        ; bitmap, 1bpp
                        sta L0_CONFIG

                        lda #$01        ; 640 wide
                        sta L0_TILEBASE 

                        ; set colour 0
                        lda #$01
                        sta ADDRx_H
                        lda #$fa
                        sta ADDRx_M
                        stz ADDRx_L

                        lda #$12
                        sta DATA0

                        ldx #128
                        stx DC_VSCALE  
                        stx DC_HSCALE

                        lda #01
                        sta IEN
                        wai
                        sta ISR     ; clear interrupt and wait for second frame
                        wai

                        stp

                    .proc blankline
                        ldx #$50

                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        rts
                    .endproc

                    .proc fullline
                        lda #$7f
                        sta DATA0
                        ldx #$4e
                        lda #$ff
                    .loop:
                        sta DATA0
                        dex
                        bne loop

                        lda #$fe
                        sta DATA0

                        rts
                    .endproc

                    .proc middleline
                        lda #$40
                        sta DATA0
                        ldx #$4e
                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        lda #$02
                        sta DATA0

                        rts
                    .endproc

                    .proc addnoise
                        ldx #$50

                    .loop:
                        stx DATA0
                        dex
                        bne loop

                        rts
                    .endproc
                    ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_1bpp_l0_normal.png");
        emulator.CompareImage(@"Vera\Images\bitmap_1bpp_l0_normal.png");
    }

    [TestMethod]
    public async Task Image_ScaledUp_Layer0()
    {
        var emulator = new Emulator();

        emulator.LoadImage(@"Vera\Images\testimage_1bpp.png", ImageHelper.ColourDepthImage.Depth_1bpp, 0);

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$11        ; enable layer 0
                        sta DC_VIDEO

                        lda #$04        ; bitmap, 1bpp
                        sta L0_CONFIG

                        lda #$01        ; 640 wide
                        sta L0_TILEBASE 

                        ; set colour 0
                        lda #$01
                        sta ADDRx_H
                        lda #$fa
                        sta ADDRx_M
                        stz ADDRx_L

                        lda #$12
                        sta DATA0

                        ldx #180
                        stx DC_VSCALE  
                        stx DC_HSCALE

                        lda #01
                        sta IEN
                        wai
                        sta ISR     ; clear interrupt and wait for second frame
                        wai

                        stp

                    .proc blankline
                        ldx #$50

                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        rts
                    .endproc

                    .proc fullline
                        lda #$7f
                        sta DATA0
                        ldx #$4e
                        lda #$ff
                    .loop:
                        sta DATA0
                        dex
                        bne loop

                        lda #$fe
                        sta DATA0

                        rts
                    .endproc

                    .proc middleline
                        lda #$40
                        sta DATA0
                        ldx #$4e
                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        lda #$02
                        sta DATA0

                        rts
                    .endproc

                    .proc addnoise
                        ldx #$50

                    .loop:
                        stx DATA0
                        dex
                        bne loop

                        rts
                    .endproc
                    ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_1bpp_l0_scaledup.png");
        emulator.CompareImage(@"Vera\Images\bitmap_1bpp_l0_scaledup.png");
    }


    [TestMethod]
    public async Task Image_ScaledDown_Layer0()
    {
        var emulator = new Emulator();

        emulator.LoadImage(@"Vera\Images\testimage_1bpp.png", ImageHelper.ColourDepthImage.Depth_1bpp, 0);

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$11        ; enable layer 0
                        sta DC_VIDEO

                        lda #$04        ; bitmap, 1bpp
                        sta L0_CONFIG

                        lda #$01        ; 640 wide
                        sta L0_TILEBASE 

                        ; set colour 0
                        lda #$01
                        sta ADDRx_H
                        lda #$fa
                        sta ADDRx_M
                        stz ADDRx_L

                        lda #$12
                        sta DATA0

                        ldx #64
                        stx DC_VSCALE  
                        stx DC_HSCALE

                        lda #01
                        sta IEN
                        wai
                        sta ISR     ; clear interrupt and wait for second frame
                        wai

                        stp
                    ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_1bpp_l0_scaleddown.png");
        emulator.CompareImage(@"Vera\Images\bitmap_1bpp_l0_scaleddown.png");
    }

    [TestMethod]
    public async Task Box_Normal_Layer1()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$21        ; enable layer 0
                        sta DC_VIDEO

                        lda #$04        ; bitmap, 1bpp
                        sta L1_CONFIG

                        lda #$01        ; 640 wide
                        sta L1_TILEBASE 

                        ; set colour 0
                        lda #$01
                        sta ADDRx_H
                        lda #$fa
                        sta ADDRx_M
                        stz ADDRx_L

                        lda #$12
                        sta DATA0

                        ; Draw box
                        lda #$10
                        sta ADDRx_H
                        stz ADDRx_M
                        stz ADDRx_L

                        jsr blankline
                        jsr fullline

                        ldy #$00
                    .loop_1:
                        jsr middleline
                        dey
                        bne loop_1
    
                        ldy #$dc
                    .loop_2:
                        jsr middleline
                        dey
                        bne loop_2

                        jsr fullline
                        jsr blankline

                        jsr addnoise

                        ldx #128
                        stx DC_VSCALE  
                        stx DC_HSCALE

                        lda #01
                        sta IEN
                        wai
                        sta ISR     ; clear interrupt and wait for second frame
                        wai

                        stp

                    .proc blankline
                        ldx #$50

                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        rts
                    .endproc

                    .proc fullline
                        lda #$7f
                        sta DATA0
                        ldx #$4e
                        lda #$ff
                    .loop:
                        sta DATA0
                        dex
                        bne loop

                        lda #$fe
                        sta DATA0

                        rts
                    .endproc

                    .proc middleline
                        lda #$40
                        sta DATA0
                        ldx #$4e
                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        lda #$02
                        sta DATA0

                        rts
                    .endproc

                    .proc addnoise
                        ldx #$50

                    .loop:
                        stx DATA0
                        dex
                        bne loop

                        rts
                    .endproc
                    ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_1bpp_l1_box_normal.png");
        emulator.CompareImage(@"Vera\Images\bitmap_1bpp_l1_box_normal.png");
    }

    [TestMethod]
    public async Task Box_Normal_PaletteOffsetLayer1()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$21        ; enable layer 0
                        sta DC_VIDEO

                        lda #$04        ; bitmap, 1bpp
                        sta L1_CONFIG

                        lda #$01        ; 640 wide
                        sta L1_TILEBASE 

                        ; set colour 0
                        lda #$01
                        sta ADDRx_H
                        lda #$fa
                        sta ADDRx_M
                        stz ADDRx_L

                        lda #$12
                        sta DATA0

                        ; Draw box
                        lda #$10
                        sta ADDRx_H
                        stz ADDRx_M
                        stz ADDRx_L

                        jsr blankline
                        jsr fullline

                        ldy #$00
                    .loop_1:
                        jsr middleline
                        dey
                        bne loop_1
    
                        ldy #$dc
                    .loop_2:
                        jsr middleline
                        dey
                        bne loop_2

                        jsr fullline
                        jsr blankline

                        jsr addnoise

                        ldx #128
                        stx DC_VSCALE  
                        stx DC_HSCALE

                        lda #$07
                        sta L0_HSCROLL_H

                        lda #01
                        sta IEN
                        wai
                        sta ISR     ; clear interrupt and wait for second frame
                        wai

                        stp

                    .proc blankline
                        ldx #$50

                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        rts
                    .endproc

                    .proc fullline
                        lda #$7f
                        sta DATA0
                        ldx #$4e
                        lda #$ff
                    .loop:
                        sta DATA0
                        dex
                        bne loop

                        lda #$fe
                        sta DATA0

                        rts
                    .endproc

                    .proc middleline
                        lda #$40
                        sta DATA0
                        ldx #$4e
                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        lda #$02
                        sta DATA0

                        rts
                    .endproc

                    .proc addnoise
                        ldx #$50

                    .loop:
                        stx DATA0
                        dex
                        bne loop

                        rts
                    .endproc
                    ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_1bpp_l1_box_paletteoffset_normal.png");
        emulator.CompareImage(@"Vera\Images\bitmap_1bpp_l1_box_paletteoffset_normal.png");
    }

    [TestMethod]
    public async Task Box_Normal_AddressChange_Layer1()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$21        ; enable layer 0
                        sta DC_VIDEO

                        lda #$04        ; bitmap, 1bpp
                        sta L1_CONFIG

                        lda #$09        ; 640 wide - starts at $1000
                        sta L1_TILEBASE 

                        ; set colour 0
                        lda #$01
                        sta ADDRx_H
                        lda #$fa
                        sta ADDRx_M
                        stz ADDRx_L

                        lda #$12
                        sta DATA0

                        ; Draw box
                        lda #$10
                        sta ADDRx_H
                        sta ADDRx_M
                        stz ADDRx_L

                        jsr blankline
                        jsr fullline

                        ldy #$00
                    .loop_1:
                        jsr middleline
                        dey
                        bne loop_1
    
                        ldy #$dc
                    .loop_2:
                        jsr middleline
                        dey
                        bne loop_2

                        jsr fullline
                        jsr blankline

                        jsr addnoise

                        ldx #128
                        stx DC_VSCALE  
                        stx DC_HSCALE

                        lda #01
                        sta IEN
                        wai
                        sta ISR     ; clear interrupt and wait for second frame
                        wai

                        stp

                    .proc blankline
                        ldx #$50

                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        rts
                    .endproc

                    .proc fullline
                        lda #$7f
                        sta DATA0
                        ldx #$4e
                        lda #$ff
                    .loop:
                        sta DATA0
                        dex
                        bne loop

                        lda #$fe
                        sta DATA0

                        rts
                    .endproc

                    .proc middleline
                        lda #$40
                        sta DATA0
                        ldx #$4e
                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        lda #$02
                        sta DATA0

                        rts
                    .endproc

                    .proc addnoise
                        ldx #$50

                    .loop:
                        stx DATA0
                        dex
                        bne loop

                        rts
                    .endproc
                    ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_1bpp_l1_box_normal_addresschange.png");
        emulator.CompareImage(@"Vera\Images\bitmap_1bpp_l1_box_normal_addresschange.png");
    }

    [TestMethod]
    public async Task Box_ScaledDown_Layer1()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$21        ; enable layer 0
                        sta DC_VIDEO

                        lda #$04        ; bitmap, 1bpp
                        sta L1_CONFIG

                        lda #$01        ; 640 wide
                        sta L1_TILEBASE 

                        ; set colour 0
                        lda #$01
                        sta ADDRx_H
                        lda #$fa
                        sta ADDRx_M
                        stz ADDRx_L

                        lda #$12
                        sta DATA0

                        ; Draw box
                        lda #$10
                        sta ADDRx_H
                        stz ADDRx_M
                        stz ADDRx_L

                        jsr blankline
                        jsr fullline

                        ldy #$00
                    .loop_1:
                        jsr middleline
                        dey
                        bne loop_1
    
                        ldy #$dc
                    .loop_2:
                        jsr middleline
                        dey
                        bne loop_2

                        jsr fullline
                        jsr blankline

                        jsr addnoise

                        ldx #64
                        stx DC_VSCALE  
                        stx DC_HSCALE

                        lda #01
                        sta IEN
                        wai
                        sta ISR     ; clear interrupt and wait for second frame
                        wai

                        stp

                    .proc blankline
                        ldx #$50

                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        rts
                    .endproc

                    .proc fullline
                        lda #$7f
                        sta DATA0
                        ldx #$4e
                        lda #$ff
                    .loop:
                        sta DATA0
                        dex
                        bne loop

                        lda #$fe
                        sta DATA0

                        rts
                    .endproc

                    .proc middleline
                        lda #$40
                        sta DATA0
                        ldx #$4e
                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        lda #$02
                        sta DATA0

                        rts
                    .endproc

                    .proc addnoise
                        ldx #$50

                    .loop:
                        stx DATA0
                        dex
                        bne loop

                        rts
                    .endproc
                    ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_1bpp_l1_box_scaleddown.png");
        emulator.CompareImage(@"Vera\Images\bitmap_1bpp_l1_box_scaleddown.png");
    }

    [TestMethod]
    public async Task Box_ScaledUp_Layer1()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$21        ; enable layer 0
                        sta DC_VIDEO

                        lda #$04        ; bitmap, 1bpp
                        sta L1_CONFIG

                        lda #$01        ; 640 wide
                        sta L1_TILEBASE 

                        ; set colour 0
                        lda #$01
                        sta ADDRx_H
                        lda #$fa
                        sta ADDRx_M
                        stz ADDRx_L

                        lda #$12
                        sta DATA0

                        ; Draw box
                        lda #$10
                        sta ADDRx_H
                        stz ADDRx_M
                        stz ADDRx_L

                        jsr blankline
                        jsr fullline

                        ldy #$00
                    .loop_1:
                        jsr middleline
                        dey
                        bne loop_1
    
                        ldy #$dc
                    .loop_2:
                        jsr middleline
                        dey
                        bne loop_2

                        jsr fullline
                        jsr blankline

                        jsr addnoise

                        ldx #$ff
                        stx DC_VSCALE  
                        stx DC_HSCALE

                        lda #01
                        sta IEN
                        wai
                        sta ISR     ; clear interrupt and wait for second frame
                        wai

                        stp

                    .proc blankline
                        ldx #$50

                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        rts
                    .endproc

                    .proc fullline
                        lda #$7f
                        sta DATA0
                        ldx #$4e
                        lda #$ff
                    .loop:
                        sta DATA0
                        dex
                        bne loop

                        lda #$fe
                        sta DATA0

                        rts
                    .endproc

                    .proc middleline
                        lda #$40
                        sta DATA0
                        ldx #$4e
                    .loop:
                        stz DATA0
                        dex
                        bne loop

                        lda #$02
                        sta DATA0

                        rts
                    .endproc

                    .proc addnoise
                        ldx #$50

                    .loop:
                        stx DATA0
                        dex
                        bne loop

                        rts
                    .endproc
                    ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_1bpp_l1_box_scaledup.png");
        emulator.CompareImage(@"Vera\Images\bitmap_1bpp_l1_box_scaledup.png");
    }

    [TestMethod]
    public async Task Image_Normal_Layer1()
    {
        var emulator = new Emulator();

        emulator.LoadImage(@"Vera\Images\testimage_1bpp.png", ImageHelper.ColourDepthImage.Depth_1bpp, 0);

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$21        ; enable layer 0
                        sta DC_VIDEO

                        lda #$04        ; bitmap, 1bpp
                        sta L1_CONFIG

                        lda #$01        ; 640 wide
                        sta L1_TILEBASE 

                        ; set colour 0
                        lda #$01
                        sta ADDRx_H
                        lda #$fa
                        sta ADDRx_M
                        stz ADDRx_L

                        lda #$12
                        sta DATA0

                        ldx #128
                        stx DC_VSCALE  
                        stx DC_HSCALE

                        lda #01
                        sta IEN
                        wai
                        sta ISR     ; clear interrupt and wait for second frame
                        wai

                        stp
                    ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_1bpp_l1_normal.png");
        emulator.CompareImage(@"Vera\Images\bitmap_1bpp_l1_normal.png");
    }

    [TestMethod]
    public async Task Image_ScaledUp_Layer1()
    {
        var emulator = new Emulator();

        emulator.LoadImage(@"Vera\Images\testimage_1bpp.png", ImageHelper.ColourDepthImage.Depth_1bpp, 0);

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$21        ; enable layer 0
                        sta DC_VIDEO

                        lda #$04        ; bitmap, 1bpp
                        sta L1_CONFIG

                        lda #$01        ; 640 wide
                        sta L1_TILEBASE 

                        ; set colour 0
                        lda #$01
                        sta ADDRx_H
                        lda #$fa
                        sta ADDRx_M
                        stz ADDRx_L

                        lda #$12
                        sta DATA0

                        ldx #180
                        stx DC_VSCALE  
                        stx DC_HSCALE

                        lda #01
                        sta IEN
                        wai
                        sta ISR     ; clear interrupt and wait for second frame
                        wai

                        stp
                    ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_1bpp_l1_scaledup.png");
        emulator.CompareImage(@"Vera\Images\bitmap_1bpp_l1_scaledup.png");
    }


    [TestMethod]
    public async Task Image_ScaledDown_Layer1()
    {
        var emulator = new Emulator();

        emulator.LoadImage(@"Vera\Images\testimage_1bpp.png", ImageHelper.ColourDepthImage.Depth_1bpp, 0);

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$21        ; enable layer 0
                        sta DC_VIDEO

                        lda #$04        ; bitmap, 1bpp
                        sta L1_CONFIG

                        lda #$01        ; 640 wide
                        sta L1_TILEBASE 

                        ; set colour 0
                        lda #$01
                        sta ADDRx_H
                        lda #$fa
                        sta ADDRx_M
                        stz ADDRx_L

                        lda #$12
                        sta DATA0

                        ldx #64
                        stx DC_VSCALE  
                        stx DC_HSCALE

                        lda #01
                        sta IEN
                        wai
                        sta ISR     ; clear interrupt and wait for second frame
                        wai

                        stp
                    ",
                emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_1bpp_l1_scaleddown.png");
        emulator.CompareImage(@"Vera\Images\bitmap_1bpp_l1_scaleddown.png");
    }
}
