using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class AND
{
    [TestMethod]
    public async Task Imm()
    {
        var emulator = new Emulator();

        emulator.A = 0x2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and #$03
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x29, emulator.Memory[0x810]);
        Assert.AreEqual(0x03, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x02, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Imm_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and #$01
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Imm_Negative()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and #0b10000001
                stp",
                emulator);

        // emulation
        emulator.AssertState(0b10000000, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(false, true, false, false);
    }


    [TestMethod]
    public async Task Imm_CarryPreserve()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000000;
        emulator.Carry = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and #0b10000001
                stp",
                emulator);

        // emulation
        emulator.AssertState(0b10000000, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(false, true, false, true);
    }

    [TestMethod]
    public async Task Abs()
    {
        var emulator = new Emulator();

        emulator.A = 0x2;
        emulator.Memory[0x1234] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $1234
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x2d, emulator.Memory[0x810]);
        Assert.AreEqual(0x34, emulator.Memory[0x811]);
        Assert.AreEqual(0x12, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x02, 0x00, 0x00, 0x814, 4);
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
                and $1234
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Abs_Negative()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000000;
        emulator.Memory[0x1234] = 0b10000001;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $1234
                stp",
                emulator);

        // emulation
        emulator.AssertState(0b10000000, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(false, true, false, false);
    }


    [TestMethod]
    public async Task Abs_CarryPreserve()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000000;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0b10000001;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $1234
                stp",
                emulator);

        // emulation
        emulator.AssertState(0b10000000, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(false, true, false, true);
    }

    [TestMethod]
    public async Task AbsX()
    {
        var emulator = new Emulator();

        emulator.A = 0x2;
        emulator.Memory[0x1234] = 0x03;
        emulator.X = 0x34;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $1200,x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x3d, emulator.Memory[0x810]);
        Assert.AreEqual(0x00, emulator.Memory[0x811]);
        Assert.AreEqual(0x12, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x02, 0x34, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsX_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x1234] = 0x01;
        emulator.X = 0x34;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $1200,x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x34, 0x00, 0x814, 4);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task AbsX_Negative()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000000;
        emulator.Memory[0x1234] = 0b10000001;
        emulator.X = 0x34;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $1200,x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0b10000000, 0x34, 0x00, 0x814, 4);
        emulator.AssertFlags(false, true, false, false);
    }


    [TestMethod]
    public async Task AbsX_CarryPreserve()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000000;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0b10000001;
        emulator.X = 0x34;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $1200,x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0b10000000, 0x34, 0x00, 0x814, 4);
        emulator.AssertFlags(false, true, false, true);
    }
    
    [TestMethod]
    public async Task AbsX_PagePenatly()
    {
        var emulator = new Emulator();

        emulator.A = 0x2;
        emulator.Memory[0x1310] = 0x03;
        emulator.X = 0x70;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $12a0,x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x02, 0x70, 0x00, 0x814, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsY()
    {
        var emulator = new Emulator();

        emulator.A = 0x2;
        emulator.Memory[0x1234] = 0x03;
        emulator.Y = 0x34;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $1200,y
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x39, emulator.Memory[0x810]);
        Assert.AreEqual(0x00, emulator.Memory[0x811]);
        Assert.AreEqual(0x12, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x02, 0x00, 0x34, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsY_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x1234] = 0x01;
        emulator.Y = 0x34;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $1200,y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x34, 0x814, 4);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task AbsY_Negative()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000000;
        emulator.Memory[0x1234] = 0b10000001;
        emulator.Y = 0x34;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $1200,y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0b10000000, 0x00, 0x34, 0x814, 4);
        emulator.AssertFlags(false, true, false, false);
    }


    [TestMethod]
    public async Task AbsY_CarryPreserve()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000000;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0b10000001;
        emulator.Y = 0x34;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $1200,y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0b10000000, 0x00, 0x34, 0x814, 4);
        emulator.AssertFlags(false, true, false, true);
    }

    [TestMethod]
    public async Task AbsY_PagePenatly()
    {
        var emulator = new Emulator();

        emulator.A = 0x2;
        emulator.Memory[0x1310] = 0x03;
        emulator.Y = 0x70;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $12a0,y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x02, 0x00, 0x70, 0x814, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Zp()
    {
        var emulator = new Emulator();

        emulator.A = 0x2;
        emulator.Memory[0x12] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $12
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x25, emulator.Memory[0x810]);
        Assert.AreEqual(0x12, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x02, 0x00, 0x00, 0x813, 3);
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
                and $12
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Zp_Negative()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000000;
        emulator.Memory[0x12] = 0b10000001;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $12
                stp",
                emulator);

        // emulation
        emulator.AssertState(0b10000000, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Zp_CarryPreserve()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000000;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0b10000001;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $12
                stp",
                emulator);

        // emulation
        emulator.AssertState(0b10000000, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(false, true, false, true);
    }

    [TestMethod]
    public async Task ZpX()
    {
        var emulator = new Emulator();

        emulator.A = 0x2;
        emulator.Memory[0x12] = 0x03;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $10,x
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x35, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x02, 0x02, 0x00, 0x813, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZpX_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x12] = 0x01;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $10,x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x02, 0x00, 0x813, 4);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task ZpX_Negative()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000000;
        emulator.Memory[0x12] = 0b10000001;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $10,x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0b10000000, 0x02, 0x00, 0x813, 4);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task ZpX_CarryPreserve()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000000;
        emulator.Carry = true;
        emulator.Memory[0x12] = 0b10000001;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and $10,x
                stp",
                emulator);

        // emulation
        emulator.AssertState(0b10000000, 0x02, 0x00, 0x813, 4);
        emulator.AssertFlags(false, true, false, true);
    }

    [TestMethod]
    public async Task IndZp()
    {
        var emulator = new Emulator();

        emulator.A = 0x2;
        emulator.Memory[0x1234] = 0x03;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and ($12)
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x32, emulator.Memory[0x810]);
        Assert.AreEqual(0x12, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x02, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task IndZp_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x1234] = 0x01;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and ($12)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task IndZp_Negative()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000000;
        emulator.Memory[0x1234] = 0b10000001;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and ($12)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0b10000000, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task IndZp_CarryPreserve()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000000;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0b10000001;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and ($12)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0b10000000, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, true, false, true);
    }

    [TestMethod]
    public async Task IndX()
    {
        var emulator = new Emulator();

        emulator.A = 0x2;
        emulator.Memory[0x1234] = 0x03;
        emulator.X = 0x02;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and ($10,x)
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x21, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x02, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task IndX_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x1234] = 0x01;
        emulator.X = 0x02;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and ($10,x)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task IndX_Negative()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000000;
        emulator.Memory[0x1234] = 0b10000001;
        emulator.X = 0x02;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and ($10,x)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0b10000000, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task IndX_CarryPreserve()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000000;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0b10000001;
        emulator.X = 0x02;
        emulator.Memory[0x12] = 0x34;
        emulator.Memory[0x13] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and ($10,x)
                stp",
                emulator);

        // emulation
        emulator.AssertState(0b10000000, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, true, false, true);
    }

    [TestMethod]
    public async Task IndY()
    {
        var emulator = new Emulator();

        emulator.A = 0x2;
        emulator.Memory[0x1234] = 0x03;
        emulator.Y = 0x34;
        emulator.Memory[0x10] = 0x00;
        emulator.Memory[0x11] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and ($10),y
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x31, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x02, 0x00, 0x34, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task IndY_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x1234] = 0x01;
        emulator.Y = 0x34;
        emulator.Memory[0x10] = 0x00;
        emulator.Memory[0x11] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and ($10),y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x34, 0x813, 5);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task IndY_Negative()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000000;
        emulator.Memory[0x1234] = 0b10000001;
        emulator.Y = 0x34;
        emulator.Memory[0x10] = 0x00;
        emulator.Memory[0x11] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and ($10),y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0b10000000, 0x00, 0x34, 0x813, 5);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task IndY_CarryPreserve()
    {
        var emulator = new Emulator();

        emulator.A = 0b10000000;
        emulator.Carry = true;
        emulator.Memory[0x1234] = 0b10000001;
        emulator.Y = 0x34;
        emulator.Memory[0x10] = 0x00;
        emulator.Memory[0x11] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and ($10),y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0b10000000, 0x00, 0x34, 0x813, 5);
        emulator.AssertFlags(false, true, false, true);
    }

    [TestMethod]
    public async Task IndY_PagePenatly()
    {
        var emulator = new Emulator();

        emulator.A = 0x2;
        emulator.Memory[0x1310] = 0x03;
        emulator.Y = 0x70;
        emulator.Memory[0x10] = 0xa0;
        emulator.Memory[0x11] = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                and ($10),y
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x02, 0x00, 0x70, 0x813, 6);
        emulator.AssertFlags(false, false, false, false);
    }
}