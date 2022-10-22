using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Via;

[TestClass]
public class Timer2
{

    [TestMethod]
    public async Task Timer2_Count()
    {
        var emulator = new Emulator();
        emulator.Via.Timer2_Counter = 0x1234;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x34, emulator.Memory[0x9f08]);
        Assert.AreEqual(0x12, emulator.Memory[0x9f09]);
    }

    [TestMethod]
    public async Task Timer2_Count_Change()
    {
        var emulator = new Emulator();
        emulator.Via.Timer2_Counter = 0x1234;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                nop
                stp",
                emulator);

        Assert.AreEqual(0x32, emulator.Memory[0x9f08]); // -2 for nop
        Assert.AreEqual(0x12, emulator.Memory[0x9f09]);
    }

    [TestMethod]
    public async Task Timer2_Count_Check()
    {
        var emulator = new Emulator();
        emulator.Via.Timer2_Counter = 0x1000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                nop
                lda V_T2_L
                ldx V_T2_H
                stp",
                emulator);

        emulator.AssertState(0xfe, 0x0f);
    }
}