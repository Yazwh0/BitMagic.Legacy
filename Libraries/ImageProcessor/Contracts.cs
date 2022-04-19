using System.IO;

namespace ImageProcessor;

public struct Colour
{
    public byte R {get;set;} = 0;
    public byte G {get;set;}= 0;
    public byte B {get;set;}= 0;

    public Colour()
    {        
    }

    public Colour(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }

    public IEnumerable<byte> VeraColour => new byte[] { (byte)((G & 0xf0) + ((B & 0xf0) >> 4)), (byte)((R & 0xf0) >> 4) };
}

public class RawImage
{
    public byte[] Pixels { get; set; } = Array.Empty<byte>();
    public byte[] VeraColours { get; set; } = Array.Empty<byte>();
    public int Width {get;set;}
    public int Height {get;set;}
}

public class X16Image
{
    public byte[] Pixels { get; set; } = Array.Empty<byte>();
    public Colour[] Colours { get; set; } = Array.Empty<Colour>();
    public byte[] VeraColours => Colours.SelectMany(i => i.VeraColour).ToArray();
    public int Width {get;set;}
    public int Height {get;set;}
}

public class TileData
{
    public List<Tile> Tiles {get;set;} = new List<Tile>();
    public List<TileIndex> Map {get;set;} = new List<TileIndex>();
    public int MapWidth {get;set;}
    public int MapHeight {get;set;}
    public Colour[] Colours {get;set;} = Array.Empty<Colour>();
}

public struct TileIndex
{
    public byte Index { get; set; } = 0;
    public byte FlipData { get; set; } = 0;

    public TileIndex()
    {
    }

    public TileIndex(byte index, byte flipData)
    {
        Index = index;
        FlipData = flipData;
    }
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

public enum TileMapSize
{
    Map_32,
    Map_64,
    Map_128,
    Map_256
}

public enum TileSize
{
    Size_8,
    Size_16
}

public enum TileExcessHandling 
{
    
    Error,
    Ignore,
    Include
}
