using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class Flags
{
    [TestMethod]
    public async Task Sec()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sec
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x38, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Clc()
    {
        var emulator = new Emulator();
        emulator.Carry = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                clc
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x18, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Sed()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sed
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xf8, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, false, false, false, false, true);
    }

    [TestMethod]
    public async Task Cld()
    {
        var emulator = new Emulator();
        emulator.Decimal = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cld
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xd8, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, false, false, false, false, false);
    }

    [TestMethod]
    public async Task Sei()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sei
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x78, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, false, false, false, true, false);
    }

    [TestMethod]
    public async Task Cli()
    {
        var emulator = new Emulator();
        emulator.InterruptDisable = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cli
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x58, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, false, false, false, false, false);
    }

    [TestMethod]
    public async Task Clv()
    {
        var emulator = new Emulator();
        emulator.Overflow = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                clv
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xb8, emulator.Memory[0x810]);

        // emulation
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 2);
        emulator.AssertFlags(false, false, false, false, false, false);
    }
}