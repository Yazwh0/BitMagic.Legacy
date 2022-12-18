using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera.Display;

[TestClass]
public class Sprites_8bpp
{
    [TestMethod]
    public async Task Render_Depth1()
    {
        var emulator = new Emulator();

        emulator.Vera.SpriteEnable = true;
        emulator.Sprites[0].Depth = 1;
        emulator.Sprites[0].Mode = 0b1000000; // 8bpp
        emulator.Sprites[0].X = 10;
        emulator.Sprites[0].Y = 10;

        emulator.Sprites[1].Depth = 1;
        emulator.Sprites[1].Mode = 0b1000000; // 8bpp
        emulator.Sprites[1].X = 14;
        emulator.Sprites[1].Y = 14;

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

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\sprites_8bpp_8x8_depth1.png");

        emulator.CompareImage(@"Vera\Images\sprites_8bpp_8x8_depth1.png");
    }

    [TestMethod]
    public async Task Render_Depth2()
    {
        var emulator = new Emulator();

        emulator.Vera.SpriteEnable = true;
        emulator.Sprites[0].Depth = 2;
        emulator.Sprites[0].Mode = 0b1000000; // 8bpp
        emulator.Sprites[0].X = 10;
        emulator.Sprites[0].Y = 10;

        emulator.Sprites[1].Depth = 2;
        emulator.Sprites[1].Mode = 0b1000000; // 8bpp
        emulator.Sprites[1].X = 14;
        emulator.Sprites[1].Y = 14;

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

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\sprites_8bpp_8x8_depth2.png");

        emulator.CompareImage(@"Vera\Images\sprites_8bpp_8x8_depth2.png");
    }

    [TestMethod]
    public async Task Render_Depth3()
    {
        var emulator = new Emulator();

        emulator.Vera.SpriteEnable = true;
        emulator.Sprites[0].Depth = 3;
        emulator.Sprites[0].Mode = 0b1000000; // 8bpp
        emulator.Sprites[0].X = 10;
        emulator.Sprites[0].Y = 10;

        emulator.Sprites[1].Depth = 3;
        emulator.Sprites[1].Mode = 0b1000000; // 8bpp
        emulator.Sprites[1].X = 14;
        emulator.Sprites[1].Y = 14;

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

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\sprites_8bpp_8x8_depth3.png");

        emulator.CompareImage(@"Vera\Images\sprites_8bpp_8x8_depth3.png");
    }

    [TestMethod]
    public async Task Render_MultiDepth()
    {
        var emulator = new Emulator();

        emulator.Vera.SpriteEnable = true;
        emulator.Sprites[0].Depth = 3;
        emulator.Sprites[0].Mode = 0b1000000; // 8bpp
        emulator.Sprites[0].X = 10;
        emulator.Sprites[0].Y = 10;

        emulator.Sprites[1].Depth = 2;
        emulator.Sprites[1].Mode = 0b1000000; // 8bpp
        emulator.Sprites[1].X = 14;
        emulator.Sprites[1].Y = 14;

        emulator.Sprites[2].Depth = 1;
        emulator.Sprites[2].Mode = 0b1000000; // 8bpp
        emulator.Sprites[2].X = 18;
        emulator.Sprites[2].Y = 18;

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

        //emulator.SaveDisplay(@"D:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\sprites_8bpp_8x8_multidepth.png");

        emulator.CompareImage(@"Vera\Images\sprites_8bpp_8x8_multidepth.png");
    }
}