using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitMagic.X16Emulator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests
{
    internal static class X16TestHelper
    {
        public static async Task<Emulator> Emulate(string code, Emulator? emulator = null)
        {
            var compiler = new Compiler.Compiler(code);

            emulator ??= new Emulator();

            var compileResult = await compiler.Compile();

            var prg = compileResult.Data["Main"].ToArray();

            int address = 0x801;
            for (var i = 2; i < prg.Length; i++) // 2 byte header
                emulator.Memory[address++] = prg[i];

            emulator.Pc = 0x810;

            var emulateResult = emulator.Emulate();

            if (emulateResult != Emulator.EmulatorResult.DebugOpCode)
                Assert.Fail($"Emulate Result is not from a stp. {emulateResult}");

            return emulator;
        }

        public static void AssertState(this Emulator emulator, byte? A = null, byte? X = null, byte? Y = null, Int32? Pc = null, int? Clock = null)
        {
            if (A != null)
                Assert.AreEqual(A, (byte)emulator.A);

            if (X != null)
                Assert.AreEqual(X, (byte)emulator.X);

            if (Y != null)
                Assert.AreEqual(Y, (byte)emulator.Y);

            if (Pc != null)
                Assert.AreEqual(Pc, (Int16)emulator.Pc);

            if (Clock != null)
                Assert.AreEqual(Clock, emulator.Clock - 3); // add on stp clock cycles
        }

        public static void AssertFlags(this Emulator emulator, bool? Zero = null, bool? Negative = null, bool? Overflow = null, bool? Carry = null)
        {
            if (Zero != null)
                Assert.AreEqual(Zero, emulator.Zero);

            if (Negative != null)
                Assert.AreEqual(Negative, emulator.Negative);

            if (Overflow != null)
                Assert.AreEqual(Overflow, emulator.Overflow);

            if (Carry != null)
                Assert.AreEqual(Carry, emulator.Carry);
        }
    }
}
