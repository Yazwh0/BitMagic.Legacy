using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace ImageProcessor;

public class RawImage
{
    public byte[] Pixels { get; set; } = Array.Empty<byte>();
    public byte[] X16Colours { get; set; } = Array.Empty<byte>();
}

public static class ImageAsset
{
    public static RawImage LoadFullImage(string filename)
    {
        using (var image = Image.Load<Rgba32>(filename))
        {
            if (image.Width != 320)
            {
                throw new Exception($"Image must be 320px wide. Image is {image.Width} x {image.Height}");
            }

            if (image.Height != 240)
            {
                throw new Exception($"Image must be 240px high. Image is {image.Width} x {image.Height}");
            }

            var toReturn = new RawImage();

            //image.Save(File.Create(@"D:\Documents\Source\BitMagic\output.png"), new PngEncoder());
            toReturn.Pixels = new byte[image.Width * image.Height];

            Dictionary<Rgba32, byte> palette = new ();
            var paletteIndex = 0;
            var idx = 0;

            for(var y = 0; y < image.Height; y++)
            {
                var rowSpan = image.GetPixelRowSpan(y);
                for(var x = 0; x < image.Width; x++)
                {
                    var thisColour = rowSpan[x];
                    if (!palette.ContainsKey(thisColour))
                        palette.Add(thisColour, (byte)paletteIndex++);                    

                    //Console.Write($"{palette[thisColour]:X2}, ");
                    toReturn.Pixels[idx++] = palette[thisColour];
                }
                //Console.WriteLine();
            }

            toReturn.X16Colours = new byte[palette.Count * 2];
            foreach(var kv in palette.OrderBy(kv => kv.Value))
            {
                toReturn.X16Colours[kv.Value*2] = (byte)((kv.Key.G & 0xf0) + (kv.Key.B & 0x0f));
                toReturn.X16Colours[kv.Value*2 + 1] = (byte)(kv.Key.R & 0x0f);
            }

            File.WriteAllBytes(@"D:\Documents\Source\BitMagic\output.bin", toReturn.Pixels);

            return toReturn;
        }        
    }
}

