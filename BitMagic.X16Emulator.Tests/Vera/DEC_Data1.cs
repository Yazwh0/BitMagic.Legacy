using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class DEC_Data1
{
    [TestMethod]
    public async Task Abs_Step0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 0;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x20;
        emulator.Vera.Vram[0x0001] = 0x30;
        emulator.Vera.Vram[0x0002] = 0x40;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                dec DATA1
                stp",
                emulator);

        Assert.AreEqual(0x00000, emulator.Vera.Data1_Address);
        Assert.AreEqual(0x1f, emulator.Memory[0x9F24]);

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
        emulator.Vera.Vram[0x0000] = 0x20;
        emulator.Vera.Vram[0x0001] = 0x30;
        emulator.Vera.Vram[0x0002] = 0x40;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                dec DATA1
                stp",
                emulator);

        Assert.AreEqual(0x00002, emulator.Vera.Data1_Address);
        Assert.AreEqual(0x20, emulator.Vera.Vram[0x0000]);
        Assert.AreEqual(0x1f, emulator.Vera.Vram[0x0001]);
        Assert.AreEqual(0x40, emulator.Vera.Vram[0x0002]);

        Assert.AreEqual(0x40, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x02, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x10, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AbsX_Step0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 0;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0x20;
        emulator.Vera.Vram[0x0001] = 0x30;
        emulator.Vera.Vram[0x0002] = 0x40;
        emulator.X = 0x24;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                dec $9f00, x
                stp",
                emulator);

        Assert.AreEqual(0x00000, emulator.Vera.Data1_Address);
        Assert.AreEqual(0x1f, emulator.Memory[0x9F24]);

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
        emulator.Vera.Vram[0x0000] = 0x20;
        emulator.Vera.Vram[0x0001] = 0x30;
        emulator.Vera.Vram[0x0002] = 0x40;
        emulator.X = 0x24;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                dec $9f00, x
                stp",
                emulator);

        Assert.AreEqual(0x00002, emulator.Vera.Data1_Address);
        Assert.AreEqual(0x20, emulator.Vera.Vram[0x0000]);
        Assert.AreEqual(0x1f, emulator.Vera.Vram[0x0001]);
        Assert.AreEqual(0x40, emulator.Vera.Vram[0x0002]);

        Assert.AreEqual(0x40, emulator.Memory[0x9F24]);

        Assert.AreEqual(0x02, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x10, emulator.Memory[0x9F22]);
    }
}