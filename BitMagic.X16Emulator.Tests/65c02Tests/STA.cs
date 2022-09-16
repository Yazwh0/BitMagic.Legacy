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
    public async Task ZeroPage_RomToZp()
    {
        var emulator = new Emulator();

        emulator.A = 0x44;

        emulator.RomBank[0x0000] = 0x85;
        emulator.RomBank[0x0001] = 0x10;
        emulator.RomBank[0x0002] = 0xdb;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                jmp $c000",
                emulator);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x10]);
        emulator.AssertState(0x44, 0x00, 0x00, 0xc003);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZeroPage_RomToZp_RomBank()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;

        emulator.RomBank[0x0000] = 0x85;
        emulator.RomBank[0x0001] = 0x01;
        emulator.RomBank[0x4000 * 2 + 0x0002] = 0xdb; // bank changes under PC

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                jmp $c000",
                emulator);

        // emulation
        Assert.AreEqual(0x02, emulator.Memory[0x01]);
        emulator.AssertState(0x02, 0x00, 0x00, 0xc003);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZeroPage_RomToZp_RamBank()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;

        emulator.RamBank[0x0000] = 0x85;
        emulator.RamBank[0x0001] = 0x00;
        emulator.RamBank[0x2000 * 2 + 0x0002] = 0xdb; // bank changes under PC

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                jmp $a000",
                emulator);

        // emulation
        Assert.AreEqual(0x02, emulator.Memory[0x00]);
        emulator.AssertState(0x02, 0x00, 0x00, 0xa003);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ZeroPage_BRamToZp()
    {
        var emulator = new Emulator();

        emulator.A = 0x44;

        emulator.RamBank[0x0000] = 0x85;
        emulator.RamBank[0x0001] = 0x10;
        emulator.RamBank[0x0002] = 0xdb;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                jmp $a000",
                emulator);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x10]);
        emulator.AssertState(0x44, 0x00, 0x00, 0xa003);
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
    public async Task Absolute_RomToRam()
    {
        var emulator = new Emulator();

        emulator.A = 0x44;

        emulator.RomBank[0x00] = 0x8d;
        emulator.RomBank[0x01] = 0x00;
        emulator.RomBank[0x02] = 0x01;
        emulator.RomBank[0x03] = 0xdb;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                jmp $c000",
                emulator);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x100]);
        emulator.AssertState(0x44, 0x00, 0x00, 0xc004);
    }

    [TestMethod]
    public async Task Absolute_BRamToRam()
    {
        var emulator = new Emulator();

        emulator.A = 0x44;

        emulator.RamBank[0x00] = 0x8d;
        emulator.RamBank[0x01] = 0x00;
        emulator.RamBank[0x02] = 0x01;
        emulator.RamBank[0x03] = 0xdb;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                jmp $a000",
                emulator);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x100]);
        emulator.AssertState(0x44, 0x00, 0x00, 0xa004);
    }

    [TestMethod]
    public async Task Absolute_RomToBRam()
    {
        var emulator = new Emulator();

        emulator.A = 0x44;

        emulator.RomBank[0x00] = 0x8d;
        emulator.RomBank[0x01] = 0x00;
        emulator.RomBank[0x02] = 0xa1;
        emulator.RomBank[0x03] = 0xdb;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                jmp $c000",
                emulator);

        // emulation
        Assert.AreEqual(0x44, emulator.RamBank[0x100]);
        emulator.AssertState(0x44, 0x00, 0x00, 0xc004);
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
    public async Task AbsoluteX_FromRom()
    {
        var emulator = new Emulator();

        emulator.A = 0x44;
        emulator.X = 0x05;

        emulator.RomBank[0x0000] = 0x9d;
        emulator.RomBank[0x0001] = 0x00;
        emulator.RomBank[0x0002] = 0x01;
        emulator.RomBank[0x0003] = 0xdb;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                jmp $c000",
                emulator);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x105]);
        emulator.AssertState(0x44, 0x05, 0x00, 0xc004);
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

    [TestMethod]
    public async Task IndirectY_FromRom()
    {
        var emulator = new Emulator();

        emulator.A = 0x44;
        emulator.Y = 0x10;

        emulator.Memory[0x10] = 0x05;
        emulator.Memory[0x11] = 0x01;

        emulator.RomBank[0x0000] = 0x91;
        emulator.RomBank[0x0001] = 0x10;
        emulator.RomBank[0x0002] = 0xdb;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                jmp $c000",
                emulator);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x115]);
        emulator.AssertState(0x44, 0x00, 0x10, 0xc003);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task IndirectZP()
    {
        var emulator = new Emulator();

        emulator.A = 0x44;

        emulator.Memory[0x10] = 0x05;
        emulator.Memory[0x11] = 0x01;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                sta ($10)
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x92, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        Assert.AreEqual(0x44, emulator.Memory[0x105]);
        emulator.AssertState(0x44, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task ReadOnly_Abs()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0000] = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda #$ff
                sta $c000
                stp",
                emulator);

        Assert.AreEqual(0x10, emulator.RomBank[0x0000]);
    }

    [TestMethod]
    public async Task ReadOnly_AbsX()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0001] = 0x10;
        emulator.X = 0x01;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda #$ff
                sta $c000, x
                stp",
                emulator);

        Assert.AreEqual(0x10, emulator.RomBank[0x0001]);
    }

    [TestMethod]
    public async Task ReadOnly_AbsY()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0001] = 0x10;
        emulator.Y = 0x01;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda #$ff
                sta $c000, y
                stp",
                emulator);

        Assert.AreEqual(0x10, emulator.RomBank[0x0001]);
    }

    [TestMethod]
    public async Task ReadOnly_Ind()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0000] = 0x10;
        emulator.Memory[0x20] = 0x00;
        emulator.Memory[0x21] = 0xc0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda #$ff
                sta ($20)
                stp",
                emulator);

        Assert.AreEqual(0x10, emulator.RomBank[0x0000]);
    }

    [TestMethod]
    public async Task ReadOnly_IndX()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0000] = 0x10;
        emulator.Memory[0x20] = 0x00;
        emulator.Memory[0x21] = 0xc0;
        emulator.X = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda #$ff
                sta ($1e, x)
                stp",
                emulator);

        Assert.AreEqual(0x10, emulator.RomBank[0x0000]);
    }

    [TestMethod]
    public async Task ReadOnly_IndY()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0002] = 0x10;
        emulator.Memory[0x20] = 0x00;
        emulator.Memory[0x21] = 0xc0;
        emulator.Y = 0x02;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda #$ff
                sta ($20), y
                stp",
                emulator);

        Assert.AreEqual(0x10, emulator.RomBank[0x0002]);
    }
}
