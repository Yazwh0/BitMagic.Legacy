using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera;

[TestClass]
public class Ctrl
{
    [TestMethod]
    public async Task Set_Addr()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x11111;
        emulator.Vera.Data1_Address = 0x12222;
        emulator.A = 0x01;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta CTRL
                stp",
                emulator);

        Assert.AreEqual(true, emulator.Vera.AddrSel);
        Assert.AreEqual(0x01, emulator.Memory[0x9F25]);

        Assert.AreEqual(0x22, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x22, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x01, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task UnSet_Addr()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x11111;
        emulator.Vera.Data1_Address = 0x12222;
        emulator.Vera.AddrSel = false;
        emulator.A = 0x00;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta CTRL
                stp",
                emulator);

        Assert.AreEqual(false, emulator.Vera.AddrSel);
        Assert.AreEqual(0x00, emulator.Memory[0x9F25]);

        Assert.AreEqual(0x11, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x11, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x01, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task Set_All()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x11111;
        emulator.Vera.Data1_Address = 0x12222;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta CTRL
                stp",
                emulator);

        Assert.AreEqual(0x03, emulator.Memory[0x9F25]);
    }
}