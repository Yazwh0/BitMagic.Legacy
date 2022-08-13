using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class BVS
{
    [TestMethod]
    public async Task Jump_Forward()
    {
        var emulator = new Emulator();

        emulator.Overflow = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bvs exit
                stp
            .exit:
                lda #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x70, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x816, 5); // 3 for bne + 2 for lda
        emulator.AssertFlags(false, false, true, false);
    }

    [TestMethod]
    public async Task Jump_Forward_Far()
    {
        var emulator = new Emulator();

        emulator.Overflow = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $8a0
                .org $8a0
                bvs exit
                stp
                .org $900
            .exit:
                lda #$10
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x903, 9); // 3 for bne + 2 for lda + 1 page change, + 3 for jmp
        emulator.AssertFlags(false, false, true, false);
    }

    [TestMethod]
    public async Task NoJump()
    {
        var emulator = new Emulator();

        emulator.Overflow = false;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                bvs exit
                stp
            .exit:
                lda #$ff
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 2);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Jump_Backward()
    {
        var emulator = new Emulator();
        emulator.Overflow = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp test
            .exit:
                lda #$10
                stp
            .test:
                bvs exit
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x10, 0x00, 0x00, 0x816, 8); // 3 for bne + 2 for lda + 3 for jmp
        emulator.AssertFlags(false, false, true, false);
    }
}