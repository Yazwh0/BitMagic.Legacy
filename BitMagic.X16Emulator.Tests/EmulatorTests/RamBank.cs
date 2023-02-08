using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Core;

[TestClass]
public class RamBank
{
    [TestMethod]
    public async Task RamBank_Read_1()
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
    }

    [TestMethod]
    public async Task RamBank_Read_2()
    {
        var emulator = new Emulator();

        emulator.RamBank[0x4000] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda #$02
                sta $00
                ldx $a000
                stp",
                emulator);

        emulator.AssertState(X: 0xff);
    }

    [TestMethod]
    public async Task RamBank_Read_255()
    {
        var emulator = new Emulator();

        emulator.RamBank[0x2000 * 255] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda #$ff
                sta $00
                ldx $a000
                stp",
                emulator);

        emulator.AssertState(X: 0xff);
    }

    [TestMethod]
    public async Task RamBank_Inc()
    {
        var emulator = new Emulator();

        emulator.RamBank[0x2000] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                inc $00
                ldx $a000
                stp",
                emulator);

        emulator.AssertState(X: 0xff);
    }

    [TestMethod]
    public async Task RamBank_Read_All()
    {
        var emulator = new Emulator();

        for (var i = 0; i < 256; i++)
        {
            emulator.RamBank[0x2000 * i + i] = 0xff;
        }

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                ldx #0
            .loop:
                stx $00
                lda $a000, x
                cmp #$ff
                bne fail
                inx
                bne loop

                ldy #$0
                stp
            .fail:
                ldy #01
                stp
                ",
                emulator);

        if (emulator.Y != 0)
        {
            for (var i = 0; i < 16; i++)
            {
                for (var j = 0; j < 16; j++)
                {
                    Console.Write($"${emulator.Memory[0xa000 + i * 16 + j]:X2} ");
                }
                Console.WriteLine();
            }
        }

        emulator.AssertState(Y: 0x00);
    }

    [TestMethod]
    public async Task RamBank_PreRomChange_Read_All()
    {
        var emulator = new Emulator();

        for (var i = 0; i < 256; i++)
        {
            emulator.RamBank[0x2000 * i + i] = 0xff;
        }

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda #1
                sta $01
                lda #2
                sta $01

                ldx #0
            .loop:
                stx $00
                lda $a000, x
                cmp #$ff
                bne fail
                inx
                bne loop

                ldy #$0
                stp
            .fail:
                ldy #01
                stp
                ",
                emulator);

        if (emulator.Y != 0)
        {
            for (var i = 0; i < 16; i++)
            {
                for (var j = 0; j < 16; j++)
                {
                    Console.Write($"${emulator.Memory[0xa000 + i * 16 + j]:X2} ");
                }
                Console.WriteLine();
            }
        }

        emulator.AssertState(Y: 0x00);
    }

    [TestMethod]
    public async Task RamBank_RomChange_Read_All()
    {
        var emulator = new Emulator();

        for (var i = 0; i < 256; i++)
        {
            emulator.RamBank[0x2000 * i + i] = 0xff;
        }

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810

                ldx #0
            .loop:
                stx $00
                lda $a000, x
                cmp #$ff
                bne fail
                inx
                stx $01

                bne loop

                ldy #$0
                stp
            .fail:
                ldy #01
                stp
                ",
                emulator);

        if (emulator.Y != 0)
        {
            for (var i = 0; i < 16; i++)
            {
                for (var j = 0; j < 16; j++)
                {
                    Console.Write($"${emulator.Memory[0xa000 + i * 16 + j]:X2} ");
                }
                Console.WriteLine();
            }
        }

        emulator.AssertState(Y: 0x00);
    }

    [TestMethod]
    public async Task RamBank_SwitchRead()
    {
        var emulator = new Emulator();

        emulator.RamBank[0x2000] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                lda #$02
                sta $00
                lda #$01
                sta $00
                ldx $a000
                stp",
                emulator);

        emulator.AssertState(X: 0xff);
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