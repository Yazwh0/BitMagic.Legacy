using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class TXA
{
    [TestMethod]
    public async Task Txa()
    {
        var emulator = new Emulator();

        emulator.X = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                txa
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x8a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x02, 0x02, 0x00, 0x812, 2);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Txa_PreserveFlags()
    {
        var emulator = new Emulator();

        emulator.X = 2;
        emulator.InterruptDisable = true;
        emulator.Carry = true;
        emulator.Decimal = true;
        emulator.Overflow = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                txa
                stp",
                emulator);

        emulator.AssertFlags(false, false, true, true, true, true);
    }

    [TestMethod]
    public async Task Txa_ToZero()
    {
        var emulator = new Emulator();

        emulator.A = 1;
        emulator.X = 0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                txa
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x8a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Txa_Negative()
    {
        var emulator = new Emulator();

        emulator.X = 0xa0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                txa
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x8a, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0xa0, 0xa0, 0x00, 0x812, 2);
        emulator.AssertFlags(false, true, false, false);
    }
}
