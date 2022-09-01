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
        public bool[] DisplayHold { get; }
        public BitImage[] Displays { get; }
        public (bool framedone, int nextCpuTick, bool releaseVideo) IncrementDisplay(IMachineRunner runner);
        void PreRender();
    }

    public class BitImage
    {
        public Memory<PixelRgba> DrawPixels { get; internal set; }
        public Memory<PixelRgba> RenderPixels { get; internal set; }
        
        private PixelRgba[] _pixelsA;
        private PixelRgba[] _pixelsB;

        private bool _state = false;

        public int Width { get; }
        public int Height { get; }

        public BitImage(int width, int height)
        {
            _pixelsA = new PixelRgba[height * width];
            _pixelsB = new PixelRgba[height * width];
            DrawPixels = _pixelsA;
            RenderPixels = _pixelsB;
            Width = width;
            Height = height;
        }

        public void Switch()
        {
            _state = !_state;

            if (_state)
            {
                DrawPixels = _pixelsA;
                RenderPixels = _pixelsB;
            } 
            else
            {
                DrawPixels = _pixelsB;
                RenderPixels = _pixelsA;
            }
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

        public override string ToString() => $"R:${R:X2} G:${G:X2} B:${B:X2} A:${A:X2}";
    }
}
