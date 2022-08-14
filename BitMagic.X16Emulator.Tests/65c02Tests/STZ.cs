using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class STZ
{
    [TestMethod]
    public async Task ZeroPage()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0xff;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                stz $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x64, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        Assert.AreEqual(0x00, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZeroPageX()
    {
        var emulator = new Emulator();

        emulator.X = 0x10;
        emulator.Memory[0x20] = 0xff;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                stz $10, X
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x74, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        Assert.AreEqual(0x00, emulator.Memory[0x20]);
        emulator.AssertState(0x00, 0x10, 0x00, 0x813, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZeroPageX_Wrap()
    {
        var emulator = new Emulator();

        emulator.X = 0x70;
        emulator.Memory[0x10] = 0xff;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                stz $a0, X
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x74, emulator.Memory[0x810]);
        Assert.AreEqual(0xa0, emulator.Memory[0x811]);

        // emulation
        Assert.AreEqual(0x00, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x70, 0x00, 0x813, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Absolute()
    {
        var emulator = new Emulator();

        emulator.Memory[0x100] = 0xff;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                stz $100
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x9c, emulator.Memory[0x810]);
        Assert.AreEqual(0x00, emulator.Memory[0x811]);
        Assert.AreEqual(0x01, emulator.Memory[0x812]);

        // emulation
        Assert.AreEqual(0x00, emulator.Memory[0x100]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsoluteX()
    {
        var emulator = new Emulator();

        emulator.X = 0x05;
        emulator.Memory[0x105] = 0xff;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                stz $100, X
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x9e, emulator.Memory[0x810]);
        Assert.AreEqual(0x00, emulator.Memory[0x811]);
        Assert.AreEqual(0x01, emulator.Memory[0x812]);

        // emulation
        Assert.AreEqual(0x00, emulator.Memory[0x105]);
        emulator.AssertState(0x00, 0x05, 0x00, 0x814, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ReadOnly_Abs()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0000] = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stz $c000
                stp",
                emulator);

        Assert.AreEqual(0x10, emulator.RomBank[0x0000]);
    }

    [TestMethod]
    public async Task ReadOnly_AbsX()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0002] = 0x10;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stz $c000, x
                stp",
                emulator);

        Assert.AreEqual(0x10, emulator.RomBank[0x0002]);
    }
}
