﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitMagic.X16Emulator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BitMagic.X16Emulator.Tests
{
    internal static class X16TestHelper
    {
        public static async Task<Emulator> Emulate(string code, Emulator? emulator = null, bool dontChangeEmulatorOptions = false,  bool brkExpected = false)
        {
            var compiler = new Compiler.Compiler(code);

            emulator ??= new Emulator();

            if (!dontChangeEmulatorOptions)
            {
                emulator.Brk_Causes_Stop = true;
            }

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
            Console.WriteLine();
            Console.WriteLine($"D0 Adr:\t${emulator.Vera.Data0_Address:X5} (step ${emulator.Vera.Data0_Step:X2})");
            Console.WriteLine($"D1 Adr:\t${emulator.Vera.Data1_Address:X5} (step ${emulator.Vera.Data1_Step:X2})");
            Console.WriteLine();
            Console.WriteLine($"Beam:\t{emulator.Vera.Beam_X}, {emulator.Vera.Beam_Y} ({emulator.Vera.Beam_Position})");
            Console.WriteLine();

            if (emulateResult != Emulator.EmulatorResult.DebugOpCode && 
                (!brkExpected || emulateResult != Emulator.EmulatorResult.BrkHit))
                Assert.Fail($"Emulate Result is not from a stp. {emulateResult}");

            return emulator;
        }

        public static void AssertState(this Emulator emulator, byte? A = null, byte? X = null, byte? Y = null, ushort? Pc = null, ulong? Clock = null, uint? stackPointer = null)
        {
            if (A != null)
                Assert.AreEqual(A, emulator.A, $"A doesn't match: ${emulator.A:X2}");   

            if (X != null)
                Assert.AreEqual(X, emulator.X, $"X doesn't match: ${emulator.X:X2}");

            if (Y != null)
                Assert.AreEqual(Y, emulator.Y, $"Y doesn't match: ${emulator.Y:X2}");

            if (Pc != null)
                Assert.AreEqual(Pc, emulator.Pc, $"PC doesn't match: ${emulator.Pc:X4}");

            if (Clock != null)
                Assert.AreEqual(Clock, emulator.Clock - 3, $"Clock doesn't match: ${emulator.Clock - 3:X4}"); // add on stp clock cycles

            if (stackPointer != null)
                Assert.AreEqual(stackPointer, emulator.StackPointer, $"SP doesn't match: ${emulator.StackPointer:X4}");
        }

        public static void AssertFlags(this Emulator emulator, bool Zero = false, bool Negative = false, bool Overflow = false, bool Carry = false, bool InterruptDisable = false, bool Decimal = false, bool Interrupt = false, bool Nmi = false)
        {
            Assert.AreEqual(Zero, emulator.Zero, "Zero flag doesn't match");
            Assert.AreEqual(Negative, emulator.Negative, "Negative flag doesn't match");
            Assert.AreEqual(Overflow, emulator.Overflow, "Overflow flag doesn't match");
            Assert.AreEqual(Carry, emulator.Carry, "Carry flag doesn't match");
            Assert.AreEqual(InterruptDisable, emulator.InterruptDisable, "Interrupt Disable flag doesn't match");
            Assert.AreEqual(Decimal, emulator.Decimal, "Decimal flag doesn't match");
            Assert.AreEqual(Interrupt, emulator.Interrupt, "Interrupt doesn't match");
            Assert.AreEqual(Nmi, emulator.Nmi, "Nmi doesn't match");
        }

        public static void SaveDisplay(this Emulator emulator, string filename)
        {
            using var image = new Image<Rgba32>(800, 640 * 6);

            var pixels = emulator.Display;

            var i = 0;
            for (var l = 0; l < 6; l++)
            {
                for (var y = 0; y < 525; y++)
                {
                    for (var x = 0; x < 800; x++)
                    {
                        var pixel = pixels[i++];
                        image[x, y + l * 525] = new Rgba32 { R = pixel.R, G = pixel.G, B = pixel.B, A = pixel.A };
                    }
                }
            }

            using var fs = new FileStream(filename, FileMode.Create);
            image.SaveAsPng(fs);
            fs.Flush();
            fs.Close();
        }

        public static void CompareImage(this Emulator emulator, string filename)
        {
            SaveDisplay(emulator, filename + ".actual.png");

            using var image = Image.Load(filename).CloneAs<Rgba32>();

            var pixels = emulator.Display;

            var i = 0;
            for (var l = 0; l < 6; l++)
            {
                for (var y = 0; y < 525; y++)
                {
                    for (var x = 0; x < 800; x++)
                    {
                        var actual = pixels[i++];
                        var expectedOriginal = image[x, y + l * 525];
                        var expected = new Common.PixelRgba { R = expectedOriginal.R, G = expectedOriginal.G, B = expectedOriginal.B, A = expectedOriginal.A };
                        Assert.AreEqual(expected, actual, $"At {x},{y} on display layer {l}.");
                    }
                }
            }
        }
    }
}
