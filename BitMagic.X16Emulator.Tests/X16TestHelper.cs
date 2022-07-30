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

            Console.WriteLine($"A:   \t${emulator.A:X2}");
            Console.WriteLine($"X:   \t${emulator.X:X2}");
            Console.WriteLine($"Y:   \t${emulator.Y:X2}");
            Console.WriteLine($"PC:  \t${emulator.Pc:X4}");
            Console.WriteLine($"SP:  \t${emulator.StackPointer:X4}");

            Console.WriteLine($"Ticks:\t${emulator.Clock:X4}");

            Console.Write("Flags:\t[");
            Console.Write(emulator.Negative ? "N" : " ");
            Console.Write(emulator.Overflow ? "V" : " ");
            Console.Write(" ");
            Console.Write(emulator.BreakFlag ? "B" : " ");
            Console.Write(emulator.Decimal ? "D" : " ");
            Console.Write(emulator.InterruptDisable ? "I" : " ");
            Console.Write(emulator.Zero ? "Z" : " ");
            Console.Write(emulator.Carry ? "C]" : " ]");
            Console.WriteLine();
            Console.WriteLine($"Speed:\t{emulator.Clock / ts.TotalSeconds / 1000000.0 :0.00}Mhz");

            if (emulateResult != Emulator.EmulatorResult.DebugOpCode)
                Assert.Fail($"Emulate Result is not from a stp. {emulateResult}");

            return emulator;
        }

        public static void AssertState(this Emulator emulator, byte? A = null, byte? X = null, byte? Y = null, Int32? Pc = null, ulong? Clock = null, uint? stackPointer = null)
        {
            if (A != null)
                Assert.AreEqual(A, (byte)emulator.A, $"A doesn't match: ${(byte)emulator.A:X2}");   

            if (X != null)
                Assert.AreEqual(X, (byte)emulator.X, $"X doesn't match: ${(byte)emulator.X:X2}");

            if (Y != null)
                Assert.AreEqual(Y, (byte)emulator.Y, $"Y doesn't match: ${(byte)emulator.Y:X2}");

            if (Pc != null)
                Assert.AreEqual(Pc, (Int16)emulator.Pc, $"PC doesn't match: ${(byte)emulator.Pc:X4}");

            if (Clock != null)
                Assert.AreEqual(Clock, emulator.Clock - 3, $"Clock doesn't match: ${(byte)emulator.Clock - 3:X4}"); // add on stp clock cycles

            if (stackPointer != null)
                Assert.AreEqual(stackPointer, emulator.StackPointer, $"SP doesn't match: ${(byte)emulator.StackPointer:X4}");
        }

        public static void AssertFlags(this Emulator emulator, bool Zero = false, bool Negative = false, bool Overflow = false, bool Carry = false, bool InterruptDisable = false, bool Decimal = false)
        {
            Assert.AreEqual(Zero, emulator.Zero, "Zero flag doesn't match");
            Assert.AreEqual(Negative, emulator.Negative, "Negative flag doesn't match");
            Assert.AreEqual(Overflow, emulator.Overflow, "Overflow flag doesn't match");
            Assert.AreEqual(Carry, emulator.Carry, "Carry flag doesn't match");
            Assert.AreEqual(InterruptDisable, emulator.InterruptDisable, "Interrupt Disable flag doesn't match");
            Assert.AreEqual(Decimal, emulator.Decimal, "Decimal flag doesn't match");
        }
    }
}
