using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class RMB
{
    [TestMethod]
    public async Task RMB0()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                rmb0 $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x07, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b11111110, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task RMB1()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                rmb1 $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x17, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b11111101, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task RMB2()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                rmb2 $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x27, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b11111011, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task RMB3()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                rmb3 $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x37, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b11110111, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task RMB4()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                rmb4 $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x47, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b11101111, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task RMB5()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                rmb5 $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x57, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b11011111, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task RMB6()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                rmb6 $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x67, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b10111111, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task RMB7()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                rmb7 $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x77, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b01111111, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }
}