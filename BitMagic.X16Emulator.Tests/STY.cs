using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class STY
{
    [TestMethod]
    public async Task ZeroPage()
    {
        var emulator = new Emulator();

        emulator.Y = 0x44;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                sty $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x84, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x00, 0x44, 0x813, 3);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZeroPageX()
    {
        var emulator = new Emulator();

        emulator.X = 0x10;
        emulator.Y = 0x44;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                sty $10, X
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x94, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x20]);
        emulator.AssertState(0x00, 0x10, 0x44, 0x813, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZeroPageX_Wrap()
    {
        var emulator = new Emulator();

        emulator.X = 0x70;
        emulator.Y = 0x44;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                sty $a0, X
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x94, emulator.Memory[0x810]);
        Assert.AreEqual(0xa0, emulator.Memory[0x811]);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x70, 0x44, 0x813, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Absolute()
    {
        var emulator = new Emulator();

        emulator.Y = 0x44;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                sty $100
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x8c, emulator.Memory[0x810]);
        Assert.AreEqual(0x00, emulator.Memory[0x811]);
        Assert.AreEqual(0x01, emulator.Memory[0x812]);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x100]);
        emulator.AssertState(0x00, 0x00, 0x44, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }
}
