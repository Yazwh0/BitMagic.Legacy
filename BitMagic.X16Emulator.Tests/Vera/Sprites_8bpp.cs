using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera;

[TestClass]
public class Sprites_8bpp
{
    [TestMethod]
    public async Task Render()
    {
        var emulator = new Emulator();

        emulator.Vera.SpriteEnable = true;
        emulator.Sprites[0].Depth = 1;
        emulator.Sprites[0].Mode = 0b1000000; // 8bpp

        var index = 0;
        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 8; x++)
            {
                emulator.Vera.Vram[index++] = (byte)(x + (y * 8) + 1);
            }
        }

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sei

                lda #01
                sta IEN
                wai
                sta ISR     ; clear interrupt and wait for second frame
                wai

                stp",
                emulator);

        emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\sprites_8bpp_8x8.png");

        //emulator.CompareImage(@"Vera\Images\sprites_8bpp_8x8.png");

    }
}