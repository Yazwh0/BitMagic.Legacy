using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class RTS
{
    [TestMethod]
    public async Task Rts()
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
                rts
                ",
                emulator);

        // compilation
        Assert.AreEqual(0x20, emulator.Memory[0x810]);

        // Stack -- doesn't get cleared
        Assert.AreEqual(0x12, emulator.Memory[0x1fe]);
        Assert.AreEqual(0x08, emulator.Memory[0x1ff]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x814, 6+6, 0x1ff);
        emulator.AssertFlags(false, false, false, false);
    }
}