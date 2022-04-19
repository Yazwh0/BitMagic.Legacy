using System.Diagnostics.CodeAnalysis;

namespace ImageProcessor;

public class TileComparer : IEqualityComparer<Tile>
{
    public bool Equals(Tile? x, Tile? y)
    {
        if (x == null || y == null)
            return false;

        if (x.Width != y.Width)
            return false;

        if (x.Height!= y.Height)
            return false;

        for(var i = 0; i < x.Width; i++)
        {
            for(var j = 0; j < y.Height; j++)
            {
                if (x.Pixels[i, j] != y.Pixels[i, j])
                    return false;
            }
        }

        return true;
    }

    public int GetHashCode([DisallowNull] Tile obj)
    {
        throw new NotImplementedException();
    }
}