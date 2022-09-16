using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class JSR
{
    [TestMethod]
    public async Task Jsr()
    {
        var emulator = new Emulator();

        emulator.StackPointer = 0x1ff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jsr test
                stp
                .org $850
                .test:
                stp
                ",
                emulator);

        // compilation
        Assert.AreEqual(0x20, emulator.Memory[0x810]);

        // Stack
        Assert.AreEqual(0x12, emulator.Memory[0x1fe]);
        Assert.AreEqual(0x08, emulator.Memory[0x1ff]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x851, 6, 0x1fd);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Jsr_RomBank()
    {
        var emulator = new Emulator();

        emulator.StackPointer = 0x1ff;
        emulator.RomBank[0x00] = 0xdb;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jsr $c000
                stp
                ",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0xc001);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Jsr_RomBank_Change()
    {
        var emulator = new Emulator();

        emulator.StackPointer = 0x1ff;
        emulator.RomBank[0x4000] = 0xdb;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda #01
                sta $01
                jsr $c000
                stp
                ",
                emulator);

        // emulation
        emulator.AssertState(0x01, 0x00, 0x00, 0xc001);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Jsr_RamBank()
    {
        var emulator = new Emulator();

        emulator.StackPointer = 0x1ff;
        emulator.RamBank[0x00] = 0xdb;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jsr $a000
                stp
                ",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0xa001);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Jsr_RamBank_Change()
    {
        var emulator = new Emulator();

        emulator.StackPointer = 0x1ff;
        emulator.RamBank[0x2000] = 0xdb;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda #01
                sta $00
                jsr $a000
                stp
                ",
                emulator);

        // emulation
        emulator.AssertState(0x01, 0x00, 0x00, 0xa001);
        emulator.AssertFlags(false, false, false, false);
    }
}