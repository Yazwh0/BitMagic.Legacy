﻿using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BitMagic.X16Emulator;

public class Emulator : IDisposable
{
    [DllImport(@"..\..\..\..\x64\Debug\EmulatorCore.dll")]
    private static extern int fnEmulatorCode(ref CpuState state);

    public enum ReadEffectType : byte
    { 
        Nothing = 0,
        Vera
    }

    public enum WriteEffectType : byte
    { 
        Nothing = 0,
        Vera,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CpuState
    {
        public ulong MemoryPtr = 0;
        public ulong RomPtr = 0;
        public ulong RamBankPtr = 0;

        public ReadEffectType[] ReadEffect;
        public WriteEffectType[] WriteEffect;

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

        public byte Interrupt = 0;

        public CpuState(ulong memory, ulong rom, ulong ramBank)
        {
            MemoryPtr = memory;
            RomPtr = rom;
            RamBankPtr = ramBank;

            ReadEffect = new ReadEffectType[0x10000];
            WriteEffect = new WriteEffectType[0x10000];
        }
    }

    public enum EmulatorResult
    {
        ExitCondition,
        UnknownOpCode,
        DebugOpCode,
        Unsupported = -1
    }

    private CpuState _state;

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
    public bool Interrupt { get => _state.Interrupt != 0; set => _state.Interrupt = (byte)(value ? 0x01 : 0x00); }

    private readonly ulong _memory_ptr;
    private readonly ulong _rom_ptr;
    private readonly ulong _ram_ptr;

    private const int RamSize = 0xa000; // only as high as banked ram
    private const int RomSize = 0x4000 * 32;
    private const int BankedRamSize = 0x2000 * 256;

    public unsafe Emulator() 
    {
        _memory_ptr = (ulong)NativeMemory.Alloc(RamSize);
        _rom_ptr = (ulong)NativeMemory.Alloc(RomSize);
        _ram_ptr = (ulong)NativeMemory.Alloc(BankedRamSize);

        _state = new CpuState(_memory_ptr, _rom_ptr, _ram_ptr);

        var memory_span = new Span<byte>((void*)_memory_ptr, RamSize);
        for (var i = 0; i < RamSize; i++)
            memory_span[i] = 0;

        var ram_span = new Span<byte>((void*)_ram_ptr, BankedRamSize);
        for (var i = 0; i < BankedRamSize; i++)
            ram_span[i] = 0;
    }

    public unsafe Span<byte> Memory => new Span<byte>((void*)_memory_ptr, RamSize);
    public unsafe Span<byte> RamBank => new Span<byte>((void*)_ram_ptr, BankedRamSize);
    public unsafe Span<byte> RomBank => new Span<byte>((void*)_rom_ptr, RomSize);

    public EmulatorResult Emulate()
    {
        var r = fnEmulatorCode(ref _state);
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
    }
}