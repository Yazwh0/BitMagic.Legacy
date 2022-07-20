using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class PLA
{
    [TestMethod]
    public async Task Pla()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1ff] = 0x20;
        emulator.StackPointer = 0x1fe; // one item on the stack

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                pla
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x68, emulator.Memory[0x810]);
        Assert.AreEqual(0x20, emulator.Memory[0x1ff]);

        // emulation
        emulator.AssertState(0x20, 0x00, 0x00, 0x812, 4);
        emulator.AssertFlags(false, false, false, false);
    }
}