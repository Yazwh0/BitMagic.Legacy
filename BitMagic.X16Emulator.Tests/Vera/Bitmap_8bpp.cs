using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera.Display;

[TestClass]
public class Bitmap_8Bpp
{
    [TestMethod]
    public async Task Image_Normal_Layer0()
    {
        var emulator = new Emulator();

        emulator.LoadImage(@"Vera\Images\testimage_8bpp.png", ImageHelper.ColourDepth.Depth_8bpp, 0);

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$11        ; enable layer 0
                        sta DC_VIDEO

                        lda #$07        ; bitmap, 8bpp
                        sta L0_CONFIG

                        lda #$00        ; 320 wide
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

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_8bpp_l0_normal.png");
        emulator.CompareImage(@"Vera\Images\bitmap_8bpp_l0_normal.png");
    }

    [TestMethod]
    public async Task Image_Normal_AddressChange_Layer0()
    {
        var emulator = new Emulator();

        emulator.LoadImage(@"Vera\Images\testimage_8bpp.png", ImageHelper.ColourDepth.Depth_8bpp, 0x1000);

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$11        ; enable layer 0
                        sta DC_VIDEO

                        lda #$07        ; bitmap, 8bpp
                        sta L0_CONFIG

                        lda #$08        ; 320 wide - starts at $1000
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

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_8bpp_l0_addresschange.png");
        emulator.CompareImage(@"Vera\Images\bitmap_8bpp_l0_addresschange.png");
    }

    [TestMethod]
    public async Task Image_Normal_PaletteOffset_Layer0()
    {
        var emulator = new Emulator();

        emulator.LoadImage(@"Vera\Images\testimage_8bpp.png", ImageHelper.ColourDepth.Depth_8bpp, 0);

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$11        ; enable layer 0
                        sta DC_VIDEO

                        lda #$07        ; bitmap, 8bpp
                        sta L0_CONFIG

                        lda #$00        ; 320 wide
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

                        lda #$07
                        sta L0_HSCROLL_H

                        lda #01
                        sta IEN
                        wai
                        sta ISR     ; clear interrupt and wait for second frame
                        wai

                        stp
                    ",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_8bpp_l0_normal_paletteoffset.png");
        emulator.CompareImage(@"Vera\Images\bitmap_8bpp_l0_normal_paletteoffset.png");
    }

    [TestMethod]
    public async Task Image_Scaled_Layer0()
    {
        var emulator = new Emulator();

        emulator.LoadImage(@"Vera\Images\testimage_8bpp.png", ImageHelper.ColourDepth.Depth_8bpp, 0);

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$11        ; enable layer 0
                        sta DC_VIDEO

                        lda #$07        ; bitmap, 2bpp
                        sta L0_CONFIG

                        lda #$00        ; 320 wide
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
                    ",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_8bpp_l0_scaled.png");
        emulator.CompareImage(@"Vera\Images\bitmap_8bpp_l0_scaled.png");
    }

    [TestMethod]
    public async Task Image_Normal_Layer1()
    {
        var emulator = new Emulator();

        emulator.LoadImage(@"Vera\Images\testimage_8bpp.png", ImageHelper.ColourDepth.Depth_8bpp, 0);

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$21        ; enable layer 0
                        sta DC_VIDEO

                        lda #$07        ; bitmap, 8bpp
                        sta L1_CONFIG

                        lda #$00        ; 320 wide
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

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_8bpp_l1_normal.png");
        emulator.CompareImage(@"Vera\Images\bitmap_8bpp_l1_normal.png");
    }

    [TestMethod]
    public async Task Image_Normal_AddressChange_Layer1()
    {
        var emulator = new Emulator();

        emulator.LoadImage(@"Vera\Images\testimage_8bpp.png", ImageHelper.ColourDepth.Depth_8bpp, 0x1000);

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$21        ; enable layer 0
                        sta DC_VIDEO

                        lda #$07        ; bitmap, 8bpp
                        sta L1_CONFIG

                        lda #$08        ; 320 wide - starts at $1000
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

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_8bpp_l1_addresschange.png");
        emulator.CompareImage(@"Vera\Images\bitmap_8bpp_l1_addresschange.png");
    }

    [TestMethod]
    public async Task Image_Normal_PaletteOffset_Layer1()
    {
        var emulator = new Emulator();

        emulator.LoadImage(@"Vera\Images\testimage_8bpp.png", ImageHelper.ColourDepth.Depth_8bpp, 0);

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$21        ; enable layer 0
                        sta DC_VIDEO

                        lda #$07        ; bitmap, 8bpp
                        sta L1_CONFIG

                        lda #$00        ; 320 wide
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

                        lda #$07
                        sta L1_HSCROLL_H

                        lda #01
                        sta IEN
                        wai
                        sta ISR     ; clear interrupt and wait for second frame
                        wai

                        stp
                    ",
                emulator);

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_8bpp_l1_normal_paletteoffset.png");
        emulator.CompareImage(@"Vera\Images\bitmap_8bpp_l1_normal_paletteoffset.png");
    }

    [TestMethod]
    public async Task Image_Scaled_Layer1()
    {
        var emulator = new Emulator();

        emulator.LoadImage(@"Vera\Images\testimage_8bpp.png", ImageHelper.ColourDepth.Depth_8bpp, 0);

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$21        ; enable layer 0
                        sta DC_VIDEO

                        lda #$07        ; bitmap, 2bpp
                        sta L1_CONFIG

                        lda #$00        ; 320 wide
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

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_8bpp_l1_scaled.png");
        emulator.CompareImage(@"Vera\Images\bitmap_8bpp_l1_scaled.png");
    }
}
