using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class VeraInitialise
{

    [TestMethod]
    public async Task Data()
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
    public async Task CtrlSet()
    {
        var emulator = new Emulator();

        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x01, emulator.Memory[0x9F25]);
    }

    [TestMethod]
    public async Task CtrlNotSet()
    {
        var emulator = new Emulator();

        emulator.Vera.AddrSel = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F25]);
    }

    [TestMethod]
    public async Task DcSelSet()
    {
        var emulator = new Emulator();

        emulator.Vera.DcSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x02, emulator.Memory[0x9F25]);
    }

    [TestMethod]
    public async Task DcSelNotSet()
    {
        var emulator = new Emulator();

        emulator.Vera.DcSel = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F25]);
    }

    [TestMethod]
    public async Task CrtlDcSelSet()
    {
        var emulator = new Emulator();

        emulator.Vera.DcSel = true;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x03, emulator.Memory[0x9F25]);
    }

    [TestMethod]
    public async Task DcSel0()
    {
        var emulator = new Emulator();

        emulator.Vera.DcSel = false;
        emulator.Vera.Dc_HScale = 0x01; // not real values
        emulator.Vera.Dc_VScale = 0x02;
        emulator.Vera.Dc_Border = 0x03;
        emulator.Vera.Dc_HStart = 0x04;
        emulator.Vera.Dc_HStop = 0x05;
        emulator.Vera.Dc_VStart = 0x06;
        emulator.Vera.Dc_VStop = 0x07;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9F29]);
        Assert.AreEqual(0x01, emulator.Memory[0x9F2A]);
        Assert.AreEqual(0x02, emulator.Memory[0x9F2B]);
        Assert.AreEqual(0x03, emulator.Memory[0x9F2C]);
    }

    [TestMethod]
    public async Task DcSel1()
    {
        var emulator = new Emulator();

        emulator.Vera.DcSel = true;
        emulator.Vera.Dc_HScale = 0x01; // not real values
        emulator.Vera.Dc_VScale = 0x02;
        emulator.Vera.Dc_Border = 0x03;
        emulator.Vera.Dc_HStart = 0x04;
        emulator.Vera.Dc_HStop = 0x05;
        emulator.Vera.Dc_VStart = 0x06;
        emulator.Vera.Dc_VStop = 0x07;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x04, emulator.Memory[0x9F29]);
        Assert.AreEqual(0x05, emulator.Memory[0x9F2A]);
        Assert.AreEqual(0x06, emulator.Memory[0x9F2B]);
        Assert.AreEqual(0x07, emulator.Memory[0x9F2C]);
    }

    [TestMethod]
    public async Task DCVideo_SpriteEnable()
    {
        var emulator = new Emulator();

        emulator.Vera.DcSel = false;
        emulator.Vera.SpriteEnable = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0b01000000, emulator.Memory[0x9F29]);
    }
    [TestMethod]
    public async Task DCVideo_Layer1Enable()
    {
        var emulator = new Emulator();

        emulator.Vera.DcSel = false;
        emulator.Vera.Layer1Enable = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0b00100000, emulator.Memory[0x9F29]);
    }

    [TestMethod]
    public async Task DCVideo_Layer0Enable()
    {
        var emulator = new Emulator();

        emulator.Vera.DcSel = false;
        emulator.Vera.Layer0Enable = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0b00010000, emulator.Memory[0x9F29]);
    }

    [TestMethod]
    public async Task DCVideo_AllEnable()
    {
        var emulator = new Emulator();

        emulator.Vera.DcSel = false;
        emulator.Vera.Layer0Enable = true;
        emulator.Vera.Layer1Enable = true;
        emulator.Vera.SpriteEnable = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0b01110000, emulator.Memory[0x9F29]);
    }
}