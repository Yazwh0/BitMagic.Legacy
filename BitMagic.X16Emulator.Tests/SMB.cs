using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class SMB
{
    [TestMethod]
    public async Task SMB0()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0x00;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                smb0 $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x87, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000001, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task SMB1()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0x00;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                smb1 $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x97, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000010, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task SMB2()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0x00;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                smb2 $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xa7, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00000100, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task SMB3()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0x00;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                smb3 $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xb7, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00001000, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task SMB4()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0x00;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                smb4 $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xc7, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00010000, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task SMB5()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0x00;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                smb5 $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xd7, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b00100000, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task SMB6()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0x00;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                smb6 $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xe7, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b01000000, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task SMB7()
    {
        var emulator = new Emulator();

        emulator.Memory[0x10] = 0x00;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                smb7 $10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xf7, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0b10000000, emulator.Memory[0x10]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x813, 5);
        emulator.AssertFlags(false, false, false, false);
    }
}