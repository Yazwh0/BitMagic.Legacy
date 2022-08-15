using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BitMagic.X16Emulator;

public class Emulator : IDisposable
{
    [DllImport(@"..\..\..\..\x64\Debug\EmulatorCore.dll")]
    private static extern int fnEmulatorCode(ref CpuState state);

    [StructLayout(LayoutKind.Sequential)]
    public struct VeraState
    {
        public ulong VramPtr = 0;
        public ulong Data0_Address = 0;
        public ulong Data1_Address = 0;
        public ulong Data0_Step = 0;
        public ulong Data1_Step = 0;

        public VeraState()
        { 
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CpuState
    {
        public ulong MemoryPtr = 0;
        public ulong RomPtr = 0;
        public ulong RamBankPtr = 0;
        public ulong VeraPtr = 0;

        public ulong Clock = 0x00;
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

        public byte Interrupt = 0;


        public unsafe CpuState(ulong memory, ulong rom, ulong ramBank, IntPtr vera)
        {
            MemoryPtr = memory;
            RomPtr = rom;
            RamBankPtr = ramBank;
            VeraPtr = (ulong)vera;
        }
    }

    public enum EmulatorResult
    {
        ExitCondition,
        UnknownOpCode,
        DebugOpCode,
        Unsupported = -1
    }

    private CpuState _cpu;
    private VeraState _vera;


    public byte A {get => _cpu.A; set => _cpu.A = value; }
    public byte X { get => _cpu.X; set => _cpu.X = value; }
    public byte Y { get => _cpu.Y; set => _cpu.Y = value; }
    public ushort Pc { get => _cpu.Pc; set => _cpu.Pc = value; }
    public ushort StackPointer { get => _cpu.StackPointer; set => _cpu.StackPointer = value; }
    public ulong Clock { get => _cpu.Clock; set => _cpu.Clock = value; }
    public bool Carry { get => _cpu.Carry != 0; set => _cpu.Carry = (byte)(value ? 0x01 : 0x00); }
    public bool Zero { get => _cpu.Zero != 0; set => _cpu.Zero = (byte)(value ? 0x01 : 0x00); }
    public bool InterruptDisable { get => _cpu.InterruptDisable != 0; set => _cpu.InterruptDisable = (byte)(value ? 0x01 : 0x00); }
    public bool Decimal { get => _cpu.Decimal != 0; set => _cpu.Decimal = (byte)(value ? 0x01 : 0x00); }
    public bool BreakFlag { get => _cpu.BreakFlag != 0; set => _cpu.BreakFlag = (byte)(value ? 0x01 : 0x00); }
    public bool Overflow { get => _cpu.Overflow != 0; set => _cpu.Overflow = (byte)(value ? 0x01 : 0x00); }
    public bool Negative { get => _cpu.Negative != 0; set => _cpu.Negative = (byte)(value ? 0x01 : 0x00); }
    public bool Interrupt { get => _cpu.Interrupt != 0; set => _cpu.Interrupt = (byte)(value ? 0x01 : 0x00); }

    private readonly ulong _memory_ptr;
    private readonly ulong _rom_ptr;
    private readonly ulong _ram_ptr;
    private readonly ulong _vram_ptr;
    private readonly IntPtr _vera_ptr;

    private const int RamSize = 0xa000; // only as high as banked ram
    private const int RomSize = 0x4000 * 32;
    private const int BankedRamSize = 0x2000 * 256;
    private const int VramSize = 0x20000;

    public unsafe Emulator() 
    {
        _memory_ptr = (ulong)NativeMemory.Alloc(RamSize);
        _rom_ptr = (ulong)NativeMemory.Alloc(RomSize);
        _ram_ptr = (ulong)NativeMemory.Alloc(BankedRamSize);
        _vram_ptr = (ulong)NativeMemory.Alloc(VramSize);

        _vera_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(_vera));
        Marshal.StructureToPtr(_vera, _vera_ptr, false);

        _vera.VramPtr = _vram_ptr;

        _cpu = new CpuState(_memory_ptr, _rom_ptr, _ram_ptr, _vera_ptr);

        var memory_span = new Span<byte>((void*)_memory_ptr, RamSize);
        for (var i = 0; i < RamSize; i++)
            memory_span[i] = 0;

        var ram_span = new Span<byte>((void*)_ram_ptr, BankedRamSize);
        for (var i = 0; i < BankedRamSize; i++)
            ram_span[i] = 0;

        var vram_span = new Span<byte>((void*)_vram_ptr, VramSize);
        for (var i = 0; i < VramSize; i++)
            vram_span[i] = 0;


        var rom_span = new Span<byte>((void*)_rom_ptr, RomSize);
        for (var i = 0; i < RomSize; i++)
            rom_span[i] = 0;
    }

    public unsafe Span<byte> Memory => new Span<byte>((void*)_memory_ptr, RamSize);
    public unsafe Span<byte> RamBank => new Span<byte>((void*)_ram_ptr, BankedRamSize);
    public unsafe Span<byte> RomBank => new Span<byte>((void*)_rom_ptr, RomSize);
    public unsafe Span<byte> Vram => new Span<byte>((void*)_vram_ptr, VramSize);    

    public EmulatorResult Emulate()
    {
        //_state.VeraStatePtr.Data1_Step = 0xffee;
        var r = fnEmulatorCode(ref _cpu);
        var result = (EmulatorResult)r;
        return result;
    }

    public unsafe void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual unsafe void Dispose(bool disposing)
    {
        NativeMemory.Free((void*)_memory_ptr);
        NativeMemory.Free((void*)_rom_ptr);
        NativeMemory.Free((void*)_ram_ptr);
        NativeMemory.Free((void*)_vram_ptr);
        Marshal.FreeHGlobal(_vera_ptr);
    }
}