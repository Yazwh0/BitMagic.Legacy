using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class SBC
{
    [TestMethod]
    public async Task Imm()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Carry = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc #$02
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xe9, emulator.Memory[0x810]);
        Assert.AreEqual(0x02, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x01, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Imm_NoCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x04;
        emulator.Carry = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc #$02
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x01, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Imm_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Carry = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc #$03
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task Imm_NoCarry_FromZero()
    {
        var emulator = new Emulator();

        emulator.A = 0x00;
        emulator.Carry = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc #$00
                stp",
                emulator);

        // emulation
        emulator.AssertState(0xff, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Imm_Carry_FromZero()
    {
        var emulator = new Emulator();

        emulator.A = 0x00;
        emulator.Carry = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc #$00
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task Imm_NegativeCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Carry = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc #$03
                stp",
                emulator);

        // emulation
        emulator.AssertState(0xff, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Imm_Overflow()
    {
        var emulator = new Emulator();

        emulator.A = 0xa0;
        emulator.Carry = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc #$20
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x7f, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(false, false, true, true);
    }

    [TestMethod]
    public async Task Abs()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $1234
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xed, emulator.Memory[0x810]);
        Assert.AreEqual(0x34, emulator.Memory[0x811]);
        Assert.AreEqual(0x12, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x01, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Abs_NoCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x04;
        emulator.Carry = false;
        emulator.Memory[0x1234] = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $1234
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x01, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Abs_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $1234
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task Abs_NegativeCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $1234
                stp",
                emulator);

        // emulation
        emulator.AssertState(0xff, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Abs_Overflow()
    {
        var emulator = new Emulator();

        emulator.A = 0xa0;
        emulator.Carry = false;
        emulator.Memory[0x1234] = 0x20;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $1234
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x7f, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, true, true);
    }

    [TestMethod]
    public async Task AbsX()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.X = 0x04;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $1230, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xfd, emulator.Memory[0x810]);
        Assert.AreEqual(0x30, emulator.Memory[0x811]);
        Assert.AreEqual(0x12, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x01, 0x04, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task AbsX_PagePenatly()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.X = 0x44;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $11f0, x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x01, 0x44, 0x00, 0x814, 5);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task AbsX_NoCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x04;
        emulator.X = 0x04;
        emulator.Carry = false;
        emulator.Memory[0x1234] = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $1230, x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x01, 0x04, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task AbsX_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.X = 0x04;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $1230, x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x04, 0x00, 0x814, 4);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task AbsX_NegativeCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.X = 0x04;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $1230, x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0xff, 0x04, 0x00, 0x814, 4);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task AbsX_Overflow()
    {
        var emulator = new Emulator();

        emulator.A = 0xa0;
        emulator.X = 0x04;
        emulator.Carry = false;
        emulator.Memory[0x1234] = 0x20;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $1230, x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x7f, 0x04, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, true, true);
    }

    [TestMethod]
    public async Task Absy()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Y = 0x04;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $1230, y
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xf9, emulator.Memory[0x810]);
        Assert.AreEqual(0x30, emulator.Memory[0x811]);
        Assert.AreEqual(0x12, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x01, 0x00, 0x04, 0x814, 4);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task AbsY_PagePenatly()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Y = 0x44;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $11f0, y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x01, 0x00, 0x44, 0x814, 5);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task AbsY_NoCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x04;
        emulator.Y = 0x04;
        emulator.Carry = false;
        emulator.Memory[0x1234] = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $1230, y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x01, 0x00, 0x04, 0x814, 4);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task AbsY_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Y = 0x04;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $1230, y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x04, 0x814, 4);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task AbsY_NegativeCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Y = 0x04;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $1230, y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0xff, 0x00, 0x04, 0x814, 4);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task AbsY_Overflow()
    {
        var emulator = new Emulator();

        emulator.A = 0xa0;
        emulator.Y = 0x04;
        emulator.Carry = false;
        emulator.Memory[0x1234] = 0x20;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $1230, y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x7f, 0x00, 0x04, 0x814, 4);
        emulator.AssertFlags(false, false, true, true);
    }

    [TestMethod]
    public async Task Zp()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $12
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xe5, emulator.Memory[0x810]);
        Assert.AreEqual(0x12, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x01, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Zp_NoCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x04;
        emulator.Carry = false;
        emulator.Memory[0x12] = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $12
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x01, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Zp_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $12
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task Zp_NegativeCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $12
                stp",
                emulator);

        // emulation
        emulator.AssertState(0xff, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Zp_Overflow()
    {
        var emulator = new Emulator();

        emulator.A = 0xa0;
        emulator.Carry = false;
        emulator.Memory[0x12] = 0x20;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $12
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x7f, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(false, false, true, true);
    }

    [TestMethod]
    public async Task ZpX()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0x02;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $10,x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xf5, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x01, 0x02, 0x00, 0x813, 4);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task ZpX_NoCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x04;
        emulator.Carry = false;
        emulator.Memory[0x12] = 0x02;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $10,x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x01, 0x02, 0x00, 0x813, 4);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task ZpX_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0x03;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $10,x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x02, 0x00, 0x813, 4);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task ZpX_NegativeCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0x03;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $10,x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0xff, 0x02, 0x00, 0x813, 4);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task ZpX_Overflow()
    {
        var emulator = new Emulator();

        emulator.A = 0xa0;
        emulator.Carry = false;
        emulator.Memory[0x12] = 0x20;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $10,x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x7f, 0x02, 0x00, 0x813, 4);
        emulator.AssertFlags(false, false, true, true);
    }

    [TestMethod]
    public async Task IndZp()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;
        emulator.Memory[0x1234] = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($12)
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xF2, emulator.Memory[0x810]);
        Assert.AreEqual(0x12, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x01, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task IndZp_NoCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x04;
        emulator.Carry = false;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;
        emulator.Memory[0x1234] = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($12)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x01, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task IndZp_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;
        emulator.Memory[0x1234] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($12)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task IndZp_NegativeCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;
        emulator.Memory[0x1234] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($12)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0xff, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task IndZp_Overflow()
    {
        var emulator = new Emulator();

        emulator.A = 0xa0;
        emulator.Carry = false;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;
        emulator.Memory[0x1234] = 0x20;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($12)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x7f, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, true, true);
    }

    [TestMethod]
    public async Task IndZpx()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;
        emulator.Memory[0x1234] = 0x02;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($10, x)
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xe1, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x01, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task IndZpX_NoCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x04;
        emulator.Carry = false;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;
        emulator.Memory[0x1234] = 0x02;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($10, x)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x01, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task IndZpX_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;
        emulator.Memory[0x1234] = 0x03;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($10, x)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task IndZpX_NegativeCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;
        emulator.Memory[0x1234] = 0x03;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($10, x)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0xff, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task IndZpX_Overflow()
    {
        var emulator = new Emulator();

        emulator.A = 0xa0;
        emulator.Carry = false;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;
        emulator.Memory[0x1234] = 0x20;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($10, x)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x7f, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, true, true);
    }

    [TestMethod]
    public async Task IndZpY()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0x30;
        emulator.Memory[0x13] = 0x12;
        emulator.Memory[0x1234] = 0x02;
        emulator.Y = 0x04;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($12), y
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xf1, emulator.Memory[0x810]);
        Assert.AreEqual(0x12, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x01, 0x00, 0x04, 0x813, 5);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task IndZpY_PagePenatly()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0xf0;
        emulator.Memory[0x13] = 0x11;
        emulator.Memory[0x1234] = 0x02;
        emulator.Y = 0x44;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($12), y
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xf1, emulator.Memory[0x810]);
        Assert.AreEqual(0x12, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x01, 0x00, 0x44, 0x813, 6);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task IndZpY_NoCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x04;
        emulator.Carry = false;
        emulator.Memory[0x12] = 0x30;
        emulator.Memory[0x13] = 0x12;
        emulator.Memory[0x1234] = 0x02;
        emulator.Y = 0x04;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($12), y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x01, 0x00, 0x04, 0x813, 5);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task IndZpY_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0x30;
        emulator.Memory[0x13] = 0x12;
        emulator.Memory[0x1234] = 0x03;
        emulator.Y = 0x04;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($12), y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x04, 0x813, 5);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task IndZpY_NegativeCarry()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0x30;
        emulator.Memory[0x13] = 0x12;
        emulator.Memory[0x1234] = 0x03;
        emulator.Y = 0x04;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($12), y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0xff, 0x00, 0x04, 0x813, 5);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task IndZpY_Overflow()
    {
        var emulator = new Emulator();

        emulator.A = 0xa0;
        emulator.Carry = false;
        emulator.Memory[0x12] = 0x30;
        emulator.Memory[0x13] = 0x12;
        emulator.Memory[0x1234] = 0x20;
        emulator.Y = 0x04;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($12), y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x7f, 0x00, 0x04, 0x813, 5);
        emulator.AssertFlags(false, false, true, true);
    }
}