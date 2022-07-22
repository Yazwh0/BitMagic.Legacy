using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class LSR
{
    [TestMethod]
    public async Task A()
    {
        var emulator = new Emulator();

        emulator.A = 0b00000010;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x4a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0b00000001, 0x00, 0x00, 0x812, 2);
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
                lsr
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x4a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0b00000001, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task A_SetCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0b00000011;
        emulator.Carry = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x4a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0b00000001, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task A_ShiftZero_SetCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0b00000001;
        emulator.Carry = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x4a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0b00000000, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(true, false, false, true);
    }
}