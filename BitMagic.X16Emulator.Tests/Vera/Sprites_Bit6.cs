using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera;

[TestClass]
public class Sprites_Bit6
{
    [TestMethod]
    public async Task SetFlip_Mode_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc00 + 6;
        emulator.A = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Mode = 0x03 }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetFlip_Mode_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc08 + 6;
        emulator.A = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Mode = 0x03 }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetFlip_Mode_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fff8 + 6;
        emulator.A = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Mode = 0x03 }, emulator.Sprites[127]);
    }

    [TestMethod]
    public async Task SetFlip_ExistingMode_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc00 + 6;
        emulator.A = 0x03;
        emulator.Sprites[0].Mode = 0x7c;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Mode = 0x7f }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetFlip_ExistingMode_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc08 + 6;
        emulator.A = 0x03;
        emulator.Sprites[1].Mode = 0x7c;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Mode = 0x7f }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetFlip_ExistingMode_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fff8 + 6;
        emulator.A = 0x03;
        emulator.Sprites[127].Mode = 0x7c;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Mode = 0x7f }, emulator.Sprites[127]);
    }

    [TestMethod]
    public async Task SetDepth_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc00 + 6;
        emulator.A = 0x0c;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Depth = 0x03 }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetDepth_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc08 + 6;
        emulator.A = 0x0c;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Depth = 0x03 }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetDepth_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fff8 + 6;
        emulator.A = 0x0c;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Depth = 0x03 }, emulator.Sprites[127]);
    }

    [TestMethod]
    public async Task SetCollisionMask_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc00 + 6;
        emulator.A = 0xf0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { CollisionMask = 0x0f }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetCollisionMask_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc08 + 6;
        emulator.A = 0xf0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { CollisionMask = 0x0f }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetCollisionMask_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fff8 + 6;
        emulator.A = 0xf0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { CollisionMask = 0x0f }, emulator.Sprites[127]);
    }
}