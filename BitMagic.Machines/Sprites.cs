using BitMagic.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Machines
{
    internal class SpritesMemory : NormalMemory
    {
        public Sprite[] Sprites = new Sprite[128];

        public SpritesMemory() : base("Sprites", 0x400) //8 * 128
        {
        }

        public override void Init(IMemory memory, int startAddress)
        {
            base.Init(memory, startAddress);

            for(var i = 0; i < 128; i++)
                Sprites[i] = new Sprite();

            for(var i = 0; i < 0x400; i++)
            {
                Memory!.WriteNotification[startAddress + i] = WriteNotification;
            }
        }

        public void WriteNotification(int address, byte value)
        {
            var spriteNumber = (address - StartAddress) >> 3;
            var idx = address % 8;

            switch(idx)
            {
                case 0:
                    Sprites[spriteNumber].Address = (Sprites[spriteNumber].Address & 0b1111_1110_0000_0000_0000) + (value << 5);
                    break;
                case 1:
                    Sprites[spriteNumber].Address = (Sprites[spriteNumber].Address & 0b0000_0001_1111_1110_0000) + ((value & 0x0f) << 13);
                    Sprites[spriteNumber].Bpp4 = (value & 0b1000_0000) == 0;
                    break;
                case 2:
                    Sprites[spriteNumber].X = (Sprites[spriteNumber].X & 0b11_0000_0000) + value;
                    break;
                case 3:
                    Sprites[spriteNumber].X = (Sprites[spriteNumber].X & 0b00_1111_1111) + ((value & 0b11) << 8);
                    break;
                case 4:
                    Sprites[spriteNumber].Y = (Sprites[spriteNumber].Y & 0b11_0000_0000) + value;
                    break;
                case 5:
                    Sprites[spriteNumber].Y = (Sprites[spriteNumber].Y & 0b00_1111_1111) + ((value & 0b11) << 8);
                    break;
                case 6:
                    Sprites[spriteNumber].HFlip = (value & 0b0000_0001) > 1;
                    Sprites[spriteNumber].VFlip = (value & 0b0000_0010) > 1;
                    Sprites[spriteNumber].Depth = (value & 0b0000_1100) >> 2;
                    // todo: collision mask
                    break;
                case 7:
                    Sprites[spriteNumber].PaletteOffset = value & 0b0000_1111;
                    Sprites[spriteNumber].Width = ((value & 0b0011_0000) >> 4) switch
                    {
                        0 => 8,
                        1 => 16,
                        2 => 32,
                        3 => 64,
                        _ => 0
                    };
                    Sprites[spriteNumber].Height = ((value & 0b1100_0000) >> 6) switch
                    {
                        0 => 8,
                        1 => 16,
                        2 => 32,
                        3 => 64,
                        _ => 0
                    };
                    break;
            }
        }
    }

    internal class Sprite
    {
        public int Address;
        public int X;
        public int Y;
        public int Depth;
        public bool Bpp4;
        public bool VFlip;
        public bool HFlip;
        public int Height;
        public int Width;
        public int PaletteOffset;
    }
}
