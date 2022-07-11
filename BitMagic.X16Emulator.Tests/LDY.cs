using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class LDY
{
    [TestMethod]
    public async Task Immediate()
    {
        var emulator = await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                ldy #$44
                stp");

        // compilation
        Assert.AreEqual(0xa0, emulator.Memory[0x810]);
        Assert.AreEqual(0x44, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x44, 0x813, 2);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Immediate_ZeroFlag()
    {
        var emulator = await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                ldy #$0
                stp");

        // compilation
        Assert.AreEqual(0xa0, emulator.Memory[0x810]);
        Assert.AreEqual(0x00, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Immediate_NegativeFlag()
    {
        var emulator = await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                ldy #$ff
                stp");

        // compilation
        Assert.AreEqual(0xa0, emulator.Memory[0x810]);
        Assert.AreEqual(0xff, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0xff, 0x813, 2);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task ZeroPage()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0x44;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                ldy $10
                stp", emulator);

        // compilation
        Assert.AreEqual(0xa4, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x44, 0x813, 3);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZeroPageX()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0x44;
        emulator.X = 2;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                ldy $10, X
                stp", emulator);

        // compilation
        Assert.AreEqual(0xb4, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x00, 0x02, 0x44, 0x813, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZeroPageX_Wrap()
    {
        var emulator = new Emulator();

        emulator.Memory[0x12] = 0x44;
        emulator.X = 0x72;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                ldy $a0, X
                stp", emulator);

        // compilation
        Assert.AreEqual(0xb4, emulator.Memory[0x810]);
        Assert.AreEqual(0xa0, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x00, 0x72, 0x44, 0x813, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Absolute()
    {
        var emulator = new Emulator();

        emulator.Memory[0x400] = 0x44;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                ldy $400
                stp", emulator);

        // compilation
        Assert.AreEqual(0xac, emulator.Memory[0x810]);
        Assert.AreEqual(0x00, emulator.Memory[0x811]);
        Assert.AreEqual(0x04, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x44, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsoluteX()
    {
        var emulator = new Emulator();

        emulator.Memory[0x402] = 0x44;
        emulator.X = 2;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                ldy $400, X
                stp", emulator);

        // compilation
        Assert.AreEqual(0xbc, emulator.Memory[0x810]);
        Assert.AreEqual(0x00, emulator.Memory[0x811]);
        Assert.AreEqual(0x04, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x00, 0x02, 0x44, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }
}
