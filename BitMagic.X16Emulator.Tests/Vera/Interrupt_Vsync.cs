using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class Interrupt_Vsync
{
    [TestMethod]
    public async Task Hit()
    {
        var emulator = new Emulator();

        emulator.Interrupt = false;

        emulator.RomBank[0x3ffe] = 0x00;
        emulator.RomBank[0x3fff] = 0x09;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda #01
                sta IEN
                ldy #$ff
        .y_loop:
                ldx #$ff
        .x_loop:
                dex
                bne x_loop
                dey
                bne y_loop                

                stp
                .org $900
                stp",
                emulator);

        // emulation
        emulator.AssertState(Pc: 0x901);
        Assert.AreEqual(true, emulator.Vera.Interrupt_Vsync_Hit);
        Assert.AreEqual(false, emulator.Vera.Interrupt_Line_Hit);
        Assert.AreEqual(false, emulator.Vera.Interrupt_SpCol_Hit);
        Assert.AreEqual(0x01, emulator.Memory[0x9F27]);
        Assert.AreEqual(0, emulator.Vera.Beam_X);
        Assert.AreEqual(480, emulator.Vera.Beam_Y);
        Assert.AreEqual((UInt32)(640 * 480), emulator.Vera.Beam_Position);
    }

    [TestMethod]
    public async Task Hit_Reset()
    {
        var emulator = new Emulator();

        emulator.Interrupt = false;

        emulator.RomBank[0x3ffe] = 0x00;
        emulator.RomBank[0x3fff] = 0x09;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda #01
                sta IEN
                ldy #$ff
        .y_loop:
                ldx #$ff
        .x_loop:
                dex
                bne x_loop
                dey
                bne y_loop                

                stp
                .org $900
                lda #01
                sta ISR
                stp",
                emulator);

        // emulation
        Assert.AreEqual(false, emulator.Vera.Interrupt_Vsync_Hit);
        Assert.AreEqual(false, emulator.Vera.Interrupt_Line_Hit);
        Assert.AreEqual(false, emulator.Vera.Interrupt_SpCol_Hit);
        Assert.AreEqual(0x00, emulator.Memory[0x9F27]);
    }

    [TestMethod]
    public async Task Hit_OnlyOnce()
    {
        var emulator = new Emulator();

        emulator.Interrupt = false;

        emulator.RomBank[0x3ffe] = 0x00;
        emulator.RomBank[0x3fff] = 0x09;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stz $03
                lda #01
                sta IEN
                ldy #$ff
        .y_loop:
                ldx #$ff
        .x_loop:
                dex
                bne x_loop
                dey
                bne y_loop                

                stp
                .org $900
                inc $03 ; dont clear ISR, so will only hit once
                rti",
                emulator);

        // emulation
        Assert.AreEqual(0x01, emulator.Memory[0x03]);
    }

    [TestMethod]
    public async Task Hit_Many()
    {
        var emulator = new Emulator();

        emulator.Interrupt = false;

        emulator.RomBank[0x3ffe] = 0x00;
        emulator.RomBank[0x3fff] = 0x09;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stz $03
                lda #01
                sta IEN
                ldy #$ff
        .y_loop:
                ldx #$ff
        .x_loop:
                dex
                bne x_loop
                dey
                bne y_loop                

                stp
                .org $900
                lda #01
                sta ISR
                inc $03 
                rti",
                emulator);

        // emulation
        Assert.AreEqual(0x02, emulator.Memory[0x03]);
    }
}