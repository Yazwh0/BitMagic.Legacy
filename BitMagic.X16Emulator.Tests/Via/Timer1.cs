using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Via;

[TestClass]
public class Timer1
{
    [TestMethod]
    public async Task Timer1_Count()
    {
        var emulator = new Emulator();
        emulator.Via.Timer1_Counter = 0x1234;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x34, emulator.Memory[0x9f04]);
        Assert.AreEqual(0x12, emulator.Memory[0x9f05]);
    }

    [TestMethod]
    public async Task Timer1_Latch()
    {
        var emulator = new Emulator();
        emulator.Via.Timer1_Latch = 0x1234;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x34, emulator.Memory[0x9f06]);
        Assert.AreEqual(0x12, emulator.Memory[0x9f07]);
    }

    [TestMethod]
    public async Task Timer1_Latch_Read()
    {
        var emulator = new Emulator();
        emulator.Via.Timer1_Latch = 0x1234;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda V_T1L_L
                ldx V_T1L_H
                stp",
                emulator);

        emulator.AssertState(0x34, 0x12);
    }

    [TestMethod]
    public async Task Timer1_Count_Change()
    {
        var emulator = new Emulator();
        emulator.Via.Timer1_Counter = 0x1234;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                nop
                stp",
                emulator);

        Assert.AreEqual(0x32, emulator.Memory[0x9f04]); // -2 for nop
        Assert.AreEqual(0x12, emulator.Memory[0x9f05]);
    }

    [TestMethod]
    public async Task Timer1_Count_Check()
    {
        var emulator = new Emulator();
        emulator.Via.Timer1_Counter = 0x1000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                nop
                lda V_T1_L
                ldx V_T1_H
                stp",
                emulator);

        emulator.AssertState(0xfe, 0x0f);
    }

    [TestMethod]
    public async Task Timer1_Count_Write_L()
    {
        var emulator = new Emulator();
        emulator.Via.Timer1_Counter = 0x1234;
        emulator.A = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_T1_L
                stp",
                emulator);

        Assert.AreEqual(0x10, emulator.Memory[0x9f06]);

        Assert.AreEqual(0x30, emulator.Memory[0x9f04]); // shouldn't be effected, is 34-4
        Assert.AreEqual(0x12, emulator.Memory[0x9f05]);
    }

    [TestMethod]
    public async Task Timer1_Count_Write_H()
    {
        var emulator = new Emulator();
        emulator.Via.Timer1_Counter = 0x1234;
        emulator.A = 0xa0;
        emulator.Memory[0x9f0d] = 0x40;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_T1_H
                stp",
                emulator);

        Assert.AreEqual(0xa0, emulator.Memory[0x9f07]);

        Assert.AreEqual(0xfc, emulator.Memory[0x9f04]); // timer has moved on by 4 ticks from $a000
        Assert.AreEqual(0x9f, emulator.Memory[0x9f05]);

        Assert.AreEqual(0x00, emulator.Memory[0x9f0d]);

        Assert.IsTrue(emulator.Via.Timer1_Running);
    }

    [TestMethod]
    public async Task Timer1_Interrupt_OneShot()
    {
        var emulator = new Emulator();
        emulator.Via.Timer1_Counter = 0x1000;
        emulator.Via.Timer1_Latch = 0x1000;
        emulator.Via.Interrupt_Timer1 = true;
        emulator.Via.Timer1_Continous = false;
        emulator.A = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sei
                sta V_T1_H
                wai
                stp
                ",
                emulator);

        Assert.AreEqual(0b11000000, emulator.Memory[0x9f0d]);
        Assert.IsFalse(emulator.Via.Timer1_Running);
    }

    [TestMethod]
    public async Task Timer1_Interrupt_Continuous()
    {
        var emulator = new Emulator();
        emulator.Via.Timer1_Counter = 0x1000;
        emulator.Via.Timer1_Latch = 0x1000;
        emulator.Via.Interrupt_Timer1 = true;
        emulator.Via.Timer1_Continous = true;
        emulator.A = 0x10;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sei
                sta V_T1_H
                wai
                stp
                ",
                emulator);

        Assert.AreEqual(0b11000000, emulator.Memory[0x9f0d]);
        Assert.IsTrue(emulator.Via.Timer1_Running);
    }
}