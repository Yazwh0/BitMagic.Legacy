using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class TSX
{
    [TestMethod]
    public async Task Tsx_Negative()
    {
        var emulator = new Emulator();

        emulator.StackPointer = 0x1ff;
        emulator.X = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                tsx
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xba, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0xff, 0x00, 0x812, 2, 0x1ff);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Tsx_PreserveFlags()
    {
        var emulator = new Emulator();

        emulator.StackPointer = 0x112;
        emulator.X = 0x10;
        emulator.Carry = true;
        emulator.Decimal = true;
        emulator.InterruptDisable = true;
        emulator.Overflow = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                tsx
                stp",
                emulator);

        // emulation
        emulator.AssertFlags(false, false, true, true, true, true);
    }

    [TestMethod]
    public async Task Txs_Zero()
    {
        var emulator = new Emulator();

        emulator.StackPointer = 0x100;
        emulator.X = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                tsx
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xba, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 2, 0x100);
        emulator.AssertFlags(true, false, false, false);
    }
}