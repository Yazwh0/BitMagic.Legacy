using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class DC_Video
{
    [TestMethod]
    public async Task Layer0_Enable()
    {
        var emulator = new Emulator();

        emulator.A = 0b00010000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DC_VIDEO
                stp",
                emulator);

        Assert.AreEqual(true, emulator.Vera.Layer0Enable);
        Assert.AreEqual(false, emulator.Vera.Layer1Enable);
        Assert.AreEqual(false, emulator.Vera.SpriteEnable);

        Assert.AreEqual(0b00010000, emulator.Memory[0x9F29]);
    }

    [TestMethod]
    public async Task Layer1_Enable()
    {
        var emulator = new Emulator();

        emulator.A = 0b00100000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DC_VIDEO
                stp",
                emulator);

        Assert.AreEqual(false, emulator.Vera.Layer0Enable);
        Assert.AreEqual(true, emulator.Vera.Layer1Enable);
        Assert.AreEqual(false, emulator.Vera.SpriteEnable);

        Assert.AreEqual(0b00100000, emulator.Memory[0x9F29]);
    }

    [TestMethod]
    public async Task Sprites_Enable()
    {
        var emulator = new Emulator();

        emulator.A = 0b01000000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DC_VIDEO
                stp",
                emulator);

        Assert.AreEqual(false, emulator.Vera.Layer0Enable);
        Assert.AreEqual(false, emulator.Vera.Layer1Enable);
        Assert.AreEqual(true, emulator.Vera.SpriteEnable);

        Assert.AreEqual(0b01000000, emulator.Memory[0x9F29]);
    }

    [TestMethod]
    public async Task AllSet()
    {
        var emulator = new Emulator();

        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DC_VIDEO
                stp",
                emulator);

        Assert.AreEqual(true, emulator.Vera.Layer0Enable);
        Assert.AreEqual(true, emulator.Vera.Layer1Enable);
        Assert.AreEqual(true, emulator.Vera.SpriteEnable);

        Assert.AreEqual(0b01110111, emulator.Memory[0x9F29]);
    }
}