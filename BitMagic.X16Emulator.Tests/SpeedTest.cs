using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class SpeedTest
{
    [TestMethod]

    public async Task Test()
    {

        var emulator = new Emulator();

        var stopWatch = new Stopwatch();

        stopWatch.Start();

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

        stopWatch.Stop();

        var ts = stopWatch.Elapsed;

        Console.WriteLine(String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10));

        Console.WriteLine($"Clock Ticks: {emulator.Clock:X4}");

        //emulator.AssertFlags(true, false, false, false);
    }
}
