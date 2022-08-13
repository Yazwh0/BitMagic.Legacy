using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class TRB
{
    [TestMethod]
    public async Task Zp()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x10] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                trb $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x14, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        Assert.AreEqual(0x01, emulator.Memory[0x10]);
        emulator.AssertState(0x02, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Zp_PreserveFlags()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x10] = 0x03;

        emulator.Carry = true;
        emulator.Negative = true;
        emulator.InterruptDisable = true;
        emulator.Overflow = true;
        emulator.Decimal = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                trb $10
                stp",
                emulator);

        emulator.AssertFlags(false, true, true, true, true, true);
    }

    [TestMethod]
    public async Task Zp_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0xff;
        emulator.Memory[0x10] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                trb $10
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0x00, emulator.Memory[0x10]);
        emulator.AssertFlags(true, false, false, false);
    }

    [TestMethod]
    public async Task Abs()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x1234] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                trb $1234
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x1c, emulator.Memory[0x810]);
        Assert.AreEqual(0x34, emulator.Memory[0x811]);
        Assert.AreEqual(0x12, emulator.Memory[0x812]);

        // emulation
        Assert.AreEqual(0x01, emulator.Memory[0x1234]);
        emulator.AssertState(0x02, 0x00, 0x00, 0x814, 6);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Abs_PreserveFlags()
    {
        var emulator = new Emulator();

        emulator.A = 0x02;
        emulator.Memory[0x1234] = 0x03;

        emulator.Carry = true;
        emulator.Negative = true;
        emulator.InterruptDisable = true;
        emulator.Overflow = true;
        emulator.Decimal = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                trb $1234
                stp",
                emulator);

        emulator.AssertFlags(false, true, true, true, true, true);
    }

    [TestMethod]
    public async Task Abs_Zero()
    {
        var emulator = new Emulator();

        emulator.A = 0xff;
        emulator.Memory[0x1234] = 0x03;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                trb $1234
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0x00, emulator.Memory[0x1234]);
        emulator.AssertFlags(true, false, false, false);
    }
}