using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class TYA
{
    [TestMethod]
    public async Task Tya()
    {
        var emulator = new Emulator();

        emulator.Y = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                tya
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x98, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x02, 0x00, 0x02, 0x812, 2);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Tya_PreserveFlags()
    {
        var emulator = new Emulator();

        emulator.Y = 2;
        emulator.InterruptDisable = true;
        emulator.Carry = true;
        emulator.Decimal = true;
        emulator.Overflow = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                tya
                stp",
                emulator);

        emulator.AssertFlags(false, false, true, true, true, true);
    }

    [TestMethod]
    public async Task Tya_ToZero()
    {
        var emulator = new Emulator();

        emulator.A = 1;
        emulator.Y = 0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                tya
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x98, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Tya_Negative()
    {
        var emulator = new Emulator();

        emulator.Y = 0xa0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                tya
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x98, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0xa0, 0x00, 0xa0, 0x812, 2);
        emulator.AssertFlags(false, true, false, false);
    }
}
