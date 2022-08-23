using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class STY_Data0
{
    [TestMethod]
    public async Task Abs_Step0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0002] = 0xff;
        emulator.Y = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sty DATA0
                stp",
                emulator);

        Assert.AreEqual(0x00000, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x10, emulator.Memory[0x9F23]);

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
        emulator.Vera.Vram[0x0000] = 0xee;
        emulator.Vera.Vram[0x0001] = 0xff;
        emulator.Y = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sty DATA0
                stp",
                emulator);

        Assert.AreEqual(0x00001, emulator.Vera.Data0_Address);
        Assert.AreEqual(0xff, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x10, emulator.Vera.Vram[0x0000]);
        Assert.AreEqual(0xff, emulator.Vera.Vram[0x0001]);

        Assert.AreEqual(0x01, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x10, emulator.Memory[0x9F22]);
    }
}