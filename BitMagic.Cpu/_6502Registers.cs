using BitMagic.Common;
using System.Diagnostics;

namespace BitMagic.Cpu
{
    public class _6502Registers : I6502Registers
    {
        public ushort PC { get; set; }
        public byte S { get; set; } = 0xff;

        public byte P { get => _flags.Register ; set => _flags.Register = value; }

        private byte _a;
        public byte A { 
            get => _a; 
            set {
                _a = value;
                _flags.SetNv(value);
            } 
        }

        private byte _x;
        public byte X
        {
            get => _x;
            set
            {
                _x = value;
                _flags.SetNv(value);
            }
        }

        private byte _y;
        public byte Y
        {
            get => _y;
            set
            {
                _y = value;
                _flags.SetNv(value);
            }
        }

        private I6502Flags _flags = new _6502Flags();
        IFlags IRegisters.Flags => _flags;
        public I6502Flags Flags => _flags;

        public int NumRegisters => 7;


        public byte GetRegister(int index) => index switch
        {
            0 => (byte)(PC & 0x00ff),
            1 => (byte)((PC & 0xff00) >> 8),
            2 => S,
            3 => P,
            4 => A,
            5 => X,
            6 => Y,
            _ => 0
        };

        public string GetRegisterName(int index) => index switch { 
            0 => "PC_L",
            1 => "PC_H",
            2 => "S",
            3 => "P",
            4 => "A",
            5 => "X",
            6 => "Y",
            _ => "Unknown"
        };

        public void SetRegister(int index, byte value)
        {
            switch(index)
            {
                case 0:
                    PC = (ushort)(PC & 0xff00 + value);
                    break;
                case 1:
                    PC = (ushort)(PC & 0x00ff + value * 256);
                    break;
                case 2:
                    S = value;
                    break;
                case 3:
                    P = value;
                    break;
                case 4:
                    A = value;
                    break;
                case 5:
                    X = value;
                    break;
                case 6:
                    Y = value;
                    break;
            }
        }

        public override string ToString() => $"A:${A:X2} X:${X:X2} Y:${Y:X2} PC:${PC:X4} S:${S:X2}";
    }
}

