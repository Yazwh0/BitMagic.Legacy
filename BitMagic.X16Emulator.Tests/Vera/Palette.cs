using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class Palette
{
    [TestMethod]
    public async Task SetPallete0_R()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fa01;
        emulator.A = 0xf;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Common.PixelRgba(0xff, 0x00, 0x00), emulator.Palette[0]);
    }

    [TestMethod]
    public async Task SetPallete0_G()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fa00;
        emulator.A = 0xf0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Common.PixelRgba(0x00, 0xff, 0x00), emulator.Palette[0]);
    }

    [TestMethod]
    public async Task SetPallete0_B()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fa00;
        emulator.A = 0x0f;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Common.PixelRgba(0x00, 0x00, 0xff), emulator.Palette[0]);
    }

    [TestMethod]
    public async Task SetPallete1_R()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fa03;

        emulator.A = 0xd;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Common.PixelRgba(0xdd, 0xff, 0xff), emulator.Palette[1]);
    }

    [TestMethod]
    public async Task SetPallete1_G()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fa02;
        emulator.A = 0xd0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Common.PixelRgba(0xff, 0xdd, 0x00), emulator.Palette[1]);
    }

    [TestMethod]
    public async Task SetPallete1_B()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fa02;
        emulator.A = 0x0d;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Common.PixelRgba(0xff, 0x00, 0xdd), emulator.Palette[1]);
    }

    [TestMethod]
    public async Task SetPallete255_R()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fbff;

        emulator.A = 0x8;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Common.PixelRgba(0x88, 0x00, 0xbb), emulator.Palette[255]);
    }

    [TestMethod]
    public async Task SetPallete255_G()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fbfe;
        emulator.A = 0x80;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Common.PixelRgba(0xff, 0x88, 0x00), emulator.Palette[255]);
    }

    [TestMethod]
    public async Task SetPallete255_B()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fbfe;
        emulator.A = 0x08;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Common.PixelRgba(0xff, 0x00, 0x88), emulator.Palette[255]);
    }
}