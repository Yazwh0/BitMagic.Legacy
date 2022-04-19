using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessor;

public static class Processor 
{
    public static X16Image LoadFullImage(string filename)
    {
        var toReturn = LoadImage(filename);

        if (toReturn.Width != 320)
        {
            throw new Exception($"Image must be 320px wide. Image is {toReturn.Width} x {toReturn.Height}");
        }

        if (toReturn.Height != 240)
        {
            throw new Exception($"Image must be 240px high. Image is {toReturn.Width} x {toReturn.Height}");
        }

        return toReturn;
    }

    public static X16Image LoadImage(string filename)
    {
        using (var image = Image.Load<Rgba32>(filename))
        {
            var toReturn = new X16Image();

            toReturn.Height = image.Height;
            toReturn.Width = image.Width;

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

                    toReturn.Pixels[idx++] = palette[thisColour];
                }
            }

            toReturn.Colours = new Colour[palette.Count];
            var index = 0;
            foreach(var kv in palette.OrderBy(kv => kv.Value))
            {
                toReturn.Colours[index].R = kv.Key.R;
                toReturn.Colours[index].G = kv.Key.G;
                toReturn.Colours[index].B = kv.Key.B;
                index++;
            }

            return toReturn;
        }
    }

    public static TileData CreateTileMap(X16Image image, Depth depth, TileSize width, TileSize height, bool checkFlips = true, bool includeBlank = false, TileExcessHandling excessHandling = TileExcessHandling.Error, IEnumerable<Tile>? existing = null)
    {
        var toReturn = new TileData();

        // cant flip 1bpp tiles.
        if (depth == Depth.Bpp_1)
        {
            checkFlips = false;
            Console.WriteLine("Warning: 1bpp tiles cannot be flipped.");
        }

        var maxColours = depth switch {
            Depth.Bpp_1 => 2,
            Depth.Bpp_2 => 4,
            Depth.Bpp_4 => 16,
            Depth.Bpp_8 => 256,
            _ => throw new ArgumentException($"Unknown depth {depth}")
        };

        if (image.Colours.Length > maxColours)
            throw new Exception($"There are too many colours in the source image for that depth. Image Colours: {image.Colours.Length}");

        var stepX = width switch {
            TileSize.Size_8 => 8,
            TileSize.Size_16 => 16,
            _ => throw new ArgumentException($"Unknown width {width}")
        };

        var stepY = height switch {
            TileSize.Size_8 => 8,
            TileSize.Size_16 => 16,
            _ => throw new ArgumentException($"Unknown height {width}")
        };

        if (excessHandling == TileExcessHandling.Error)
        {
            var ratio = (image.Width / (stepX * 1.0));
            if (ratio - Math.Truncate(ratio) != 0)
                throw new Exception($"image width is not divisible by {stepX}. Width: {image.Width}");

            ratio = (image.Height / (stepY * 1.0));
            if (ratio - Math.Truncate(ratio) != 0)
                throw new Exception($"image height is not divisible by {stepY}. Width: {image.Height}");
        }

        //var done = false;
        var curX = 0;
        var curY = 0;
        var comparer = new TileComparer();

        if (includeBlank)
            toReturn.Tiles.Add(new Tile(stepX, stepY, depth));

        if (existing != null)
        {
            toReturn.Tiles.AddRange(existing);
        }

        while (true)
        {
            if (excessHandling == TileExcessHandling.Ignore)
            {
                if (curX + stepX > image.Width)
                {
                    curX = 0;
                    curY += stepY;
                    continue;
                }

                if (curY + stepY > image.Height)
                {
                    break;
                }
            }

            var tile = new Tile(stepX, stepY, depth);
            for(var x = 0; x < stepX; x++)
            {
                for(var y = 0; y < stepY; y++)
                {
                    if (curX + x < image.Width && curY + y < image.Height)
                        tile.Pixels[x, y] = image.Pixels[(curY + y) * image.Width + curX + x];
                }
            }

            var index = toReturn.Tiles.FindIndex(t => comparer.Equals(t, tile));
            if (index == -1)
            {
                byte flipData = 0;
                if (checkFlips)
                {
                    var flip = tile.FlipX();

                    index = toReturn.Tiles.FindIndex(t => comparer.Equals(t, flip));

                    if (index == -1)
                    {
                        flip = tile.FlipY();

                        index = toReturn.Tiles.FindIndex(t => comparer.Equals(t, flip));
            
                        if (index == -1)
                        {
                            flip = flip.FlipX();

                            index = toReturn.Tiles.FindIndex(t => comparer.Equals(t, flip));

                            if (index != -1)
                            {
                                flipData = 0xc; // 1100
                            }
                        }
                        else
                        {
                            flipData = 0x4; // 100
                        }
                    }
                    else
                    {
                        flipData = 0x8; // 1000
                    }
                }

                if (index == -1)
                {
                    toReturn.Map.Add(new TileIndex((byte)toReturn.Tiles.Count, flipData));
                    toReturn.Tiles.Add(tile);
                }
                else
                {
                    toReturn.Map.Add(new TileIndex((byte)index, flipData));
                }
            }
            else
            {
                toReturn.Map.Add(new TileIndex((byte)index, (byte)0));
            }

            curX += stepX;

            if (excessHandling == TileExcessHandling.Include)
            {
                if (curX >= image.Width)
                {
                    curX = 0;
                    curY += stepY;
                    continue;
                }

                if (curY >= image.Height)
                {
                    break;
                }      
            }

            if (excessHandling == TileExcessHandling.Error)
            {
                if (curX == image.Width)
                {
                    curX = 0;
                    curY += stepY;
                }

                if (curY == image.Height)
                {
                    break;
                }
            }
        }

        toReturn.Colours = image.Colours.ToArray();
        toReturn.MapWidth = (int)Math.Ceiling(image.Width / stepX * 1.0);
        toReturn.MapHeight = (int)Math.Ceiling(image.Height / stepY * 1.0);

        return toReturn;
    }
}
