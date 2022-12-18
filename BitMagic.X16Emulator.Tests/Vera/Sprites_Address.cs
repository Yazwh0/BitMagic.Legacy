using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera;

[TestClass]
public class Sprites
{
    [TestMethod]
    public async Task SetAddress_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc00;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Address = 0x1fe0, Width = 8, Height = 8 }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetAddress_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc08;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Address = 0x1fe0, Width = 8, Height = 8 }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetAddress_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fff8;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Address = 0x1fe0, Width = 8, Height = 8 }, emulator.Sprites[127]);
    }

    [TestMethod]
    public async Task SetAddress_High_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc01;
        emulator.A = 0x0f;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Address = 0x1e000, Width = 8, Height = 8 }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetAddress_High_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc09;
        emulator.A = 0x0f;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Address = 0x1e000, Width = 8, Height = 8 }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetAddress_High_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fff9;
        emulator.A = 0x0f;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Address = 0x1e000, Width = 8, Height = 8 }, emulator.Sprites[127]);
    }

    [TestMethod]
    public async Task SetAddress_High_Clip_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc01;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Address = 0x1e000, Mode = 0x40, Width = 8, Height = 8 }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetAddress_High_Clip_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc09;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Address = 0x1e000, Mode = 0x40, Width = 8, Height = 8 }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetAddress_High_Clip_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fff9;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Address = 0x1e000, Mode = 0x40, Width = 8, Height = 8 }, emulator.Sprites[127]);
    }

    [TestMethod]
    public async Task SetAddress_High_LowSet_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc01;
        emulator.A = 0x0f;
        emulator.Sprites[0].Address = 0x1fe0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Address = 0x1ffe0, Width = 8, Height = 8 }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetAddress_High_LowSet_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc09;
        emulator.A = 0x0f;
        emulator.Sprites[1].Address = 0x1fe0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Address = 0x1ffe0, Width = 8, Height = 8 }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetAddress_High_LowSet_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fff9;
        emulator.A = 0x0f;
        emulator.Sprites[127].Address = 0x1fe0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Address = 0x1ffe0, Width = 8, Height = 8 }, emulator.Sprites[127]);
    }

    [TestMethod]
    public async Task SetAddress_Low_HighSet_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc00;
        emulator.A = 0xff;
        emulator.Sprites[0].Address = 0x1e000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Address = 0x1ffe0, Width = 8, Height = 8 }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetAddress_Low_HighSet_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc08;
        emulator.A = 0xff;
        emulator.Sprites[1].Address = 0x1e000;
        
        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Address = 0x1ffe0, Width = 8, Height = 8 }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetAddress_Low_HighSet_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fff8;
        emulator.A = 0xff;
        emulator.Sprites[127].Address = 0x1e000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Address = 0x1ffe0, Width = 8, Height = 8 }, emulator.Sprites[127]);
    }


    [TestMethod]
    public async Task SetAddress_High_Mode_0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc01;
        emulator.A = 0x80;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Mode = 0x40, Width = 8, Height = 8 }, emulator.Sprites[0]);
    }

    [TestMethod]
    public async Task SetAddress_High_Mode_1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fc09;
        emulator.A = 0x80;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Mode = 0x40, Width = 8, Height = 8 }, emulator.Sprites[1]);
    }

    [TestMethod]
    public async Task SetAddress_High_Mode_127()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x1fff9;
        emulator.A = 0x80;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DATA0
                stp",
                emulator);

        Assert.AreEqual(new Sprite() { Mode = 0x40, Width = 8, Height = 8 }, emulator.Sprites[127]);
    }
}