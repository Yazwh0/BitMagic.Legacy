using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BitMagic.X16Emulator;

public class Emulator : IDisposable
{
    [DllImport(@"..\..\..\..\X16Emulator\EmulatorCore\x64\Debug\EmulatorCore.dll")]
    private static extern int fnEmulatorCode(ref CpuState state);

    public class VeraState
    {
        private readonly Emulator _emulator;
        public VeraState(Emulator emulator)
        {
            _emulator = emulator;
        }

        public unsafe Span<byte> Vram => new Span<byte>((void*)_emulator._state.VramPtr, VramSize);
        public int Data0_Address { get => (int)_emulator._state.Data0_Address; set => _emulator._state.Data0_Address = (ulong)value; }
        public int Data1_Address { get => (int)_emulator._state.Data1_Address; set => _emulator._state.Data1_Address = (ulong)value; }
        public int Data0_Step { get => (int)_emulator._state.Data0_Step; set => _emulator._state.Data0_Step = (ulong)value; }
        public int Data1_Step { get => (int)_emulator._state.Data1_Step; set => _emulator._state.Data1_Step = (ulong)value; }
        public bool AddrSel { get => _emulator._state.AddrSel != 0; set => _emulator._state.AddrSel = (value ? (byte)1 : (byte)0); }
        public bool DcSel { get => _emulator._state.DcSel != 0; set => _emulator._state.DcSel = (value ? (byte)1 : (byte)0); }
        public byte Dc_HScale { get => _emulator._state.Dc_HScale; set => _emulator._state.Dc_HScale = value; }
        public byte Dc_VScale { get => _emulator._state.Dc_VScale; set => _emulator._state.Dc_VScale = value; }
        public byte Dc_Border { get => _emulator._state.Dc_Border; set => _emulator._state.Dc_Border = value; }
        public byte Dc_HStart { get => _emulator._state.Dc_HStart; set => _emulator._state.Dc_HStart = value; }
        public byte Dc_HStop { get => _emulator._state.Dc_HStop; set => _emulator._state.Dc_HStop = value; }
        public byte Dc_VStart { get => _emulator._state.Dc_VStart; set => _emulator._state.Dc_VStart = value; }
        public byte Dc_VStop { get => _emulator._state.Dc_VStop; set => _emulator._state.Dc_VStop = value; }
        public bool SpriteEnable { get => _emulator._state.SpriteEnable != 0; set => _emulator._state.SpriteEnable = (value ? (byte)1 : (byte)0); }
        public bool Layer0Enable { get => _emulator._state.Layer0Enable != 0; set => _emulator._state.Layer0Enable = (value ? (byte)1 : (byte)0); }
        public bool Layer1Enable { get => _emulator._state.Layer1Enable != 0; set => _emulator._state.Layer1Enable = (value ? (byte)1 : (byte)0); }

        public byte Layer0_MapHeight { get => _emulator._state.Layer0_MapHeight; set => _emulator._state.Layer0_MapHeight = value; }
        public byte Layer0_MapWidth { get => _emulator._state.Layer0_MapWidth; set => _emulator._state.Layer0_MapWidth = value; }
        public bool Layer0_BitMapMode { get => _emulator._state.Layer0_BitMapMode != 0; set => _emulator._state.Layer0_BitMapMode = (value ? (byte)1 : (byte)0); }
        public byte Layer0_ColourDepth { get => _emulator._state.Layer0_ColourDepth; set => _emulator._state.Layer0_ColourDepth = value; }
        public UInt32 Layer0_MapAddress { get => _emulator._state.Layer0_MapAddress; set => _emulator._state.Layer0_MapAddress = value; }
        public UInt32 Layer0_TileAddress { get => _emulator._state.Layer0_TileAddress; set => _emulator._state.Layer0_TileAddress = value; }
        public byte Layer0_TileHeight { get => _emulator._state.Layer0_TileHeight; set => _emulator._state.Layer0_TileHeight = value; }
        public byte Layer0_TileWidth { get => _emulator._state.Layer0_TileWidth; set => _emulator._state.Layer0_TileWidth = value; }
        public ushort Layer0_HScroll { get => _emulator._state.Layer0_HScroll; set => _emulator._state.Layer0_HScroll = value; }
        public ushort Layer0_VScroll { get => _emulator._state.Layer0_VScroll; set => _emulator._state.Layer0_VScroll = value; }

        public byte Layer1_MapHeight { get => _emulator._state.Layer1_MapHeight; set => _emulator._state.Layer1_MapHeight = value; }
        public byte Layer1_MapWidth { get => _emulator._state.Layer1_MapWidth; set => _emulator._state.Layer1_MapWidth = value; }
        public bool Layer1_BitMapMode { get => _emulator._state.Layer1_BitMapMode != 0; set => _emulator._state.Layer1_BitMapMode = (value ? (byte)1 : (byte)0); }
        public byte Layer1_ColourDepth { get => _emulator._state.Layer1_ColourDepth; set => _emulator._state.Layer1_ColourDepth = value; }
        public UInt32 Layer1_MapAddress { get => _emulator._state.Layer1_MapAddress; set => _emulator._state.Layer1_MapAddress = value; }
        public UInt32 Layer1_TileAddress { get => _emulator._state.Layer1_TileAddress; set => _emulator._state.Layer1_TileAddress = value; }
        public byte Layer1_TileHeight { get => _emulator._state.Layer1_TileHeight; set => _emulator._state.Layer1_TileHeight = value; }
        public byte Layer1_TileWidth { get => _emulator._state.Layer1_TileWidth; set => _emulator._state.Layer1_TileWidth = value; }
        public ushort Layer1_HScroll { get => _emulator._state.Layer1_HScroll; set => _emulator._state.Layer1_HScroll = value; }
        public ushort Layer1_VScroll { get => _emulator._state.Layer1_VScroll; set => _emulator._state.Layer1_VScroll = value; }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CpuState
    {
        public ulong MemoryPtr = 0;
        public ulong RomPtr = 0;
        public ulong RamBankPtr = 0;
        public ulong VramPtr = 0;

        public ulong Data0_Address = 0;
        public ulong Data1_Address = 0;
        public ulong Data0_Step = 0;
        public ulong Data1_Step = 0;

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

        public byte AddrSel = 0;
        public byte DcSel = 0;

        public byte Dc_HScale = 0;
        public byte Dc_VScale = 0;
        public byte Dc_Border = 0;
        public byte Dc_HStart = 0;
        public byte Dc_HStop = 0;
        public byte Dc_VStart = 0;
        public byte Dc_VStop = 0;

        public byte SpriteEnable = 0;
        public byte Layer0Enable = 0;
        public byte Layer1Enable = 0;

        // 1 byte of padding

        // layer 0
        public UInt32 Layer0_MapAddress = 0;
        public UInt32 Layer0_TileAddress = 0;
        public ushort Layer0_HScroll = 0;
        public ushort Layer0_VScroll = 0;
        public byte Layer0_MapHeight = 0;
        public byte Layer0_MapWidth = 0;
        public byte Layer0_BitMapMode = 0;
        public byte Layer0_ColourDepth = 0;
        public byte Layer0_TileHeight = 0;
        public byte Layer0_TileWidth = 0;

        // 2 bytes of padding

        // layer 1
        public UInt32 Layer1_MapAddress = 0;
        public UInt32 Layer1_TileAddress = 0;
        public ushort Layer1_HScroll = 0;
        public ushort Layer1_VScroll = 0;
        public byte Layer1_MapHeight = 0;
        public byte Layer1_MapWidth = 0;
        public byte Layer1_BitMapMode = 0;
        public byte Layer1_ColourDepth = 0;
        public byte Layer1_TileHeight = 0;
        public byte Layer1_TileWidth = 0;

        public unsafe CpuState(ulong memory, ulong rom, ulong ramBank, ulong vram)
        {
            MemoryPtr = memory;
            RomPtr = rom;
            RamBankPtr = ramBank;
            VramPtr = vram;
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


    public byte A { get => _state.A; set => _state.A = value; }
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

    public VeraState Vera => new VeraState(this);

    private readonly ulong _memory_ptr;
    private readonly ulong _rom_ptr;
    private readonly ulong _ram_ptr;
    private readonly ulong _vram_ptr;

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

        _state = new CpuState(_memory_ptr, _rom_ptr, _ram_ptr, _vram_ptr);


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
        NativeMemory.Free((void*)_vram_ptr);
    }
}