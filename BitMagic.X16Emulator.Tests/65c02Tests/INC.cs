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
    public async Task A_PreserveFlags()
    {
        var emulator = new Emulator();

        emulator.A = 2;
        emulator.InterruptDisable = true;
        emulator.Carry = true;
        emulator.Overflow = true;
        emulator.Decimal = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc
                stp",
                emulator);

        emulator.AssertFlags(false, false, true, true, true, true);
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
    public async Task A_Negative()
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
    public async Task Abs()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc $1234
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xee, emulator.Memory[0x810]);
        Assert.AreEqual(0x34, emulator.Memory[0x811]);
        Assert.AreEqual(0x12, emulator.Memory[0x812]);

        // emulation
        Assert.AreEqual(0x03, emulator.Memory[0x1234]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Abs_RomToRam()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0x02;

        emulator.RomBank[0x0000] = 0xee;
        emulator.RomBank[0x0001] = 0x34;
        emulator.RomBank[0x0002] = 0x12;
        emulator.RomBank[0x0003] = 0xdb;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $c000",
                emulator);

        // emulation
        Assert.AreEqual(0x03, emulator.Memory[0x1234]);
        emulator.AssertState(0x00, 0x00, 0x00, 0xc004);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Abs_RomToBRam()
    {
        var emulator = new Emulator();

        emulator.RamBank[0x0234] = 0x02;

        emulator.RomBank[0x0000] = 0xee;
        emulator.RomBank[0x0001] = 0x34;
        emulator.RomBank[0x0002] = 0xa2;
        emulator.RomBank[0x0003] = 0xdb;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $c000",
                emulator);

        // emulation
        Assert.AreEqual(0x03, emulator.RamBank[0x0234]);
        emulator.AssertState(0x00, 0x00, 0x00, 0xc004);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Abs_PreserveFlags()
    {
        var emulator = new Emulator();

        emulator.A = 2;
        emulator.InterruptDisable = true;
        emulator.Carry = true;
        emulator.Overflow = true;
        emulator.Decimal = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc $1234
                stp",
                emulator);

        emulator.AssertFlags(false, false, true, true, true, true);
    }

    [TestMethod]
    public async Task Abs_ToZero()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc $1234
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0x00, emulator.Memory[0x1234]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 6);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Abs_Negative()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0xa0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc $1234
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0xa1, emulator.Memory[0x1234]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 6);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task AbsX()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0x02;
        emulator.X = 4;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc $1230, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xfe, emulator.Memory[0x810]);
        Assert.AreEqual(0x30, emulator.Memory[0x811]);
        Assert.AreEqual(0x12, emulator.Memory[0x812]);

        // emulation
        Assert.AreEqual(0x03, emulator.Memory[0x1234]);
        emulator.AssertState(0x00, 0x04, 0x00, 0x814, 7);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsX_PreserveFlags()
    {
        var emulator = new Emulator();

        emulator.A = 2;
        emulator.X = 4;
        emulator.InterruptDisable = true;
        emulator.Carry = true;
        emulator.Overflow = true;
        emulator.Decimal = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc $1230, x
                stp",
                emulator);

        emulator.AssertFlags(false, false, true, true, true, true);
    }

    [TestMethod]
    public async Task AbsX_ToZero()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0xff;
        emulator.X = 4;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc $1230, x
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0x00, emulator.Memory[0x1234]);
        emulator.AssertState(0x00, 0x04, 0x00, 0x814, 7);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task AbsX_Negative()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1234] = 0xa0;
        emulator.X = 4;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc $1230, x
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0xa1, emulator.Memory[0x1234]);
        emulator.AssertState(0x00, 0x04, 0x00, 0x814, 7);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Zp()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc $12
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xe6, emulator.Memory[0x810]);
        Assert.AreEqual(0x12, emulator.Memory[0x811]);

        // emulation
        Assert.AreEqual(0x03, emulator.Memory[0x12]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Zp_PreserveFlags()
    {
        var emulator = new Emulator();

        emulator.A = 2;
        emulator.InterruptDisable = true;
        emulator.Carry = true;
        emulator.Overflow = true;
        emulator.Decimal = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc $12
                stp",
                emulator);

        emulator.AssertFlags(false, false, true, true, true, true);
    }

    [TestMethod]
    public async Task Zp_ToZero()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc $12
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0x00, emulator.Memory[0x12]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Zp_Negative()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0xa0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc $12
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0xa1, emulator.Memory[0x12]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task ZpX()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0x02;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc $10,x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xf6, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        Assert.AreEqual(0x03, emulator.Memory[0x12]);
        emulator.AssertState(0x00, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZpX_PreserveFlags()
    {
        var emulator = new Emulator();

        emulator.X = 0x02;
        emulator.InterruptDisable = true;
        emulator.Carry = true;
        emulator.Overflow = true;
        emulator.Decimal = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc $10,x
                stp",
                emulator);

        emulator.AssertFlags(false, false, true, true, true, true);
    }

    [TestMethod]
    public async Task ZpX_ToZero()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0xff;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc $10,x
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0x00, emulator.Memory[0x12]);
        emulator.AssertState(0x00, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task ZpX_Negative()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0xa0;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc $10,x
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0xa1, emulator.Memory[0x12]);
        emulator.AssertState(0x00, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task ReadOnly_Abs()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0000] = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc $c000
                stp",
                emulator);

        Assert.AreEqual(0x10, emulator.RomBank[0x0000]);
    }

    [TestMethod]
    public async Task ReadOnly_AbsX()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0002] = 0x10;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc $c000, x
                stp",
                emulator);

        Assert.AreEqual(0x10, emulator.RomBank[0x0002]);
    }
}
