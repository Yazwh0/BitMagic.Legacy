using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class DEC
{
    [TestMethod]
    public async Task A()
    {
        var emulator = new Emulator();

        emulator.A = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                dec
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x3a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x01, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task A_ToZero()
    {
        var emulator = new Emulator();

        emulator.A = 1;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                dec
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x3a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task A_Negative()
    {
        var emulator = new Emulator();

        emulator.A = 0xa0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                dec
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x3a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x9f, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task A_ToNegative()
    {
        var emulator = new Emulator();

        emulator.A = 0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                dec
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x3a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0xff, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, true, false, false);
    }
}
