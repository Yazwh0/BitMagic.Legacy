using Microsoft.VisualStudio.TestTools.UnitTesting;
using BitMagic.Compiler;
using BitMagic.Emulation;

namespace BitMagic.Compiler.Tests;

// These are considered obsolete, as we move to the new Emulator

[TestClass]
public class CommanderX16EmulatorTests
{
    [TestMethod]
    public async Task Lda_Immediate()
    {
        var result = await CommanderX16Test.UntilStp(@"
                .machine CommanderX16R40
                .org $810
                lda #$ff
                stp
                ");

        // compilation
        Assert.AreEqual(0xa9, result.Cpu.Memory.PeekByte(0x810));
        Assert.AreEqual(0xff, result.Cpu.Memory.PeekByte(0x811));

        // emulation
        Assert.AreEqual(0xff, result.Cpu.Registers.A);
        Assert.AreEqual(0x813, result.Cpu.Registers.PC);
    }

    [TestMethod]
    public async Task Ldx_Immediate()
    {
        var result = await CommanderX16Test.UntilStp(@"
                .machine CommanderX16R40
                .org $810
                ldx #$ff
                stp
                ");

        // compilation
        Assert.AreEqual(0xa2, result.Cpu.Memory.PeekByte(0x810));
        Assert.AreEqual(0xff, result.Cpu.Memory.PeekByte(0x811));

        // emulation
        Assert.AreEqual(0xff, result.Cpu.Registers.X);
        Assert.AreEqual(0x813, result.Cpu.Registers.PC);
    }

    [TestMethod]
    public async Task Ldy_Immediate()
    {
        var result = await CommanderX16Test.UntilStp(@"
                .machine CommanderX16R40
                .org $810
                ldy #$ff
                stp
                ");

        // compilation
        Assert.AreEqual(0xa0, result.Cpu.Memory.PeekByte(0x810));
        Assert.AreEqual(0xff, result.Cpu.Memory.PeekByte(0x811));

        // emulation
        Assert.AreEqual(0xff, result.Cpu.Registers.Y);
        Assert.AreEqual(0x813, result.Cpu.Registers.PC);
    }
}