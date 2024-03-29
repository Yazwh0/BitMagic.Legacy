﻿using BitMagic.Machines;
using BitMagic.Cpu;
using BitMagic.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.Compiler.Tests
{
    public static class CommanderX16Test
    {
        public static async Task<CommanderX16R39> Emulate(string code, Func<IMachineRunner, bool>? exitCheck, Func<IMachineRunner, bool> failCheck)
        {
            var compiler = new Compiler(code);
            var emulator = new Emulation.Emulator(await compiler.Compile(), Machine.CommanderX16R40, await File.ReadAllBytesAsync("D:\\Documents\\Source\\BitMagic\\rom\\rom.bin"));
            emulator.LoadPrg();

            var runner = emulator.Emulate(true, exitCheck); // todo add timeout

            if (failCheck(runner))
                Assert.Fail("Fail Check is true");

            return (emulator.Machine as CommanderX16R39)!;
        }

        public static Task<CommanderX16R39> UntilStp(string code)
        {
            return Emulate(code, m => (m.Cpu as WDC65c02)!.LastOpCode == 0xdb || m.CpuTicks > 10000, m => m.CpuTicks > 10000); // stp
        }
    }
}