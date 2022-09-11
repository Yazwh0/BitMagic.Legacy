using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class Beam
{
    [TestMethod]
    public async Task Movement_Nop()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                nop
                stp",
                emulator);

        // NOP is 2+3 cycles, so should be 5 * 3.125 = 15 pixels on.
        Assert.AreEqual(15, emulator.Vera.Beam_X);
        Assert.AreEqual(0, emulator.Vera.Beam_Y);
    }

    [TestMethod]
    public async Task Movement_3Nops()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                nop
                nop
                nop
                stp",
                emulator);

        // NOP is 2 cycles, so should be (2 * 3 + 3) * 3.125 = 28 pixels on.
        Assert.AreEqual(28, emulator.Vera.Beam_X);
        Assert.AreEqual(0, emulator.Vera.Beam_Y);
    }

    [TestMethod]
    public async Task Movement_4Nops()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                nop
                nop
                nop
                nop
                stp",
                emulator);

        // NOP is 2 cycles, so should be (2 * 4 + 3) * 3.125 = 34 pixels on. First 'extra' pixel
        Assert.AreEqual(34, emulator.Vera.Beam_X);
        Assert.AreEqual(0, emulator.Vera.Beam_Y);
    }

    [TestMethod]
    public async Task Movement_5Nops()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                nop
                nop
                nop
                nop
                nop
                stp",
                emulator);

        // NOP is 2 cycles, so should be (2 * 5 + 3) * 3.125 = 40 pixels on.
        Assert.AreEqual(40, emulator.Vera.Beam_X);
        Assert.AreEqual(0, emulator.Vera.Beam_Y);
    }

    [TestMethod]
    public async Task Movement_10Nops()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                nop
                nop
                nop
                nop
                nop
                nop
                nop
                nop
                nop
                nop
                stp",
                emulator);

        // NOP is 2 cycles, so should be (10 * 2 + 3)* 3.125 = 71 pixels on. 
        Assert.AreEqual(71, emulator.Vera.Beam_X);
        Assert.AreEqual(0, emulator.Vera.Beam_Y);
    }

    [TestMethod]
    public async Task Movement_20Nops()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                nop
                nop
                nop
                nop
                nop
                nop
                nop
                nop
                nop
                nop

                nop
                nop
                nop
                nop
                nop
                nop
                nop
                nop
                nop
                nop
                stp",
                emulator);

        // NOP is 2 cycles, so should be (20 * 2 + 3) * 3.125 = 134 pixels on.
        Assert.AreEqual(134, emulator.Vera.Beam_X);
        Assert.AreEqual(0, emulator.Vera.Beam_Y);
    }
}