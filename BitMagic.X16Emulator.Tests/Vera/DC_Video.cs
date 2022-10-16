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

    [TestMethod]
    public async Task CurrentField_Odd()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sei
                lda #$11        ; set video output to some test value
                sta DC_VIDEO
                lda #11
                sta IRQLINE_L
                lda #$2         ; line interrupts
                sta IEN
                wai
                lda DC_VIDEO
                stp",
        emulator);

        Assert.AreEqual(0b10010001, emulator.Memory[0x9f29]);
        Assert.AreEqual(0b10010001, emulator.A);
    }

    [TestMethod]
    public async Task CurrentField_Odd_2Frames()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sei
                lda #$11        ; set video output to some test value
                sta DC_VIDEO
                lda #11
                sta IRQLINE_L
                lda #$2         ; line interrupts
                sta IEN
                wai
                sta ISR
                wai
                lda DC_VIDEO
                stp",
        emulator);

        Assert.AreEqual(0b10010001, emulator.Memory[0x9f29]);
        Assert.AreEqual(0b10010001, emulator.A);
    }

    [TestMethod]
    public async Task CurrentField_Even()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sei
                lda #$11        ; set video output to some test value
                sta DC_VIDEO
                lda #12
                sta IRQLINE_L
                lda #$2         ; line interrupts
                sta IEN
                wai
                lda DC_VIDEO
                stp",
        emulator);

        Assert.AreEqual(0b00010001, emulator.Memory[0x9f29]);
        Assert.AreEqual(0b00010001, emulator.A);
    }

    [TestMethod]
    public async Task CurrentField_Even_2Frames()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sei
                lda #$11        ; set video output to some test value
                sta DC_VIDEO
                lda #12
                sta IRQLINE_L
                lda #$2         ; line interrupts
                sta IEN
                wai
                sta ISR
                wai
                lda DC_VIDEO
                stp",
        emulator);

        Assert.AreEqual(0b00010001, emulator.Memory[0x9f29]);
        Assert.AreEqual(0b00010001, emulator.A);
    }
}