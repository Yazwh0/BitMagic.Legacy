using BitMagic.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public BitImage[] Displays { get; }

        private readonly Vera _vera;
        private readonly int _scale;

        public VeraDisplay(int scale, Vera vera)
        {
            _vera = vera;
            _scale = scale;

            Displays = new BitImage[]
            {
                new BitImage(640, 480),
                new BitImage(640, 480),
                new BitImage(640, 480),
                new BitImage(640, 480),
                new BitImage(640, 480),
                new BitImage(640, 480)
            };
        }

        private const int _width = 800;
        private const int _height = 524;
        private const int _displayWidth = 640;
        private const int _displayHeight = 480;
        private const int _fps = 60;

        private int _currentX = 0;
        private int _currentY = 0;
        private int _outputPosition = 0;

//        public int GetSteps(IMachineRunner runner) => (int)((_cpuTicks + runner.TickDelta) / (runner.DeltaFrequency / _fps) * (_width * _height));


        // render a line at a time, so _displayWidth pixels.
        private void Run(IMachineRunner runner, BitImage image, int idx, Action<int, int,int, BitImage> action)
        {
            while (true)
            {
                var pos = _outputPosition;

                var myX = _currentX;
                var myY = _currentY;

                for (var i = 0; i < _displayWidth; i++)
                {
                    action(myX++, myY, pos, image);
                    pos++;
                }

                runner.DisplayEvents[idx].Set();
                runner.DisplayStart[idx].WaitOne();
            }
        }

        private void Background(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[BackgroundIdx];

            Run(runner, image, BackgroundIdx, (x, y, p, bitmap) =>
            {
                bitmap.Pixels.Span[p] = _vera.Palette.Colours[0];
            });
        }

        private void Sprite0(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[Sprite0Idx];

            Run(runner, image, Sprite0Idx , (x, y, p, bitmap) =>
            {

            });
        }

        private void Sprite1(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[Sprite1Idx];

            Run(runner, image, Sprite1Idx, (x, y, p, bitmap) =>
            {
            });

        }

        private void Sprite2(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[Sprite2Idx];

            Run(runner, image, Sprite2Idx, (x, y, p, bitmap) =>
            {
            });
        }

        private void Layer0(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[Layer0Idx];

            Run(runner, image, Layer0Idx, (x, y, p, bitmap) =>
            {
            });
        }

        private void Layer1(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[Layer1Idx];

            Run(runner, image, Layer1Idx, (x, y, p, bitmap) =>
            {
            });
        }

        // needs to calculate next cpu cycle for the display to work on.
        // and return if frame is done.
        public (bool framedone, int nextCpuTick) IncrementDisplay(IMachineRunner runner)
        {
            _currentX = 0;
            _currentY++;

            if (_currentY >= _displayHeight)
            {
                _outputPosition = 0;
                _currentY = 0;

                return (true, CpuTicks(runner)); // vblank period
            }

            _outputPosition += _displayWidth;

            return (false, CpuTicks(runner));
        }

        // number of dots from currency cpu position to _outputPosition
        public int CpuTicks(IMachineRunner runner)
        {
            if (_outputPosition == 0) // we're done, return cpu ticks that are left
            {
                return (int)(runner.CpuFrequency / _fps) - runner.CpuTicks;
            }

            var reqPct = (_currentY * _width + _currentX) / (double)(_width * _height);
            var cpuTicksFrame = (runner.CpuFrequency / _fps);

            var reqCpuTicks = (reqPct * cpuTicksFrame) - runner.CpuTicks;

            return (int)(reqCpuTicks );
        }
    }
}
