using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class Vera_Initialise
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
    public async Task Data_HighAddress()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 1;
        emulator.Vera.Data0_Address = 0x12345;

        emulator.Vera.Data1_Step = 2;
        emulator.Vera.Data1_Address = 0x1ffff;

        emulator.Vera.Vram[0x12345] = 0xee;
        emulator.Vera.Vram[0x1ffff] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0xee, emulator.Memory[0x9F23]);

        Assert.AreEqual(0x45, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x23, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x11, emulator.Memory[0x9F22]);

        Assert.AreEqual(0x12345, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x01, emulator.Vera.Data0_Step);

        Assert.AreEqual(0xff, emulator.Memory[0x9F24]);
        Assert.AreEqual(0x1ffff, emulator.Vera.Data1_Address);
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
        emulator.Vera.Dc_HScale = 0x010000; // 1:1
        emulator.Vera.Dc_VScale = 0x010000;
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
        Assert.AreEqual(0x80, emulator.Memory[0x9F2A]);
        Assert.AreEqual(0x80, emulator.Memory[0x9F2B]);
        Assert.AreEqual(0x03, emulator.Memory[0x9F2C]);
    }

    [TestMethod]
    public async Task DcSel1()
    {
        var emulator = new Emulator();

        emulator.Vera.DcSel = true;
        emulator.Vera.Dc_HScale = 0x010000; // not real values
        emulator.Vera.Dc_VScale = 0x010000;
        emulator.Vera.Dc_Border = 0x03;
        emulator.Vera.Dc_HStart = 0x44;
        emulator.Vera.Dc_HStop = 0x55;
        emulator.Vera.Dc_VStart = 0x66;
        emulator.Vera.Dc_VStop = 0x77;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x11, emulator.Memory[0x9F29]);
        Assert.AreEqual(0x15, emulator.Memory[0x9F2A]);
        Assert.AreEqual(0x33, emulator.Memory[0x9F2B]);
        Assert.AreEqual(0x3b, emulator.Memory[0x9F2C]);
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

    [TestMethod]
    public async Task Layer0_Config_Height()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_MapHeight = 0x3;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0b11000000, emulator.Memory[0x9f2d]);
    }

    [TestMethod]
    public async Task Layer0_Config_Width()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_MapWidth = 0x3;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0b00110000, emulator.Memory[0x9f2d]);
    }

    [TestMethod]
    public async Task Layer0_Config_YShift_W0_H0()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_MapWidth = 0x0;
        emulator.Vera.Layer0_TileHeight = 0x0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(5, emulator.Vera.Layer0_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer0_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_HShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_VShift);
    }

    [TestMethod]
    public async Task Layer0_Config_YShift_W1_H0()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_MapWidth = 0x1;
        emulator.Vera.Layer0_TileHeight = 0x0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(6, emulator.Vera.Layer0_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer0_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_HShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_VShift);
    }

    [TestMethod]
    public async Task Layer0_Config_YShift_W2_H0()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_MapWidth = 0x2;
        emulator.Vera.Layer0_TileHeight = 0x0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(7, emulator.Vera.Layer0_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer0_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_HShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_VShift);
    }

    [TestMethod]
    public async Task Layer0_Config_YShift_W3_H0()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_MapWidth = 0x3;
        emulator.Vera.Layer0_TileHeight = 0x0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(8, emulator.Vera.Layer0_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer0_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_HShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_VShift);
    }

    [TestMethod]
    public async Task Layer0_Config_YShift_W0_H1()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_MapWidth = 0x0;
        emulator.Vera.Layer0_TileHeight = 0x1;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(5, emulator.Vera.Layer0_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer0_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_HShift);
        Assert.AreEqual(4, emulator.Vera.Layer0_Tile_VShift);
    }

    [TestMethod]
    public async Task Layer0_Config_YShift_W1_H1()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_MapWidth = 0x1;
        emulator.Vera.Layer0_TileHeight = 0x1;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(6, emulator.Vera.Layer0_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer0_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_HShift);
        Assert.AreEqual(4, emulator.Vera.Layer0_Tile_VShift);
    }

    [TestMethod]
    public async Task Layer0_Config_YShift_W2_H1()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_MapWidth = 0x2;
        emulator.Vera.Layer0_TileHeight = 0x1;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(7, emulator.Vera.Layer0_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer0_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_HShift);
        Assert.AreEqual(4, emulator.Vera.Layer0_Tile_VShift);
    }

    [TestMethod]
    public async Task Layer0_Config_YShift_W3_H1()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_MapWidth = 0x3;
        emulator.Vera.Layer0_TileHeight = 0x1;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(8, emulator.Vera.Layer0_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer0_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer0_Tile_HShift);
        Assert.AreEqual(4, emulator.Vera.Layer0_Tile_VShift);
    }

    [TestMethod]
    public async Task Layer0_Config_Bitmap()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_BitMapMode = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0b00000100, emulator.Memory[0x9f2d]);
    }

    [TestMethod]
    public async Task Layer0_Config_ColourDepth()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_ColourDepth = 0x3;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0b00000011, emulator.Memory[0x9f2d]);
    }

    [TestMethod]
    public async Task Layer0_Config_All()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_ColourDepth = 0x3;
        emulator.Vera.Layer0_BitMapMode = true;
        emulator.Vera.Layer0_MapWidth = 0x3;
        emulator.Vera.Layer0_MapHeight = 0x3;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0b11110111, emulator.Memory[0x9f2d]);
    }

    [TestMethod]
    public async Task Layer0_MapAddress()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_MapAddress = 0x10000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x80, emulator.Memory[0x9f2e]);
    }

    [TestMethod]
    public async Task Layer0_TileAddress()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_TileAddress = 0x10000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x80, emulator.Memory[0x9f2f]);
    }

    [TestMethod]
    public async Task Layer0_TileAddress_Full()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_TileAddress = 0x1ffff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0xfc, emulator.Memory[0x9f2f]);
    }

    [TestMethod]
    public async Task Layer0_TileAddress_TileHeight()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_TileHeight = 1;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x02, emulator.Memory[0x9f2f]);
    }

    [TestMethod]
    public async Task Layer0_TileAddress_TileWidth()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_TileWidth = 1;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x01, emulator.Memory[0x9f2f]);
    }

    [TestMethod]
    public async Task Layer0_TileAddress_All()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_TileWidth = 1;
        emulator.Vera.Layer0_TileHeight = 1;
        emulator.Vera.Layer0_TileAddress = 0x1ffff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0xff, emulator.Memory[0x9f2f]);
    }

    [TestMethod]
    public async Task Layer0_HScroll()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_HScroll = 0xffed;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0xed, emulator.Memory[0x9f30]);
        Assert.AreEqual(0x0f, emulator.Memory[0x9f31]);
    }

    [TestMethod]
    public async Task Layer0_VScroll()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer0_VScroll = 0xffed;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0xed, emulator.Memory[0x9f32]);
        Assert.AreEqual(0x0f, emulator.Memory[0x9f33]);
    }

    [TestMethod]
    public async Task Layer1_Config_Height()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_MapHeight = 0x3;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0b11000000, emulator.Memory[0x9f34]);
    }

    [TestMethod]
    public async Task Layer1_Config_Width()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_MapWidth = 0x3;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0b00110000, emulator.Memory[0x9f34]);
    }

    [TestMethod]
    public async Task Layer1_Config_YShift_W0_H0()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_MapWidth = 0x0;
        emulator.Vera.Layer1_TileHeight = 0x0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(5, emulator.Vera.Layer1_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer1_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer1_Tile_HShift);
        Assert.AreEqual(3, emulator.Vera.Layer1_Tile_VShift);
    }

    [TestMethod]
    public async Task Layer1_Config_YShift_W1_H0()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_MapWidth = 0x1;
        emulator.Vera.Layer1_TileHeight = 0x0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(6, emulator.Vera.Layer1_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer1_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer1_Tile_HShift);
        Assert.AreEqual(3, emulator.Vera.Layer1_Tile_VShift);
    }

    [TestMethod]
    public async Task Layer1_Config_YShift_W2_H0()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_MapWidth = 0x2;
        emulator.Vera.Layer1_TileHeight = 0x0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(7, emulator.Vera.Layer1_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer1_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer1_Tile_HShift);
        Assert.AreEqual(3, emulator.Vera.Layer1_Tile_VShift);
    }

    [TestMethod]
    public async Task Layer1_Config_YShift_W3_H0()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_MapWidth = 0x3;
        emulator.Vera.Layer1_TileHeight = 0x0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(8, emulator.Vera.Layer1_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer1_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer1_Tile_HShift);
        Assert.AreEqual(3, emulator.Vera.Layer1_Tile_VShift);
    }

    [TestMethod]
    public async Task Layer1_Config_YShift_W0_H1()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_MapWidth = 0x0;
        emulator.Vera.Layer1_TileHeight = 0x1;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(5, emulator.Vera.Layer1_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer1_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer1_Tile_HShift);
        Assert.AreEqual(4, emulator.Vera.Layer1_Tile_VShift);
    }

    [TestMethod]
    public async Task Layer1_Config_YShift_W1_H1()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_MapWidth = 0x1;
        emulator.Vera.Layer1_TileHeight = 0x1;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(6, emulator.Vera.Layer1_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer1_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer1_Tile_HShift);
        Assert.AreEqual(4, emulator.Vera.Layer1_Tile_VShift);
    }

    [TestMethod]
    public async Task Layer1_Config_YShift_W2_H1()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_MapWidth = 0x2;
        emulator.Vera.Layer1_TileHeight = 0x1;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(7, emulator.Vera.Layer1_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer1_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer1_Tile_HShift);
        Assert.AreEqual(4, emulator.Vera.Layer1_Tile_VShift);
    }

    [TestMethod]
    public async Task Layer1_Config_YShift_W3_H1()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_MapWidth = 0x3;
        emulator.Vera.Layer1_TileHeight = 0x1;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(8, emulator.Vera.Layer1_Map_HShift);
        Assert.AreEqual(5, emulator.Vera.Layer1_Map_VShift);
        Assert.AreEqual(3, emulator.Vera.Layer1_Tile_HShift);
        Assert.AreEqual(4, emulator.Vera.Layer1_Tile_VShift);
    }

    [TestMethod]
    public async Task Layer1_Config_Bitmap()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_BitMapMode = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0b00000100, emulator.Memory[0x9f34]);
    }

    [TestMethod]
    public async Task Layer1_Config_ColourDepth()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_ColourDepth = 0x3;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0b00000011, emulator.Memory[0x9f34]);
    }

    [TestMethod]
    public async Task Layer1_Config_All()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_ColourDepth = 0x3;
        emulator.Vera.Layer1_BitMapMode = true;
        emulator.Vera.Layer1_MapWidth = 0x3;
        emulator.Vera.Layer1_MapHeight = 0x3;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0b11110111, emulator.Memory[0x9f34]);
    }

    [TestMethod]
    public async Task Layer1_MapAddress()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_MapAddress = 0x10000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x80, emulator.Memory[0x9f35]);
    }

    [TestMethod]
    public async Task Layer1_TileAddress()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_TileAddress = 0x10000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x80, emulator.Memory[0x9f36]);
    }

    [TestMethod]
    public async Task Layer1_TileAddress_Full()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_TileAddress = 0x1ffff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0xfc, emulator.Memory[0x9f36]);
    }

    [TestMethod]
    public async Task Layer1_TileAddress_TileHeight()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_TileHeight = 1;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x02, emulator.Memory[0x9f36]);
    }

    [TestMethod]
    public async Task Layer1_TileAddress_TileWidth()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_TileWidth = 1;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x01, emulator.Memory[0x9f36]);
    }

    [TestMethod]
    public async Task Layer1_TileAddress_All()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_TileWidth = 1;
        emulator.Vera.Layer1_TileHeight = 1;
        emulator.Vera.Layer1_TileAddress = 0x1ffff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0xff, emulator.Memory[0x9f36]);
    }

    [TestMethod]
    public async Task Layer1_HScroll()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_HScroll = 0xffed;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0xed, emulator.Memory[0x9f37]);
        Assert.AreEqual(0x0f, emulator.Memory[0x9f38]);
    }

    [TestMethod]
    public async Task Layer1_VScroll()
    {
        var emulator = new Emulator();

        emulator.Vera.Layer1_VScroll = 0xffed;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0xed, emulator.Memory[0x9f39]);
        Assert.AreEqual(0x0f, emulator.Memory[0x9f3a]);
    }

    [TestMethod]
    public async Task Interrupt_Vsync()
    {
        var emulator = new Emulator();

        emulator.Vera.Interrupt_VSync = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x01, emulator.Memory[0x9f26]);
        Assert.AreEqual(0x00, emulator.Memory[0x9f28]);
    }

    [TestMethod]
    public async Task Interrupt_Line()
    {
        var emulator = new Emulator();

        emulator.Vera.Interrupt_Line = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x02, emulator.Memory[0x9f26]);
        Assert.AreEqual(0x00, emulator.Memory[0x9f28]);
    }

    [TestMethod]
    public async Task Interrupt_SpCol()
    {
        var emulator = new Emulator();

        emulator.Vera.Interrupt_SpCol = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x04, emulator.Memory[0x9f26]);
        Assert.AreEqual(0x00, emulator.Memory[0x9f28]);
    }

    [TestMethod]
    public async Task Interrupt_Aflow()
    {
        var emulator = new Emulator();

        emulator.Vera.Interrupt_AFlow = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x08, emulator.Memory[0x9f26]);
        Assert.AreEqual(0x00, emulator.Memory[0x9f28]);
    }

    [TestMethod]
    public async Task Interrupt_LineNum()
    {
        var emulator = new Emulator();

        emulator.Vera.Interrupt_LineNum = 0x1ff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x80, emulator.Memory[0x9f26]);
        Assert.AreEqual(0xff, emulator.Memory[0x9f28]);
    }

    [TestMethod]
    public async Task Palette_1()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(new Common.PixelRgba(0xff, 0xff, 0xff), emulator.Palette[1]);
    }


    [TestMethod]
    public async Task Palette_2()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(new Common.PixelRgba(0x88, 0x00, 0x00), emulator.Palette[2]);
    }


    [TestMethod]
    public async Task Palette_3()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(new Common.PixelRgba(0xaa, 0xff, 0xee), emulator.Palette[3]);
    }

    [TestMethod]
    public async Task Palette_255()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(new Common.PixelRgba(0xff, 0x00, 0xbb), emulator.Palette[255]);
    }
}