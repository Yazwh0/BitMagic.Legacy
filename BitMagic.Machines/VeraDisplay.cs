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
        //private void Run(IMachineRunner runner, BitImage image, int idx, Action<int, int,int, BitImage> action)
        //{
        //    while (true)
        //    {
        //        var pos = _outputPosition;

        //        var myX = _currentX;
        //        var myY = _currentY;

        //        for (var i = 0; i < _displayWidth; i++)
        //        {
        //            action(myX++, myY, pos, image);
        //            pos++;
        //        }

        //        runner.DisplayEvents[idx].Set();
        //        runner.DisplayStart[idx].WaitOne();
        //    }
        //}

        private void Background(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[BackgroundIdx];

            while (true)
            {
                var pos = _outputPosition;

                //var myX = EffectiveX(_currentX);
                //var myY = EffectiveY(_currentY);

                for (var i = 0; i < _displayWidth; i++)
                {
                    image.Pixels.Span[pos++] = _vera.Palette.Colours[0];
                }

                runner.DisplayEvents[BackgroundIdx].Set();
                runner.DisplayStart[BackgroundIdx].WaitOne();
            }
        }

        private void Sprite0(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[Sprite0Idx];

            while (true)
            {
                var pos = _outputPosition;

                if (_vera.SpritesEnabled)
                {
                    var myX = EffectiveX(_currentX);
                    var myY = EffectiveY(_currentY);

                    for (var i = 0; i < _displayWidth; i++)
                    {
                    }
                }

                runner.DisplayEvents[Sprite0Idx].Set();
                runner.DisplayStart[Sprite0Idx].WaitOne();
            }
        }

        private void Sprite1(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[Sprite1Idx];

            while (true)
            {
                var pos = _outputPosition;

                if (_vera.SpritesEnabled)
                {
                    var myX = EffectiveX(_currentX);
                    var myY = EffectiveY(_currentY);

                    for (var i = 0; i < _displayWidth; i++)
                    {
                    }
                }

                runner.DisplayEvents[Sprite1Idx].Set();
                runner.DisplayStart[Sprite1Idx].WaitOne();
            }

        }

        private void Sprite2(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[Sprite2Idx];

            while (true)
            {
                var pos = _outputPosition;

                if (_vera.SpritesEnabled)
                {
                    var myX = EffectiveX(_currentX);
                    var myY = EffectiveY(_currentY);

                    for (var i = 0; i < _displayWidth; i++)
                    {

                    }
                }

                runner.DisplayEvents[Sprite2Idx].Set();
                runner.DisplayStart[Sprite2Idx].WaitOne();
            }
        }

        private void Layer0(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[Layer0Idx];

            while (true)
            {
                var pos = _outputPosition;

                if (_vera.Layer0.Enabled)
                {
                    var myX = EffectiveX(_currentX);
                    var myY = EffectiveY(_currentY);

                    if (_vera.Layer0.BitmapMode)
                    {

                    }
                    else if (_vera.Layer0.ColourDepth != VeraLayer.LayerColourDepth.bpp1)
                    {
                        // todo: make these effective y and x
                        LayerTiles(_vera, _vera.Layer0, image, pos, myX, myY);
                    }
                    else
                    {
                    }
                }

                runner.DisplayEvents[Layer0Idx].Set();
                runner.DisplayStart[Layer0Idx].WaitOne();
            }
        }

        public int EffectiveX(int displayX) => _vera.HScale * (displayX - _vera.HStart) >> 7;
        public int EffectiveY(int displayY) => _vera.VScale * (displayY - _vera.VStart) >> 7;

        private void Layer1(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[Layer1Idx];

            while (true)
            {
                var pos = _outputPosition;

                if (_vera.Layer1.Enabled)
                {
                    var myX = EffectiveX(_currentX);
                    var myY = EffectiveY(_currentY);

                    if (_vera.Layer1.BitmapMode)
                    {

                    } else if (_vera.Layer1.ColourDepth != VeraLayer.LayerColourDepth.bpp1)
                    {
                        // todo: make these effective y and x
                        LayerTiles(_vera, _vera.Layer1, image, pos, myX, myY);
                    }
                    else
                    {
                    }
                } 

                runner.DisplayEvents[Layer1Idx].Set();
                runner.DisplayStart[Layer1Idx].WaitOne();
            }
        }

        // x and y are effective, ie after scaling.
        private static void LayerTiles(Vera vera, VeraLayer layer, BitImage image, int pos, int x, int y)
        {
            var mapAddressLine = layer.MapBase + (((y >> layer.TileHeightShift) * layer.MapWidth) + layer.VScroll) * 2;

            int tileSizeBytes = (layer.TileHeight * layer.TileWidth) >> layer.ColourDepthShift;

            var tileLineStart = 0;
            var newTileCnt = 0;
            var newValueCnt = 0;
            int tileValue = 0;
            int mask = 0;
            int addrStep = 0;
            int tilePos = 0;
            int tileAddress = 0;
            int paletteOffset = 0;

            for (var i = x; i < _displayWidth; i++) // todo: consider hstop, but that should chagne _display width?
            {
                int tileIndex;
                int mapAddress;
                int tileData;
                if (newTileCnt == 0)
                {
                    // todo: adjust i to display
                    mapAddress = mapAddressLine + (((i + layer.HScroll) >> layer.ColourDepthShift) * 2);
                    tileData = vera.Vram.GetByte(mapAddress + 1);

                    var hFlip = (tileData & 0b100) != 0;
                    var vFlip = (tileData & 0b1000) != 0;

                    tileIndex = vera.Vram.GetByte(mapAddress);
                    tileIndex += (tileData & 0b11) << 8;

                    paletteOffset = (tileData & 0xf0) >> 4;
                    tileAddress = layer.TileBase + tileIndex * tileSizeBytes;

                    tileLineStart = y % layer.TileHeight;

                    if (hFlip)
                    {
                        tileLineStart = layer.TileHeight - tileLineStart;
                    }

                    if (vFlip)
                    {
                        addrStep = -1;
                        tileLineStart = tileLineStart + (layer.TileWidth >> layer.ColourDepthShift);
                    }
                    else
                    {
                        addrStep = 1;
                    }
                    newTileCnt = layer.TileWidth;
                    tilePos = 0;
                }

                if (newValueCnt == 0)
                {
                    tileValue = vera.Vram.GetByte(((tileLineStart * (layer.TileWidth >> layer.ColourDepthShift)) + tileAddress + tilePos++) % 0x1ffff);
                    newValueCnt = layer.ColourDepthShift +1;
                    mask = (layer.ColourDepth, addrStep) switch
                    {
                        (VeraLayer.LayerColourDepth.bpp8, _)  => 0b1111_1111,
                        (VeraLayer.LayerColourDepth.bpp4, 1)  => 0b1111_0000,
                        (VeraLayer.LayerColourDepth.bpp2, 1)  => 0b1100_0000,
                        (VeraLayer.LayerColourDepth.bpp1, 1)  => 0b1000_0000,
                        (VeraLayer.LayerColourDepth.bpp4, -1) => 0b0000_1111,
                        (VeraLayer.LayerColourDepth.bpp2, -1) => 0b0000_0011,
                        (VeraLayer.LayerColourDepth.bpp1, -1) => 0b0000_0001,
                        _ => throw new Exception($"unhandled depth\\direction {layer.ColourDepth} {addrStep}.")
                    };
                }

                int actValue = (tileValue & mask) >> mask switch { 
                    0b1111_1111 => 0,
                    0b0000_1111 => 0,
                    0b1111_0000 => 4,
                    0b0000_0011 => 0,
                    0b0000_1100 => 2,                    
                    0b0011_0000 => 4,
                    0b1100_0000 => 6,
                    0b0000_0001 => 0,
                    0b0000_0010 => 1,
                    0b0000_0100 => 2,
                    0b0000_1000 => 3,
                    0b0001_0000 => 4,
                    0b0010_0000 => 5,
                    0b0100_0000 => 6,
                    0b1000_0000 => 7,
                    _ => throw new Exception($"Unknown mask")
                };

                mask = (layer.ColourDepth, addrStep) switch
                {
                    (VeraLayer.LayerColourDepth.bpp8, _) => mask,
                    (VeraLayer.LayerColourDepth.bpp4, 1) => mask >> 4,
                    (VeraLayer.LayerColourDepth.bpp2, 1) => mask >> 2,
                    (VeraLayer.LayerColourDepth.bpp1, 1) => mask >> 1,
                    (VeraLayer.LayerColourDepth.bpp4, -1) => mask << 4,
                    (VeraLayer.LayerColourDepth.bpp2, -1) => mask << 2,
                    (VeraLayer.LayerColourDepth.bpp1, -1) => mask << 1,
                    _ => throw new Exception($"unhandled depth\\direction {layer.ColourDepth} {addrStep}.")
                };

                if (actValue == 0)
                {
                    image.Pixels.Span[pos] = new PixelRgba(0, 0, 0, 0);
                }
                else 
                {
                    image.Pixels.Span[pos] = vera.Palette.Colours[actValue + paletteOffset];
                }

                newValueCnt--;
                newTileCnt--;
                pos++;
            }
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
                // set vsync
                if ((_vera.ISR & _vera.ISR_Vsync) == 0 && _vera.VsyncInterupt) 
                {
                    _vera.ISR |= _vera.ISR_Vsync;
                    runner.Cpu.SetInterrupt();
                }

                return (int)(runner.CpuFrequency / _fps) - runner.CpuTicks;
            }

            if (_vera.IrqLine == _currentY-1 && (_vera.ISR & _vera.ISR_Line) == 0 && _vera.LineInterupt)
            {
                _vera.ISR |= _vera.ISR_Line;
                runner.Cpu.SetInterrupt();
               // Console.WriteLine($"Interrupt on {_currentY}");
            }

            var reqPct = (_currentY * _width + _currentX) / (double)(_width * _height);
            var cpuTicksFrame = (runner.CpuFrequency / _fps);

            var reqCpuTicks = (reqPct * cpuTicksFrame) - runner.CpuTicks;

            return (int)(reqCpuTicks );
        }
    }
}
