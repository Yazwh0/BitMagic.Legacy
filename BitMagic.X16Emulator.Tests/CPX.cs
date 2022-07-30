using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class CPX
{
    [TestMethod]
    public async Task Imm_Equal()
    {
        var emulator = new Emulator();

        emulator.X = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cpx #$10
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xe0, emulator.Memory[0x810]);
        Assert.AreEqual(0x10, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x00, 0x10, 0x00, 0x813, 2);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task Imm_LessThan()
    {
        var emulator = new Emulator();

        emulator.X = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cpx #$20
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x10, 0x00, 0x813, 2);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Imm_GreaterThan()
    {
        var emulator = new Emulator();

        emulator.X = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cpx #$05
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x10, 0x00, 0x813, 2);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Abs_Equal()
    {
        var emulator = new Emulator();

        emulator.X = 0x10;
        emulator.Memory[0x1234] = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cpx $1234
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xec, emulator.Memory[0x810]);
        Assert.AreEqual(0x34, emulator.Memory[0x811]);
        Assert.AreEqual(0x12, emulator.Memory[0x812]);

        // emulation
        emulator.AssertState(0x00, 0x10, 0x00, 0x814, 4);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task Abs_LessThan()
    {
        var emulator = new Emulator();

        emulator.X = 0x10;
        emulator.Memory[0x1234] = 0x20;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cpx $1234
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x10, 0x00, 0x814, 4);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Abs_GreaterThan()
    {
        var emulator = new Emulator();

        emulator.X = 0x10;
        emulator.Memory[0x1234] = 0x05;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cpx $1234
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x10, 0x00, 0x814, 4);
        emulator.AssertFlags(false, false, false, true);
    }

    [TestMethod]
    public async Task Zp_Equal()
    {
        var emulator = new Emulator();

        emulator.X = 0x10;
        emulator.Memory[0x12] = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cpx $12
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0xe4, emulator.Memory[0x810]);
        Assert.AreEqual(0x12, emulator.Memory[0x811]);

        // emulation
        emulator.AssertState(0x00, 0x10, 0x00, 0x813, 3);
        emulator.AssertFlags(true, false, false, true);
    }

    [TestMethod]
    public async Task Zp_LessThan()
    {
        var emulator = new Emulator();

        emulator.X = 0x10;
        emulator.Memory[0x12] = 0x20;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cpx $12
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x10, 0x00, 0x813, 3);
        emulator.AssertFlags(false, true, false, false);
    }

    [TestMethod]
    public async Task Zp_GreaterThan()
    {
        var emulator = new Emulator();

        emulator.X = 0x10;
        emulator.Memory[0x12] = 0x05;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                cpx $12
                stp",
                emulator);

        // emulation
        emulator.AssertState(0x00, 0x10, 0x00, 0x813, 3);
        emulator.AssertFlags(false, false, false, true);
    }
}