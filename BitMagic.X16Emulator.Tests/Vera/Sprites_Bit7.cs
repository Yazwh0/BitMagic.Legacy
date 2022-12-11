using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera;

[TestClass]
public class Sprites_Bit7
{
    [TestMethod]
    public async Task PaletteOffset_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc00 + 7;
        emulator.A = 0x0f;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { PaletteOffset = 0x0f, YHeight = 0x08 }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task PaletteOffset_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc08 + 7;
        emulator.A = 0x0f;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { PaletteOffset = 0x0f, YHeight = 0x08 }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task PaletteOffset_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fff8 + 7;
        emulator.A = 0x0f;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { PaletteOffset = 0x0f, YHeight = 0x08 }, emulator.Sprites[127]);
    }

    [TestMethod]
    public async Task HeightWidth_Mode_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc00 + 7;
        emulator.A = 0xf0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Mode = 0x3c, YHeight = 0x40 }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task HeightWidth_Mode_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc08 + 7;
        emulator.A = 0xf0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Mode = 0x3c, YHeight = 0x40 }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task HeightWidth_Mode_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fff8 + 7;
        emulator.A = 0xf0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Mode = 0x3c, YHeight = 0x40 }, emulator.Sprites[127]);
    }

    [TestMethod]
    public async Task HeightWidth_ExistingMode_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc00 + 7;
        emulator.A = 0xf0;
        emulator.Sprites[0].Mode = 0x43;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Mode = 0x7f, YHeight = 0x40 }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task HeightWidth_ExistingMode_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc08 + 7;
        emulator.A = 0xf0;
        emulator.Sprites[1].Mode = 0x43;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Mode = 0x7f, YHeight = 0x40 }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task HeightWidth_ExistingMode_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fff8 + 7;
        emulator.A = 0xf0;
        emulator.Sprites[127].Mode = 0x43;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Mode = 0x7f, YHeight = 0x40 }, emulator.Sprites[127]);
    }
}
