using BitMagic.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private const int Layer0ShadowIdx = 2;
        private const int Sprite1Idx = 3;
        private const int Layer1ShadowIdx = 4;
        private const int Sprite2Idx = 5;

        private const int BackgroundThreadIdx = 0;
        private const int LayersThreadIdx = 1;

        public Action<object?>[] DisplayThreads => new Action<object?>[] { BackgroundAndSprites, Layers };
        public bool[] DisplayHold { get; } = new bool[2];

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
        private const int _porch = 33;
        private const int _displayWidth = 640;
        private const int _displayHeight = 480;
        private const int _fps = 60;

        private int _currentX = 0;
        private int _currentY = -33;
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

        private void BackgroundAndSprites(object? r)
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
                    image.DrawPixels.Span[pos++] = _vera.Palette.Colours[0];
                }

                DisplayHold[BackgroundThreadIdx] = true;
                while (DisplayHold[BackgroundThreadIdx]) { }

                //runner.DisplayEvents[BackgroundThreadIdx].Set();
                //runner.DisplayStart[BackgroundThreadIdx].WaitOne();
            }
        }

       /* private void Sprite0(object? r)
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

                DisplayHold[Sprite0Idx] = true;
                while (DisplayHold[Sprite0Idx]) { }

                //runner.DisplayEvents[Sprite0Idx].Set();
                //runner.DisplayStart[Sprite0Idx].WaitOne();
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

                DisplayHold[Sprite1Idx] = true;
                while (DisplayHold[Sprite1Idx]) { }

                //runner.DisplayEvents[Sprite1Idx].Set();
                //runner.DisplayStart[Sprite1Idx].WaitOne();
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

                DisplayHold[Sprite2Idx] = true;
                while (DisplayHold[Sprite2Idx]) { }

                //runner.DisplayEvents[Sprite2Idx].Set();
                //runner.DisplayStart[Sprite2Idx].WaitOne();
            }
        }*/

        const int _maxPixelsPerByte = 8;

       /* private void Layer0Shadow(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            var image = Displays[Layer0ShadowIdx];

            int[] _buffer = new int[_maxPixelsPerByte];

            while (true)
            {
                var pos = _outputPosition;

                if (_vera.Layer0Shadow.Enabled)
                {
                    var myX = EffectiveX(_currentX);
                    var myY = EffectiveY(_currentY);

                    if (_vera.Layer0Shadow.BitmapMode)
                    {

                    }
                    else if (_vera.Layer0Shadow.ColourDepth != VeraLayer.LayerColourDepth.bpp1)
                    {
                        myX += _vera.Layer0Shadow.HScroll;
                        myY += _vera.Layer0Shadow.VScroll;

                        LayerTiles(_vera, ref _vera.Layer0Shadow, image, pos, myX, myY, _buffer);
                    }
                    else
                    {
                    }
                }

                DisplayHold[Layer0ShadowIdx] = true;
                while (DisplayHold[Layer0ShadowIdx]) { }

                //runner.DisplayEvents[Layer0ShadowIdx].Set();
                //runner.DisplayStart[Layer0ShadowIdx].WaitOne();
            }
        }*/

        private void Layers(object? r)
        {
            var runner = r as IMachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            //var image = Displays[Layer1ShadowIdx];

            int[] _buffer = new int[_maxPixelsPerByte];

            while (true)
            {

                if (_vera.Layer1Shadow.Enabled)
                {
                    var pos = _outputPosition;

                    var myX = EffectiveX(_currentX);
                    var myY = EffectiveY(_currentY);

                    if (_vera.Layer1Shadow.BitmapMode)
                    {

                    } 
                    else if (_vera.Layer1Shadow.ColourDepth != VeraLayer.LayerColourDepth.bpp1)
                    {
                        myX += _vera.Layer1Shadow.HScroll;
                        myY += _vera.Layer1Shadow.VScroll;

                        LayerTiles(_vera, ref _vera.Layer1Shadow, Displays[Layer1ShadowIdx], pos, myX, myY, _buffer);
                    }
                    else
                    {
                    }
                }

                if (_vera.Layer0Shadow.Enabled)
                {
                    var pos = _outputPosition;

                    var myX = EffectiveX(_currentX);
                    var myY = EffectiveY(_currentY);

                    if (_vera.Layer0Shadow.BitmapMode)
                    {

                    }
                    else if (_vera.Layer0Shadow.ColourDepth != VeraLayer.LayerColourDepth.bpp1)
                    {
                        myX += _vera.Layer0Shadow.HScroll;
                        myY += _vera.Layer0Shadow.VScroll;

                        LayerTiles(_vera, ref _vera.Layer0Shadow, Displays[Layer0ShadowIdx], pos, myX, myY, _buffer);
                    }
                    else
                    {
                    }
                }


                DisplayHold[LayersThreadIdx] = true;
                while (DisplayHold[LayersThreadIdx]) { }

                //runner.DisplayEvents[LayersThreadIdx].Set();
                //runner.DisplayStart[LayersThreadIdx].WaitOne();
            }
        }

        // x and y are effective, ie after scaling.
        private void LayerTiles(Vera vera, ref VeraLayer layer, BitImage image, int pos, int startX, int y, int[] buffer)
        {
            var mapAddressLine = layer.MapBase + (y >> layer.TileHeightShift) * layer.MapWidth * 2;

            int tileSizeBytes = (layer.TileHeight * layer.TileWidth) >> layer.ColourDepthShift;

            var tileLine = 0;
            var pixelsUntilNextTile = 0;
            var pixelsUtilNextTileByte = 0;
            int addrStep = 0;
            var tileAddress = 0;
            int tileBytePosition = 0;
            int paletteOffset = 0;
            int lastX = -1;
            PixelRgba lastPixel = new PixelRgba(0, 0, 0, 0);
            bool first = true;

            //if (y == 0)
                //Debug.Assert(false);

            var (pixelsPerByte, initMask, shift) = layer.ColourDepth switch
            {
                VeraLayer.LayerColourDepth.bpp1 => (8, 0b0000_0001, 1),
                VeraLayer.LayerColourDepth.bpp2 => (4, 0b0000_0011, 2),
                VeraLayer.LayerColourDepth.bpp4 => (2, 0b0000_1111, 4),
                VeraLayer.LayerColourDepth.bpp8 => (1, 0b1111_1111, 0),
                _ => throw new Exception()
            };

            // x is the offset at this point
            int mapAddress = mapAddressLine + ((startX >> layer.TileWidthShift) * 2);

            for (var i = startX; i < _displayWidth+ startX; i++) // todo: consider hstop, but that should chagne _display width?
            {
                var thisX = EffectiveX(i);
                if (thisX == lastX)
                {
                    image.DrawPixels.Span[pos] = lastPixel;

                    pos++;

                    continue;
                }
                lastX = thisX;

                int tileIndex;
                int tileData;
                if (pixelsUntilNextTile == 0)
                {
                    tileData = vera.VramShadow.GetByte(mapAddress + 1);

                    var hFlip = (tileData & 0b100) != 0;
                    var vFlip = (tileData & 0b1000) != 0;

                    tileIndex = vera.VramShadow.GetByte(mapAddress);
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
                        var startPos = i % layer.TileWidth;
                        tileBytePosition = startPos >> shift;   // how many bytes into the tile
                        pixelsUntilNextTile = layer.TileWidth - startPos;        // how many pixels left until the next tile.
                    }
                    else 
                    {
                        pixelsUntilNextTile = layer.TileWidth;
                        tileBytePosition = 0;
                    }

                    mapAddress += 2;
                }

                if (pixelsUtilNextTileByte == 0)
                {
                    if (first)
                    {
                        pixelsUtilNextTileByte = pixelsPerByte - ((i % layer.TileWidth) % pixelsPerByte);
                        first = false;
                    } 
                    else
                    {
                        pixelsUtilNextTileByte = pixelsPerByte;
                    }

                    // fill buffer with pixel values for the byte, buffer is reversed.
                    int mask = initMask;
                    var tileValue = vera.VramShadow.GetByte((tileAddress + tileBytePosition++) % 0x1ffff);

                    for (int px = 0; px < pixelsPerByte; px++)
                    {
                        buffer[px] = (tileValue & mask);
                        tileValue = (byte)(tileValue >> shift);
                    }
                }

                var actValue = buffer[--pixelsUtilNextTileByte];
                if (actValue == 0)
                {
                    lastPixel = new PixelRgba(0, 0, 0, 0);
                }
                else
                {
                    lastPixel = vera.Palette.Colours[actValue + paletteOffset];
                }
                image.DrawPixels.Span[pos] = lastPixel;

                pixelsUntilNextTile--;
                pos++;
            }
        }


        // needs to calculate next cpu cycle for the display to work on.
        // and return if frame is done.
        public (bool framedone, int nextCpuTick, bool releaseVideo) IncrementDisplay(IMachineRunner runner)
        {
            if (_currentY == _displayHeight + 1) // vsync
            {
                // set vsync
                if ((_vera.ISR & _vera.ISR_Vsync) == 0 && _vera.VsyncInterupt)
                {
                    _vera.ISR |= _vera.ISR_Vsync;
                    runner.Cpu.SetInterrupt();
                }
            }

            var reqPct = _width / (double)(_width * _height) * (_currentY + _porch + 1); // 1 line
            var cpuTicksFrame = (runner.CpuFrequency / _fps);

            var reqCpuTicks = (reqPct * cpuTicksFrame) - runner.CpuTicks;

            _currentX = 0;
            _currentY++;

            if (_vera.IrqLine == _currentY && (_vera.ISR & _vera.ISR_Line) == 0 && _vera.LineInterupt && _currentY <= _displayHeight)
            {
                _vera.ISR |= _vera.ISR_Line;
                runner.Cpu.SetInterrupt();
            }

            bool frameDone = _currentY == 0;
            if (frameDone)
            {
                _outputPosition = 0;
                foreach(var d in Displays)
                {
                    d.Switch();
                }
            }
            else if(_currentY >= 0 && _currentY < _displayHeight)
            {
                _outputPosition += _displayWidth;
            } else if (_currentY > _height - _porch - 1)
            {
                _currentY = 0 - _porch;
            }

            bool release = _currentY >= 0 && _currentY < _displayHeight-1;
            return (frameDone, (int)reqCpuTicks, release);
        }

        public void PreRender()
        {
            _vera.CopyToShadow();
        }
    }
}
