using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class DEY
{
    [TestMethod]
    public async Task Dey()
    {
        var emulator = new Emulator();

        emulator.Y = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                dey
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x88, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x01, 0x812, 2);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Dey_ToZero()
    {
        var emulator = new Emulator();

        emulator.Y = 1;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                dey
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x88, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Dey_Negative()
    {
        var emulator = new Emulator();

        emulator.Y = 0xa0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                dey
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x88, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x9f, 0x812, 2);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Dey_ToNegative()
    {
        var emulator = new Emulator();

        emulator.Y = 0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                dey
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x88, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0xff, 0x812, 2);
        emulator.AssertFlags(false, true, false, false);
    }
}
