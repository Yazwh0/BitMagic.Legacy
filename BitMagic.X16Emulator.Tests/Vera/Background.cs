using BitMagic.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera;

[TestClass]
public class Background
{
    [TestMethod]
    public async Task Area()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sei
                lda #01
                sta IEN
                wai            
                stp",
                emulator);

        emulator.CompareImage(@"Vera\Images\background_area.png");
    }

    [TestMethod]
    public async Task Colour()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                    sei
                    lda #$01
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$ab
                    sta DATA0
                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai
                    stp",
                emulator);

        emulator.CompareImage(@"Vera\Images\background_colour.png");
    }

    [TestMethod]
    public async Task VStart()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                    sei
                    lda #02
                    sta DC_BORDER   ; set border colour red

                    lda #2          ; required for DC_START\STOP
                    sta CTRL

                    lda #$01
                    sta DC_VSTART   ; start 2 pixels from the top
    
                    lda #$01        ; background colour
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$ab
                    sta DATA0

                    lda #01
                    sta IEN
                    wai
                    sta ISR         ; clear interrupt and wait for second frame
                    wai
                    stp",
                emulator);

        emulator.CompareImage(@"Vera\Images\background_vstart.png");
    }

    [TestMethod]
    public async Task VStop()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                    sei
                    lda #02
                    sta DC_BORDER   ; set border colour red

                    lda #2          ; required for DC_START\STOP
                    sta CTRL

                    lda #$ef
                    sta DC_VSTOP    ; stop 2 pixels from the bottom
    
                    lda #$01        ; background colour
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$ab
                    sta DATA0

                    lda #01
                    sta IEN
                    wai
                    sta ISR         ; clear interrupt and wait for second frame
                    wai
                    stp",
                emulator);

        emulator.CompareImage(@"Vera\Images\background_vstop.png");
    }

    [TestMethod]
    public async Task HStart()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                    sei
                    lda #02
                    sta DC_BORDER   ; set border colour red

                    lda #2          ; required for DC_START\STOP
                    sta CTRL

                    lda #$01
                    sta DC_HSTART   ; start 4 pixels from the edge
    
                    lda #$01        ; background colour
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$ab
                    sta DATA0

                    lda #01
                    sta IEN
                    wai
                    sta ISR         ; clear interrupt and wait for second frame
                    wai
                    stp",
                emulator);

        emulator.CompareImage(@"Vera\Images\background_hstart.png");
    }

    [TestMethod]
    public async Task HStop()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                    sei
                    lda #02
                    sta DC_BORDER   ; set border colour red

                    lda #2          ; required for DC_START\STOP
                    sta CTRL

                    lda #$9f
                    sta DC_HSTOP   ; stop 4 pixels from the edge
    
                    lda #$01        ; background colour
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$ab
                    sta DATA0

                    lda #01
                    sta IEN
                    wai
                    sta ISR         ; clear interrupt and wait for second frame
                    wai
                    stp",
                emulator);

        emulator.CompareImage(@"Vera\Images\background_hstop.png");
    }
}