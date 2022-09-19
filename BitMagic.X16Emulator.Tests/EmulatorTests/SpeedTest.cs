using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class SpeedTest
{
    [TestMethod]

    public async Task ZeroPage()
    {
        var emulator = new Emulator();
        emulator.Headless = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sei
                lda #$50
                sta $02
                sta $03
                ldy #$ff
                .mainloop:
                ldx #$ff
                .loop:
                dex
                bne loop
                dey
                bne mainloop
                lda $02
                tax
                dex
                txa
                sta $02
                bne mainloop
                lda $03
                tax
                dex
                txa
                sta $03
                bne mainloop
                stp
                ",
                emulator);
    }

    [TestMethod]

    public async Task BankedRam()
    {
        var emulator = new Emulator();
        emulator.Headless = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sei
                lda #$50
                sta $a002
                sta $a003
                ldy #$ff
                .mainloop:
                ldx #$ff
                .loop:
                dex
                bne loop
                dey
                bne mainloop
                lda $a002
                tax
                dex
                txa
                sta $a002
                bne mainloop
                lda $a003
                tax
                dex
                txa
                sta $a003
                bne mainloop
                stp
                ",
                emulator);
    }

    [TestMethod]
    public async Task VeraDataPort()
    {
        var emulator = new Emulator();
        emulator.Headless = true;
        // need this until register writes are added.
        emulator.Vera.Data0_Address = 0x00001;
        emulator.Vera.Data1_Address = 0x00002;
        emulator.Vera.Vram[0x00001] = 0x50;
        emulator.Vera.Vram[0x00002] = 0x50;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sei
                lda #$50
                sta $a002
                sta $a003
                ldy #$ff
                .mainloop:
                ldx #$ff
                .loop:
                dex
                bne loop
                dey
                bne mainloop
                lda DATA0
                tax
                dex
                txa
                sta DATA0
                bne mainloop
                lda DATA1
                tax
                dex
                txa
                sta DATA1
                bne mainloop
                stp
                ",
                emulator);
    }
}
