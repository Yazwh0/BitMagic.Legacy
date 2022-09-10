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

        // NOP is 2 cycles, so should be 6 pixels on.
        Assert.AreEqual(6, emulator.Vera.Beam_X);
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

        // NOP is 2 cycles, so should be 2 * 3 * 3.125 = 18 pixels on.
        Assert.AreEqual(18, emulator.Vera.Beam_X);
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

        // NOP is 2 cycles, so should be 2 * 4 * 3.125 = 25 pixels on. First 'extra' pixel
        Assert.AreEqual(25, emulator.Vera.Beam_X);
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

        // NOP is 2 cycles, so should be 2 * 5 * 3.125 = 31 pixels on.
        Assert.AreEqual(31, emulator.Vera.Beam_X);
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

        // NOP is 2 cycles, so should be 10 * 2 * 3.125 = 63 pixels on. 
        Assert.AreEqual(62, emulator.Vera.Beam_X);
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

        // NOP is 2 cycles, so should be 20 * 2 * 3.125 = 125 pixels on.
        Assert.AreEqual(125, emulator.Vera.Beam_X);
        Assert.AreEqual(0, emulator.Vera.Beam_Y);
    }
}