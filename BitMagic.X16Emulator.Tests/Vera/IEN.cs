using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class Ien
{
    [TestMethod]
    public async Task Set_Vsync()
    {
        var emulator = new Emulator();

        emulator.A = 0x01;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta IEN
                stp",
                emulator);

        Assert.AreEqual(true, emulator.Vera.Interrupt_VSync);
        Assert.AreEqual(false, emulator.Vera.Interrupt_Line);
        Assert.AreEqual(false, emulator.Vera.Interrupt_SpCol);
        Assert.AreEqual(false, emulator.Vera.Interrupt_AFlow);
        Assert.AreEqual(0x000, emulator.Vera.Interrupt_LineNum);

        Assert.AreEqual(0x01, emulator.Memory[0x9F26]);
    }

    [TestMethod]
    public async Task Set_Line()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta IEN
                stp",
                emulator);

        Assert.AreEqual(false, emulator.Vera.Interrupt_VSync);
        Assert.AreEqual(true, emulator.Vera.Interrupt_Line);
        Assert.AreEqual(false, emulator.Vera.Interrupt_SpCol);
        Assert.AreEqual(false, emulator.Vera.Interrupt_AFlow);
        Assert.AreEqual(0x000, emulator.Vera.Interrupt_LineNum);

        Assert.AreEqual(0x02, emulator.Memory[0x9F26]);
    }

    [TestMethod]
    public async Task Set_SpCol()
    {
        var emulator = new Emulator();

        emulator.A = 0x04;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta IEN
                stp",
                emulator);

        Assert.AreEqual(false, emulator.Vera.Interrupt_VSync);
        Assert.AreEqual(false, emulator.Vera.Interrupt_Line);
        Assert.AreEqual(true, emulator.Vera.Interrupt_SpCol);
        Assert.AreEqual(false, emulator.Vera.Interrupt_AFlow);
        Assert.AreEqual(0x000, emulator.Vera.Interrupt_LineNum);

        Assert.AreEqual(0x04, emulator.Memory[0x9F26]);
    }

    [TestMethod]
    public async Task Set_AFlow()
    {
        var emulator = new Emulator();

        emulator.A = 0x08;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta IEN
                stp",
                emulator);

        Assert.AreEqual(false, emulator.Vera.Interrupt_VSync);
        Assert.AreEqual(false, emulator.Vera.Interrupt_Line);
        Assert.AreEqual(false, emulator.Vera.Interrupt_SpCol);
        Assert.AreEqual(true, emulator.Vera.Interrupt_AFlow);
        Assert.AreEqual(0x000, emulator.Vera.Interrupt_LineNum);

        Assert.AreEqual(0x08, emulator.Memory[0x9F26]);
    }

    [TestMethod]
    public async Task Set_HighLineBit()
    {
        var emulator = new Emulator();

        emulator.A = 0x80;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta IEN
                stp",
                emulator);

        Assert.AreEqual(false, emulator.Vera.Interrupt_VSync);
        Assert.AreEqual(false, emulator.Vera.Interrupt_Line);
        Assert.AreEqual(false, emulator.Vera.Interrupt_SpCol);
        Assert.AreEqual(false, emulator.Vera.Interrupt_AFlow);
        Assert.AreEqual(0x100, emulator.Vera.Interrupt_LineNum);

        Assert.AreEqual(0x80, emulator.Memory[0x9F26]);
    }

    [TestMethod]
    public async Task Set_All()
    {
        var emulator = new Emulator();

        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta IEN
                stp",
                emulator);

        Assert.AreEqual(true, emulator.Vera.Interrupt_VSync);
        Assert.AreEqual(true, emulator.Vera.Interrupt_Line);
        Assert.AreEqual(true, emulator.Vera.Interrupt_SpCol);
        Assert.AreEqual(true, emulator.Vera.Interrupt_AFlow);
        Assert.AreEqual(0x100, emulator.Vera.Interrupt_LineNum);

        Assert.AreEqual(0x8f, emulator.Memory[0x9F26]);
    }

    [TestMethod]
    public async Task Set_IrqLine()
    {
        var emulator = new Emulator();

        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta IRQLINE_L
                stp",
                emulator);

        Assert.AreEqual(0x0ff, emulator.Vera.Interrupt_LineNum);

        Assert.AreEqual(0xff, emulator.Memory[0x9F28]);
    }


    [TestMethod]
    public async Task Set_IrqLineFull()
    {
        var emulator = new Emulator();

        emulator.A = 0xff;
        emulator.X = 0x80;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta IRQLINE_L
                stx IEN
                stp",
                emulator);

        Assert.AreEqual(0x1ff, emulator.Vera.Interrupt_LineNum);

        Assert.AreEqual(0xff, emulator.Memory[0x9F28]);
    }
}