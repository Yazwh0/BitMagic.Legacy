using BitMagic.Common;
using BitMagic.Cpu;
using BitMagic.Cpu._6502;
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

        public CommanderX16()
        {
            var banks = new List<IMemory>();

            for (var i = 0; i < 0x100; i++)
            {
                banks.Add(new Ram(0x2000));
            }

            var rom = new List<IMemory>();

            for (var i = 0; i < 8; i++)
            {
                rom.Add(new Rom(0x4000));
            }

            Memory = new MemoryMap(0x10000, new IMemory[] {
                new Ram(0x9f00), // Fixed RAM
                new Ram(0x100), // IO
                new Banked(0x2000, banks), // Banked Ram
                new Banked(0x4000, rom)
            });

            Cpu = new WDC65c02();
        }
    }
}
