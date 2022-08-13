using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class BBR
{
    [TestMethod]
    public async Task BBR0_Jump_Forward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b11111110;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbr0 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x0f, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);
        Assert.AreEqual(0x02, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x817, 8); // 5 for bbr + 1 for taken + 2 for lda
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR0_No_Jump()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbr0 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 5); // 5 for bbr
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR0_Jump_Far()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b11111110;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $8a0
                .org $8a0
                bbr0 $10, exit
                stp
                .org $900
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x903, 6+2+1+3); // 6 for bbr + 2 lda + 1 page change + 3 jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR0_Jump_Backward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b11111110;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp test
            .exit:
                lda #$10
                stp
            .test:
                bbr0 $10, exit
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x816, 6 + 2 + 3); // 6 for bne + 2 for lda + 3 for jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR1_Jump_Forward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b11111101;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbr1 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x1f, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);
        Assert.AreEqual(0x02, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x817, 8); // 5 for bbr + 1 for taken + 2 for lda
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR1_No_Jump()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbr1 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 5); // 5 for bbr
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR1_Jump_Far()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b11111101;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $8a0
                .org $8a0
                bbr1 $10, exit
                stp
                .org $900
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x903, 6 + 2 + 1 + 3); // 6 for bbr + 2 lda + 1 page change + 3 jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR1_Jump_Backward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b11111101;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp test
            .exit:
                lda #$10
                stp
            .test:
                bbr1 $10, exit
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x816, 6 + 2 + 3); // 6 for bne + 2 for lda + 3 for jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR2_Jump_Forward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b11111011;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbr2 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x2f, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);
        Assert.AreEqual(0x02, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x817, 8); // 5 for bbr + 1 for taken + 2 for lda
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR2_No_Jump()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbr2 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 5); // 5 for bbr
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR2_Jump_Far()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b11111011;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $8a0
                .org $8a0
                bbr2 $10, exit
                stp
                .org $900
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x903, 6 + 2 + 1 + 3); // 6 for bbr + 2 lda + 1 page change + 3 jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR2_Jump_Backward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b11111011;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp test
            .exit:
                lda #$10
                stp
            .test:
                bbr2 $10, exit
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x816, 6 + 2 + 3); // 6 for bne + 2 for lda + 3 for jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR3_Jump_Forward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b11110111;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbr3 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x3f, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);
        Assert.AreEqual(0x02, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x817, 8); // 5 for bbr + 1 for taken + 2 for lda
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR3_No_Jump()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbr3 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 5); // 5 for bbr
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR3_Jump_Far()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b11110111;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $8a0
                .org $8a0
                bbr3 $10, exit
                stp
                .org $900
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x903, 6 + 2 + 1 + 3); // 6 for bbr + 2 lda + 1 page change + 3 jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR3_Jump_Backward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b11110111;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp test
            .exit:
                lda #$10
                stp
            .test:
                bbr3 $10, exit
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x816, 6 + 2 + 3); // 6 for bne + 2 for lda + 3 for jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR4_Jump_Forward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b11101111;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbr4 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x4f, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);
        Assert.AreEqual(0x02, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x817, 8); // 5 for bbr + 1 for taken + 2 for lda
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR4_No_Jump()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbr4 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 5); // 5 for bbr
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR4_Jump_Far()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b11101111;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $8a0
                .org $8a0
                bbr4 $10, exit
                stp
                .org $900
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x903, 6 + 2 + 1 + 3); // 6 for bbr + 2 lda + 1 page change + 3 jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR4_Jump_Backward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b11101111;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp test
            .exit:
                lda #$10
                stp
            .test:
                bbr4 $10, exit
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x816, 6 + 2 + 3); // 6 for bne + 2 for lda + 3 for jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR5_Jump_Forward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b11011111;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbr5 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x5f, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);
        Assert.AreEqual(0x02, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x817, 8); // 5 for bbr + 1 for taken + 2 for lda
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR5_No_Jump()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbr5 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 5); // 5 for bbr
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR5_Jump_Far()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b11011111;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $8a0
                .org $8a0
                bbr5 $10, exit
                stp
                .org $900
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x903, 6 + 2 + 1 + 3); // 6 for bbr + 2 lda + 1 page change + 3 jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR5_Jump_Backward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b11011111;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp test
            .exit:
                lda #$10
                stp
            .test:
                bbr5 $10, exit
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x816, 6 + 2 + 3); // 6 for bne + 2 for lda + 3 for jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR6_Jump_Forward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b10111111;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbr6 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x6f, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);
        Assert.AreEqual(0x02, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x817, 8); // 5 for bbr + 1 for taken + 2 for lda
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR6_No_Jump()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbr6 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 5); // 5 for bbr
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR6_Jump_Far()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b10111111;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $8a0
                .org $8a0
                bbr6 $10, exit
                stp
                .org $900
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x903, 6 + 2 + 1 + 3); // 6 for bbr + 2 lda + 1 page change + 3 jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR6_Jump_Backward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b10111111;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp test
            .exit:
                lda #$10
                stp
            .test:
                bbr6 $10, exit
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x816, 6 + 2 + 3); // 6 for bne + 2 for lda + 3 for jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR7_Jump_Forward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b01111111;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbr7 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x7f, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);
        Assert.AreEqual(0x02, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x817, 8); // 5 for bbr + 1 for taken + 2 for lda
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR7_No_Jump()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bbr7 $10, exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 5); // 5 for bbr
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR7_Jump_Far()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b01111111;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $8a0
                .org $8a0
                bbr7 $10, exit
                stp
                .org $900
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x903, 6 + 2 + 1 + 3); // 6 for bbr + 2 lda + 1 page change + 3 jmp
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task BBR7_Jump_Backward()
    {
        var emulator = new Emulator();

        emulator.Carry = false;
        emulator.Memory[0x10] = 0b01111111;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp test
            .exit:
                lda #$10
                stp
            .test:
                bbr7 $10, exit
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x816, 6 + 2 + 3); // 6 for bne + 2 for lda + 3 for jmp
        emulator.AssertFlags(false, false, false, false);
    }
}