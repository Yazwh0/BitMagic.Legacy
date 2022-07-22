using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class EmulatorTests
{
    [TestMethod]
    public async Task CarryFlag()
    {
        var emulator = new Emulator();

        emulator.Carry = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xdb, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x811, 0);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task ZeroFlag()
    {
        var emulator = new Emulator();

        emulator.Zero = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xdb, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x811, 0);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task NegativeFlag()
    {
        var emulator = new Emulator();

        emulator.Negative = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xdb, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x811, 0);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task OverflowFlag()
    {
        var emulator = new Emulator();

        emulator.Overflow = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xdb, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x811, 0);
        emulator.AssertFlags(false, false, true, false);
    }
}