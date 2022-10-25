using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Via;

[TestClass]
public class Acr
{
    [TestMethod]
    public async Task Acr_Timer1Continous()
    {
        var emulator = new Emulator();
        emulator.A = 0x40;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_ACR
                stp",
                emulator);

        Assert.AreEqual(0x40, emulator.Memory[0x9f0b]);
        Assert.IsTrue(emulator.Via.Timer1_Continous);
    }

    [TestMethod]
    public async Task Acr_Timer1Pb7()
    {
        var emulator = new Emulator();
        emulator.A = 0x80;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_ACR
                stp",
                emulator);

        Assert.AreEqual(0x80, emulator.Memory[0x9f0b]);
        Assert.IsTrue(emulator.Via.Timer1_Pb7);
    }

    [TestMethod]
    public async Task Acr_Timer2Pulse()
    {
        var emulator = new Emulator();
        emulator.A = 0x20;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_ACR
                stp",
                emulator);

        Assert.AreEqual(0x20, emulator.Memory[0x9f0b]);
        Assert.IsTrue(emulator.Via.Timer2_PulseCount);
    }
}