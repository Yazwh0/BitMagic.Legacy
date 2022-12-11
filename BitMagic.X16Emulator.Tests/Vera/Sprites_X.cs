using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera;

[TestClass]
internal class Sprite_X
{
    [TestMethod]
    public async Task SetX_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc02;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { X = 0xff }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetX_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc0a;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { X = 0xff }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetX_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fffa;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { X = 0xff }, emulator.Sprites[127]);
    }

    [TestMethod]
    public async Task SetX_High_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc03;
        emulator.A = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { X = 0x300 }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetX_High_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc0b;
        emulator.A = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { X = 0x300 }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetX_High_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fffb;
        emulator.A = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { X = 0x300 }, emulator.Sprites[127]);
    }
    [TestMethod]
    public async Task SetX_High_Clip_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc03;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { X = 0x300 }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetX_High_Clip_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc0b;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { X = 0x300 }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetX_High_Clip_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fffb;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { X = 0x300 }, emulator.Sprites[127]);
    }

    [TestMethod]
    public async Task SetX_High_LowSet_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc03;
        emulator.A = 0x03;
        emulator.Sprites[0].X = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { X = 0x3ff }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetX_High_LowSet_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc0b;
        emulator.A = 0x03;
        emulator.Sprites[1].X = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { X = 0x3ff }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetX_High_LowSet_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fffb;
        emulator.A = 0x03;
        emulator.Sprites[127].X = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { X = 0x3ff }, emulator.Sprites[127]);
    }

    [TestMethod]
    public async Task SetX_Low_HighSet_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc02;
        emulator.A = 0xff;
        emulator.Sprites[0].X = 0x300;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { X = 0x3ff }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetX_Low_HighSet_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc0a;
        emulator.A = 0xff;
        emulator.Sprites[1].X = 0x300;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { X = 0x3ff }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetX_Low_HighSet_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fffa;
        emulator.A = 0xff;
        emulator.Sprites[127].X = 0x300;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { X = 0x3ff }, emulator.Sprites[127]);
    }

}
