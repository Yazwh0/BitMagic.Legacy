﻿using BitMagic.Common;
using BitMagic.Cpu;
using BitMagic.Cpu.Memory;
using System;
using System.Collections.Generic;

namespace BitMagic.Machines
{
    public class CommanderX16 : IMachine
    {
        private IMemory Memory { get; }

        public ICpu Cpu { get; internal set; }

        public string Name => "Commander X16 R38";

        IMemory IMachine.Memory => Memory;

        public IDisplay Display => Vera;
        public Vera Vera { get; }

        private CommanderX16Defaults _defaultVariables = new();
        public IVariables Variables => _defaultVariables;

        public CommanderX16(byte[] rom)
        {
            var banks = new List<IMemory>();

            for (var i = 0; i < 0x100; i++)
            {
                banks.Add(new MemoryMap(0, 0x2000, new IMemoryBlock[] { new Ram($"RAM Bank {i}", 0x2000) }));
            }

            var roms = new List<IMemory>();

            for (var i = 0; i < 8; i++)
            {
                roms.Add(new MemoryMap(0, 0x4000, new IMemoryBlock[] { new Rom($"ROM Bank {i}", 0x4000, i == 0 ? rom : null) }));
            }

            var stack = new Ram("Stack", 0x100);
            var bankedRam = new Banked("RAM Bank", 0x2000, banks);
            var bankedRom = new Banked("ROM Bank", 0x4000, roms);
            Vera = new Vera();

            Memory = new MemoryMap(0, 0x10000, new IMemoryBlock[] {
                new Ram("ZP", 0x100),               // ZP           $0
                stack,                              // Stack        $100
                new Ram("Fixed Ram", 0x9d00),       // Fixed RAM    $200
                new Ram("IO", 0x20),                // IO - VIA 0/1 $9f00
                Vera,                               // Vera         $9f20
                new Ram("YM 1251", 0x02),           // YM 2151      $9f40
                new Ram("Reserved IO", 0x1e),       // Reserved     $9f42
                new Ram("Expansion IO", 0xa0),      // Expansion    $9f60
                bankedRam,                          // Banked Ram   $a000
                bankedRom                           // ROM          $c000
            }, new (int, Action<int, byte>)[] {
                (0x9F61, bankedRam.Switch),
                (0x9F60, bankedRom.Switch)
            });

            Cpu = new WDC65c02(Memory, 8000000);
        }
    }

    internal class CommanderX16Defaults : IVariables
    {
        private static Dictionary<string, int> _defaults = new Dictionary<string, int>
        {
            {"ADDRx_L", 0x9F20 },
            {"ADDRx_M", 0x9F21},
            {"ADDRx_H", 0x9F22},
            {"DATA0", 0x9F23},
            {"DATA1", 0x9F24},
            {"CTRL", 0x9F25},
            {"IEN", 0x9F26},
            {"ISR", 0x9F27},
            {"IRQLINE_L", 0x9F28},
            {"DC_VIDEO", 0x9F29},
            {"DC_HSCALE", 0x9F2A},
            {"DC_VSCALE", 0x9F2B},
            {"DC_BORDER", 0x9F2C},
            {"DC_HSTART", 0x9F29},
            {"DC_HSTOP", 0x9F2A},
            {"DC_VSTART", 0x9F2B},
            {"DC_VSTOP", 0x9F2C},
            {"L0_CONFIG", 0x9F2D},
            {"L0_MAPBASE", 0x9F2E},
            {"L0_TILEBASE", 0x9F2F},
            {"L0_HSCROLL_L", 0x9F30},
            {"L0_HSCROLL_H", 0x9F31},
            {"L0_VSCROLL_L", 0x9F32},
            {"L0_VSCROLL_H", 0x9F33},
            {"L1_CONFIG", 0x9F34},
            {"L1_MAPBASE", 0x9F35},
            {"L1_TILEBASE", 0x9F36},
            {"L1_HSCROLL_L", 0x9F37},
            {"L1_HSCROLL_H", 0x9F38},
            {"L1_VSCROLL_L", 0x9F39},
            {"L1_VSCROLL_H", 0x9F3A},
            {"AUDIO_CTRL", 0x9F3B},
            {"AUDIO_RATE", 0x9F3C},
            {"AUDIO_DATA", 0x9F3D},
            {"SPI_DATA", 0x9F3E},
            {"SPI_CTRL", 0x9F3F},

            {"INTERUPT", 0x0314},
            {"INTERUPT_L", 0x0314},
            {"INTERUPT_H", 0x0315},

            {"RAM_BANK", 0x9F61}
        };

        public IReadOnlyDictionary<string, int> Values => _defaults;
    }
}
