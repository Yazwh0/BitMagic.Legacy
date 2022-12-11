using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera;

[TestClass]
internal class Sprite_Y
{
    [TestMethod]
    public async Task SetY_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc04;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Y = 0xff }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetY_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc0c;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Y = 0xff }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetY_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fffc;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Y = 0xff }, emulator.Sprites[127]);
    }

    [TestMethod]
    public async Task SetY_High_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc05;
        emulator.A = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Y = 0x300 }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetY_High_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc0d;
        emulator.A = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Y = 0x300 }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetY_High_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fffd;
        emulator.A = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Y = 0x300 }, emulator.Sprites[127]);
    }
    [TestMethod]
    public async Task SetY_High_Clip_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc05;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Y = 0x300 }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetY_High_Clip_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc0d;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Y = 0x300 }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetY_High_Clip_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fffd;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Y = 0x300 }, emulator.Sprites[127]);
    }

    [TestMethod]
    public async Task SetY_High_LowSet_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc05;
        emulator.A = 0x03;
        emulator.Sprites[0].Y = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Y = 0x3ff }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetY_High_LowSet_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc0d;
        emulator.A = 0x03;
        emulator.Sprites[1].Y = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Y = 0x3ff }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetY_High_LowSet_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fffd;
        emulator.A = 0x03;
        emulator.Sprites[127].Y = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Y = 0x3ff }, emulator.Sprites[127]);
    }

    [TestMethod]
    public async Task SetY_Low_HighSet_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc04;
        emulator.A = 0xff;
        emulator.Sprites[0].Y = 0x300;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Y = 0x3ff }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetY_Low_HighSet_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc0c;
        emulator.A = 0xff;
        emulator.Sprites[1].Y = 0x300;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Y = 0x3ff }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetY_Low_HighSet_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fffc;
        emulator.A = 0xff;
        emulator.Sprites[127].Y = 0x300;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Y = 0x3ff }, emulator.Sprites[127]);
    }
}
