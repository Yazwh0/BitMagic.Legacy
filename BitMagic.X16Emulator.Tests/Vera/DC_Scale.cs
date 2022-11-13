using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera;

[TestClass]
public class DC_Scale
{
    [TestMethod]
    public async Task DC_HScale()
    {
        var emulator = new Emulator();

        emulator.A = 0b01000000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DC_HSCALE
                stp",
                emulator);

        Assert.AreEqual(0b01000000, emulator.Memory[0x9F2A]);
        Assert.AreEqual((UInt32)0x08000, emulator.Vera.Dc_HScale);
    }

    [TestMethod]
    public async Task DC_VScale()
    {
        var emulator = new Emulator();

        emulator.A = 0b01000000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta DC_VSCALE
                stp",
                emulator);

        Assert.AreEqual(0b01000000, emulator.Memory[0x9F2B]);
        Assert.AreEqual((UInt32)0x08000, emulator.Vera.Dc_VScale);
    }
}