using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class INX
{
    [TestMethod]
    public async Task Inx()
    {
        var emulator = new Emulator();

        emulator.X = 2;
        
        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inx
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xe8, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x03, 0x00, 0x812, 2);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Inx_PreserveFlags()
    {
        var emulator = new Emulator();

        emulator.X = 2;
        emulator.InterruptDisable = true;
        emulator.Carry = true;
        emulator.Overflow = true;
        emulator.Decimal = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inx
                stp",
                emulator);

        emulator.AssertFlags(false, false, true, true, true, true);
    }

    [TestMethod]
    public async Task Inx_ToZero()
    {
        var emulator = new Emulator();

        emulator.X = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inx
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xe8, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Inx_Negative()
    {
        var emulator = new Emulator();

        emulator.X = 0xa0;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inx
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xe8, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0xa1, 0x00, 0x812, 2);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Inx_ToNegative()
    {
        var emulator = new Emulator();

        emulator.X = 0x7f;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inx
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xe8, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x80, 0x00, 0x812, 2);
        emulator.AssertFlags(false, true, false, false);
    }
}

