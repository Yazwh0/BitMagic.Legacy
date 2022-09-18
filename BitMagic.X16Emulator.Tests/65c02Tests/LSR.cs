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
    public async Task A_OverflowPreserve()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000010;
        emulator.Overflow = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x4a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0b01000001, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, false, true, false);
    }

    [TestMethod]
    public async Task A_NegativeReset()
    {
        var emulator = new Emulator();

        emulator.A = 0b00000010;
        emulator.Negative = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr
                stp",
                emulator);

        emulator.AssertState(0x01);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task A_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0b00000000;
        emulator.Negative = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr
                stp",
                emulator);

        emulator.AssertState(0x00);
        emulator.AssertFlags(true, false, false, false);
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

    [TestMethod]
    public async Task Abs()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b00000010;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $1234
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x4e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000001, emulator.Memory[0x1234]);

        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Abs_NegativeReset()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b00000010;
        emulator.Negative = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $1234
                stp",
                emulator);

        emulator.AssertFlags(false, false, false, false);
    }


    [TestMethod]
    public async Task Abs_OverflowPreserve()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b10000010;
        emulator.Overflow = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $1234
                stp",
                emulator);

        emulator.AssertFlags(false, false, true, false);
    }

    [TestMethod]
    public async Task Abs_Zero()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b00000000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $1234
                stp",
                emulator);

        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Abs_CarrySet()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b00000010;
        emulator.Carry = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $1234
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x4e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000001, emulator.Memory[0x1234]);

        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Abs_SetCarry()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b00000011;
        emulator.Carry = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $1234
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x4e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000001, emulator.Memory[0x1234]);

        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 6);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Abs_ShiftZero_SetCarry()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b00000001;
        emulator.Carry = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $1234
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x4e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000000, emulator.Memory[0x1234]);

        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 6);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task AbsX()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b00000010;
        emulator.X = 0x04;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $1230, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x5e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000001, emulator.Memory[0x1234]);

        emulator.AssertState(0x00, 0x04, 0x00, 0x814, 7);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsX_CarrySet()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b00000010;
        emulator.Carry = true;
        emulator.X = 0x04;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $1230, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x5e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000001, emulator.Memory[0x1234]);

        emulator.AssertState(0x00, 0x04, 0x00, 0x814, 7);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsX_SetCarry()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b00000011;
        emulator.Carry = false;
        emulator.X = 0x04;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $1230, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x5e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000001, emulator.Memory[0x1234]);

        emulator.AssertState(0x00, 0x04, 0x00, 0x814, 7);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task AbsX_ShiftZero_SetCarry()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b00000001;
        emulator.Carry = false;
        emulator.X = 0x04;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $1230, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x5e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000000, emulator.Memory[0x1234]);

        emulator.AssertState(0x00, 0x04, 0x00, 0x814, 7);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task Zp()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0b00000010;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $12
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x46, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000001, emulator.Memory[0x12]);

        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Zp_CarrySet()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0b00000010;
        emulator.Carry = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $12
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x46, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000001, emulator.Memory[0x12]);

        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Zp_SetCarry()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0b00000011;
        emulator.Carry = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $12
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x46, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000001, emulator.Memory[0x12]);

        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Zp_ShiftZero_SetCarry()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0b00000001;
        emulator.Carry = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $12
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x46, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000000, emulator.Memory[0x12]);

        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task ZpX()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0b00000010;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $10, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x56, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000001, emulator.Memory[0x12]);

        emulator.AssertState(0x00, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZpX_Wrap()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0b00000010;
        emulator.X = 0x72;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $a0, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x56, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000001, emulator.Memory[0x12]);

        emulator.AssertState(0x00, 0x72, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZpX_CarrySet()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0b00000010;
        emulator.Carry = true;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $10, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x56, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000001, emulator.Memory[0x12]);

        emulator.AssertState(0x00, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZpX_SetCarry()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0b00000011;
        emulator.Carry = false;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $10, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x56, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000001, emulator.Memory[0x12]);

        emulator.AssertState(0x00, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task ZpX_ShiftZero_SetCarry()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0b00000001;
        emulator.Carry = false;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $10, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x56, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000000, emulator.Memory[0x12]);

        emulator.AssertState(0x00, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task Readonly_Abs()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0000] = 0b00000010;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $c000
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0b00000010, emulator.RomBank[0x0000]);

        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Readonly_AbsSet()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0000] = 0b10000001;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $c000
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0b10000001, emulator.RomBank[0x0000]);

        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Readonly_AbsX()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0002] = 0b00000010;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $c000, x
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0b00000010, emulator.RomBank[0x0002]);

        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Readonly_AbsXSet()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0002] = 0b10000001;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lsr $c000, x
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0b10000001, emulator.RomBank[0x0002]);

        emulator.AssertFlags(false, false, false, true);
    }
}