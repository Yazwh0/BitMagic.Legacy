using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Common
{
    public interface IDisplay
    {
        public Action<object?>[] DisplayThreads { get; }
        public Image<Rgba32>[] Displays { get; }
    }
}
