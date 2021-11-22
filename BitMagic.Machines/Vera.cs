using BitMagic.Common;
using BitMagic.Cpu.Memory;
using System;

namespace BitMagic.Machines
{
    public class VeraLayer
    {
        public enum LayerColourDepth
        {
            bpp1,
            bpp2,
            bpp4,
            bp8p
        }

        public int MapHeight { get; set; }
        public int MapWidth { get; set; }
        public bool BitmapMode { get; set; }
        public bool T256C { get; set; }
        public LayerColourDepth ColourDepth { get; set; }
        public int MapBase { get; set; }
        public int TileBase { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

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

        public override int Length => 0x20;

        private readonly IMemory _vram;
        internal readonly Palette Palette;

        public int Data0 { get; set; } = 0;
        public int Data1 { get; set; } = 0;

        public int Data0Step { get; set; } = 0;
        public int Data1Step { get; set; } = 0;

        public bool Data1Mode { get; set; } = false;
        public bool DcMode { get; set; } = false;

        public int IrqLine { get; set; }

        public bool LineInterupt { get; set; } = false;
        public bool VsyncInterupt { get; set; } = false;
        public bool SpriteColInterupt { get; set; } = false;
        public bool AflowInterupt { get; set; } = false;

        public int HScale { get; set; } = 1;
        public int VScale { get; set; } = 1;
        public int BorderColour { get; set; } = 0;

        public int HStart { get; set; }
        public int HStop { get; set; }
        public int VStart { get; set; }
        public int VStop { get; set; }

        public bool SpritesEnabled { get; set; } = false;
        public bool ChromaDisable { get; set; } = false;

        public VeraLayer Layer0 { get; } = new VeraLayer();
        public VeraLayer Layer1 { get; } = new VeraLayer();

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

        BitImage[] IDisplay.Displays => _display.Displays;

        (bool framedone, int nextCpuTick) IDisplay.IncrementDisplay(IMachineRunner runner) => _display.IncrementDisplay(runner);

        public Vera()
        {
            Palette = new Palette();
            _display = new VeraDisplay(2, this);

            _vram = new MemoryMap(0x20000, new IMemory[] {
                new Ram(0x1f9c0),
                new Ram(0x40),      // PSG
                Palette,            // Pallete
                new Ram(0x400)      // Sprites
            });
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

        public int GetTileSize(int size) => size switch
        {
            0 => 8,
            1 => 16,
            _ => throw new ArgumentException(nameof(size))
        };

        public override void SetByte(int address, byte value)
        {
            switch ((VeraRegisters)address)
            {
                case VeraRegisters.ADDRx_L:
                    if (!Data1Mode)
                    {
                        Data0 = (Data0 & 0x1ff00) + value;
                    } 
                    else
                    {
                        Data1 = (Data1 & 0x1ff00) + value;
                    }
                    break;
                case VeraRegisters.ADDRx_M:
                    if (!Data1Mode)
                    {
                        Data0 = (Data0 & 0x100ff) + (value << 8);
                    } 
                    else
                    {
                        Data1 = (Data1 & 0x100ff) + (value << 8);
                    }
                    break;
                case VeraRegisters.ADDRx_H:
                    var decr = (value & 0b1000) != 0;
                    var inc = (value & 0xf0) >> 4; 
                    if (!Data1Mode)
                    {
                        Data0 = (Data0 & 0xffff) + ((value | 0x1) << 16);
                        Data0Step = GetStep(inc, decr);
                    }
                    else
                    {
                        Data1 = (Data1 & 0xffff) + ((value | 0x1) << 16);
                        Data1Step = GetStep(inc, decr);
                    }
                    break;
                case VeraRegisters.DATA0:
                    _vram.SetByte(Data0, value);
                    Data0 += Data0Step;
                    Data0 &= 0x1ffff;
                    break;
                case VeraRegisters.DATA1:
                    _vram.SetByte(Data1, value);
                    Data1 += Data1Step;
                    Data1 &= 0x1ffff;
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
                    IrqLine = (IrqLine & 0xff) + ((value & 0b1000_000) << 1);

                    VsyncInterupt = (value & 1) != 0;
                    LineInterupt = (value & 2) != 0;
                    SpriteColInterupt = (value & 4) != 0;
                    AflowInterupt = (value & 8) != 0;

                    break;
                case VeraRegisters.ISR:
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
                    Layer0.MapBase = value << 10; // 16:9
                    break;
                case VeraRegisters.L0_TILEBASE:
                    Layer0.TileBase = (value & 0b1111_1100) << 10; // 16:11
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
                    Layer1.MapBase = value << 10; // 16:9
                    break;
                case VeraRegisters.L1_TILEBASE:
                    Layer1.TileBase = (value & 0b1111_1100) << 10; // 16:11
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

            base.SetByte(address, value);
        }

        public override byte GetByte(int address)
        {
            byte toReturn;
            switch ((VeraRegisters)address)
            {
                case VeraRegisters.ADDRx_L:
                    break;
                case VeraRegisters.ADDRx_M:
                    break;
                case VeraRegisters.ADDRx_H:
                    break;
                case VeraRegisters.DATA0:
                    toReturn = _vram.GetByte(Data0);
                    Data0 += Data0Step;
                    Data0 &= 0b1_1111_1111_1111;
                    return toReturn;
                case VeraRegisters.DATA1:
                    toReturn = _vram.GetByte(Data0);
                    Data0 += Data0Step;
                    Data0 &= 0b1_1111_1111_1111;
                    return toReturn;
                case VeraRegisters.CTRL:
                    break;
                case VeraRegisters.IEN:
                    break;
                case VeraRegisters.ISR:
                    break;
                case VeraRegisters.IRQLINE_L:
                    break;
                case VeraRegisters.DC_VIDEO_HSTART:
                    break;
                case VeraRegisters.DC_HSCALE_HSTOP:
                    break;
                case VeraRegisters.DC_VSCALE_VSTART:
                    break;
                case VeraRegisters.DC_BORDER_VSTOP:
                    break;
                case VeraRegisters.L0_CONFIG:
                    break;
                case VeraRegisters.L0_MAPBASE:
                    break;
                case VeraRegisters.L0_TILEBASE:
                    break;
                case VeraRegisters.L0_HSCROLL_L:
                    break;
                case VeraRegisters.L0_HSCROLL_H:
                    break;
                case VeraRegisters.L0_VSCROLL_L:
                    break;
                case VeraRegisters.L0_VSCROLL_H:
                    break;
                case VeraRegisters.L1_CONFIG:
                    break;
                case VeraRegisters.L1_MAPBASE:
                    break;
                case VeraRegisters.L1_TILEBASE:
                    break;
                case VeraRegisters.L1_HSCROLL_L:
                    break;
                case VeraRegisters.L1_HSCROLL_H:
                    break;
                case VeraRegisters.L1_VSCROLL_L:
                    break;
                case VeraRegisters.L1_VSCROLL_H:
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
            
            return base.GetByte(address);
        }

        public override byte PeekByte(int Address) => Memory[Address];

    }
}
