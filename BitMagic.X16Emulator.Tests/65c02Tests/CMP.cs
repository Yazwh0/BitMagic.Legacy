using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class CMP
{
    [TestMethod]
    public async Task Imm_Equal()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xc9, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task Imm_LessThan()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp #$20
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Imm_GreaterThan()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp #$05
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Abs_Equal()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.Memory[0x1234] = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp $1234
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xcd, emulator.Memory[0x810]);
        Assert.AreEqual(0x34, emulator.Memory[0x811]);
        Assert.AreEqual(0x12, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task Abs_LessThan()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.Memory[0x1234] = 0x20;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp $1234
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Abs_GreaterThan()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.Memory[0x1234] = 0x05;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp $1234
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task AbsX_Equal()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.X = 0x04;
        emulator.Memory[0x1234] = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp $1230,x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xdd, emulator.Memory[0x810]);
        Assert.AreEqual(0x30, emulator.Memory[0x811]);
        Assert.AreEqual(0x12, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x10, 0x04, 0x00, 0x814, 4);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task AbsX_EqualPagePentalty()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.X = 0x44;
        emulator.Memory[0x1234] = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp $11f0,x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x44, 0x00, 0x814, 5);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task AbsX_LessThan()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.X = 0x04;
        emulator.Memory[0x1234] = 0x20;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp $1230,x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x04, 0x00, 0x814, 4);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task AbsX_GreaterThan()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.X = 0x04;
        emulator.Memory[0x1234] = 0x05;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp $1230,x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x04, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task AbsY_Equal()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.Y = 0x04;
        emulator.Memory[0x1234] = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp $1230,y
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xd9, emulator.Memory[0x810]);
        Assert.AreEqual(0x30, emulator.Memory[0x811]);
        Assert.AreEqual(0x12, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x04, 0x814, 4);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task AbsY_EqualPagePentalty()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.Y = 0x44;
        emulator.Memory[0x1234] = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp $11f0,y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x44, 0x814, 5);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task AbsY_LessThan()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.Y = 0x04;
        emulator.Memory[0x1234] = 0x20;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp $1230,y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x04, 0x814, 4);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task AbsY_GreaterThan()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.Y = 0x04;
        emulator.Memory[0x1234] = 0x05;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp $1230,y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x04, 0x814, 4);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Zp_Equal()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.Memory[0x12] = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp $12
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xc5, emulator.Memory[0x810]);
        Assert.AreEqual(0x12, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task Zp_LessThan()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.Memory[0x12] = 0x20;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp $12
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Zp_GreaterThan()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.Memory[0x12] = 0x05;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp $12
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task ZpX_Equal()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.Memory[0x12] = 0x10;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp $10, x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xd5, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x10, 0x02, 0x00, 0x813, 4);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task ZpX_LessThan()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.Memory[0x12] = 0x20;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp $10, x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x02, 0x00, 0x813, 4);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task ZpX_GreaterThan()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.X = 0x02;
        emulator.Memory[0x12] = 0x05;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp $10, x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x02, 0x00, 0x813, 4);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Ind_Equal()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.Memory[0x1234] = 0x10;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp ($12)
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xd2, emulator.Memory[0x810]);
        Assert.AreEqual(0x12, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task Ind_LessThan()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.Memory[0x1234] = 0x20;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp ($12)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Ind_GreaterThan()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.Memory[0x1234] = 0x05;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp ($12)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task IndX_Equal()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.X = 0x02;
        emulator.Memory[0x1234] = 0x10;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp ($10,x)
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xc1, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x10, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task IndX_LessThan()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.X = 0x02;
        emulator.Memory[0x1234] = 0x20;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp ($10,x)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task IndX_GreaterThan()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.X = 0x02;
        emulator.Memory[0x1234] = 0x05;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp ($10,x)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task IndY_Equal()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.Y = 0x04;
        emulator.Memory[0x1234] = 0x10;
        emulator.Memory[0x12] = 0x30;
        emulator.Memory[0x13] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp ($12),y
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xd1, emulator.Memory[0x810]);
        Assert.AreEqual(0x12, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x04, 0x813, 5);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task IndY_EqualPagePentalty()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.Y = 0x44;
        emulator.Memory[0x1234] = 0x10;
        emulator.Memory[0x12] = 0xf0;
        emulator.Memory[0x13] = 0x11;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp ($12),y
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xd1, emulator.Memory[0x810]);
        Assert.AreEqual(0x12, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x44, 0x813, 6);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task IndY_LessThan()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.Y = 0x04;
        emulator.Memory[0x1234] = 0x20;
        emulator.Memory[0x12] = 0x30;
        emulator.Memory[0x13] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp ($12),y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x04, 0x813, 5);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task IndY_GreaterThan()
    {
        var emulator = new Emulator();

        emulator.A = 0x10;
        emulator.Y = 0x04;
        emulator.Memory[0x1234] = 0x05;
        emulator.Memory[0x12] = 0x30;
        emulator.Memory[0x13] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cmp ($12),y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x04, 0x813, 5);
        emulator.AssertFlags(false, false, false, true);
    }
}