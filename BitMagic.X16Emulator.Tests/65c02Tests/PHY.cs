﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class PHY
{
    [TestMethod]
    public async Task Phx()
    {
        var emulator = new Emulator();

        emulator.Y = 0xff;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                phy
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x5a, emulator.Memory[0x810]);
        Assert.AreEqual(0xff, emulator.Memory[0x1fd]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0xff, 0x812, 3);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Phy_Full()
    {
        var emulator = new Emulator();

        emulator.Y = 0xff;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                ldx #$ff
                txs
                ldy #$ff
            .loop:
                phy
                dey
                bne loop
                stp",
                emulator);

        // compilation
        for (var i = 0x100; i < 0x200; i++)
            Assert.AreEqual(i - 0x100, emulator.Memory[i]);

        // emulation
        emulator.AssertState(0x00, 0xff, 0x00, stackPointer: 0x100);
        emulator.AssertFlags(true, false, false, false);
    }
}

