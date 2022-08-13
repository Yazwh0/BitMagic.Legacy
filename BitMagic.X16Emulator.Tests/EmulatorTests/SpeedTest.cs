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

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
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

    public async Task BankedRamp()
    {

        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
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
}
