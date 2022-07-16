using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class STX
{
    [TestMethod]
    public async Task ZeroPage()
    {
        var emulator = new Emulator();

        emulator.X = 0x44;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                stx $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x86, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x44, 0x00, 0x813, 3);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZeroPageY()
    {
        var emulator = new Emulator();

        emulator.X = 0x44;
        emulator.Y = 0x10;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                stx $10, Y
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x96, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x20]);
        emulator.AssertState(0x00, 0x44, 0x10, 0x813, 4);
        emulator.AssertFlags(false, false, false, false);
    }


    [TestMethod]
    public async Task ZeroPageY_Wrap()
    {
        var emulator = new Emulator();

        emulator.X = 0x44;
        emulator.Y = 0x70;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                stx $a0, Y
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x96, emulator.Memory[0x810]);
        Assert.AreEqual(0xa0, emulator.Memory[0x811]);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x44, 0x70, 0x813, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Absolute()
    {
        var emulator = new Emulator();

        emulator.X = 0x44;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                stx $100
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x8e, emulator.Memory[0x810]);
        Assert.AreEqual(0x00, emulator.Memory[0x811]);
        Assert.AreEqual(0x01, emulator.Memory[0x812]);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x100]);
        emulator.AssertState(0x00, 0x44, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }
}
