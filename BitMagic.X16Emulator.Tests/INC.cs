using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class INC
{
    [TestMethod]
    public async Task A()
    {
        var emulator = new Emulator();

        emulator.A = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x1a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x03, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task A_ToZero()
    {
        var emulator = new Emulator();

        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x1a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Inc_Negative()
    {
        var emulator = new Emulator();

        emulator.A = 0xa0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x1a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0xa1, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task A_ToNegative()
    {
        var emulator = new Emulator();

        emulator.A = 0xa0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x1a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0xa1, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, true, false, false);
    }
}
