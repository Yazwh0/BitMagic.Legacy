using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class TXS
{
    [TestMethod]
    public async Task Txs()
    {
        var emulator = new Emulator();

        emulator.X = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                txs
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x9a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x02, 0x00, 0x812, 2, 0x102);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Txs_NoFlags()
    {
        var emulator = new Emulator();

        emulator.X = 2;
        emulator.Zero = true;
        emulator.Negative = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                txs
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x9a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x02, 0x00, 0x812, 2, 0x102);
        emulator.AssertFlags(true, true, false, false);
    }
}