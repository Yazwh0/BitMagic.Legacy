using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class ADC
{
    [TestMethod]
    public async Task Imm()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc #$03
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x69, emulator.Memory[0x810]);
        Assert.AreEqual(0x03, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x05, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Imm_WithCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Carry = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc #$03
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x06, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Imm_DoNothing()
    {
        var emulator = new Emulator();

        emulator.A = 0x0;
        emulator.Carry = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc #0
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Imm_ZeroCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc #$fe
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task Imm_NegativeOverflow()
    {
        var emulator = new Emulator();

        emulator.A = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc #$70
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x82, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(false, true, true, false);
    }

    [TestMethod]
    public async Task Abs()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x1234] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $1234
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x6d, emulator.Memory[0x810]);
        Assert.AreEqual(0x34, emulator.Memory[0x811]);
        Assert.AreEqual(0x12, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x05, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Abs_DoNothing()
    {
        var emulator = new Emulator();

        emulator.A = 0x00;
        emulator.Memory[0x1234] = 0x00;
        emulator.Carry = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $1234
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Abs_FromRom()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x1234] = 0x03;

        emulator.RomBank[0x0000] = 0x6d;
        emulator.RomBank[0x0001] = 0x34;
        emulator.RomBank[0x0002] = 0x12;
        emulator.RomBank[0x0003] = 0xdb;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $c000",
                emulator);

        // emulation
        emulator.AssertState(0x05, 0x00, 0x00, 0xc004);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Abs_WithCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $1234
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x06, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Abs_ZeroCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x1234] = 0xfe;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $1234
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task Abs_NegativeOverflow()
    {
        var emulator = new Emulator();

        emulator.A = 0x12;
        emulator.Memory[0x1234] = 0x70;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $1234
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x82, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(false, true, true, false);
    }

    [TestMethod]
    public async Task AbsX()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.X = 0x04;
        emulator.Memory[0x1234] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $1230, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x7d, emulator.Memory[0x810]);
        Assert.AreEqual(0x30, emulator.Memory[0x811]);
        Assert.AreEqual(0x12, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x05, 0x04, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsX_PageChange()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.X = 0x44;
        emulator.Memory[0x1234] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $11f0, x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x05, 0x44, 0x00, 0x814, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsX_WithCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.X = 0x04;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $1230, x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x06, 0x04, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsX_ZeroCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.X = 0x04;
        emulator.Memory[0x1234] = 0xfe;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $1230, x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x04, 0x00, 0x814, 4);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task AbsX_NegativeOverflow()
    {
        var emulator = new Emulator();

        emulator.A = 0x12;
        emulator.X = 0x04;
        emulator.Memory[0x1234] = 0x70;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $1230, x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x82, 0x04, 0x00, 0x814, 4);
        emulator.AssertFlags(false, true, true, false);
    }

    [TestMethod]
    public async Task AbsY()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Y = 0x04;
        emulator.Memory[0x1234] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $1230, y
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x79, emulator.Memory[0x810]);
        Assert.AreEqual(0x30, emulator.Memory[0x811]);
        Assert.AreEqual(0x12, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x05, 0x00, 0x04, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsY_PageChange()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Y = 0x44;
        emulator.Memory[0x1234] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $11f0, y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x05, 0x00, 0x44, 0x814, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsY_WithCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Y = 0x04;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $1230, y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x06, 0x00, 0x04, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsY_ZeroCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Y = 0x04;
        emulator.Memory[0x1234] = 0xfe;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $1230, y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x04, 0x814, 4);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task AbsY_NegativeOverflow()
    {
        var emulator = new Emulator();

        emulator.A = 0x12;
        emulator.Y = 0x04;
        emulator.Memory[0x1234] = 0x70;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $1230, y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x82, 0x00, 0x04, 0x814, 4);
        emulator.AssertFlags(false, true, true, false);
    }

    [TestMethod]
    public async Task Zp()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x12] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $12
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x65, emulator.Memory[0x810]);
        Assert.AreEqual(0x12, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x05, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Zp_WithCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $12
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x06, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Zp_ZeroCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x12] = 0xfe;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $12
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task Zp_NegativeOverflow()
    {
        var emulator = new Emulator();

        emulator.A = 0x12;
        emulator.Memory[0x12] = 0x70;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $12
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x82, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(false, true, true, false);
    }

    [TestMethod]
    public async Task ZpX()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x12] = 0x03;
        emulator.X = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $10, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x75, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x05, 0x02, 0x00, 0x813, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZpX_WithCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0x03;
        emulator.X = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $10, x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x06, 0x02, 0x00, 0x813, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZpX_ZeroCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x12] = 0xfe;
        emulator.X = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $10, x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x02, 0x00, 0x813, 4);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task ZpX_NegativeOverflow()
    {
        var emulator = new Emulator();

        emulator.A = 0x12;
        emulator.Memory[0x12] = 0x70;
        emulator.X = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $10, x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x82, 0x02, 0x00, 0x813, 4);
        emulator.AssertFlags(false, true, true, false);
    }

    [TestMethod]
    public async Task ZpX_Wrap()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x12] = 0x03;
        emulator.X = 0x72;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc $a0, x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x05, 0x72, 0x00, 0x813, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Ind()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x1234] = 0x03;
        emulator.Memory[0x10] = 0x34;
        emulator.Memory[0x11] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc ($10)
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x72, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x05, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Ind_WithCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0x03;
        emulator.Memory[0x10] = 0x34;
        emulator.Memory[0x11] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc ($10)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x06, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Ind_ZeroCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x1234] = 0xfe;
        emulator.Memory[0x10] = 0x34;
        emulator.Memory[0x11] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc ($10)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task Ind_NegativeOverflow()
    {
        var emulator = new Emulator();

        emulator.A = 0x12;
        emulator.Memory[0x1234] = 0x70;
        emulator.Memory[0x10] = 0x34;
        emulator.Memory[0x11] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc ($10)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x82, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, true, true, false);
    }

    [TestMethod]
    public async Task IndX()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.X = 0x01;
        emulator.Memory[0x1234] = 0x03;
        emulator.Memory[0x10] = 0x34;
        emulator.Memory[0x11] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc ($0f, x)
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x61, emulator.Memory[0x810]);
        Assert.AreEqual(0x0f, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x05, 0x01, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task IndX_Wrap()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x1234] = 0x03;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;
        emulator.X = 0x72;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc ($a0, x)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x05, 0x72, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task IndX_WithCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.X = 0x01;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0x03;
        emulator.Memory[0x10] = 0x34;
        emulator.Memory[0x11] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc ($0f, x)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x06, 0x01, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task IndX_ZeroCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.X = 0x01;
        emulator.Memory[0x1234] = 0xfe;
        emulator.Memory[0x10] = 0x34;
        emulator.Memory[0x11] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc ($0f, x)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x01, 0x00, 0x813, 6);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task IndX_NegativeOverflow()
    {
        var emulator = new Emulator();

        emulator.A = 0x12;
        emulator.X = 0x01;
        emulator.Memory[0x1234] = 0x70;
        emulator.Memory[0x10] = 0x34;
        emulator.Memory[0x11] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc ($0f, x)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x82, 0x01, 0x00, 0x813, 6);
        emulator.AssertFlags(false, true, true, false);
    }


    [TestMethod]
    public async Task IndY()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Y = 0x04;
        emulator.Memory[0x1234] = 0x03;
        emulator.Memory[0x10] = 0x30;
        emulator.Memory[0x11] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc ($10), y
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x71, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x05, 0x00, 0x04, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task IndY_Page()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x1234] = 0x03;
        emulator.Memory[0x10] = 0xf0;
        emulator.Memory[0x11] = 0x11;
        emulator.Y = 0x44;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc ($10),y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x05, 0x00, 0x44, 0x813, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task IndY_WithCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Y = 0x04;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0x03;
        emulator.Memory[0x10] = 0x30;
        emulator.Memory[0x11] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc ($0f), y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x03, 0x00, 0x04, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task IndY_ZeroCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Y = 0x04;
        emulator.Memory[0x1234] = 0xfe;
        emulator.Memory[0x10] = 0x30;
        emulator.Memory[0x11] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc ($10), y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x04, 0x813, 5);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task IndY_NegativeOverflow()
    {
        var emulator = new Emulator();

        emulator.A = 0x12;
        emulator.Y = 0x04;
        emulator.Memory[0x1234] = 0x70;
        emulator.Memory[0x10] = 0x30;
        emulator.Memory[0x11] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                adc ($10), y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x82, 0x00, 0x04, 0x813, 5);
        emulator.AssertFlags(false, true, true, false);
    }
}