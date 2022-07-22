using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class ASL
{
    [TestMethod]
    public async Task A()
    {
        var emulator = new Emulator();

        emulator.A = 0b00000010;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                asl
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x0a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0b00000100, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task A_CarrySet()
    {
        var emulator = new Emulator();

        emulator.A = 0b00000010;
        emulator.Carry = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                asl
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x0a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0b00000100, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task A_SetCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000010;
        emulator.Carry = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                asl
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x0a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0b00000100, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, false, false, true);
    }
}