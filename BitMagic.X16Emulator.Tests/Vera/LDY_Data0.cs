using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class LDY_Data0
{
    [TestMethod]
    public async Task Read_Data0_Step1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 1;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0001] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ldy DATA0
                stp",
                emulator);

        emulator.AssertState(Y: 0xee);

        Assert.AreEqual(0x00001, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x01, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x10, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data0_Step2()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 2;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0002] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ldy DATA0
                stp",
                emulator);

        emulator.AssertState(Y: 0xee);

        Assert.AreEqual(0x00002, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x02, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x20, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data0_Step3()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 4;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0004] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ldy DATA0
                stp",
                emulator);

        emulator.AssertState(Y: 0xee);

        Assert.AreEqual(0x00004, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x04, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x30, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data0_Step4()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 8;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0008] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ldy DATA0
                stp",
                emulator);

        emulator.AssertState(Y: 0xee);

        Assert.AreEqual(0x00008, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x08, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x40, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data0_Step5()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 16;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0010] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ldy DATA0
                stp",
                emulator);

        emulator.AssertState(Y: 0xee);

        Assert.AreEqual(0x00010, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x10, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x50, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data0_Step6()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 32;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0020] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ldy DATA0
                stp",
                emulator);

        emulator.AssertState(Y: 0xee);

        Assert.AreEqual(0x00020, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x20, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x60, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data0_Step7()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 64;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0040] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ldy DATA0
                stp",
                emulator);

        emulator.AssertState(Y: 0xee);

        Assert.AreEqual(0x00040, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x40, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x70, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data0_Step8()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 128;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0080] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ldy DATA0
                stp",
                emulator);

        emulator.AssertState(Y: 0xee);

        Assert.AreEqual(0x00080, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x80, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x80, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data0_Step9()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 256;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0100] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ldy DATA0
                stp",
                emulator);

        emulator.AssertState(Y: 0xee);

        Assert.AreEqual(0x00100, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x01, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x90, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data0_StepA()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 512;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0200] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ldy DATA0
                stp",
                emulator);

        emulator.AssertState(Y: 0xee);

        Assert.AreEqual(0x00200, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x02, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xa0, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data0_StepB()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 40;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0028] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ldy DATA0
                stp",
                emulator);

        emulator.AssertState(Y: 0xee);

        Assert.AreEqual(0x00028, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x28, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xb0, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data0_StepC()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 80;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0050] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ldy DATA0
                stp",
                emulator);

        emulator.AssertState(Y: 0xee);

        Assert.AreEqual(0x00050, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x50, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xc0, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data0_StepD()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 160;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x00a0] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ldy DATA0
                stp",
                emulator);

        emulator.AssertState(Y: 0xee);

        Assert.AreEqual(0x000a0, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0xa0, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xd0, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data0_StepE()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 320;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0140] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ldy DATA0
                stp",
                emulator);

        emulator.AssertState(Y: 0xee);

        Assert.AreEqual(0x00140, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x40, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x01, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xe0, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data0_StepF()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 640;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0280] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ldy DATA0
                stp",
                emulator);

        emulator.AssertState(Y: 0xee);

        Assert.AreEqual(0x00280, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x80, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x02, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xf0, emulator.Memory[0x9F22]);
    }
}