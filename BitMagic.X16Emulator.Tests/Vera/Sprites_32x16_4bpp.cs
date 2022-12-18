using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera.Display;

[TestClass]
public class Sprites_32x16_4bpp
{
    [TestMethod]
    public async Task Render_Depth1()
    {
        var emulator = new Emulator();

        emulator.Vera.SpriteEnable = true;
        emulator.Sprites[0].Depth = 1;
        emulator.Sprites[0].Mode = 0b0011000; // 4bpp 32 x 16
        emulator.Sprites[0].Width = 32;
        emulator.Sprites[0].Height = 16;
        emulator.Sprites[0].X = 10;
        emulator.Sprites[0].Y = 10;

        var index = 0;
        for (var y = 0; y < 32; y++) // 4bpp so halve
        {
            for (var x = 0; x < 16; x++)
            {
                emulator.Vera.Vram[index++] = (byte)(x == 15 ? 0x44 : 0x22);
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

        emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\sprites_4bpp_32x16_depth1.png");

        //emulator.CompareImage(@"Vera\Images\sprites_4bpp_32x16_depth1.png");
    }
}