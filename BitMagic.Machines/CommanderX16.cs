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

        public IDisplay Display => Vera;
        public Vera Vera { get; }

        public CommanderX16(byte[] rom)
        {
            var banks = new List<IMemoryBlockMap>();

            for (var i = 0; i < 0x100; i++)
            {
                banks.Add(new MemoryMap(0, 0x2000, new IMemoryBlock[] { new Ram($"RAM Bank {i}", 0x2000) }));
            }

            var roms = new List<IMemoryBlockMap>();

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
}
