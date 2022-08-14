using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class RomBank
{
    [TestMethod]
    public async Task Read()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x4000] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda #$01
                sta $01
                ldx $c000
                stp",
                emulator);

        emulator.AssertState(X: 0xff);
    }

    [TestMethod]
    public async Task ReadIgnoreHighBits()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x4000] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda #$e1 ; still bank 1
                sta $01
                ldx $c000
                stp",
                emulator);

        emulator.AssertState(X: 0xff);
    }

}