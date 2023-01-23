using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class BIT
{
    [TestMethod]
    public async Task Imm_NoFlags()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit #$01
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x89, emulator.Memory[0x810]);
        Assert.AreEqual(0x01, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x03, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Imm_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit #$01
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Imm_Zero_PreserveCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Carry = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit #$01
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task Imm_Negative()
    {
        var emulator = new Emulator();

        emulator.A = 0x82;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit #$81
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Imm_Overflow()
    {
        var emulator = new Emulator();

        emulator.A = 0x42;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit #$41
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(false, false, true, false);
    }

    [TestMethod]
    public async Task Imm_FlagPreserve()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Decimal = true;
        emulator.Carry = true;
        emulator.InterruptDisable = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit #$01
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(false, false, false, true, true, true);
    }

    [TestMethod]
    public async Task Abs_NoFlags()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Memory[0x1234] = 0x01;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $1234
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x2c, emulator.Memory[0x810]);
        Assert.AreEqual(0x34, emulator.Memory[0x811]);
        Assert.AreEqual(0x12, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x03, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Abs_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x1234] = 0x01;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $1234
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Abs_Negative()
    {
        var emulator = new Emulator();

        emulator.A = 0x82;
        emulator.Memory[0x1234] = 0x81;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $1234
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Abs_Overflow()
    {
        var emulator = new Emulator();

        emulator.A = 0x42;
        emulator.Memory[0x1234] = 0x41;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $1234
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(false, false, true, false);
    }

    [TestMethod]
    public async Task Abs_FlagPreserve()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Decimal = true;
        emulator.Carry = true;
        emulator.InterruptDisable = true;
        emulator.Memory[0x1234] = 0x01;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $1234
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(false, false, false, true, true, true);
    }


    [TestMethod]
    public async Task Abs_FromRom()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x1234] = 0x01;
        emulator.RomBank[0x0000] = 0x2c;
        emulator.RomBank[0x0001] = 0x34;
        emulator.RomBank[0x0002] = 0x12;
        emulator.RomBank[0x0003] = 0xdb;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $c000
                ",
                emulator);

        // emulation
        emulator.AssertState(0x02, 0x00, 0x00, 0xc004);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task AbsX_NoFlags()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Memory[0x1234] = 0x01;
        emulator.X = 0x04;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $1230, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x3c, emulator.Memory[0x810]);
        Assert.AreEqual(0x30, emulator.Memory[0x811]);
        Assert.AreEqual(0x12, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x03, 0x04, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsX_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x1234] = 0x01;
        emulator.X = 0x04;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $1230, x
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task AbsX_Negative()
    {
        var emulator = new Emulator();

        emulator.A = 0x82;
        emulator.Memory[0x1234] = 0x81;
        emulator.X = 0x04;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $1230, x
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task AbsX_Overflow()
    {
        var emulator = new Emulator();

        emulator.A = 0x42;
        emulator.Memory[0x1234] = 0x41;
        emulator.X = 0x04;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $1230, x
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(false, false, true, false);
    }

    [TestMethod]
    public async Task AbsX_FlagPreserve()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Decimal = true;
        emulator.Carry = true;
        emulator.InterruptDisable = true;
        emulator.Memory[0x1234] = 0x01;
        emulator.X = 0x04;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $1230, x
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(false, false, false, true, true, true);
    }

    [TestMethod]
    public async Task Zp_NoFlags()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Memory[0x12] = 0x01;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $12
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x24, emulator.Memory[0x810]);
        Assert.AreEqual(0x12, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x03, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Zp_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x12] = 0x01;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $12
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Zp_Negative()
    {
        var emulator = new Emulator();

        emulator.A = 0x82;
        emulator.Memory[0x12] = 0x81;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $12
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Zp_Overflow()
    {
        var emulator = new Emulator();

        emulator.A = 0x42;
        emulator.Memory[0x12] = 0x41;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $12
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(false, false, true, false);
    }

    [TestMethod]
    public async Task Zp_FlagPreserve()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Decimal = true;
        emulator.Carry = true;
        emulator.InterruptDisable = true;
        emulator.Memory[0x12] = 0x01;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $12
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(false, false, false, true, true, true);
    }

    [TestMethod]
    public async Task ZpX_NoFlags()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Memory[0x12] = 0x01;
        emulator.X = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $10, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x34, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x03, 0x02, 0x00, 0x813, 3);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZpX_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x12] = 0x01;
        emulator.X = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $10, x
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task ZpX_Negative()
    {
        var emulator = new Emulator();

        emulator.A = 0x82;
        emulator.Memory[0x12] = 0x81;
        emulator.X = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $10, x
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task ZpX_Overflow()
    {
        var emulator = new Emulator();

        emulator.A = 0x42;
        emulator.Memory[0x12] = 0x41;
        emulator.X = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $10, x
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(false, false, true, false);
    }

    [TestMethod]
    public async Task ZpX_FlagPreserve()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Decimal = true;
        emulator.Carry = true;
        emulator.InterruptDisable = true;
        emulator.Memory[0x12] = 0x01;
        emulator.X = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bit $10, x
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(false, false, false, true, true, true);
    }
}