using BitMagic.Common;
using BitMagic.Cpu.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Cpu
{
    public enum Cpu
    {
        WDC65c02
    }

    public static class CpuFactory
    {
        public static ICpu? GetCpu(string name)
        {
            var cpu = Enum.Parse<Cpu>(name);
            return GetCpu(cpu);
        }

        public static ICpu? GetCpu(Cpu cpu) => cpu switch
        {
            Cpu.WDC65c02 => new WDC65c02(new MemoryMap(0, 0x10000, new[] { new Ram("Memory", 0x10000) }), 8000000),
            _ => null
        };
    }
}
