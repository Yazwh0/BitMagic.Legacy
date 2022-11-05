using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class WAI
{
    [TestMethod]
    public async Task Interrupts_Enabled()
    {
        var emulator = new Emulator();

        emulator.InterruptDisable = false;
        emulator.Interrupt = false;

        emulator.RomBank[0x3ffe] = 0x00;
        emulator.RomBank[0x3fff] = 0x09;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda #01
                sta IEN 
                wai            
                stp

                .org $900
                stp",
                emulator);

        // emulation
        Assert.IsTrue(emulator.Clock > 0x1000);         // we're waiting for an interrupt, so this will take some time
        emulator.AssertState(0x01, 0x00, 0x00, 0x901);
        emulator.AssertFlags(false, false, false, false, true, Interrupt: true);
    }

    [TestMethod]
    public async Task Interrupts_Disabled()
    {
        var emulator = new Emulator();

        emulator.InterruptDisable = true;
        emulator.Interrupt = false;

        emulator.RomBank[0x3ffe] = 0x00;
        emulator.RomBank[0x3fff] = 0x09;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda #01
                sta IEN
                wai            
                stp

                .org $900
                stp",
                emulator);

        // emulation
        Assert.IsTrue(emulator.Clock > 0x1000);         // we're waiting for an interrupt, so this will take some time
        emulator.AssertState(0x01, 0x00, 0x00, 0x817);
        emulator.AssertFlags(false, false, false, false, true, Interrupt: true);
    }
}