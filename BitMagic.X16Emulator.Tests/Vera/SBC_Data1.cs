using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class SBC_Data1
{
    [TestMethod]
    public async Task Abs_Step0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 0;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.A = 0xee;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc DATA1
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00000, emulator.Vera.Data1_Address);
        Assert.AreEqual(0x01, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Abs_Step1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 1;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.A = 0xee;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc DATA1
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00001, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x01, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x10, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AbsX_Step0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 0;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.A = 0xee;
        emulator.X = 0x24;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $9f00, x
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00000, emulator.Vera.Data1_Address);
        Assert.AreEqual(0x01, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AbsX_Step1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 1;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.A = 0xee;
        emulator.X = 0x24;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $9f00, x
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00001, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x01, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x10, emulator.Memory[0x9F22]);
    }


    [TestMethod]
    public async Task AbsY_Step0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 0;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.A = 0xee;
        emulator.Y = 0x24;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $9f00, y
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00000, emulator.Vera.Data1_Address);
        Assert.AreEqual(0x01, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AbsY_Step1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 1;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.A = 0xee;
        emulator.Y = 0x24;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $9f00, y
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00001, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x01, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x10, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Ind_Step0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 0;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.Memory[0x10] = 0x24;
        emulator.Memory[0x11] = 0x9f;
        emulator.A = 0xee;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($10)
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00000, emulator.Vera.Data1_Address);
        Assert.AreEqual(0x01, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Ind_Step1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 1;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.Memory[0x10] = 0x24;
        emulator.Memory[0x11] = 0x9f;
        emulator.A = 0xee;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($10)
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00001, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x01, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x10, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task IndX_Step0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 0;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.Memory[0x10] = 0x24;
        emulator.Memory[0x11] = 0x9f;
        emulator.A = 0xee;
        emulator.X = 0x10;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($0, x)
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00000, emulator.Vera.Data1_Address);
        Assert.AreEqual(0x01, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task IndX_Step1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 1;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.Memory[0x10] = 0x24;
        emulator.Memory[0x11] = 0x9f;
        emulator.A = 0xee;
        emulator.X = 0x10;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($0, x)
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00001, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x01, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x10, emulator.Memory[0x9F22]);
    }


    [TestMethod]
    public async Task IndY_Step0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 0;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.Memory[0x10] = 0x14;
        emulator.Memory[0x11] = 0x9f;
        emulator.A = 0xee;
        emulator.Y = 0x10;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($10), y
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00000, emulator.Vera.Data1_Address);
        Assert.AreEqual(0x01, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task IndY_Step1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 1;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.Memory[0x10] = 0x14;
        emulator.Memory[0x11] = 0x9f;
        emulator.A = 0xee;
        emulator.Y = 0x10;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($10), y
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00001, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x01, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x10, emulator.Memory[0x9F22]);
    }
}