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

        public CommanderX16()
        {
            var banks = new List<IMemory>();

            for (var i = 0; i < 0x100; i++)
            {
                banks.Add(new Ram(0x2000));
            }

            var roms = new List<IMemory>();

            for (var i = 0; i < 8; i++)
            {
                roms.Add(new Rom(0x4000));
            }

            var stack = new Ram(0x100);

            var bankedRam = new Banked(0x2000, banks);
            var bankedRom = new Banked(0x4000, roms);
            Vera = new Vera();

            Memory = new MemoryMap(0x10000, new IMemory[] {
                new Ram(0x100),             // ZP           $0
                stack,                      // Stack        $100
                new Ram(0x9d00),            // Fixed RAM    $200
                new Ram(0x20),              // IO - VIA 0/1 $9f00
                Vera,                       // Vera         $9f20
                new Ram(0x02),              // YM 2151      $9f40
                new Ram(0x1e),              // Reserved     $9f42
                new Ram(0xa0),              // Expansion    $9f60
                bankedRam,                  // Banked Ram   $a000
                bankedRom                   // ROM          $c000
            }, new [] {
                (0x9F61, bankedRam.GetSwitch(), 0),
                (0x9F60, bankedRom.GetSwitch(), 0)
            });

            Cpu = new WDC65c02(stack, 8000000);
        }
    }
}
