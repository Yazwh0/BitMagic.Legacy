using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var emulateResult = emulator.Emulate();

            stopWatch.Stop();

            var ts = stopWatch.Elapsed;

            Console.WriteLine($"Time:\t{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{(ts.Milliseconds / 10):00}");

            Console.WriteLine($"A:\t${emulator.A:X2}");
            Console.WriteLine($"X:\t${emulator.X:X2}");
            Console.WriteLine($"Y:\t${emulator.Y:X2}");
            Console.WriteLine($"PC:\t${emulator.Pc:X4}");
            Console.WriteLine($"SP:\t${emulator.StackPointer:X4}");

            Console.WriteLine($"Ticks:\t${emulator.Clock:X4}");

            if (emulateResult != Emulator.EmulatorResult.DebugOpCode)
                Assert.Fail($"Emulate Result is not from a stp. {emulateResult}");

            return emulator;
        }

        public static void AssertState(this Emulator emulator, byte? A = null, byte? X = null, byte? Y = null, Int32? Pc = null, ulong? Clock = null, uint? stackPointer = null)
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

            if (stackPointer != null)
                Assert.AreEqual(stackPointer, emulator.StackPointer);
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
