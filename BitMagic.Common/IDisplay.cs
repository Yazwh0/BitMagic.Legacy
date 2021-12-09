using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Common
{
    public interface IDisplay
    {
        public Action<object?>[] DisplayThreads { get; }
        public BitImage[] Displays { get; }
        public (bool framedone, int nextCpuTick, bool releaseVideo) IncrementDisplay(IMachineRunner runner);        
    }

    public class BitImage
    {
        public Memory<PixelRgba> Pixels { get; }
        private PixelRgba[] _pixels;

        public int Width { get; }
        public int Height { get; }

        public BitImage(int width, int height)
        {
            _pixels = new PixelRgba[height * width];
            Pixels = _pixels;
            Width = width;
            Height = height;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PixelRgba
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public PixelRgba()
        {
            R = 0;
            G = 0;
            B = 0;
            A = 0;
        }

        public PixelRgba(byte r, byte g, byte b, byte a = 0xff)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

    }
}
