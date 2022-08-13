using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class BBS
{
    [TestMethod]
    public async Task BBS0_Jump_Forward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b00000001;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbs0 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x8f, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);
        Assert.AreEqual(0x02, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x817, 8); // 5 for bbs + 1 for taken + 2 for lda
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS0_No_Jump()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0x00;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbs0 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 5); // 5 for bbs
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS0_Jump_Far()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b00000001;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $8a0
                .org $8a0
                bbs0 $10, exit
                stp
                .org $900
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x903, 6 + 2 + 1 + 3); // 6 for bbs + 2 lda + 1 page change + 3 jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS0_Jump_Backward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b00000001;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp test
            .exit:
                lda #$10
                stp
            .test:
                bbs0 $10, exit
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x816, 6 + 2 + 3); // 6 for bne + 2 for lda + 3 for jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS1_Jump_Forward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b00000010;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbs1 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x9f, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);
        Assert.AreEqual(0x02, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x817, 8); // 5 for bbs + 1 for taken + 2 for lda
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS1_No_Jump()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0x00;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbs1 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 5); // 5 for bbs
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS1_Jump_Far()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b00000010;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $8a0
                .org $8a0
                bbs1 $10, exit
                stp
                .org $900
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x903, 6 + 2 + 1 + 3); // 6 for bbs + 2 lda + 1 page change + 3 jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS1_Jump_Backward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b00000010;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp test
            .exit:
                lda #$10
                stp
            .test:
                bbs1 $10, exit
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x816, 6 + 2 + 3); // 6 for bne + 2 for lda + 3 for jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS2_Jump_Forward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b00000100;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbs2 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xaf, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);
        Assert.AreEqual(0x02, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x817, 8); // 5 for bbs + 1 for taken + 2 for lda
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS2_No_Jump()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0x00;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbs2 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 5); // 5 for bbs
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS2_Jump_Far()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b00000100;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $8a0
                .org $8a0
                bbs2 $10, exit
                stp
                .org $900
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x903, 6 + 2 + 1 + 3); // 6 for bbs + 2 lda + 1 page change + 3 jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS2_Jump_Backward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b00000100;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp test
            .exit:
                lda #$10
                stp
            .test:
                bbs2 $10, exit
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x816, 6 + 2 + 3); // 6 for bne + 2 for lda + 3 for jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS3_Jump_Forward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b00001000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbs3 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xbf, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);
        Assert.AreEqual(0x02, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x817, 8); // 5 for bbs + 1 for taken + 2 for lda
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS3_No_Jump()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0x00;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbs3 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 5); // 5 for bbs
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS3_Jump_Far()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b00001000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $8a0
                .org $8a0
                bbs3 $10, exit
                stp
                .org $900
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x903, 6 + 2 + 1 + 3); // 6 for bbs + 2 lda + 1 page change + 3 jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS3_Jump_Backward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b00001000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp test
            .exit:
                lda #$10
                stp
            .test:
                bbs3 $10, exit
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x816, 6 + 2 + 3); // 6 for bne + 2 for lda + 3 for jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS4_Jump_Forward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b00010000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbs4 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xcf, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);
        Assert.AreEqual(0x02, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x817, 8); // 5 for bbs + 1 for taken + 2 for lda
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS4_No_Jump()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0x00;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbs4 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 5); // 5 for bbs
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS4_Jump_Far()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b00010000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $8a0
                .org $8a0
                bbs4 $10, exit
                stp
                .org $900
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x903, 6 + 2 + 1 + 3); // 6 for bbs + 2 lda + 1 page change + 3 jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS4_Jump_Backward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b00010000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp test
            .exit:
                lda #$10
                stp
            .test:
                bbs4 $10, exit
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x816, 6 + 2 + 3); // 6 for bne + 2 for lda + 3 for jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS5_Jump_Forward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b00100000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbs5 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xdf, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);
        Assert.AreEqual(0x02, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x817, 8); // 5 for bbs + 1 for taken + 2 for lda
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS5_No_Jump()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0x00;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbs5 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 5); // 5 for bbs
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS5_Jump_Far()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b00100000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $8a0
                .org $8a0
                bbs5 $10, exit
                stp
                .org $900
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x903, 6 + 2 + 1 + 3); // 6 for bbs + 2 lda + 1 page change + 3 jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS5_Jump_Backward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b00100000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp test
            .exit:
                lda #$10
                stp
            .test:
                bbs5 $10, exit
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x816, 6 + 2 + 3); // 6 for bne + 2 for lda + 3 for jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS6_Jump_Forward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b01000000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbs6 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xef, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);
        Assert.AreEqual(0x02, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x817, 8); // 5 for bbs + 1 for taken + 2 for lda
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS6_No_Jump()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0x00;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbs6 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 5); // 5 for bbs
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS6_Jump_Far()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b01000000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $8a0
                .org $8a0
                bbs6 $10, exit
                stp
                .org $900
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x903, 6 + 2 + 1 + 3); // 6 for bbs + 2 lda + 1 page change + 3 jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS6_Jump_Backward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b01000000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp test
            .exit:
                lda #$10
                stp
            .test:
                bbs6 $10, exit
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x816, 6 + 2 + 3); // 6 for bne + 2 for lda + 3 for jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS7_Jump_Forward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b10000000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbs7 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xff, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);
        Assert.AreEqual(0x02, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x817, 8); // 5 for bbs + 1 for taken + 2 for lda
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS7_No_Jump()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0x00;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbs7 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 5); // 5 for bbs
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS7_Jump_Far()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b10000000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $8a0
                .org $8a0
                bbs7 $10, exit
                stp
                .org $900
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x903, 6 + 2 + 1 + 3); // 6 for bbs + 2 lda + 1 page change + 3 jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBS7_Jump_Backward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b10000000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp test
            .exit:
                lda #$10
                stp
            .test:
                bbs7 $10, exit
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x816, 6 + 2 + 3); // 6 for bne + 2 for lda + 3 for jmp
        emulator.AssertFlags(false, false, false, false);
    }
}