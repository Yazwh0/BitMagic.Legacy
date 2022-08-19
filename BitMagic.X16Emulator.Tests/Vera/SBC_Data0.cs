using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class SBC_Data0
{
    [TestMethod]
    public async Task Abs_Step0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.A = 0xee;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc DATA0
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00000, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x01, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Abs_Step1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 1;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.A = 0xee;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc DATA0
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00001, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x01, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x10, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AbsX_Step0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.A = 0xee;
        emulator.X = 0x23;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $9f00, x
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00000, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x01, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AbsX_Step1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 1;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.A = 0xee;
        emulator.X = 0x23;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $9f00, x
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00001, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x01, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x10, emulator.Memory[0x9F22]);
    }


    [TestMethod]
    public async Task AbsY_Step0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.A = 0xee;
        emulator.Y = 0x23;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $9f00, y
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00000, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x01, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AbsY_Step1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 1;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.A = 0xee;
        emulator.Y = 0x23;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc $9f00, y
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00001, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x01, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x10, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Ind_Step0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.Memory[0x10] = 0x23;
        emulator.Memory[0x11] = 0x9f;
        emulator.A = 0xee;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($10)
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00000, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x01, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Ind_Step1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 1;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.Memory[0x10] = 0x23;
        emulator.Memory[0x11] = 0x9f;
        emulator.A = 0xee;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($10)
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00001, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x01, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x10, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task IndX_Step0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.Memory[0x10] = 0x23;
        emulator.Memory[0x11] = 0x9f;
        emulator.A = 0xee;
        emulator.X = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($0, x)
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00000, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x01, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task IndX_Step1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 1;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.Memory[0x10] = 0x23;
        emulator.Memory[0x11] = 0x9f;
        emulator.A = 0xee;
        emulator.X = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($0, x)
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00001, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x01, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x10, emulator.Memory[0x9F22]);
    }


    [TestMethod]
    public async Task IndY_Step0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.Memory[0x10] = 0x13;
        emulator.Memory[0x11] = 0x9f;
        emulator.A = 0xee;
        emulator.Y = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($10), y
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00000, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x01, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task IndY_Step1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 1;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x01;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.Memory[0x10] = 0x13;
        emulator.Memory[0x11] = 0x9f;
        emulator.A = 0xee;
        emulator.Y = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sbc ($10), y
                stp",
                emulator);

        emulator.AssertState(A: 0xec);

        Assert.AreEqual(0x00001, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x01, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x10, emulator.Memory[0x9F22]);
    }
}