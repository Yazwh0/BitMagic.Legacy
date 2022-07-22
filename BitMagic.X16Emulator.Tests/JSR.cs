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

        Assert.AreEqual(0x12, emulator.Memory[0x1ff]);
        Assert.AreEqual(0x08, emulator.Memory[0x1fe]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x851, 6, 0x1fd);
        emulator.AssertFlags(false, false, false, false);
    }
}