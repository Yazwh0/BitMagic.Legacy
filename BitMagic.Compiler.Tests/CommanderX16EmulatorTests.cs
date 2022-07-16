using Microsoft.VisualStudio.TestTools.UnitTesting;
using BitMagic.Compiler;
using BitMagic.Emulation;
using System.Diagnostics;

namespace BitMagic.Compiler.Tests;

// These are considered obsolete, as we move to the new Emulator

[TestClass]
public class CommanderX16EmulatorTests
{
    [TestMethod]
    public async Task Lda_Immediate()
    {
        var result = await CommanderX16Test.UntilStp(@"
                .machine CommanderX16R40
                .org $810
                lda #$ff
                stp
                ");

        // compilation
        Assert.AreEqual(0xa9, result.Cpu.Memory.PeekByte(0x810));
        Assert.AreEqual(0xff, result.Cpu.Memory.PeekByte(0x811));

        // emulation
        Assert.AreEqual(0xff, result.Cpu.Registers.A);
        Assert.AreEqual(0x813, result.Cpu.Registers.PC);
    }

    [TestMethod]
    public async Task Ldx_Immediate()
    {
        var result = await CommanderX16Test.UntilStp(@"
                .machine CommanderX16R40
                .org $810
                ldx #$ff
                stp
                ");

        // compilation
        Assert.AreEqual(0xa2, result.Cpu.Memory.PeekByte(0x810));
        Assert.AreEqual(0xff, result.Cpu.Memory.PeekByte(0x811));

        // emulation
        Assert.AreEqual(0xff, result.Cpu.Registers.X);
        Assert.AreEqual(0x813, result.Cpu.Registers.PC);
    }

    [TestMethod]
    public async Task Ldy_Immediate()
    {
        var result = await CommanderX16Test.UntilStp(@"
                .machine CommanderX16R40
                .org $810
                ldy #$ff
                stp
                ");

        // compilation
        Assert.AreEqual(0xa0, result.Cpu.Memory.PeekByte(0x810));
        Assert.AreEqual(0xff, result.Cpu.Memory.PeekByte(0x811));

        // emulation
        Assert.AreEqual(0xff, result.Cpu.Registers.Y);
        Assert.AreEqual(0x813, result.Cpu.Registers.PC);
    }

    //[TestMethod]
    //public async Task SpeedTest()
    //{

    //    var stopWatch = new Stopwatch();

    //    stopWatch.Start();

    //    var result = await CommanderX16Test.UntilStp(@"
    //            .machine CommanderX16R40
    //            .org $810
    //            lda #$50
    //            sta $02
    //            sta $03
    //            ldy #$ff
    //            .mainloop:
    //            ldx #$ff
    //            .loop:
    //            dex
    //            bne loop
    //            dey
    //            bne mainloop
    //            lda $02
    //            tax
    //            dex
    //            txa
    //            sta $02
    //            bne mainloop
    //            lda $03
    //            tax
    //            dex
    //            txa
    //            sta $03
    //            bne mainloop
    //            stp
    //            "
    //            );

    //    stopWatch.Stop();

    //    var ts = stopWatch.Elapsed;

    //    Console.WriteLine(String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
    //        ts.Hours, ts.Minutes, ts.Seconds,
    //        ts.Milliseconds / 10));

    //  // Console.WriteLine($"Clock Ticks: {emulator.Clock:X4}");

    //    //emulator.AssertFlags(true, false, false, false);
    //}
}