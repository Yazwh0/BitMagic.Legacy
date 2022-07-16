using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class JMP
{
    [TestMethod]
    public async Task Absolute ()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $900
                stp
                .org $900
                lda #$01
                stp
                ",
                emulator);

        // compilation
        Assert.AreEqual(0x4c, emulator.Memory[0x810]);
        Assert.AreEqual(0x00, emulator.Memory[0x811]);
        Assert.AreEqual(0x09, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x01, 0x00, 0x00, 0x903, 5); // 3 for JMP, +2 for lda
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Indirect()
    {
        var emulator = new Emulator();

        emulator.Memory[0xa00] = 0x00;
        emulator.Memory[0xa01] = 0x09;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp ($a00)
                stp
                .org $900
                lda #$01
                stp
                ",
                emulator);

        // compilation
        Assert.AreEqual(0x6c, emulator.Memory[0x810]);
        Assert.AreEqual(0x00, emulator.Memory[0x811]);
        Assert.AreEqual(0x0a, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x01, 0x00, 0x00, 0x903, 7); // 5 for JMP, +2 for lda
        emulator.AssertFlags(false, false, false, false);
    }


    [TestMethod]
    public async Task AbsX()
    {
        var emulator = new Emulator();
        emulator.X = 0x10;
        emulator.Memory[0xa10] = 0x00;
        emulator.Memory[0xa11] = 0x09;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp ($a00,x)
                stp
                .org $900
                lda #$01
                stp
                ",
                emulator);

        // compilation
        Assert.AreEqual(0x7c, emulator.Memory[0x810]);
        Assert.AreEqual(0x00, emulator.Memory[0x811]);
        Assert.AreEqual(0x0a, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x01, 0x10, 0x00, 0x903, 8); // 6 for JMP, +2 for lda
        emulator.AssertFlags(false, false, false, false);
    }
}
