using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera;

[TestClass]
public class Sprites_Bit7
{

    [TestMethod]
    public async Task Test()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 8;
        emulator.Vera.Data0_Address = 0x1fc00 + 7;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stz $02
                lda #$ff
    
                lda $02
                eor #1
                sta $02

                ora #$40
                sta DATA0

                lda $02
                eor #1
                sta $02

                ora #$40
                sta DATA0

                stp",
                emulator);

        Assert.AreEqual(new Sprite() { PaletteOffset = 0x01, Height = 16, Width = 8, Mode = 0x0010 }, emulator.Sprites[0]);
        Assert.AreEqual(0x41, emulator.Vera.Vram[0x1fc07]);
        Assert.AreEqual(0x40, emulator.Vera.Vram[0x1fc07 + 0x08]);
    }

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

        Assert.AreEqual(new Sprite() { PaletteOffset = 0x0f, Height = 0x08, Width = 8 }, emulator.Sprites[0]);
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

        Assert.AreEqual(new Sprite() { PaletteOffset = 0x0f, Height = 0x08, Width = 8 }, emulator.Sprites[1]);
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

        Assert.AreEqual(new Sprite() { PaletteOffset = 0x0f, Height = 0x08, Width = 8 }, emulator.Sprites[127]);
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

        Assert.AreEqual(new Sprite() { Mode = 0x3c, Height = 0x40, Width = 0x40 }, emulator.Sprites[0]);
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

        Assert.AreEqual(new Sprite() { Mode = 0x3c, Height = 0x40, Width = 0x40 }, emulator.Sprites[1]);
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

        Assert.AreEqual(new Sprite() { Mode = 0x3c, Height = 0x40, Width = 0x40 }, emulator.Sprites[127]);
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

        Assert.AreEqual(new Sprite() { Mode = 0x7f, Height = 0x40, Width = 0x40 }, emulator.Sprites[0]);
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

        Assert.AreEqual(new Sprite() { Mode = 0x7f, Height = 0x40, Width = 0x40 }, emulator.Sprites[1]);
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

        Assert.AreEqual(new Sprite() { Mode = 0x7f, Height = 0x40, Width = 0x40 }, emulator.Sprites[127]);
    }

    [TestMethod]
    public async Task Mixed_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc00 + 7;
        emulator.A = 0x41;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { PaletteOffset = 0x01, Height = 16, Width = 8, Mode = 0x0010 }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task Mixed_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 8;
        emulator.Vera.Data0_Address = 0x1fc08 + 7;
        emulator.A = 0x41;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                eor #01
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { PaletteOffset = 0x01, Height = 16, Width = 8, Mode = 0x0010 }, emulator.Sprites[1]);
        Assert.AreEqual(new Sprite() { PaletteOffset = 0x00, Height = 16, Width = 8, Mode = 0x0010 }, emulator.Sprites[2]);
    }

    [TestMethod]
    public async Task Mixed_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fff8 + 7;
        emulator.A = 0x41;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { PaletteOffset = 0x01, Height = 16, Width = 8, Mode = 0x0010 }, emulator.Sprites[127]);
    }
}
