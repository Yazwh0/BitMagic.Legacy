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

            if (image.Height != 480)
                Assert.Fail("Source image must be 480px high.");

            if (image.Width != 640)
                Assert.Fail("Source image must be 640px wide.");

            var counter = 1;
            var colourIndex = new Dictionary<Rgba32, int>();
            colourIndex.Add(new Rgba32(0, 0, 0, 255), 0);

            var maxColours = depth switch {
                ColourDepth.Depth_1bpp => 2,
                ColourDepth.Depth_2bpp => 4,
                ColourDepth.Depth_4bpp => 16,
                ColourDepth.Depth_8bpp => 256,
                _ => throw new Exception("Unknown depth")
            };

            var maxSubCount = depth switch
            {
                ColourDepth.Depth_1bpp => 8,
                ColourDepth.Depth_2bpp => 4,
                ColourDepth.Depth_4bpp => 2,
                ColourDepth.Depth_8bpp => 1,
                _ => throw new Exception("Unknown depth")
            };

            var dataCount = 0;
            sb.Append(".byte ");
            image.ProcessPixelRows(accessor =>
            {
                for (var y = 0; y < 480; y++)
                {
                    var rowSpan = accessor.GetRowSpan(y);

                    byte thisEntry = 0;
                    int subByteCount = maxSubCount;

                    for (var x = 0; x < 640; x++)
                    {
                        var pixel = rowSpan[x];

                        if (!colourIndex.ContainsKey(pixel))
                        {
                            colourIndex.Add(pixel, counter++);
                            if (colourIndex.Count > maxColours)
                                throw new Exception("Too many colours.");
                        }

                        var index = colourIndex[pixel];

                        subByteCount--;

                        thisEntry |= (byte)(index << subByteCount);

                        if (subByteCount == 0)
                        {
                            emulator.Vera.Vram[address++] = thisEntry;
                            thisEntry = 0;
                            subByteCount = maxSubCount;
                        }
                    }
                }
            });
        }
    }
}
