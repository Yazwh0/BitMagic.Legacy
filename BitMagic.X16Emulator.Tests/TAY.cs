using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class TAY
{
    [TestMethod]
    public async Task Tay()
    {
        var emulator = new Emulator();

        emulator.A = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                tay
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xa8, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x02, 0x00, 0x02, 0x812, 2);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Tay_ToZero()
    {
        var emulator = new Emulator();

        emulator.Y = 1;
        emulator.A = 0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                tay
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xa8, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Tay_Negative()
    {
        var emulator = new Emulator();

        emulator.A = 0xa0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                tay
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xa8, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0xa0, 0x00, 0xa0, 0x812, 2);
        emulator.AssertFlags(false, true, false, false);
    }
}
