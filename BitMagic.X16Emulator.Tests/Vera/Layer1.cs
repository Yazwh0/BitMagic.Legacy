﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class Layer1
{
    [TestMethod]
    public async Task Config_ColourDepth()
    {
        var emulator = new Emulator();

        emulator.A = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L1_CONFIG
                stp",
                emulator);

        Assert.AreEqual(0x03, emulator.Vera.Layer1_ColourDepth);
        Assert.AreEqual(false, emulator.Vera.Layer1_BitMapMode);
        Assert.AreEqual(0x00, emulator.Vera.Layer1_MapWidth);
        Assert.AreEqual(0x00, emulator.Vera.Layer1_MapHeight);

        Assert.AreEqual(0x03, emulator.Memory[0x9f34]);
    }

    [TestMethod]
    public async Task Config_BitmapMode()
    {
        var emulator = new Emulator();

        emulator.A = 0x04;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L1_CONFIG
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Vera.Layer1_ColourDepth);
        Assert.AreEqual(true, emulator.Vera.Layer1_BitMapMode);
        Assert.AreEqual(0x00, emulator.Vera.Layer1_MapWidth);
        Assert.AreEqual(0x00, emulator.Vera.Layer1_MapHeight);

        Assert.AreEqual(0x04, emulator.Memory[0x9f34]);
    }

    [TestMethod]
    public async Task Config_MapWidth()
    {
        var emulator = new Emulator();

        emulator.A = 0x30;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L1_CONFIG
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Vera.Layer1_ColourDepth);
        Assert.AreEqual(false, emulator.Vera.Layer1_BitMapMode);
        Assert.AreEqual(0x03, emulator.Vera.Layer1_MapWidth);
        Assert.AreEqual(0x00, emulator.Vera.Layer1_MapHeight);

        Assert.AreEqual(0x30, emulator.Memory[0x9f34]);
    }

    [TestMethod]
    public async Task Config_MapHeight()
    {
        var emulator = new Emulator();

        emulator.A = 0xc0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L1_CONFIG
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Vera.Layer1_ColourDepth);
        Assert.AreEqual(false, emulator.Vera.Layer1_BitMapMode);
        Assert.AreEqual(0x00, emulator.Vera.Layer1_MapWidth);
        Assert.AreEqual(0x03, emulator.Vera.Layer1_MapHeight);

        Assert.AreEqual(0xc0, emulator.Memory[0x9f34]);
    }

    [TestMethod]
    public async Task Config_All()
    {
        var emulator = new Emulator();

        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L1_CONFIG
                stp",
                emulator);

        Assert.AreEqual(0x03, emulator.Vera.Layer1_ColourDepth);
        Assert.AreEqual(true, emulator.Vera.Layer1_BitMapMode);
        Assert.AreEqual(0x03, emulator.Vera.Layer1_MapWidth);
        Assert.AreEqual(0x03, emulator.Vera.Layer1_MapHeight);

        Assert.AreEqual(0xff, emulator.Memory[0x9f34]);
    }

    [TestMethod]
    public async Task MapBase()
    {
        var emulator = new Emulator();

        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L1_MAPBASE
                stp",
                emulator);

        Assert.AreEqual((UInt32)0x1fe00, emulator.Vera.Layer1_MapAddress);

        Assert.AreEqual(0xff, emulator.Memory[0x9F35]);
    }

    [TestMethod]
    public async Task TileWidth()
    {
        var emulator = new Emulator();

        emulator.A = 0x01;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L1_TILEBASE
                stp",
                emulator);

        Assert.AreEqual(0x01, emulator.Vera.Layer1_TileWidth);
        Assert.AreEqual(0x00, emulator.Vera.Layer1_TileHeight);
        Assert.AreEqual((UInt32)0x00, emulator.Vera.Layer1_TileAddress);

        Assert.AreEqual(0x01, emulator.Memory[0x9F36]);
    }

    [TestMethod]
    public async Task TileHeight()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L1_TILEBASE
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Vera.Layer1_TileWidth);
        Assert.AreEqual(0x01, emulator.Vera.Layer1_TileHeight);
        Assert.AreEqual((UInt32)0x00, emulator.Vera.Layer1_TileAddress);

        Assert.AreEqual(0x02, emulator.Memory[0x9F36]);
    }

    [TestMethod]
    public async Task TileAddress()
    {
        var emulator = new Emulator();

        emulator.A = 0xfc;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L1_TILEBASE
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Vera.Layer1_TileWidth);
        Assert.AreEqual(0x00, emulator.Vera.Layer1_TileHeight);
        Assert.AreEqual((UInt32)0x1f800, emulator.Vera.Layer1_TileAddress);

        Assert.AreEqual(0xfc, emulator.Memory[0x9F36]);
    }


    [TestMethod]
    public async Task TileAll()
    {
        var emulator = new Emulator();

        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta L1_TILEBASE
                stp",
                emulator);

        Assert.AreEqual(0x01, emulator.Vera.Layer1_TileWidth);
        Assert.AreEqual(0x01, emulator.Vera.Layer1_TileHeight);
        Assert.AreEqual((UInt32)0x1f800, emulator.Vera.Layer1_TileAddress);

        Assert.AreEqual(0xff, emulator.Memory[0x9F36]);
    }
}