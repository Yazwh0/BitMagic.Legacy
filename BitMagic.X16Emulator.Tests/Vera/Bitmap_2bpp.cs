using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera.Display;

[TestClass]
public class Bitmap_2Bpp
{

    [TestMethod]
    public async Task Image_Normal_Layer0()
    {
        var emulator = new Emulator();

        emulator.LoadImage(@"Vera\Images\testimage_2bpp.png", ImageHelper.ColourDepth.Depth_2bpp, 0);

        await X16TestHelper.Emulate(@$"
                    .machine CommanderX16R40
                    .org $810
                        sei

                        lda #02
                        sta DC_BORDER

                        lda #$11        ; enable layer 0
                        sta DC_VIDEO

                        lda #$05        ; bitmap, 2bpp
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

        emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\bitmap_2bpp_l0_normal.png");
        emulator.CompareImage(@"Vera\Images\bitmap_2bpp_l0_normal.png");
    }
}