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

            var background = Displays[BackgroundIdx];
            var sprite0 = Displays[Sprite0Idx];
            var sprite1 = Displays[Sprite1Idx];
            var sprite2 = Displays[Sprite2Idx];

            var zLayer = new int[_displayWidth];

            while (true)
            {
                var pos = _outputPosition;

                var myX = EffectiveX(_currentX);
                var myY = EffectiveY(_currentY);

                var spriteBudget = 801;
                var startPos = pos;

                for (var i = 0; i < _displayWidth; i++)
                {
                    background.DrawPixels.Span[pos++] = _vera.Palette.Colours[0];
                    zLayer[i] = 0;

                    sprite0.DrawPixels.Span[i] = new (0, 0, 0, 0);
                    sprite1.DrawPixels.Span[i] = new (0, 0, 0, 0);
                    sprite2.DrawPixels.Span[i] = new (0, 0, 0, 0);
                }

                var xScale = _vera.VScaleStep;

                if (_vera.SpritesEnabled)
                {
                    if (spriteBudget > 0)
                    {
                        var lastPixel = new PixelRgba(0, 0, 0, 0);

                        foreach (var sprite in _vera.Sprites.Sprites)
                        {
                            spriteBudget--;

                            if (sprite.Depth == 0)
                                continue;

                            if (myY < sprite.Y || myY > sprite.Y + sprite.Height)
                                continue;

                            for (var spriteX = 0; spriteX < sprite.Width; spriteX++)
                            {
                                spriteBudget--; // one per pixel

                                if ((spriteX & 3) == 0) // one every read of data?
                                    spriteBudget--;

                                if (spriteBudget <= 0)
                                    break;

                                var actX = sprite.X + spriteX;

                                if (sprite.X > actX)
                                    continue;

                                if (zLayer[actX] >= sprite.Depth)
                                    continue;

                                var spriteY = myY - sprite.Y;
                                if (sprite.VFlip)
                                    spriteY = sprite.Height - spriteY;

                                int colourIdx = 0;
                                if (sprite.Bpp4)
                                {
                                    var pixlIndex = spriteX + (spriteY * sprite.Width);
                                    var pixelAddress = sprite.Address + (pixlIndex >> 1);
                                    var value = (int)_vera.VramShadow.Memory[pixelAddress];

                                    if ((pixlIndex & 1) > 0)
                                    {
                                        value = value & 0x0f;
                                    }
                                    else
                                    {
                                        value = (value & 0xf0) >> 4;
                                    }

                                    colourIdx = value == 0 ? - 0 : (value + sprite.PaletteOffset * 16) & 0xff;
                                }
                                else
                                {
                                    var pixelAddress = sprite.Address + (spriteX + (spriteY * sprite.Width));
                                    var value = _vera.VramShadow.Memory[pixelAddress];
                                    colourIdx = value == 0 ? -0 : ( + (sprite.PaletteOffset * 16)) & 0xff;
                                }

                                if (colourIdx == 0)
                                {
                                    lastPixel = new PixelRgba(0, 0, 0, 0);
                                }
                                else
                                {
                                    lastPixel = _vera.Palette.Colours[colourIdx];
                                }

                                var outputPos = startPos + (actX * xScale);
                                for (var pxc = 0; pxc < xScale; pxc++)
                                {
                                    if (sprite.Depth == 1)
                                        sprite0.DrawPixels.Span[outputPos] = lastPixel;
                                    else if (sprite.Depth == 2)
                                        sprite1.DrawPixels.Span[outputPos] = lastPixel;
                                    else if (sprite.Depth == 3)
                                        sprite2.DrawPixels.Span[outputPos] = lastPixel;
                                    outputPos++;
                                }

                                zLayer[actX] = sprite.Depth;
                            }

                            if (spriteBudget <= 0)
                                break;
                        }
                    }
                }                

                DisplayHold[BackgroundThreadIdx] = true;
                while (DisplayHold[BackgroundThreadIdx]) { }

                //runner.DisplayEvents[BackgroundThreadIdx].Set();
                //runner.DisplayStart[BackgroundThreadIdx].WaitOne();
            }
        }


        const int _maxPixelsPerByte = 8;


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
