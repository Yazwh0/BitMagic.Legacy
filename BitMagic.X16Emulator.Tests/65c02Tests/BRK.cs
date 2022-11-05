using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class Brk
{
    [TestMethod]
    public async Task Nmi_Hit()
    {
        var emulator = new Emulator();

        emulator.Nmi = false;

        emulator.RomBank[0x3ffa] = 0x00;
        emulator.RomBank[0x3ffb] = 0x09;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                brk
                stp
                .org $900
                stp",
                emulator);

        // emulation
        emulator.AssertState(Pc: 0x901);
        emulator.AssertFlags(InterruptDisable: true, Interrupt: false, Nmi: false);
    }

    [TestMethod]
    public async Task Nmi_Hit_Return()
    {
        var emulator = new Emulator();

        emulator.Nmi = false;

        emulator.RomBank[0x3ffa] = 0x00;
        emulator.RomBank[0x3ffb] = 0x09;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                brk
                stp
                .org $900
                rti",
                emulator);

        // emulation
        emulator.AssertState(Pc: 0x812);
        emulator.AssertFlags(InterruptDisable: false, Interrupt: false, Nmi: false);
    }
}