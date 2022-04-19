using System;

namespace ImageProcessor;

public class Tile
{
    public byte[,] Pixels { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Depth Depth {get;set;}

    public Tile(int width, int height, Depth depth)
    {
        Width = width;
        Height = height;
        Pixels = new byte[width, height];
        Depth = depth;
    }

    public IEnumerable<byte> Data()
    {
        var stepX = Depth switch {
                Depth.Bpp_1 => 8, 
                Depth.Bpp_2 => 4, 
                Depth.Bpp_4 => 2, 
                Depth.Bpp_8 => 1,
                _ => throw new Exception() 
            };

        for(var y = 0; y < Height; y++)
        {
            for(var x = 0; x < Width; x += stepX)
            {
                switch(Depth)
                {
                    case Depth.Bpp_1:
                        yield return (byte)(
                            ((Pixels[x + 0, y] & 0x01) << 7) +
                            ((Pixels[x + 1, y] & 0x01) << 6) +
                            ((Pixels[x + 2, y] & 0x01) << 5) +
                            ((Pixels[x + 3, y] & 0x01) << 4) +
                            ((Pixels[x + 4, y] & 0x01) << 3) +
                            ((Pixels[x + 5, y] & 0x01) << 2) +
                            ((Pixels[x + 6, y] & 0x01) << 1) +
                            (Pixels[x + 7, y] & 0x01));
                        break;
                    case Depth.Bpp_2:
                        yield return (byte)(
                            ((Pixels[x + 0, y] & 0x03) << 6) +
                            ((Pixels[x + 1, y] & 0x03) << 4) +
                            ((Pixels[x + 2, y] & 0x03) << 2) +
                            (Pixels[x + 3, y] & 0x03));
                        break;
                    case Depth.Bpp_4:
                        yield return (byte)(
                            ((Pixels[x + 0, y] & 0x0f) << 4) +
                            (Pixels[x + 1, y] & 0x0f));
                            break;
                    case Depth.Bpp_8:
                        yield return Pixels[x, y];
                        break;
                }
            }
        }
    }
    
    public Tile FlipX()
    {
        var toReturn = new Tile(Width, Height, Depth);

        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                toReturn.Pixels[x, y] = Pixels[Width - x - 1, y];
            }
        }

        return toReturn;
    }

    public Tile FlipY()
    {
        var toReturn = new Tile(Width, Height, Depth);

        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                toReturn.Pixels[x, y] = Pixels[x, Height - y - 1];
            }
        }

        return toReturn;
    }
}
