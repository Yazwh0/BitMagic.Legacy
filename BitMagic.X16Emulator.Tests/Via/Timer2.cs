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
        emulator.Via.Timer2_Latch = 0x1234;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x34, emulator.Memory[0x9f08]);
        Assert.AreEqual(0x12, emulator.Memory[0x9f09]);
        Assert.AreEqual(0x1234, emulator.Via.Timer2_Counter);
    }

    [TestMethod]
    public async Task Timer2_Count_Change()
    {
        var emulator = new Emulator();
        emulator.Via.Timer2_Counter = 0x1234;
        emulator.Via.Timer2_Latch = 0x1234;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                nop
                stp",
                emulator);

        Assert.AreEqual(0x32, emulator.Memory[0x9f08]); // -2 for nop
        Assert.AreEqual(0x12, emulator.Memory[0x9f09]);
        Assert.AreEqual(0x1232, emulator.Via.Timer2_Counter);
    }

    [TestMethod]
    public async Task Timer2_Count_Check()
    {
        var emulator = new Emulator();
        emulator.Via.Timer2_Counter = 0x1000;
        emulator.Via.Timer2_Latch = 0x1000;

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

    [TestMethod]
    public async Task Timer2_Pulse()
    {
        var emulator = new Emulator();
        emulator.Via.Timer2_Counter = 0x1234;
        emulator.Via.Timer2_Latch = 0x1234;
        emulator.Via.Timer2_PulseCount = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                nop
                stp",
                emulator);

        Assert.AreEqual(0x34, emulator.Memory[0x9f08]);
        Assert.AreEqual(0x12, emulator.Memory[0x9f09]);
        Assert.AreEqual(0x1234, emulator.Via.Timer2_Counter);
    }

    [TestMethod]
    public async Task Timer2_LatchSet()
    {
        var emulator = new Emulator();
        emulator.Via.Timer2_Counter = 0xeeee;
        emulator.Via.Timer2_Latch = 0xeeee;
        emulator.A = 0x34;
        emulator.X = 0x12;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                nop
                sta V_T2_L
                stx V_T2_H
                stp",
                emulator);

        Assert.AreEqual(0x1234 - 4, emulator.Via.Timer2_Counter); // -4 for the stx, but this value is wrong
        Assert.IsTrue(emulator.Via.Timer2_Running);
    }

    [TestMethod]
    public async Task Timer2_Interrupt_OneShot()
    {
        var emulator = new Emulator();
        emulator.Via.Timer2_Counter = 0x1000;
        emulator.Via.Timer2_Latch = 0x1000;
        emulator.Via.Interrupt_Timer2 = true;
        emulator.A = 0x10;

        emulator.RomBank[0x3ffa] = 0x00;
        emulator.RomBank[0x3ffb] = 0x09;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_T2_H
                wai
                stp
                .org $900
                stp
                ",
                emulator);

        emulator.AssertState(Pc: 0x901);
        Assert.AreEqual(0b10100000, emulator.Memory[0x9f0d]);
        Assert.IsTrue(emulator.Clock > 0x1000);
        Assert.IsFalse(emulator.Via.Timer2_Running);
    }
}