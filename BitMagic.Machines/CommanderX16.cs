using BitMagic.Common;
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

            var stack = new Ram(0x100);

            Memory = new MemoryMap(0x10000, new IMemory[] {
                new Ram(0x100), // ZP
                stack,
                new Ram(0x9d00), // Fixed RAM
                new Ram(0x100), // IO
                new Banked(0x2000, banks), // Banked Ram
                new Banked(0x4000, rom)
            });

            Cpu = new WDC65c02(stack, 8000000);
        }
    }
}
