using System.ComponentModel.DataAnnotations;

namespace Vera;

[Flags]
public enum Layers
{
    None = 0,
    Layer0  = 0b00010000,
    Layer1  = 0b00100000,
    Sprites = 0b01000000
}

public enum OutputMode
{
    Disabled,
    VGA,
    NTSC,
    RGB_Interlaced
}

public enum Resolution
{
    [Display(Name = "640x480")]
    Full,
    [Display(Name = "320x240")]
    Half
}

public enum ConfigLayer 
{
    Layer0,
    Layer1
}

public enum Depth
{
    Bpp_1,
    Bpp_2,
    Bpp_4,
    Bpp_8
}

public enum BitmapWidth
{
    Half_320,
    Full_640
}

public static class Video {

    public static void Mode(Layers layers, OutputMode mode = OutputMode.VGA, bool chromaDisable = false)
    {
        int output = 0;

        if (chromaDisable)
            output = output & 0b100;
        
        output = output + (int)layers;
        output = output + (int)mode;

BitMagic.AsmTemplate.Template.WriteLiteral($@";     |.s10.coo");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #0b{Convert.ToString(output, 2).PadLeft(8, '0')}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta DC_VIDEO");
    }

    public static void Scaling(Resolution resolution)
    {
        var scale = resolution switch {
            Resolution.Full => 128,
            Resolution.Half => 64,
            _ => throw new Exception($"Unhandled resolution {resolution}")
        };

BitMagic.AsmTemplate.Template.WriteLiteral($@"; Set resolution to {resolution}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #{scale}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta DC_HSCALE");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta DC_VSCALE");
    }

    public static void LayerBitmap(ConfigLayer layer, Depth depth, BitmapWidth width, int baseAddress, int paletteOffset = 0)
    {
        var prefix = layer switch {
            ConfigLayer.Layer0 => "L0",
            ConfigLayer.Layer1 => "L1",
            _ => throw new Exception($"Unhandled layer {layer}")
        };

        var config = (int)depth;
        config = config + 0b100; // bitmap

BitMagic.AsmTemplate.Template.WriteLiteral($@";   256 Bitmap mode");
BitMagic.AsmTemplate.Template.WriteLiteral($@";   |       bdd");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #0b{Convert.ToString(config, 2).PadLeft(8, '0')}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta {prefix}_CONFIG");
        var mapBase = (baseAddress >> 11) << 2;
        mapBase += (int)width;

BitMagic.AsmTemplate.Template.WriteLiteral($@";   Base address ${baseAddress.ToString("X4")}");
BitMagic.AsmTemplate.Template.WriteLiteral($@";   Width {width}");
        if (mapBase == 0)
        {
BitMagic.AsmTemplate.Template.WriteLiteral($@"stz {prefix}_TILEBASE");
        } 
        else 
        {
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #{mapBase}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta {prefix}_TILEBASE");
        }

BitMagic.AsmTemplate.Template.WriteLiteral($@";   Palette Offset");
        if (paletteOffset == 0)
        {
BitMagic.AsmTemplate.Template.WriteLiteral($@"stz {prefix}_HSCROLL_L");
        } 
        else 
        {
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #paletteOffset");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta {prefix}_HSCROLL_L");
        }
    }

}
