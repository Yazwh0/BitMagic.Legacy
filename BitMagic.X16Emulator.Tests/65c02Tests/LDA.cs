﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class LDA
{
    [TestMethod]
    public async Task Immediate()
    {
        var emulator = await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                lda #$44
                stp");

        // compilation
        Assert.AreEqual(0xa9, emulator.Memory[0x810]);
        Assert.AreEqual(0x44, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x44, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Immediate_Flags()
    {
        var emulator = new Emulator();

        emulator.Carry = true;
        emulator.Decimal = true;
        emulator.InterruptDisable = true;
        emulator.Overflow = true;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                lda #$44
                stp", emulator);

        emulator.AssertFlags(false, false, true, true, true, true);
    }

    [TestMethod]
    public async Task Immediate_ZeroFlag()
    {
        var emulator = await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                lda #$0
                stp");

        // compilation
        Assert.AreEqual(0xa9, emulator.Memory[0x810]);
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
                lda #$ff
                stp");

        // compilation
        Assert.AreEqual(0xa9, emulator.Memory[0x810]);
        Assert.AreEqual(0xff, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0xff, 0x00, 0x00, 0x813, 2);
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
                lda $10
                stp", emulator);

        // compilation
        Assert.AreEqual(0xa5, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x44, 0x00, 0x00, 0x813, 3);
        emulator.AssertFlags(false, false, false, false);
    }


    [TestMethod]
    public async Task ZeroPage_FromRom()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0x44;

        emulator.RomBank[0x0000] = 0xa5;
        emulator.RomBank[0x0001] = 0x10;
        emulator.RomBank[0x0002] = 0xdb;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                jmp $c000", emulator);

        // emulation
        emulator.AssertState(0x44, 0x00, 0x00, 0xc003);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZeroPage_FlagPreserve()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0x44;
        emulator.Carry = true;
        emulator.Decimal = true;
        emulator.InterruptDisable = true;
        emulator.Overflow = true;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                lda $10
                stp", emulator);

        emulator.AssertFlags(false, false, true, true, true, true);
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
                lda $10, X
                stp", emulator);

        // compilation
        Assert.AreEqual(0xb5, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x44, 0x02, 0x00, 0x813, 4);
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
                lda $a0, X
                stp", emulator);

        // compilation
        Assert.AreEqual(0xb5, emulator.Memory[0x810]);
        Assert.AreEqual(0xa0, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x44, 0x72, 0x00, 0x813, 4);
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
                lda $400
                stp", emulator);

        // compilation
        Assert.AreEqual(0xad, emulator.Memory[0x810]);
        Assert.AreEqual(0x00, emulator.Memory[0x811]);
        Assert.AreEqual(0x04, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x44, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Absolute_Rom()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0] = 0x44;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                lda $c000
                stp", emulator);

        // emulation
        emulator.AssertState(0x44, 0x00, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Absolute_BankedRam()
    {
        var emulator = new Emulator();

        emulator.RamBank[0x0] = 0x44;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                lda $a000
                stp", emulator);

        // emulation
        emulator.AssertState(0x44, 0x00, 0x00, 0x814, 4);
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
                lda $400, X
                stp", emulator);

        // compilation
        Assert.AreEqual(0xbd, emulator.Memory[0x810]);
        Assert.AreEqual(0x00, emulator.Memory[0x811]);
        Assert.AreEqual(0x04, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x44, 0x02, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsoluteX_FromRom()
    {
        var emulator = new Emulator();

        emulator.Memory[0x402] = 0x44;
        emulator.X = 2;

        emulator.RomBank[0x0000] = 0xbd;
        emulator.RomBank[0x0001] = 0x00;
        emulator.RomBank[0x0002] = 0x04;
        emulator.RomBank[0x0003] = 0xdb;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                jmp $c000
                ", emulator);

        // emulation
        emulator.AssertState(0x44, 0x02, 0x00, 0xc004);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsoluteX_PageBoundry()
    {
        var emulator = new Emulator();

        emulator.Memory[0x590] = 0x44;
        emulator.X = 0xf0;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                lda $4a0, X
                stp", emulator);

        // compilation
        Assert.AreEqual(0xbd, emulator.Memory[0x810]);
        Assert.AreEqual(0xa0, emulator.Memory[0x811]);
        Assert.AreEqual(0x04, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x44, 0xf0, 0x00, 0x814, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsoluteY()
    {
        var emulator = new Emulator();

        emulator.Memory[0x402] = 0x44;
        emulator.Y = 2;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                lda $400, Y
                stp", emulator);

        // compilation
        Assert.AreEqual(0xb9, emulator.Memory[0x810]);
        Assert.AreEqual(0x00, emulator.Memory[0x811]);
        Assert.AreEqual(0x04, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x44, 0x00, 0x02, 0x814, 4);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task AbsoluteY_PageBoundry()
    {
        var emulator = new Emulator();

        emulator.Memory[0x590] = 0x44;
        emulator.Y = 0xf0;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                lda $4a0, Y
                stp", emulator);

        // compilation
        Assert.AreEqual(0xb9, emulator.Memory[0x810]);
        Assert.AreEqual(0xa0, emulator.Memory[0x811]);
        Assert.AreEqual(0x04, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x44, 0x00, 0xf0, 0x814, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task IndirectX()
    {
        var emulator = new Emulator();

        emulator.Memory[0x402] = 0x44;
        emulator.Memory[0x12] = 0x02;
        emulator.Memory[0x13] = 0x04;
        emulator.X = 2;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                lda ($10, X)
                stp", emulator);

        // compilation
        Assert.AreEqual(0xa1, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x44, 0x02, 0x00, 0x813, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task IndirectY()
    {
        var emulator = new Emulator();

        emulator.Memory[0x402] = 0x44;
        emulator.Memory[0x10] = 0x01;
        emulator.Memory[0x11] = 0x04;
        emulator.Y = 1;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                lda ($10), Y
                stp", emulator);

        // compilation
        Assert.AreEqual(0xb1, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x44, 0x00, 0x01, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task IndirectY_Zero()
    {
        var emulator = new Emulator();

        emulator.Memory[0x402] = 0x00;
        emulator.Memory[0x10] = 0x01;
        emulator.Memory[0x11] = 0x04;
        emulator.Y = 1;
        emulator.Zero = false;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                lda ($10), Y
                stp", emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x01, 0x813, 5);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task IndirectY_FromRom()
    {
        var emulator = new Emulator();

        emulator.Memory[0x402] = 0x44;
        emulator.Memory[0x10] = 0x01;
        emulator.Memory[0x11] = 0x04;
        emulator.Y = 1;

        emulator.RomBank[0x0000] = 0xb1;
        emulator.RomBank[0x0001] = 0x10;
        emulator.RomBank[0x0002] = 0xdb;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                jmp $c000
                ", emulator);

        // emulation
        emulator.AssertState(0x44, 0x00, 0x01, 0xc003);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task IndirectY_FromBRam()
    {
        var emulator = new Emulator();

        emulator.Memory[0x402] = 0x44;
        emulator.Memory[0x10] = 0x01;
        emulator.Memory[0x11] = 0x04;
        emulator.Y = 1;

        emulator.RamBank[0x0000] = 0xb1;
        emulator.RamBank[0x0001] = 0x10;
        emulator.RamBank[0x0002] = 0xdb;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                jmp $a000
                ", emulator);

        // emulation
        emulator.AssertState(0x44, 0x00, 0x01, 0xa003);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task IndirectY_PageBoundary()
    {
        var emulator = new Emulator();

        emulator.Memory[0x4a0+0xf0] = 0x44;
        emulator.Memory[0xa0] = 0xa0;
        emulator.Memory[0xa1] = 0x04;
        emulator.Y = 0xf0;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                lda ($a0), Y
                stp", emulator);

        // compilation
        Assert.AreEqual(0xb1, emulator.Memory[0x810]);
        Assert.AreEqual(0xa0, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x44, 0x00, 0xf0, 0x813, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task IndirectZp()
    {
        var emulator = new Emulator();

        emulator.Memory[0x402] = 0x44;
        emulator.Memory[0x10] = 0x02;
        emulator.Memory[0x11] = 0x04;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                lda ($10)
                stp", emulator);

        // compilation
        Assert.AreEqual(0xb2, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x44, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }
}