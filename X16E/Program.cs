﻿
using BitMagic.Compiler;
using BitMagic.X16Emulator;
using CommandLine;

namespace X16E;

static class Program
{
    private static Thread? EmulatorThread;

    public class Options
    {
        [Option('p', "prg", Required = false, HelpText = ".prg file to load.")]
        public string PrgFilename { get; set; } = "";

        [Option('r', "rom", Required = false, HelpText = ".rom file to load, will look for rom.bin otherwise.")]
        public string RomFilename { get; set; } = "rom.bin";

        [Option('a', "address", Required = false, HelpText = "Start address.")]
        public ushort StartAddress { get; set; } = 0x810;

        [Option('c', "code", Required = false, HelpText = "Code file to compile. Result will be loaded at 0x801.")]
        public string CodeFilename { get; set; } = "";
    }

    static async Task<int> Main(string[] args)
    {
        Console.WriteLine("BitMagic - X16E");

        var emulator = new Emulator();

        var argumentsResult = Parser.Default.ParseArguments<Options>(args);

        var options = argumentsResult.Value;

        if (options == null)
        {
            Console.WriteLine("Args could not be parsed.");
            return 1;
        }

        if (!string.IsNullOrWhiteSpace(options.PrgFilename) && !string.IsNullOrWhiteSpace(options.CodeFilename))
        {
            Console.WriteLine("Cannot have both a prg file and code file set.");
            return 1;
        }

        if (!string.IsNullOrWhiteSpace(options.PrgFilename))
        {
            if (File.Exists(options.PrgFilename))
            {
                Console.WriteLine($"Loading '{options.PrgFilename}'.");
                var prgData = await File.ReadAllBytesAsync(options.PrgFilename);
                int destAddress = (prgData[1]) << 8 + prgData[0];
                for (var i = 2; i < prgData.Length; i++)
                {
                    emulator.Memory[destAddress++] = prgData[i];
                }
            }
            else
            {
                Console.WriteLine($"PRG '{options.PrgFilename}' not found.");
                return 2;
            }
        }

        if (!string.IsNullOrWhiteSpace(options.CodeFilename))
        {
            if (File.Exists(options.CodeFilename))
            {
                var code = await File.ReadAllTextAsync(options.CodeFilename);
                var compiler = new Compiler(code);
                try
                {
                    var compileResult = await compiler.Compile();

                    if (compileResult.Warnings.Any())
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Warnings:");
                        foreach (var warning in compileResult.Warnings)
                        {
                            Console.WriteLine(warning);
                        }
                        Console.ResetColor();
                    }

                    var prg = compileResult.Data["Main"].ToArray();
                    var destAddress = 0x801;
                    for (var i = 2; i < prg.Length; i++)
                    {
                        emulator.Memory[destAddress++] = prg[i];
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Compile Error:");
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                    return 2;
                }
            } 
            else
            {
                Console.WriteLine($"Code file '{options.PrgFilename}' not found.");
                return 2;
            }
        }

        if (File.Exists(options.RomFilename))
        {
            Console.WriteLine($"Loading '{options.RomFilename}'.");
            var romData = await File.ReadAllBytesAsync(options.RomFilename);
            for (var i = 0; i < romData.Length; i++)
            {
                emulator.RomBank[i] = romData[i];
            }
        }
        else
        {
            Console.WriteLine($"ROM '{options.RomFilename}' not found.");
            return 2;
        }

        emulator.Pc = options.StartAddress;

        EmulatorWork.Emulator = emulator;
        EmulatorThread = new Thread(EmulatorWork.DoWork);

        EmulatorThread.Priority = ThreadPriority.Highest;
        EmulatorThread.Start();

        EmulatorWindow.Run(emulator);

        EmulatorThread.Join();

        Console.WriteLine($"Emulator finished with {EmulatorWork.Return}");

        return 0;
    }

    public static class EmulatorWork
    {
        public static Emulator.EmulatorResult Return { get; set; }
        public static Emulator? Emulator { get; set; }

        public static void DoWork()
        {
            if (Emulator == null)
                throw new ArgumentNullException(nameof(Emulator), "Emulator is null");

            Return = Emulator.Emulate();

            if (Return != Emulator.EmulatorResult.ExitCondition)
            {
                Console.WriteLine($"Result: {Return}");
                var history = Emulator.History;
                var idx = (int)Emulator.HistoryPosition - 1;
                Console.WriteLine("Last 50 steps:");

                var toOutput = new List<string>();
                for (var i = 0; i < 50; i++)
                {
                    toOutput.Add($"${history[idx].PC:X4} - ${history[idx].OpCode:X2} - A:${history[idx].A:X2} X:${history[idx].X:X2} Y:${history[idx].Y:X2}");
                    if (idx <= 0)
                        idx = 1024;
                    idx--;
                }
                toOutput.Reverse();
                foreach (var l in toOutput)
                {
                    Console.WriteLine(l);
                }
            }
        }
    }
}
