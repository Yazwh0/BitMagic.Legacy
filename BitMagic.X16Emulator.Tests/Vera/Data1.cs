using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera;

[TestClass]
public class Data1_Initialise
{
    [TestMethod]
    public async Task Initial_Data_Step2()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 2;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

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

        emulator.Vera.Data1_Step = 4;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

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

        emulator.Vera.Data1_Step = 8;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

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

        emulator.Vera.Data1_Step = 16;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

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

        emulator.Vera.Data1_Step = 32;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

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

        emulator.Vera.Data1_Step = 64;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

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

        emulator.Vera.Data1_Step = 128;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

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

        emulator.Vera.Data1_Step = 256;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

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

        emulator.Vera.Data1_Step = 512;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

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

        emulator.Vera.Data1_Step = 40;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

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

        emulator.Vera.Data1_Step = 80;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

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

        emulator.Vera.Data1_Step = 160;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

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

        emulator.Vera.Data1_Step = 320;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

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

        emulator.Vera.Data1_Step = 640;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

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
    public async Task Initial_Data_Step_Minus2()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = -2;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x28, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step_Minus3()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = -4;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x38, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step_Minus4()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = -8;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x48, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step_Minus5()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = -16;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x58, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step_Minus6()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = -32;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x68, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step_Minus7()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = -64;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x78, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step_Minus8()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = -128;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x88, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step_Minus9()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = -256;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x98, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step_MinusA()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = -512;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xA8, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step_MinusB()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = -40;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xB8, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step_MinusC()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = -80;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xC8, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step_MinusD()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = -160;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xD8, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step_MinusE()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = -320;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xE8, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Initial_Data_Step_MinusF()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = -640;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xF8, emulator.Memory[0x9F22]);
    }
}