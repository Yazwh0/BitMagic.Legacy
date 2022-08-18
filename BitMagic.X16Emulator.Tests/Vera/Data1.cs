using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class Data1
{
    [TestMethod]
    public async Task Read_Data1_Step0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 0;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R40
            .org $810
            lda DATA1
            stp",
                emulator);

        emulator.AssertState(A: 0xee);

        Assert.AreEqual(0x00000, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xee, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F22]);
    }


    [TestMethod]
    public async Task Read_Data1_Step1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 1;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R40
            .org $810
            lda DATA1
            stp",
                emulator);

        emulator.AssertState(A: 0xee);

        Assert.AreEqual(0x00001, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x01, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x10, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data1_Step2()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 2;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0002] = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R40
            .org $810
            lda DATA1
            stp",
                emulator);

        emulator.AssertState(A: 0xee);

        Assert.AreEqual(0x00002, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x02, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x20, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data1_Step3()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 4;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0004] = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R40
            .org $810
            lda DATA1
            stp",
                emulator);

        emulator.AssertState(A: 0xee);

        Assert.AreEqual(0x00004, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x04, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x30, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data1_Step4()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 8;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0008] = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R40
            .org $810
            lda DATA1
            stp",
                emulator);

        emulator.AssertState(A: 0xee);

        Assert.AreEqual(0x00008, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x08, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x40, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data1_Step5()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 16;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0010] = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R40
            .org $810
            lda DATA1
            stp",
                emulator);

        emulator.AssertState(A: 0xee);

        Assert.AreEqual(0x00010, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x10, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x50, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data1_Step6()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 32;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0020] = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R40
            .org $810
            lda DATA1
            stp",
                emulator);

        emulator.AssertState(A: 0xee);

        Assert.AreEqual(0x00020, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x20, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x60, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data1_Step7()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 64;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0040] = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R40
            .org $810
            lda DATA1
            stp",
                emulator);

        emulator.AssertState(A: 0xee);

        Assert.AreEqual(0x00040, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x40, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x70, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data1_Step8()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 128;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0080] = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R40
            .org $810
            lda DATA1
            stp",
                emulator);

        emulator.AssertState(A: 0xee);

        Assert.AreEqual(0x00080, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x80, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x80, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data1_Step9()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 256;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0100] = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R40
            .org $810
            lda DATA1
            stp",
                emulator);

        emulator.AssertState(A: 0xee);

        Assert.AreEqual(0x00100, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x01, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x90, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data1_StepA()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 512;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0200] = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R40
            .org $810
            lda DATA1
            stp",
                emulator);

        emulator.AssertState(A: 0xee);

        Assert.AreEqual(0x00200, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x02, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xa0, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data1_StepB()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 40;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0028] = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R40
            .org $810
            lda DATA1
            stp",
                emulator);

        emulator.AssertState(A: 0xee);

        Assert.AreEqual(0x00028, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x28, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xb0, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data1_StepC()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 80;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0050] = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R40
            .org $810
            lda DATA1
            stp",
                emulator);

        emulator.AssertState(A: 0xee);

        Assert.AreEqual(0x00050, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x50, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xc0, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data1_StepD()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 160;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x00a0] = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R40
            .org $810
            lda DATA1
            stp",
                emulator);

        emulator.AssertState(A: 0xee);

        Assert.AreEqual(0x000a0, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0xa0, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xd0, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data1_StepE()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 320;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0140] = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R40
            .org $810
            lda DATA1
            stp",
                emulator);

        emulator.AssertState(A: 0xee);

        Assert.AreEqual(0x00140, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x40, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x01, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xe0, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Read_Data1_StepF()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 640;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0280] = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R40
            .org $810
            lda DATA1
            stp",
                emulator);

        emulator.AssertState(A: 0xee);

        Assert.AreEqual(0x00280, emulator.Vera.Data1_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x80, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x02, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xf0, emulator.Memory[0x9F22]);
    }
}

