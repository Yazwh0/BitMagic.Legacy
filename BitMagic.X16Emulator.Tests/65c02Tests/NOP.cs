using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class NOP
{
    [TestMethod]
    public async Task Nop()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                nop
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xea, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, false, false, false);
    }
}

