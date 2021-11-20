using BitMagic.Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Machines
{
    internal class VeraDisplay : IDisplay
    {
        // order is important
        private const int BackgroundIdx = 0;
        private const int Sprite0Idx = 1;
        private const int Layer0Idx = 2;
        private const int Sprite1Idx = 3;
        private const int Layer1Idx = 4;
        private const int Sprite2Idx = 5;

        public Action<object?>[] DisplayThreads => new Action<object?>[] { Background, Sprite0, Sprite1, Sprite2, Layer0, Layer1 };

        public Image<Rgba32>[] Displays { get; }

        public VeraDisplay(int scale)
        {
            Displays = new Image<Rgba32>[]
            {
                new Image<Rgba32>(640 * scale, 320 * scale),
                new Image<Rgba32>(640 * scale, 320 * scale),
                new Image<Rgba32>(640 * scale, 320 * scale),
                new Image<Rgba32>(640 * scale, 320 * scale),
                new Image<Rgba32>(640 * scale, 320 * scale),
                new Image<Rgba32>(640 * scale, 320 * scale)
            };
        }

        private async void Background(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[BackgroundIdx];

            while (true)
            {
                await runner.SignalAndWait();
            }
        }

        private async void Sprite0(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[Sprite0Idx];

            while (true)
            {
                await runner.SignalAndWait();
            }
        }

        private async void Sprite1(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[Sprite1Idx];

            while (true)
            {
                await runner.SignalAndWait();
            }
        }

        private async void Sprite2(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[Sprite2Idx];

            while (true)
            {
                await runner.SignalAndWait();
            }
        }

        private async void Layer0(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[Layer0Idx];

            while (true)
            {
                await runner.SignalAndWait();
            }
        }

        private async void Layer1(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[Layer1Idx];

            while (true)
            {
                await runner.SignalAndWait();
            }
        }
    }
}
