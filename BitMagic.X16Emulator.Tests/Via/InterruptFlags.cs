using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Via;

[TestClass]
public class InerruptFlags
{
    [TestMethod]
    public async Task Interrupt_Timer1()
    {
        var emulator = new Emulator();
        emulator.Via.Interrupt_Timer1 = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.IsTrue(emulator.Via.Interrupt_Timer1);
        Assert.AreEqual(0x040, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_Timer2()
    {
        var emulator = new Emulator();
        emulator.Via.Interrupt_Timer2 = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.IsTrue(emulator.Via.Interrupt_Timer2);
        Assert.AreEqual(0x20, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_Cb1()
    {
        var emulator = new Emulator();
        emulator.Via.Interrupt_Cb1 = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.IsTrue(emulator.Via.Interrupt_Cb1);
        Assert.AreEqual(0x10, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_Cb2()
    {
        var emulator = new Emulator();
        emulator.Via.Interrupt_Cb2 = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.IsTrue(emulator.Via.Interrupt_Cb2);
        Assert.AreEqual(0x08, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_ShiftRegister()
    {
        var emulator = new Emulator();
        emulator.Via.Interrupt_ShiftRegister = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.IsTrue(emulator.Via.Interrupt_ShiftRegister);
        Assert.AreEqual(0x04, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_Ca1()
    {
        var emulator = new Emulator();
        emulator.Via.Interrupt_Ca1= true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.IsTrue(emulator.Via.Interrupt_Ca1);
        Assert.AreEqual(0x02, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_Ca2()
    {
        var emulator = new Emulator();
        emulator.Via.Interrupt_Ca2 = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.IsTrue(emulator.Via.Interrupt_Ca2);
        Assert.AreEqual(0x01, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_Set_Ca2()
    {
        var emulator = new Emulator();
        emulator.A = 0b10000001;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_IER
                stp",
                emulator);

        Assert.IsTrue(emulator.Via.Interrupt_Ca2);
        Assert.AreEqual(0x81, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_Set_Ca1()
    {
        var emulator = new Emulator();
        emulator.A = 0b10000010;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_IER
                stp",
                emulator);

        Assert.IsTrue(emulator.Via.Interrupt_Ca1);
        Assert.AreEqual(0x82, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_Set_ShiftRegister()
    {
        var emulator = new Emulator();
        emulator.A = 0b10000100;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_IER
                stp",
                emulator);

        Assert.IsTrue(emulator.Via.Interrupt_ShiftRegister);
        Assert.AreEqual(0x84, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_Set_Cb2()
    {
        var emulator = new Emulator();
        emulator.A = 0b10001000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_IER
                stp",
                emulator);

        Assert.IsTrue(emulator.Via.Interrupt_Cb2);
        Assert.AreEqual(0x88, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_Set_Cb1()
    {
        var emulator = new Emulator();
        emulator.A = 0b10010000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_IER
                stp",
                emulator);

        Assert.IsTrue(emulator.Via.Interrupt_Cb1);
        Assert.AreEqual(0x90, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_Set_Timer2()
    {
        var emulator = new Emulator();
        emulator.A = 0b10100000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sei
                sta V_IER
                stp",
                emulator);

        Assert.IsTrue(emulator.Via.Interrupt_Timer2);
        Assert.AreEqual(0xa0, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_Set_Timer1()
    {
        var emulator = new Emulator();
        emulator.A = 0b11000000;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sei
                sta V_IER
                stp",
                emulator);

        Assert.IsTrue(emulator.Via.Interrupt_Timer1);
        Assert.AreEqual(0xc0, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_UnSet_Ca2()
    {
        var emulator = new Emulator();
        emulator.A = 0b00000001;
        emulator.Via.Interrupt_Ca2 = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_IER
                stp",
                emulator);

        Assert.IsFalse(emulator.Via.Interrupt_Ca2);
        Assert.AreEqual(0x80, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_UnSet_Ca1()
    {
        var emulator = new Emulator();
        emulator.A = 0b00000010;
        emulator.Via.Interrupt_Ca1 = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_IER
                stp",
                emulator);

        Assert.IsFalse(emulator.Via.Interrupt_Ca1);
        Assert.AreEqual(0x80, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_UnSet_ShiftRegister()
    {
        var emulator = new Emulator();
        emulator.A = 0b00000100;
        emulator.Via.Interrupt_ShiftRegister = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_IER
                stp",
                emulator);

        Assert.IsFalse(emulator.Via.Interrupt_ShiftRegister);
        Assert.AreEqual(0x80, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_UnSet_Cb2()
    {
        var emulator = new Emulator();
        emulator.A = 0b00001000;
        emulator.Via.Interrupt_Cb2 = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_IER
                stp",
                emulator);

        Assert.IsFalse(emulator.Via.Interrupt_Cb2);
        Assert.AreEqual(0x80, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_UnSet_Cb1()
    {
        var emulator = new Emulator();
        emulator.A = 0b00010000;
        emulator.Via.Interrupt_Cb1 = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_IER
                stp",
                emulator);

        Assert.IsFalse(emulator.Via.Interrupt_Cb1);
        Assert.AreEqual(0x80, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_UnSet_Timer2()
    {
        var emulator = new Emulator();
        emulator.A = 0b00100000;
        emulator.Via.Interrupt_Timer2 = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_IER
                stp",
                emulator);

        Assert.IsFalse(emulator.Via.Interrupt_Timer2);
        Assert.AreEqual(0x80, emulator.Memory[0x9f0e]);
    }

    [TestMethod]
    public async Task Interrupt_UnSet_Timer1()
    {
        var emulator = new Emulator();
        emulator.A = 0b01000000;
        emulator.Via.Interrupt_Timer1 = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_IER
                stp",
                emulator);

        Assert.IsFalse(emulator.Via.Interrupt_Timer1);
        Assert.AreEqual(0x80, emulator.Memory[0x9f0e]);
    }
}