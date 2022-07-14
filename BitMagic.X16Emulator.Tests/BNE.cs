using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class BNE
{
    [TestMethod]
    public async Task Bne_Jump_Forward()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bne exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xd0, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x816, 5); // 3 for bne + 2 for lda
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Bne_NoJump()
    {
        var emulator = new Emulator();

        emulator.Zero = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bne exit
                stp
            .exit:
                lda #$ff
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xd0, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task SpeedTest()
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


    // Uncomment when jmp is implemented
    //[TestMethod]
    //public async Task Bne_Jump_Backward()
    //{
    //    var emulator = new Emulator();

    //    await X16TestHelper.Emulate(@"
    //            .machine CommanderX16R40
    //            .org $810
    //            jmp test
    //        .exit:
    //            lda #$10
    //            stp
    //        .test:
    //            bne exit
    //            stp",
    //            emulator);

    //    // compilation
    //    Assert.AreEqual(0xd0, emulator.Memory[0x810]);

    //    // emulation
    //    emulator.AssertState(0x10, 0x00, 0x00, 0x815, 5); // 3 for bne + 2 for lda
    //    emulator.AssertFlags(false, false, false, false);
    //}
}