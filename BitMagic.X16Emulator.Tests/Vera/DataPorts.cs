using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class DataPorts
{
    [TestMethod]
    public async Task Initial_Data()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 1;
        emulator.Vera.Data0_Address = 0x0000;

        emulator.Vera.Data1_Step = 2;
        emulator.Vera.Data1_Address = 0x0001;

        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0001] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0xee, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x10, emulator.Memory[0x9F22]);

        Assert.AreEqual(0x00000, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x01, emulator.Vera.Data0_Step);

        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);
        Assert.AreEqual(0x00001, emulator.Vera.Data1_Address);
        Assert.AreEqual(0x02, emulator.Vera.Data1_Step);
    }


    [TestMethod]
    public async Task Initial_Data_Step2()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 2;
        emulator.Vera.Data0_Address = 0x0000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x20, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step3()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 4;
        emulator.Vera.Data0_Address = 0x0000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x30, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step4()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 8;
        emulator.Vera.Data0_Address = 0x0000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x40, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step5()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 16;
        emulator.Vera.Data0_Address = 0x0000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x50, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step6()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 32;
        emulator.Vera.Data0_Address = 0x0000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x60, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step7()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 64;
        emulator.Vera.Data0_Address = 0x0000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x70, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step8()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 128;
        emulator.Vera.Data0_Address = 0x0000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x80, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step9()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 256;
        emulator.Vera.Data0_Address = 0x0000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x90, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_StepA()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 512;
        emulator.Vera.Data0_Address = 0x0000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xA0, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_StepB()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 40;
        emulator.Vera.Data0_Address = 0x0000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xB0, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_StepC()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 80;
        emulator.Vera.Data0_Address = 0x0000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xC0, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_StepD()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 160;
        emulator.Vera.Data0_Address = 0x0000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xD0, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_StepE()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 320;
        emulator.Vera.Data0_Address = 0x0000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xE0, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_StepF()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 640;
        emulator.Vera.Data0_Address = 0x0000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xF0, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 1;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0001] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda DATA0
                stp",
                emulator);

        emulator.AssertState(A: 0xee);

        Assert.AreEqual(0x00001, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);
    }

    [TestMethod]
    public async Task Read_Data1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 1;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0001] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda DATA1
                stp",
                emulator);

        emulator.AssertState(A: 0xee);

        Assert.AreEqual(0x00001, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);
    }
}