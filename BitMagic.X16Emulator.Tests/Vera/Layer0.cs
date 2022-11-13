using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera;

[TestClass]
public class Layer0
{
    [TestMethod]
    public async Task Config_ColourDepth()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L0_CONFIG
                stp",
                emulator);

        Assert.AreEqual(0x03, emulator.Vera.Layer0_ColourDepth);
        Assert.AreEqual(false, emulator.Vera.Layer0_BitMapMode);
        Assert.AreEqual(0x00, emulator.Vera.Layer0_MapWidth);
        Assert.AreEqual(0x00, emulator.Vera.Layer0_MapHeight);
        Assert.AreEqual(5, emulator.Vera.Layer0_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer0_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_HShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_VShift);

        Assert.AreEqual(0x03, emulator.Memory[0x9F2d]);
    }

    [TestMethod]
    public async Task Config_BitmapMode()
    {
        var emulator = new Emulator();

        emulator.A = 0x04;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L0_CONFIG
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Vera.Layer0_ColourDepth);
        Assert.AreEqual(true, emulator.Vera.Layer0_BitMapMode);
        Assert.AreEqual(0x00, emulator.Vera.Layer0_MapWidth);
        Assert.AreEqual(0x00, emulator.Vera.Layer0_MapHeight);
        Assert.AreEqual(5, emulator.Vera.Layer0_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer0_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_HShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_VShift);

        Assert.AreEqual(0x04, emulator.Memory[0x9F2d]);
    }

    [TestMethod]
    public async Task Config_MapWidth()
    {
        var emulator = new Emulator();

        emulator.A = 0x30;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L0_CONFIG
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Vera.Layer0_ColourDepth);
        Assert.AreEqual(false, emulator.Vera.Layer0_BitMapMode);
        Assert.AreEqual(0x03, emulator.Vera.Layer0_MapWidth);
        Assert.AreEqual(0x00, emulator.Vera.Layer0_MapHeight);
        Assert.AreEqual(5, emulator.Vera.Layer0_Map_HShift);
        Assert.AreEqual(8, emulator.Vera.Layer0_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_HShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_VShift);

        Assert.AreEqual(0x30, emulator.Memory[0x9F2d]);
    }

    [TestMethod]
    public async Task Config_MapHeight()
    {
        var emulator = new Emulator();

        emulator.A = 0xc0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L0_CONFIG
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Vera.Layer0_ColourDepth);
        Assert.AreEqual(false, emulator.Vera.Layer0_BitMapMode);
        Assert.AreEqual(0x00, emulator.Vera.Layer0_MapWidth);
        Assert.AreEqual(0x03, emulator.Vera.Layer0_MapHeight);
        Assert.AreEqual(8, emulator.Vera.Layer0_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer0_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_HShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_VShift);

        Assert.AreEqual(0xc0, emulator.Memory[0x9F2d]);
    }

    [TestMethod]
    public async Task Config_All()
    {
        var emulator = new Emulator();

        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L0_CONFIG
                stp",
                emulator);

        Assert.AreEqual(0x03, emulator.Vera.Layer0_ColourDepth);
        Assert.AreEqual(true, emulator.Vera.Layer0_BitMapMode);
        Assert.AreEqual(0x03, emulator.Vera.Layer0_MapWidth);
        Assert.AreEqual(0x03, emulator.Vera.Layer0_MapHeight);
        Assert.AreEqual(8, emulator.Vera.Layer0_Map_HShift);
        Assert.AreEqual(8, emulator.Vera.Layer0_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_HShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_VShift);

        Assert.AreEqual(0xff, emulator.Memory[0x9F2d]);
    }

    [TestMethod]
    public async Task MapBase()
    {
        var emulator = new Emulator();

        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L0_MAPBASE
                stp",
                emulator);

        Assert.AreEqual((UInt32)0x1fe00, emulator.Vera.Layer0_MapAddress);

        Assert.AreEqual(0xff, emulator.Memory[0x9F2e]);
    }

    [TestMethod]
    public async Task TileWidth()
    {
        var emulator = new Emulator();

        emulator.A = 0x01;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L0_TILEBASE
                stp",
                emulator);

        Assert.AreEqual(0x01, emulator.Vera.Layer0_TileWidth);
        Assert.AreEqual(0x00, emulator.Vera.Layer0_TileHeight);
        Assert.AreEqual((UInt32)0x00, emulator.Vera.Layer0_TileAddress);
        Assert.AreEqual(5, emulator.Vera.Layer0_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer0_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_HShift);
        Assert.AreEqual(4, emulator.Vera.Layer0_Tile_VShift);

        Assert.AreEqual(0x01, emulator.Memory[0x9F2f]);
    }

    [TestMethod]
    public async Task TileHeight()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L0_TILEBASE
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Vera.Layer0_TileWidth);
        Assert.AreEqual(0x01, emulator.Vera.Layer0_TileHeight);
        Assert.AreEqual((UInt32)0x00, emulator.Vera.Layer0_TileAddress);
        Assert.AreEqual(5, emulator.Vera.Layer0_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer0_Map_VShift);
        Assert.AreEqual(4, emulator.Vera.Layer0_Tile_HShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_VShift);

        Assert.AreEqual(0x02, emulator.Memory[0x9F2f]);
    }

    [TestMethod]
    public async Task TileAddress()
    {
        var emulator = new Emulator();

        emulator.A = 0xfc;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L0_TILEBASE
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Vera.Layer0_TileWidth);
        Assert.AreEqual(0x00, emulator.Vera.Layer0_TileHeight);
        Assert.AreEqual((UInt32)0x1f800, emulator.Vera.Layer0_TileAddress);

        Assert.AreEqual(0xfc, emulator.Memory[0x9F2f]);
    }

    [TestMethod]
    public async Task TileAll()
    {
        var emulator = new Emulator();

        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L0_TILEBASE
                stp",
                emulator);

        Assert.AreEqual(0x01, emulator.Vera.Layer0_TileWidth);
        Assert.AreEqual(0x01, emulator.Vera.Layer0_TileHeight);
        Assert.AreEqual((UInt32)0x1f800, emulator.Vera.Layer0_TileAddress);

        Assert.AreEqual(0xff, emulator.Memory[0x9F2f]);
    }

    [TestMethod]
    public async Task HScroll_L()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_HScroll = 0xf00;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L0_HSCROLL_L
                stp",
                emulator);

        Assert.AreEqual(0xfff, emulator.Vera.Layer0_HScroll);

        Assert.AreEqual(0xff, emulator.Memory[0x9F30]);
    }

    [TestMethod]
    public async Task HScroll_H()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_HScroll = 0x0ff;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L0_HSCROLL_H
                stp",
                emulator);

        Assert.AreEqual(0xfff, emulator.Vera.Layer0_HScroll);

        Assert.AreEqual(0x0f, emulator.Memory[0x9F31]);
    }

    [TestMethod]
    public async Task VScroll_L()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_VScroll = 0xf00;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L0_VSCROLL_L
                stp",
                emulator);

        Assert.AreEqual(0xfff, emulator.Vera.Layer0_VScroll);

        Assert.AreEqual(0xff, emulator.Memory[0x9F32]);
    }

    [TestMethod]
    public async Task VScroll_H()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_VScroll = 0x0ff;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L0_VSCROLL_H
                stp",
                emulator);

        Assert.AreEqual(0xfff, emulator.Vera.Layer0_VScroll);

        Assert.AreEqual(0x0f, emulator.Memory[0x9F33]);
    }
}