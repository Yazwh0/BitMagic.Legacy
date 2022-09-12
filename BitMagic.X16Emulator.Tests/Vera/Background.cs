using BitMagic.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class Background
{
    [TestMethod]
    public async Task Area()
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
                stp",
                emulator);

        for (var y = 0; y < 480; y++) {
            for (var x = 0; x < 640; x++)
            {
                Assert.AreEqual(new PixelRgba(0, 0, 0), emulator.Display[x + y * 800], $"At {x}, {y}");
            }
            for (var x = 640; x < 800; x++)
            {
                Assert.AreEqual(new PixelRgba(0, 0, 0, 0), emulator.Display[x + y * 800], $"At {x}, {y}");
            }
        }

        for (var y = 480; y < 525; y++)
        {
            for (var x = 0; x < 800; x++)
            {
                Assert.AreEqual(new PixelRgba(0, 0, 0, 0), emulator.Display[x + y * 800], $"At {x}, {y}");
            }
        }
    }

    [TestMethod]
    public async Task Colour()
    {
        var emulator = new Emulator();

        emulator.InterruptDisable = true;
        emulator.Interrupt = false;

        emulator.RomBank[0x3ffe] = 0x00;
        emulator.RomBank[0x3fff] = 0x09;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                    lda #$01
                    sta ADDRx_H
                    lda #$fa
                    sta ADDRx_M
                    lda #$ab
                    sta DATA0
                    lda #01
                    sta IEN
                    wai
                    sta ISR     ; clear interrupt and wait for second frame
                    wai
                    stp",
                emulator);

        for (var y = 0; y < 480; y++)
        {
            for (var x = 0; x < 640; x++)
            {
                Assert.AreEqual(new PixelRgba(0, 0xaa, 0xbb), emulator.Display[x + y * 800], $"At {x}, {y}");
            }
            for (var x = 640; x < 800; x++)
            {
                Assert.AreEqual(new PixelRgba(0, 0, 0, 0), emulator.Display[x + y * 800], $"At {x}, {y}");
            }
        }

        for (var y = 480; y < 525; y++)
        {
            for (var x = 0; x < 800; x++)
            {
                Assert.AreEqual(new PixelRgba(0, 0, 0, 0), emulator.Display[x + y * 800], $"At {x}, {y}");
            }
        }
    }
    
    

}