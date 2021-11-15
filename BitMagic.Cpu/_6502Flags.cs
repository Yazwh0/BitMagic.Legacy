using System;

namespace BitMagic.Cpu
{
    public class _6502Flags : I6502Flags
    {
        public bool Negative { get; set; }
        public bool Overflow { get; set; }
        public bool Unused => true;
        public bool Break { get; set; }
        public bool Decimal { get; set; }
        public bool InterruptDisable { get; set; }
        public bool Zero { get; set; }
        public bool Carry { get; set; }

        public int NumFlags => 8;

        public byte Register { get => (byte)((Negative ? 128 : 0) +
                                        (Overflow ? 64 : 0) +
                                        (Unused ? 32 : 0) +
                                        (Break ? 16 : 0) +
                                        (Decimal ? 8 : 0) +
                                        (InterruptDisable ? 4 : 0) +
                                        (Zero ? 2 : 0) +
                                        (Carry ? 1 : 0));
            set
            {
                Negative = (value & 128) != 0;
                Overflow = (value & 64) != 0;
                //Unused = (value & 32) != 0;
                Break = (value & 16) != 0;
                Decimal = (value & 8) != 0;
                InterruptDisable = (value & 4) != 0;
                Zero = (value & 2) != 0;
                Carry = (value & 1) != 0;
            }
        }

        public void SetNv(byte value)
        {
            Zero = value == 0;
            Negative = (value & 128) != 0;
        }

        public bool GetFlag(int index) => index switch {
            0 => Carry,
            1 => Zero,
            2 => InterruptDisable,
            3 => Decimal,
            4 => Break,
            5 => Unused,
            6 => Overflow,
            7 => Negative,
            _ => throw new Exception($"Unknown flag {index}")
        };

        public string GetFlagName(int index) => index switch {
            0 => "Carry",
            1 => "Zero",
            2 => "InterruptDisable",
            3 => "Decimal",
            4 => "Break",
            5 => "Unused",
            6 => "Overflow",
            7 => "Negative",
            _ => throw new Exception($"Unknown flag {index}")
        };

        public void SetFlag(int index, bool value)
        {
            switch (index)
            {
                case 0:
                    Carry = value;
                    break;
                case 1:
                    Zero = value;
                    break;
                case 2:
                    InterruptDisable = value;
                    break;
                case 3:
                    Decimal = value;
                    break;
                case 4:
                    Break = value;
                    break;
                case 5:
                    //Unused = value;
                    break;
                case 6:
                    Overflow = value;
                    break;
                case 7:
                    Negative = value;
                    break;
            }
        }

        public override string ToString() =>
            (Negative ? "N" : ".") +
            (Overflow ? "V" : ".") +
            (".") +
            (Break ? "B" : ".") +
            (Decimal ? "D" : ".") +
            (InterruptDisable ? "I" : ".") +
            (Zero ? "Z" : ".") +
            (Carry ? "C" : ".");

    }
}

