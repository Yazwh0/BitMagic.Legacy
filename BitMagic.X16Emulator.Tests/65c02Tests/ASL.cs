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

    [TestMethod]
    public async Task Abs()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b00000010;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                asl $1234
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x0e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000100, emulator.Memory[0x1234]);

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
                asl $1234
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x0e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000100, emulator.Memory[0x1234]);

        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Abs_SetCarry()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b10000010;
        emulator.Carry = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                asl $1234
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x0e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000100, emulator.Memory[0x1234]);

        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 6);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task AbsX()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b00000010;
        emulator.X = 4;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                asl $1230, X
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x1e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000100, emulator.Memory[0x1234]);

        emulator.AssertState(0x00, 0x04, 0x00, 0x814, 7);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsX_CarrySet()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b00000010;
        emulator.Carry = true;
        emulator.X = 4;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                asl $1230, X
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x1e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000100, emulator.Memory[0x1234]);

        emulator.AssertState(0x00, 0x04, 0x00, 0x814, 7);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsX_SetCarry()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0b10000010;
        emulator.Carry = false;
        emulator.X = 4;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                asl $1230, X
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x1e, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000100, emulator.Memory[0x1234]);

        emulator.AssertState(0x00, 0x04, 0x00, 0x814, 7);
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
                asl $12
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x06, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000100, emulator.Memory[0x12]);

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
                asl $12
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x06, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000100, emulator.Memory[0x12]);

        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Zp_SetCarry()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0b10000010;
        emulator.Carry = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                asl $12
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x06, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000100, emulator.Memory[0x12]);

        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Zpx()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0b00000010;
        emulator.X = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                asl $10, X
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x16, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000100, emulator.Memory[0x12]);

        emulator.AssertState(0x00, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Zpx_Wrap()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0b00000010;
        emulator.X = 0x72;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                asl $a0, X
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x16, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000100, emulator.Memory[0x12]);

        emulator.AssertState(0x00, 0x72, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Zpx_CarrySet()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0b00000010;
        emulator.Carry = true;
        emulator.X = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                asl $10, X
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x16, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000100, emulator.Memory[0x12]);

        emulator.AssertState(0x00, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Zpx_SetCarry()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0b10000010;
        emulator.Carry = false;
        emulator.X = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                asl $10, X
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x16, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000100, emulator.Memory[0x12]);

        emulator.AssertState(0x00, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Readonly_Abs()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0000] = 0b00000010;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                asl $c000
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
                asl $c000
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
                asl $c000, x
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
                asl $c000, x
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0b10000001, emulator.RomBank[0x0002]);

        emulator.AssertFlags(false, false, false, true);
    }
}