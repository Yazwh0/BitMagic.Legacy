using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class Interrupts
{
    [TestMethod]
    public async Task Interrupt_Hit()
    {
        var emulator = new Emulator();

        emulator.Interrupt = true;

        emulator.RomBank[0x3ffe] = 0x00;
        emulator.RomBank[0x3fff] = 0x09;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                nop
                stp
                .org $900
                stp",
                emulator);

        // emulation
        emulator.AssertState(Pc: 0x901);
        emulator.AssertFlags(InterruptDisable: true, Interrupt: true, Nmi: false);
    }

    [TestMethod]
    public async Task Interrupt_Stack()
    {
        var emulator = new Emulator();

        emulator.Interrupt = true;

        emulator.RomBank[0x3ffe] = 0x00;
        emulator.RomBank[0x3fff] = 0x09;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                nop
                stp
                .org $900
                stp",
                emulator);

        emulator.AssertState(stackPointer: 0x1fa); // 3 bytes
        Assert.AreEqual(0b00100000, emulator.Memory[0x1fb]);
        Assert.AreEqual(0x08, emulator.Memory[0x1fc]);
        Assert.AreEqual(0x10, emulator.Memory[0x1fd]);
    }

    [TestMethod]
    public async Task Interrupt_Stack_FlagsSet()
    {
        var emulator = new Emulator();

        emulator.Interrupt = true;
        emulator.Carry = true;
        emulator.Zero = true;
        emulator.Decimal = true;
        emulator.Overflow = true;
        emulator.Negative = true;

        emulator.RomBank[0x3ffe] = 0x00;
        emulator.RomBank[0x3fff] = 0x09;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                nop
                stp
                .org $900
                stp",
                emulator);

        emulator.AssertState(stackPointer: 0x1fa); // 3 bytes
        Assert.AreEqual(0b11101011, emulator.Memory[0x1fb]); // no interrupt disable obv
        Assert.AreEqual(0x08, emulator.Memory[0x1fc]);
        Assert.AreEqual(0x10, emulator.Memory[0x1fd]);
    }

    [TestMethod]
    public async Task Nmi_Hit()
    {
        var emulator = new Emulator();

        emulator.Nmi = true;

        emulator.RomBank[0x3ffa] = 0x00;
        emulator.RomBank[0x3ffb] = 0x09;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                nop
                stp
                .org $900
                stp",
                emulator);

        // emulation
        emulator.AssertState(Pc: 0x901);
        emulator.AssertFlags(InterruptDisable: true, Interrupt: false, Nmi: true);
    }
}