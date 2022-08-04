using System.Runtime.InteropServices;

namespace BitMagic.X16Emulator;

public class Emulator
{
    [DllImport(@"..\..\..\..\x64\Debug\EmulatorCore.dll")]
    private static extern int fnEmulatorCode(byte[] memory, ref CpuState state);

    [StructLayout(LayoutKind.Sequential)]
    public struct CpuState
    {
        public ulong Clock = 0;
        public ushort Pc = 0;
        public ushort StackPointer = 0x1ff;
        public byte A = 0;
        public byte X = 0;
        public byte Y = 0;
        
        public byte Decimal = 0;
        public byte BreakFlag = 0;
        public byte Overflow = 0;
        public byte Negative = 0;
        public byte Carry = 0;
        public byte Zero = 0;
        public byte InterruptDisable = 0;

        public CpuState()
        {
        }
    }

    public enum EmulatorResult
    {
        ExitCondition,
        UnknownOpCode,
        DebugOpCode,
        Unsupported = -1
    }

    private byte[] _memory;
    private CpuState _state;

    public byte[] Memory => _memory;

    public byte A {get => _state.A; set => _state.A = value; }
    public byte X { get => _state.X; set => _state.X = value; }
    public byte Y { get => _state.Y; set => _state.Y = value; }
    public ushort Pc { get => _state.Pc; set => _state.Pc = value; }
    public ushort StackPointer { get => _state.StackPointer; set => _state.StackPointer = value; }
    public ulong Clock { get => _state.Clock; set => _state.Clock = value; }
    public bool Carry { get => _state.Carry != 0; set => _state.Carry = (byte)(value ? 0x01 : 0x00); }
    public bool Zero { get => _state.Zero != 0; set => _state.Zero = (byte)(value ? 0x01 : 0x00); }
    public bool InterruptDisable { get => _state.InterruptDisable != 0; set => _state.InterruptDisable = (byte)(value ? 0x01 : 0x00); }
    public bool Decimal { get => _state.Decimal != 0; set => _state.Decimal = (byte)(value ? 0x01 : 0x00); }
    public bool BreakFlag { get => _state.BreakFlag != 0; set => _state.BreakFlag = (byte)(value ? 0x01 : 0x00); }
    public bool Overflow { get => _state.Overflow != 0; set => _state.Overflow = (byte)(value ? 0x01 : 0x00); }
    public bool Negative { get => _state.Negative != 0; set => _state.Negative = (byte)(value ? 0x01 : 0x00); }

    public Emulator()
    {
        _memory = new byte[0xffff];
        _state = new CpuState();
    }

    public EmulatorResult Emulate()
    {
        return (EmulatorResult)fnEmulatorCode(_memory, ref _state);
    }

    public void HardReset()
    { 
        for(var i = 0; i < 0xffff; i++)
            _memory[i] = 0;

        _state = new CpuState();
    }
}

