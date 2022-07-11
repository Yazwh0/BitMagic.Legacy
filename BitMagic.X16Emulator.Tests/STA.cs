using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class STA
{
    [TestMethod]
    public async Task ZeroPage()
    {
        var emulator = new Emulator();

        emulator.A = 0x44;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                sta $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x85, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x10]);
        emulator.AssertState(0x44, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZeroPageX()
    {
        var emulator = new Emulator();

        emulator.A = 0x44;
        emulator.X = 0x10;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                sta $10, X
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x95, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x20]);
        emulator.AssertState(0x44, 0x10, 0x00, 0x813, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZeroPageX_Wrap()
    {
        var emulator = new Emulator();

        emulator.A = 0x44;
        emulator.X = 0x70;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                sta $a0, X
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x95, emulator.Memory[0x810]);
        Assert.AreEqual(0xa0, emulator.Memory[0x811]);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x10]);
        emulator.AssertState(0x44, 0x70, 0x00, 0x813, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Absolute()
    {
        var emulator = new Emulator();

        emulator.A = 0x44;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                sta $100
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x8d, emulator.Memory[0x810]);
        Assert.AreEqual(0x00, emulator.Memory[0x811]);
        Assert.AreEqual(0x01, emulator.Memory[0x812]);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x100]);
        emulator.AssertState(0x44, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsoluteX()
    {
        var emulator = new Emulator();

        emulator.A = 0x44;
        emulator.X = 0x05;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                sta $100, X
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x9d, emulator.Memory[0x810]);
        Assert.AreEqual(0x00, emulator.Memory[0x811]);
        Assert.AreEqual(0x01, emulator.Memory[0x812]);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x105]);
        emulator.AssertState(0x44, 0x05, 0x00, 0x814, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsoluteY()
    {
        var emulator = new Emulator();

        emulator.A = 0x44;
        emulator.Y = 0x05;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                sta $100, Y
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x99, emulator.Memory[0x810]);
        Assert.AreEqual(0x00, emulator.Memory[0x811]);
        Assert.AreEqual(0x01, emulator.Memory[0x812]);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x105]);
        emulator.AssertState(0x44, 0x00, 0x05, 0x814, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task IndirectX()
    {
        var emulator = new Emulator();

        emulator.A = 0x44;
        emulator.X = 0x10;

        emulator.Memory[0x20] = 0x05;
        emulator.Memory[0x21] = 0x01;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                sta ($10, X)
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x81, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x105]);
        emulator.AssertState(0x44, 0x10, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task IndirectX_Wrap()
    {
        var emulator = new Emulator();

        emulator.A = 0x44;
        emulator.X = 0x70;

        emulator.Memory[0x10] = 0x05;
        emulator.Memory[0x11] = 0x01;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                sta ($a0, X)
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x81, emulator.Memory[0x810]);
        Assert.AreEqual(0xa0, emulator.Memory[0x811]);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x105]);
        emulator.AssertState(0x44, 0x70, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task IndirectY()
    {
        var emulator = new Emulator();

        emulator.A = 0x44;
        emulator.Y = 0x10;

        emulator.Memory[0x10] = 0x05;
        emulator.Memory[0x11] = 0x01;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                sta ($10), Y
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x91, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x115]);
        emulator.AssertState(0x44, 0x00, 0x10, 0x813);
        emulator.AssertFlags(false, false, false, false);
    }
}
