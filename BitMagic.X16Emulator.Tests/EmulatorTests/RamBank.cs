using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class RamBank
{
    [TestMethod]
    public async Task RamBank_Read()
    {
        var emulator = new Emulator();

        emulator.RamBank[0x2000] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda #$01
                sta $00
                ldx $a000
                stp",
                emulator);

        emulator.AssertState(X: 0xff);

        // emulation
        //emulator.AssertState(0x00, 0x00, 0x00, 0x811, 0);
        //emulator.AssertFlags(false, false, false, true);
    }
}