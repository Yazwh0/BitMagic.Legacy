using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class Addr
{
    [TestMethod]
    public async Task AddrL_Set_Data0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 2;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta ADDRx_L
                stp",
                emulator);

        Assert.AreEqual(0x000ff, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x00000, emulator.Vera.Data1_Address);

        Assert.AreEqual(0xff, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x20, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AddrL_Set_Data1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 2;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.A = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta ADDRx_L
                stp",
                emulator);

        Assert.AreEqual(0x00000, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x000ff, emulator.Vera.Data1_Address);

        Assert.AreEqual(0xff, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x20, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AddrM_Set_Data0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 2;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta ADDRx_M
                stp",
                emulator);

        Assert.AreEqual(0x0ff00, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x00000, emulator.Vera.Data1_Address);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0xff, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x20, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AddrM_Set_Data1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 2;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.A = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta ADDRx_M
                stp",
                emulator);

        Assert.AreEqual(0x00000, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x0ff00, emulator.Vera.Data1_Address);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0xff, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x20, emulator.Memory[0x9F22]);
    }
}