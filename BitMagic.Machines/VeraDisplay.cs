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

        public int EffectiveX(int displayX) => _vera.HScale * (displayX - _vera.HStart) >> 7;
        public int EffectiveY(int displayY) => _vera.VScale * (displayY - _vera.VStart) >> 7;

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

        const int _maxPixelsPerByte = 8;

        private void Layer0(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[Layer0Idx];

            int[] _buffer = new int[_maxPixelsPerByte];

            while (true)
            {
                var pos = _outputPosition;

                if (_vera.Layer0.Enabled)
                {
                    var myX = EffectiveX(_currentX);
                    var myY = EffectiveY(_currentY) + _vera.Layer0.VScroll;

                    if (_vera.Layer0.BitmapMode)
                    {

                    }
                    else if (_vera.Layer0.ColourDepth != VeraLayer.LayerColourDepth.bpp1)
                    {
                        myX += _vera.Layer0.HScroll;
                        myY += _vera.Layer0.VScroll;

                        LayerTiles(_vera, _vera.Layer0, image, pos, myX, myY, _buffer);
                    }
                    else
                    {
                    }
                }

                runner.DisplayEvents[Layer0Idx].Set();
                runner.DisplayStart[Layer0Idx].WaitOne();
            }
        }


        private void Layer1(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[Layer1Idx];

            int[] _buffer = new int[_maxPixelsPerByte];

            while (true)
            {
                var pos = _outputPosition;

                if (_vera.Layer1.Enabled)
                {
                    var myX = EffectiveX(_currentX);
                    var myY = EffectiveY(_currentY) + _vera.Layer1.VScroll;

                    if (_vera.Layer1.BitmapMode)
                    {

                    } else if (_vera.Layer1.ColourDepth != VeraLayer.LayerColourDepth.bpp1)
                    {
                        myX += _vera.Layer1.HScroll;
                        myY += _vera.Layer1.VScroll;

                        LayerTiles(_vera, _vera.Layer1, image, pos, myX, myY, _buffer);
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
        private void LayerTiles(Vera vera, VeraLayer layer, BitImage image, int pos, int x, int y, int[] buffer)
        {
            var mapAddressLine = layer.MapBase + (y >> layer.TileHeightShift) * layer.MapWidth * 2;
            int mapAddress;

            int tileSizeBytes = (layer.TileHeight * layer.TileWidth) >> layer.ColourDepthShift;

            var tileLine = 0;
            var newTileCnt = 0;
            var newValueCnt = 0;
            int addrStep = 0;
            var tileAddress = 0;
            int tilePosX = 0;
            int paletteOffset = 0;
            int lastX = -1;
            PixelRgba lastPixel = new PixelRgba(0, 0, 0, 0);
            bool first = true;

            var (pxPerByte, initMask, shift) = layer.ColourDepth switch
            {
                VeraLayer.LayerColourDepth.bpp1 => (8, 0b0000_0001, 1),
                VeraLayer.LayerColourDepth.bpp2 => (4, 0b0000_0011, 2),
                VeraLayer.LayerColourDepth.bpp4 => (2, 0b0000_1111, 4),
                VeraLayer.LayerColourDepth.bpp8 => (1, 0b1111_1111, 0),
                _ => throw new Exception()
            };

            // x is the offset at this point
            mapAddress = mapAddressLine + ((x >> layer.TileWidthShift >> layer.ColourDepthShift) * 2);

            for (var i = x; i < _displayWidth + x; i++) // todo: consider hstop, but that should chagne _display width?
            {
                var thisX = EffectiveX(i) + layer.HScroll;
                if (thisX == lastX)
                {
                    image.Pixels.Span[pos] = lastPixel;

                    pos++;

                    continue;
                }
                lastX = thisX;

                int tileIndex;
                int tileData;
                if (newTileCnt == 0)
                {
                    tileData = vera.Vram.GetByte(mapAddress + 1);

                    var hFlip = (tileData & 0b100) != 0;
                    var vFlip = (tileData & 0b1000) != 0;

                    tileIndex = vera.Vram.GetByte(mapAddress);
                    tileIndex += (tileData & 0b11) << 8;

                    paletteOffset = ((tileData & 0xf0) >> 4) * 16;

                    tileLine = y % layer.TileHeight;

                    if (hFlip)
                    {
                        tileLine = layer.TileHeight - tileLine;
                    }

                    if (vFlip)
                    {
                        addrStep = -1;
                        tileLine = tileLine + (layer.TileWidth >> layer.ColourDepthShift);
                    }
                    else
                    {
                        addrStep = 1;
                    }

                    tileAddress = (tileLine * (layer.TileWidth >> layer.ColourDepthShift)) + layer.TileBase + tileIndex * tileSizeBytes;

                    // first pixel on the line will need to be stepped into the tile
                    if (first)
                    {
                        tilePosX = (i % layer.TileWidth) >> layer.ColourDepthShift;
                        newTileCnt = layer.TileWidth - (i % (layer.TileWidth));
                    }
                    else 
                    {
                        newTileCnt = layer.TileWidth;
                        tilePosX = 0;
                    }

                    mapAddress += 2;
                }

                if (newValueCnt == 0)
                {
                    if (first)
                    {
                        first = false;
                        var ofset = newTileCnt % pxPerByte;
                        newValueCnt = pxPerByte - ofset;
                    }
                    else
                    {
                        newValueCnt = pxPerByte;
                    }

                    // fill buffer with pixel values for the byte
                    int mask = initMask;
                    var tileValue = vera.Vram.GetByte((tileAddress + tilePosX++) % 0x1ffff);

                    for (int px = 0; px < pxPerByte; px++)
                    {
                        buffer[px] = (tileValue & mask);
                        tileValue = (byte)(tileValue >> shift);
                    }

                }

                var actValue = buffer[newValueCnt - 1];
                if (actValue == 0)
                {
                    lastPixel = new PixelRgba(0, 0, 0, 0);
                }
                else
                {
                    lastPixel = vera.Palette.Colours[actValue + paletteOffset];
                }
                image.Pixels.Span[pos] = lastPixel;

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
