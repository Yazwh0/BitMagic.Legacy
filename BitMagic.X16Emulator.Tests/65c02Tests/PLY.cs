﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class PLY
{
    [TestMethod]
    public async Task Ply()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1ff] = 0x20;
        emulator.StackPointer = 0x1fe; // one item on the stack

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                ply
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x7a, emulator.Memory[0x810]);
        Assert.AreEqual(0x20, emulator.Memory[0x1ff]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x20, 0x812, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Ply_PreserveFlags()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1ff] = 0x20;
        emulator.StackPointer = 0x1fe; // one item on the stack
        emulator.Carry = true;
        emulator.Decimal = true;
        emulator.InterruptDisable = true;
        emulator.Overflow = true;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                ply
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(false, false, true, true, true, true);
    }

    [TestMethod]
    public async Task Ply_Zero()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1ff] = 0x00;
        emulator.StackPointer = 0x1fe; // one item on the stack

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                ply
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x7a, emulator.Memory[0x810]);
        Assert.AreEqual(0x00, emulator.Memory[0x1ff]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 4);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Ply_Negative()
    {
        var emulator = new Emulator();

        emulator.Memory[0x1ff] = 0xff;
        emulator.StackPointer = 0x1fe; // one item on the stack

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                ply
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x7a, emulator.Memory[0x810]);
        Assert.AreEqual(0xff, emulator.Memory[0x1ff]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0xff, 0x812, 4);
        emulator.AssertFlags(false, true, false, false);
    }
}