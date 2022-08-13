using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class INY
{
    [TestMethod]
    public async Task Iny()
    {
        var emulator = new Emulator();

        emulator.Y = 2;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                iny
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xc8, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x03, 0x812, 2);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Iny_PreserveFlags()
    {
        var emulator = new Emulator();

        emulator.Y = 2;
        emulator.InterruptDisable = true;
        emulator.Carry = true;
        emulator.Overflow = true;
        emulator.Decimal = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                iny
                stp",
                emulator);

        emulator.AssertFlags(false, false, true, true, true, true);
    }

    [TestMethod]
    public async Task Iny_ToZero()
    {
        var emulator = new Emulator();

        emulator.Y = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                iny
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xc8, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Iny_Negative()
    {
        var emulator = new Emulator();

        emulator.Y = 0xa0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                iny
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xc8, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0xa1, 0x812, 2);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Iny_ToNegative()
    {
        var emulator = new Emulator();

        emulator.Y = 0x7f;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                iny
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xc8, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x80, 0x812, 2);
        emulator.AssertFlags(false, true, false, false);
    }
}

