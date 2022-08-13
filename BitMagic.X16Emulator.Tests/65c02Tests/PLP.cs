using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class PLP
{
    [TestMethod]
    public async Task Plp_Carry()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1ff] = 0x01 + 0x30;
        emulator.StackPointer = 0x1fe; // one item on the stack

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                plp
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x28, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 4);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Plp_Zero()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1ff] = 0x02 + 0x30;
        emulator.StackPointer = 0x1fe; // one item on the stack

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                plp
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x28, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 4);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Plp_Interrupt()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1ff] = 0x04 + 0x30;
        emulator.StackPointer = 0x1fe; // one item on the stack

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                plp
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x28, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 4);
        emulator.AssertFlags(false, false, false, false, true);
    }

    [TestMethod]
    public async Task Plp_Decimal ()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1ff] = 0x08 + 0x30;
        emulator.StackPointer = 0x1fe; // one item on the stack

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                plp
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x28, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 4);
        emulator.AssertFlags(false, false, false, false, false, true);
    }

    [TestMethod]
    public async Task Plp_Overflow()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1ff] = 0x40 + 0x30;
        emulator.StackPointer = 0x1fe; // one item on the stack

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                plp
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x28, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 4);
        emulator.AssertFlags(false, false, true, false, false, false);
    }

    [TestMethod]
    public async Task Plp_Negative()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1ff] = 0x80 + 0x30;
        emulator.StackPointer = 0x1fe; // one item on the stack

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                plp
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x28, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 4);
        emulator.AssertFlags(false, true, false, false, false, false);
    }

    [TestMethod]
    public async Task Plp_All()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1ff] = 0xff;
        emulator.StackPointer = 0x1fe; // one item on the stack

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                plp
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x28, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 4);
        emulator.AssertFlags(true, true, true, true, true, true);
    }
}