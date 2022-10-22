using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Core;

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

    [TestMethod]
    public async Task RamBank_GetBanks()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810

            .const ram_bank=$00
                stz ram_bank

                ldx $a000
                inx

                lda #1
            .loop:
            	sta ram_bank

                ldy $a000
                stx $a000
                stz ram_bank
                cpx $a000
                sta ram_bank
                sty $a000
                beq done

                asl
                bne loop
            .done:
            	tay
                stz ram_bank
                dex

                stx $a000

                tya; number of RAM banks
                stp",
                emulator);

        emulator.AssertState(Y: 0x00); // 2048 apparently.
    }



}