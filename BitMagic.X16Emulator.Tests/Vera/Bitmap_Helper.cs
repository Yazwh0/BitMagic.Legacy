using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera
{
    internal static class ImageHelper
    {
        public enum ColourDepth
        {
            Depth_1bpp,
            Depth_2bpp,
            Depth_4bpp,
            Depth_8bpp
        }

        public static void LoadImage(this Emulator emulator, string filename, ColourDepth depth, int address)
        {
            var sb = new StringBuilder();

            using var image = Image.Load<Rgba32>(filename);

            if ((image.Height != 480 || image.Width != 640) && (image.Height != 240 || image.Width != 320))
                Assert.Fail("Source image must be 640x480px or 320x240.");

            var counter = 1;
            var colourIndex = new Dictionary<Rgba32, int>();

            var maxColours = depth switch {
                ColourDepth.Depth_1bpp => 2,
                ColourDepth.Depth_2bpp => 4,
                ColourDepth.Depth_4bpp => 16,
                ColourDepth.Depth_8bpp => 256,
                _ => throw new Exception("Unknown depth")
            };

            var startShift = depth switch
            {
                ColourDepth.Depth_1bpp => 7,
                ColourDepth.Depth_2bpp => 6,
                ColourDepth.Depth_4bpp => 4,
                ColourDepth.Depth_8bpp => 0,
                _ => throw new Exception("Unknown depth")
            };

            var shift = depth switch
            {
                ColourDepth.Depth_1bpp => 1,
                ColourDepth.Depth_2bpp => 2,
                ColourDepth.Depth_4bpp => 4,
                ColourDepth.Depth_8bpp => 0,
                _ => throw new Exception("Unknown depth")
            };

            sb.Append(".byte ");
            image.ProcessPixelRows(accessor =>
            {
                for (var y = 0; y < image.Height; y++)
                {
                    var rowSpan = accessor.GetRowSpan(y);

                    byte thisEntry = 0;
                    int currentShift = startShift;

                    for (var x = 0; x < image.Width; x++)
                    {
                        var pixel = rowSpan[x];

                        if (!colourIndex.ContainsKey(pixel))
                        {
                            colourIndex.Add(pixel, counter++);
                            if (colourIndex.Count > maxColours)
                                throw new Exception("Too many colours.");
                        }

                        var index = colourIndex[pixel];

                        thisEntry |= (byte)(index << currentShift);

                        if (currentShift == 0)
                        {
                            emulator.Vera.Vram[address++] = thisEntry;
                            thisEntry = 0;
                            currentShift = startShift;
                        }
                        else 
                            currentShift -= shift;
                    }
                }
            });
        }
    }
}
