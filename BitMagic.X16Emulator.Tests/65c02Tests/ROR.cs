using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class ROR
{
    [TestMethod]
    public async Task A()
    {
        var emulator = new Emulator();

        emulator.A = 0b00000010;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ror
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x6a, emulator.Memory[0x810]);

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
                ror
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x6a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0b10000001, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task A_SetCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000001;
        emulator.Carry = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ror
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x6a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0b01000000, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Abs()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b00000010;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ror $1234
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x6e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000001, emulator.Memory[0x1234]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 6);
        emulator.AssertFlags(false, false, false, false);
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
                ror $1234
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x6e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b10000001, emulator.Memory[0x1234]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 6);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Abs_SetCarry()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b10000001;
        emulator.Carry = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ror $1234
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x6e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b01000000, emulator.Memory[0x1234]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 6);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task AbsX()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b00000010;
        emulator.X = 0x34;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ror $1200, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x7e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000001, emulator.Memory[0x1234]);
        emulator.AssertState(0x00, 0x34, 0x00, 0x814, 7);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsX_PageChange()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1310] = 0b00000010;
        emulator.X = 0x11;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ror $12ff, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x7e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000001, emulator.Memory[0x1310]);
        emulator.AssertState(0x00, 0x11, 0x00, 0x814, 7);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsX_CarrySet()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b10000001;
        emulator.Carry = true;
        emulator.X = 0x34;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ror $1200, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x7e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b11000000, emulator.Memory[0x1234]);
        emulator.AssertState(0x00, 0x34, 0x00, 0x814, 7);
        emulator.AssertFlags(false, true, false, true);
    }

    [TestMethod]
    public async Task AbsX_SetCarry()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b10000001;
        emulator.Carry = false;
        emulator.X = 0x34;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ror $1200, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x7e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b01000000, emulator.Memory[0x1234]);
        emulator.AssertState(0x00, 0x34, 0x00, 0x814, 7);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Zp()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0b00000010;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ror $12
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x66, emulator.Memory[0x810]);

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
                ror $12
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x66, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b10000001, emulator.Memory[0x12]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Zp_SetCarry()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0b10000001;
        emulator.Carry = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ror $12
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x66, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b01000000, emulator.Memory[0x12]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, true);
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
                ror $10, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x76, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000001, emulator.Memory[0x12]);
        emulator.AssertState(0x00, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZpX_CarrySet()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0b10000010;
        emulator.Carry = true;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ror $10, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x76, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b11000001, emulator.Memory[0x12]);
        emulator.AssertState(0x00, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task ZpX_SetCarry()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0b10000001;
        emulator.Carry = false;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ror $10, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x76, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b01000000, emulator.Memory[0x12]);
        emulator.AssertState(0x00, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task ZpX_Wrap()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0b10000010;
        emulator.Carry = false;
        emulator.X = 0x72;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ror $a0, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x76, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b01000001, emulator.Memory[0x12]);
        emulator.AssertState(0x00, 0x72, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Readonly_Abs()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0000] = 0b00000010;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ror $c000
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
                ror $c000
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
                ror $c000, x
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
                ror $c000, x
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0b10000001, emulator.RomBank[0x0002]);

        emulator.AssertFlags(false, false, false, true);
    }
}