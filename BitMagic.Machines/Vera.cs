using BitMagic.Common;
using BitMagic.Cpu.Memory;
using System;

namespace BitMagic.Machines
{
    public struct VeraLayer
    {
        public enum LayerColourDepth
        {
            bpp1 = 0,
            bpp2 = 1,
            bpp4 = 2,
            bpp8 = 3
        }

        public int _mapHeight;
        public int MapHeight
        {
            get => _mapHeight; 
            set
            {
                _mapHeight = value;
                MapHeightShift = value switch
                {
                    32 => 5,
                    64 => 6,
                    128 => 7,
                    256 => 8,
                    _ => 0
                };
            }
        }
        public int MapHeightShift { get; set; }

        private int _mapWidth;
        public int MapWidth
        {
            get => _mapWidth; 
            set
            {
                _mapWidth = value;
                MapWidthShift = value switch
                {
                    32 => 5,
                    64 => 6,
                    128 => 7,
                    256 => 8,
                    _ => 0
                };
            }
        }
        public int MapWidthShift { get; set; }

        public bool BitmapMode { get; set; }
        public bool T256C { get; set; }

        private LayerColourDepth _colourDepth;
        public LayerColourDepth ColourDepth { get => _colourDepth;
            set {
                _colourDepth = value;
                ColourDepthShift = value switch
                {
                    LayerColourDepth.bpp1 => 4,
                    LayerColourDepth.bpp2 => 2,
                    LayerColourDepth.bpp4 => 1,
                    LayerColourDepth.bpp8 => 0,
                    _ => throw new Exception($"unhandled colour depth {value}")
                };
            } 
        }
        public int ColourDepthShift { get; set; }

        public int MapBase { get; set; }
        public int TileBase { get; set; }

        private int _tileWidth;
        public int TileWidth
        {
            get => _tileWidth; 
            set
            {
                _tileWidth = value;
                TileWidthShift = value == 8 ? 3 : 4;
            }
        }
        public int TileWidthShift { get; set; }

        private int _tileHeight;
        public int TileHeight
        {
            get => _tileHeight; 
            set
            {
                _tileHeight = value;
                TileHeightShift = value == 8 ? 3 : 4;
            }
        }
        public int TileHeightShift { get; set; }

        public int HScroll { get; set; }
        public int VScroll { get; set; }
        public bool Enabled { get; set; }
    }

    public class Vera : NormalMemory, IDisplay
    {
        public enum VeraRegisters
        {
            ADDRx_L,
            ADDRx_M,
            ADDRx_H,
            DATA0,
            DATA1,
            CTRL,
            IEN,
            ISR,
            IRQLINE_L,
            DC_VIDEO_HSTART,
            DC_HSCALE_HSTOP,
            DC_VSCALE_VSTART,
            DC_BORDER_VSTOP,
            L0_CONFIG,
            L0_MAPBASE,
            L0_TILEBASE,
            L0_HSCROLL_L,
            L0_HSCROLL_H,
            L0_VSCROLL_L,
            L0_VSCROLL_H,
            L1_CONFIG,
            L1_MAPBASE,
            L1_TILEBASE,
            L1_HSCROLL_L,
            L1_HSCROLL_H,
            L1_VSCROLL_L,
            L1_VSCROLL_H,
            AUDIO_CTRL,
            AUDIO_RATE,
            AUDIO_DATA,
            SPI_DATA,
            SPI_CTRL,
        };

        public IMemory Vram { get; }

        public IMemory VramShadow { get; }

        internal readonly Palette Palette;
        internal readonly SpritesMemory Sprites;

        public int Data0Addr { get; set; } = 0;
        public int Data1Addr { get; set; } = 0;

        public int Data0Step { get; set; } = 0;
        public int Data1Step { get; set; } = 0;

        public bool Data1Mode { get; set; } = false;
        public bool DcMode { get; set; } = false;

        public byte ISR_Line { get; } = 0x02;
        public byte ISR_Vsync { get; } = 0x01;

        private byte _isr { get; set; } = 0;
        public byte ISR
        {
            get => _isr; set
            {
                _isr = value;
                Memory!.Memory[StartAddress + (int)VeraRegisters.ISR] = value; // reflect in main ram
            }
        }
        public int IrqLine { get; set; }

        public bool LineInterupt { get; set; } = false;
        public bool VsyncInterupt { get; set; } = false;
        public bool SpriteColInterupt { get; set; } = false;
        public bool AflowInterupt { get; set; } = false;

        private int _hScale = 128;
        public int HScale
        {
            get => _hScale; 
            set
            {
                _hScale = value;
                HScaleStep = value switch
                {
                    128 => 1,
                    64 => 2,
                    32 => 4,
                    16 => 8,
                    _ => throw new Exception($"Cannot handle scale of {value}")
                };
            }
        }
        public int HScaleStep { get; set; } = 1;

        public int _vScale = 128;
        public int VScale
        {
            get => _vScale; 
            set
            {
                _vScale = value;
                VScaleStep = value switch
                {
                    128 => 1,
                    64 => 2,
                    32 => 4,
                    16 => 8,
                    _ => throw new Exception($"Cannot handle scale of {value}")
                };
            }
        }
        public int VScaleStep { get; set; } = 1;

        public int BorderColour { get; set; } = 0;

        public int HStart { get; set; }
        public int HStop { get; set; }
        public int VStart { get; set; }
        public int VStop { get; set; }

        public bool SpritesEnabled { get; set; } = false;
        public bool ChromaDisable { get; set; } = false;

        public VeraLayer Layer0 = new VeraLayer();
        public VeraLayer Layer1 = new VeraLayer();

        public VeraLayer Layer0Shadow = new VeraLayer();
        public VeraLayer Layer1Shadow = new VeraLayer();

        public enum VeraOutputMode
        { 
            Disabled,
            VGA,
            NTSC,
            RGBinterlaced
        }

        public VeraOutputMode OutputMode { get; set; } = VeraOutputMode.Disabled;

        private VeraDisplay _display { get; } 

        Action<object?>[] IDisplay.DisplayThreads => _display.DisplayThreads;
        bool[] IDisplay.DisplayHold => _display.DisplayHold;

        BitImage[] IDisplay.Displays => _display.Displays;

        (bool framedone, int nextCpuTick, bool releaseVideo) IDisplay.IncrementDisplay(IMachineRunner runner) => _display.IncrementDisplay(runner);

        public Vera() : base("Vera", 0x20)
        {
            Palette = new Palette();
            Sprites = new SpritesMemory();
            _display = new VeraDisplay(2, this);

            Vram = new MemoryMap(0, 0x20000, new IMemoryBlock[] {
                new Ram("VRAM", 0x1f9c0),
                new Ram("PSG", 0x40),      // PSG
                Palette,                   // Pallete
                Sprites                    // Sprites
            });

            VramShadow = new MemoryMap(0, 0x20000, new IMemoryBlock[] { 
                new Ram("VRAMShadow", 0x20000)
            });
        }

        public void CopyToShadow()
        {
            Vram.MemoryStruct.CopyTo(VramShadow.MemoryStruct);
            Layer0Shadow = Layer0;
            Layer1Shadow = Layer1;
        }

        public override void Init(IMemory memory, int startAddress)
        {
            base.Init(memory, startAddress);

            for (var i = 0; i < Length; i++)
            {
                Memory!.WriteNotification[StartAddress + i] = WriteNotification;
                Memory!.ReadNotification[startAddress + i] = ReadNotification;
            }
        }

        private int GetStep(int step, bool decr) => step switch
        {
            0 => 0,
            1 => 1,
            2 => 2,
            3 => 4,
            4 => 8,
            5 => 16,
            6 => 32,
            7 => 64,
            8 => 128,
            9 => 256,
            10 => 512,
            11 => 40,
            12 => 80,
            13 => 160,
            14 => 320,
            15 => 640,
            _ => throw new ArgumentException(nameof(step))
        } * (decr ? -1 : 1);

        public int GetSize(int size) => size switch
        {
            0 => 32,
            1 => 64,
            2 => 128,
            3 => 256,
            _ => throw new ArgumentException(nameof(size))
        };

/*        private VeraLayer.LayerColourDepth GetDepth(int depth) => depth switch { 
            0 => VeraLayer.LayerColourDepth.bpp1,
            1 => VeraLayer.LayerColourDepth.bpp2,
            2 => VeraLayer.LayerColourDepth.bpp4,
            3 => VeraLayer.LayerColourDepth.bpp8
        };*/

        public int GetTileSize(int size) => size switch
        {
            0 => 8,
            1 => 16,
            _ => throw new ArgumentException(nameof(size))
        };

        public void WriteNotification(int address, byte value)
        {
            switch ((VeraRegisters)(address - StartAddress))
            {
                case VeraRegisters.ADDRx_L:
                    if (!Data1Mode)
                    {
                        Data0Addr = (Data0Addr & 0x1ff00) + value;
                    } 
                    else
                    {
                        Data1Addr = (Data1Addr & 0x1ff00) + value;
                    }
                    break;
                case VeraRegisters.ADDRx_M:
                    if (!Data1Mode)
                    {
                        Data0Addr = (Data0Addr & 0x100ff) + (value << 8);
                    } 
                    else
                    {
                        Data1Addr = (Data1Addr & 0x100ff) + (value << 8);
                    }
                    break;
                case VeraRegisters.ADDRx_H:
                    var decr = (value & 0b1000) != 0;
                    var inc = (value & 0xf0) >> 4; 
                    if (!Data1Mode)
                    {
                        Data0Addr = (Data0Addr & 0xffff) + ((value & 0x1) << 16);
                        Data0Step = GetStep(inc, decr);
                    }
                    else
                    {
                        Data1Addr = (Data1Addr & 0xffff) + ((value & 0x1) << 16);
                        Data1Step = GetStep(inc, decr);
                    }
                    break;
                case VeraRegisters.DATA0:
                    Vram.SetByte(Data0Addr, value);
                    Data0Addr += Data0Step;
                    Data0Addr &= 0x1ffff;
                    break;
                case VeraRegisters.DATA1:
                    Vram.SetByte(Data1Addr, value);
                    Data1Addr += Data1Step;
                    Data1Addr &= 0x1ffff;
                    break;
                case VeraRegisters.CTRL:
                    if ((value & 0b1000_000) != 0)
                    {
                        // todo: reset.
                        return;
                    }
                    Data1Mode = (value & 1) != 0;
                    DcMode = (value & 2) != 0;
                    break;
                case VeraRegisters.IEN:
                    IrqLine = (IrqLine & 0xff) + ((value & 0b1000_0000) << 1);

                    VsyncInterupt = (value & 1) != 0;
                    LineInterupt = (value & 2) != 0;
                    SpriteColInterupt = (value & 4) != 0;
                    AflowInterupt = (value & 8) != 0;

                    break;
                case VeraRegisters.ISR:
                    value = (byte)((~value & 0x0f) + 0xf0);

                    _isr = (byte)(_isr & value);
                    value = _isr;
                    break;
                case VeraRegisters.IRQLINE_L:
                    IrqLine = (IrqLine & 0x100) + value;
                    break;
                case VeraRegisters.DC_VIDEO_HSTART:
                    if (!DcMode)
                    {
                        value =    (byte)(value & 0b01111111); // setting current field could break things.
                        SpritesEnabled = (value & 0b01000000) != 0;
                        Layer1.Enabled = (value & 0b00100000) != 0;
                        Layer0.Enabled = (value & 0b00010000) != 0;
                        ChromaDisable =  (value & 0b00000100) != 0;
                        OutputMode = (VeraOutputMode)(value & 0b11);
                    }
                    else
                    {
                        HStart = value << 2;
                    }
                    break;
                case VeraRegisters.DC_HSCALE_HSTOP:
                    if (!DcMode)
                    {
                        HScale = value;
                    }
                    else
                    {
                        HStop = value << 2;
                    }
                    break;
                case VeraRegisters.DC_VSCALE_VSTART:
                    if (!DcMode)
                    {
                        VScale = value;
                    }
                    else
                    {
                        VStart = value << 1;
                    }
                    break;
                case VeraRegisters.DC_BORDER_VSTOP:
                    if (!DcMode)
                    {
                        BorderColour = value;
                    }
                    else
                    {
                        VStart = value << 2;
                    }
                    break;
                case VeraRegisters.L0_CONFIG:
                    Layer0.BitmapMode = (value & 0b100) != 0;
                    Layer0.T256C = (value & 0b1000) != 0;
                    Layer0.ColourDepth = (VeraLayer.LayerColourDepth)(value & 0b11);
                    Layer0.MapHeight = GetSize((value & 0b1100_0000) >> 6);
                    Layer0.MapWidth = GetSize((value & 0b11_0000) >> 4);
                    break;
                case VeraRegisters.L0_MAPBASE:
                    Layer0.MapBase = value << 9; // 16:9
                    break;
                case VeraRegisters.L0_TILEBASE:
                    Layer0.TileBase = (value & 0b1111_1100) << 9; // 16:11
                    Layer0.TileWidth = GetTileSize(value & 1);
                    Layer0.TileHeight = GetTileSize((value & 2) >> 1);
                    break;
                case VeraRegisters.L0_HSCROLL_L:
                    Layer0.HScroll = (Layer0.HScroll & 0xff00) + value;
                    break;
                case VeraRegisters.L0_HSCROLL_H:
                    Layer0.HScroll = (Layer0.HScroll & 0xff) + ((value & 0x0f) << 8);
                    break;
                case VeraRegisters.L0_VSCROLL_L:
                    Layer0.VScroll = (Layer0.VScroll & 0xff00) + value;
                    break;
                case VeraRegisters.L0_VSCROLL_H:
                    Layer0.VScroll = (Layer0.VScroll & 0xff) + ((value & 0x0f) << 8);
                    break;
                case VeraRegisters.L1_CONFIG:
                    Layer1.BitmapMode = (value & 0b100) != 0;
                    Layer1.T256C = (value & 0b1000) != 0;
                    Layer1.ColourDepth = (VeraLayer.LayerColourDepth)(value & 0b11);
                    Layer1.MapHeight = GetSize((value & 0b1100_0000) >> 6);
                    Layer1.MapWidth = GetSize((value & 0b11_0000) >> 4);
                    break;
                case VeraRegisters.L1_MAPBASE:
                    Layer1.MapBase = value << 9; // 16:9
                    break;
                case VeraRegisters.L1_TILEBASE:
                    Layer1.TileBase = (value & 0b1111_1100) << 9; // 16:11
                    Layer1.TileWidth = GetTileSize(value & 1);
                    Layer1.TileHeight = GetTileSize((value & 2) >> 1);
                    break;
                case VeraRegisters.L1_HSCROLL_L:
                    Layer1.HScroll = (Layer1.HScroll & 0xff00) + value;
                    break;
                case VeraRegisters.L1_HSCROLL_H:
                    Layer1.HScroll = (Layer1.HScroll & 0xff) + ((value & 0x0f) << 8);
                    break;
                case VeraRegisters.L1_VSCROLL_L:
                    Layer1.VScroll = (Layer1.VScroll & 0xff00) + value;
                    break;
                case VeraRegisters.L1_VSCROLL_H:
                    Layer1.VScroll = (Layer1.VScroll & 0xff) + ((value & 0x0f) << 8);
                    break;
                case VeraRegisters.AUDIO_CTRL:
                    break;
                case VeraRegisters.AUDIO_RATE:
                    break;
                case VeraRegisters.AUDIO_DATA:
                    break;
                case VeraRegisters.SPI_DATA:
                    break;
                case VeraRegisters.SPI_CTRL:
                    break;
            }

            Memory!.Memory[address] = value;
        }

        public byte ReadNotification(int address)
        {
            byte toReturn;
            switch ((VeraRegisters)(address - StartAddress))
            {
                case VeraRegisters.DATA0:
                    toReturn = Vram.GetByte(Data0Addr);
                    Data0Addr += Data0Step;
                    Data0Addr &= 0b1_1111_1111_1111_1111;
                    return toReturn;
                case VeraRegisters.DATA1:
                    toReturn = Vram.GetByte(Data1Addr);
                    Data1Addr += Data1Step;
                    Data1Addr &= 0b1_1111_1111_1111_1111;
                    return toReturn;
            }
            return Memory!.Memory[address];
        }

        public void PreRender()
        {
            CopyToShadow();
        }
    }
}
