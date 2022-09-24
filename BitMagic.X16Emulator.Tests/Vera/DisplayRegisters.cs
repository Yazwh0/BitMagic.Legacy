using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class DisplayRegisters
{
    [TestMethod]
    public async Task DC_HStart()
    {
        var emulator = new Emulator();

        emulator.Vera.DcSel = true;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DC_HSTART 
                stp",
                emulator);

        Assert.AreEqual(false, emulator.Vera.Layer0Enable);
        Assert.AreEqual(false, emulator.Vera.Layer1Enable);
        Assert.AreEqual(false, emulator.Vera.SpriteEnable);
        Assert.AreEqual(0x3fc, emulator.Vera.Dc_HStart);

        Assert.AreEqual(0xff, emulator.Memory[0x9F29]);
    }

    [TestMethod]
    public async Task DC_HStop()
    {
        var emulator = new Emulator();

        emulator.Vera.DcSel = true;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DC_HSTOP  
                stp",
                emulator);

        Assert.AreEqual(0x3fc, emulator.Vera.Dc_HStop);

        Assert.AreEqual(0xff, emulator.Memory[0x9F2a]);
    }

    [TestMethod]
    public async Task DC_VStart()
    {
        var emulator = new Emulator();

        emulator.Vera.DcSel = true;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DC_VSTART
                stp",
                emulator);

        Assert.AreEqual(0x1fe, emulator.Vera.Dc_VStart);

        Assert.AreEqual(0xff, emulator.Memory[0x9F2b]);
    }

    [TestMethod]
    public async Task DC_VStop()
    {
        var emulator = new Emulator();

        emulator.Vera.DcSel = true;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DC_VSTOP
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Vera.Dc_Border);
        Assert.AreEqual(0x1fe, emulator.Vera.Dc_VStop);

        Assert.AreEqual(0xff, emulator.Memory[0x9F2c]);
    }

    [TestMethod]
    public async Task DC_Border()
    {
        var emulator = new Emulator();

        emulator.Vera.DcSel = false;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DC_BORDER
                stp",
                emulator);

        Assert.AreEqual(0xff, emulator.Vera.Dc_Border);
        Assert.AreEqual(480, emulator.Vera.Dc_VStop);

        Assert.AreEqual(0xff, emulator.Memory[0x9F2c]);
    }
}