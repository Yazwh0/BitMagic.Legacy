
using BitMagic.Compiler;
using BitMagic.X16Emulator;
using CommandLine;
using System.Text;
using System.Transactions;
using static X16E.Program.AddressModes;
using Thread = System.Threading.Thread;

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

        [Option('w', "write", Required = false, HelpText = "Write the result of the compilation.")]
        public bool WritePrg { get; set; } = false;

        [Option("warp", Required = false, HelpText = "Run as fast as possible.")]
        public bool Warp { get; set; } = false;

        [Option('s', "sdcard", Required = false, HelpText = "SD Card to attach.")]
        public string? SdCardFileName { get; set; }

        [Option('d', "sdcard-source", Required = false, HelpText = "Folder to mount as a SD Card.")]
        public string? SdCardSource { get; set; }

        [Option("sdcard-write", Required = false, HelpText = "SD Card file to write at the end of emulation.")]
        public string? SdCardWrite { get; set; }

        [Option("sdcard-overwrite", Required = false, HelpText = "When writing the SD Card file, it can overwrite.")]
        public bool SdCardOverrwrite { get; set; } = false;

        //[Option('m', "autorun", Required = false, HelpText = "Automatically run at startup. Ignored if address is specified. NOT YET IMPLEMENTED")]
        public bool AutoRun { get; set; } = false;
    }

    static async Task<int> Main(string[] args)
    {
        Console.WriteLine("BitMagic - X16E");

        var emulator = new Emulator();

        var argumentsResult = Parser.Default.ParseArguments<Options>(args);

        var options = argumentsResult.Value;

        if (options == null)
        {
            return 1;
        }

        if (options.WritePrg && string.IsNullOrWhiteSpace(options.CodeFilename))
        {
            Console.WriteLine("Cannot have write the result of compilation if no codefile is set.");
            return 1;
        }

        if (!string.IsNullOrWhiteSpace(options.PrgFilename) && !string.IsNullOrWhiteSpace(options.CodeFilename) && !options.WritePrg)
        {
            Console.WriteLine("Cannot have both a prg file and code file set when not outputing the result of compilation");
            return 1;
        }

        var prgLoaded = false;
        if (!string.IsNullOrWhiteSpace(options.PrgFilename) && string.IsNullOrWhiteSpace(options.CodeFilename) && !options.WritePrg)
        {
            if (File.Exists(options.PrgFilename))
            {
                Console.WriteLine($"Loading '{options.PrgFilename}'.");
                var prgData = await File.ReadAllBytesAsync(options.PrgFilename);
                int destAddress = (prgData[1] << 8) + prgData[0];
                for (var i = 2; i < prgData.Length; i++)
                {
                    emulator.Memory[destAddress++] = prgData[i];
                }
                prgLoaded = true;
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
                Console.WriteLine($"Compiling '{options.CodeFilename}'.");
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

                    if (options.WritePrg)
                    {
                        Console.WriteLine($"Writing to '{options.PrgFilename}'.");
                        await File.WriteAllBytesAsync(options.PrgFilename, prg);
                    }
                    prgLoaded = true;
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

        if (options.StartAddress != 0 && prgLoaded)
            emulator.Pc = options.StartAddress;
        else
        {
            emulator.Pc = (ushort)((emulator.RomBank[0x3ffd] << 8) + emulator.RomBank[0x3ffc]);
            if (options.AutoRun)
            {
                emulator.SmcBuffer.KeyDown(Silk.NET.Input.Key.R);
                emulator.SmcBuffer.KeyUp(Silk.NET.Input.Key.R);
                emulator.SmcBuffer.KeyDown(Silk.NET.Input.Key.U);
                emulator.SmcBuffer.KeyUp(Silk.NET.Input.Key.U);
                emulator.SmcBuffer.KeyDown(Silk.NET.Input.Key.N);
                emulator.SmcBuffer.KeyUp(Silk.NET.Input.Key.N);
                emulator.SmcBuffer.KeyDown(Silk.NET.Input.Key.Enter);
                emulator.SmcBuffer.KeyUp(Silk.NET.Input.Key.Enter);
            }
        }

        emulator.Control = Control.Paused; // window load start the emulator

        if (options.Warp)
            emulator.FrameControl = FrameControl.Run;
        else
            emulator.FrameControl = FrameControl.Synced;

        emulator.Brk_Causes_Stop = true;

        emulator.LoadSdCard(new SdCard());

        // create the sdcard
        if (!string.IsNullOrWhiteSpace(options.SdCardSource))
            emulator.SdCard!.AddDirectory(options.SdCardSource);



        // currently need this to run
        //emulator.SmcBuffer.KeyDown(Silk.NET.Input.Key.Enter);
        //emulator.SmcBuffer.KeyUp(Silk.NET.Input.Key.Enter);

        EmulatorWork.Emulator = emulator;
        EmulatorThread = new Thread(EmulatorWork.DoWork);

        EmulatorThread.Priority = System.Threading.ThreadPriority.Highest;
        EmulatorThread.Start();

        EmulatorWindow.Run(emulator);

        EmulatorThread.Join();

        Console.WriteLine($"Emulator finished with return '{EmulatorWork.Return}'.");

        // once emulation is over write sdcard if requested
        if (!string.IsNullOrWhiteSpace(options.SdCardWrite))
            emulator.SdCard!.Save(options.SdCardWrite, options.SdCardOverrwrite);

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

            var done = false;

            while (!done)
            {
                Return = Emulator.Emulate();

                if (Return != Emulator.EmulatorResult.ExitCondition)
                {
                    Console.WriteLine($"Result: {Return}");
                    var history = Emulator.History;
                    var idx = (int)Emulator.HistoryPosition - 1;
                    Console.WriteLine("Last 50 steps:");

                    var toOutput = new List<string>();
                    for (var i = 0; i < 1000; i++)
                    {
                        var opCodeDef = OpCodes.GetOpcode(history[idx].OpCode);
                        var opCode = $"{opCodeDef.OpCode.ToLower()} {AddressModes.GetModeText(opCodeDef.AddressMode, history[idx].Params)}".PadRight(15);

                        toOutput.Add($"Ram:${history[idx].RamBank:X2} Rom:${history[idx].RomBank:X2} ${history[idx].PC:X4} - ${history[idx].OpCode:X2}: {opCode} -> A:${history[idx].A:X2} X:${history[idx].X:X2} Y:${history[idx].Y:X2} SP:${history[idx].SP:X2} {Flags(history[idx].Flags)}");
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

                //Console.WriteLine($"Ram: ${Emulator.Memory[0x00]:X2} Rom: ${Emulator.Memory[0x01]:X2}");
                //for (var i = 0; i < 512; i += 16)
                //{
                //    Console.ForegroundColor = ConsoleColor.White;
                //    Console.Write($"{i:X4}: ");
                //    for (var j = 0; j < 16; j++)
                //    {
                //        if (Emulator.Memory[i + j] != 0)
                //            Console.ForegroundColor = ConsoleColor.White;
                //        else
                //            Console.ForegroundColor = ConsoleColor.DarkGray;
                //        Console.Write($"{Emulator.Memory[i + j]:X2} ");
                //        if (j == 7)
                //            Console.Write(" ");
                //    }
                //    Console.WriteLine();
                //}

                //DisplayMemory(0x800-1, 0xd00 - 0x800);
                //DisplayMemory(0x9f30, 16);

                if (Return == Emulator.EmulatorResult.DebugOpCode)
                {
                    Console.WriteLine("(C)ontinue?");
                    var inp = Console.ReadKey(true);
                    if (inp.Key != ConsoleKey.C)
                    {
                        done = true;
                    }
                }
                else
                    done = true;

            }


            if (Emulator.Control != Control.Stop)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("*** Close emulator window to exit ***");
            }
            Console.ResetColor();
        }

        public static void DisplayMemory(int start, int length)
        {
            Console.WriteLine();
            for (var i = start; i < start + length; i += 16)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"{i:X4}: ");
                for (var j = 0; j < 16; j++)
                {
                    var val = i+j >= 0xa000 ? Emulator.RamBank[i + j - 0xa000]  : Emulator.Memory[i + j];
                    if (val != 0)
                        Console.ForegroundColor = ConsoleColor.White;
                    else
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($"{val:X2} ");
                    if (j == 7)
                        Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
    }

    public static string Flags(byte flags)
    {
        var sb = new StringBuilder();

        sb.Append("[");

        if ((flags & (byte)CpuFlags.Negative) > 0)
            sb.Append("N");
        else
            sb.Append(" ");

        if ((flags & (byte)CpuFlags.Overflow) > 0)
            sb.Append("V");
        else
            sb.Append(" ");

        sb.Append(" "); // unused
        if ((flags & (byte)CpuFlags.Break) > 0)
            sb.Append("B");
        else
            sb.Append(" ");

        if ((flags & (byte)CpuFlags.Decimal) > 0)
            sb.Append("D");
        else
            sb.Append(" ");

        if ((flags & (byte)CpuFlags.InterruptDisable) > 0)
            sb.Append("I");
        else
            sb.Append(" ");

        if ((flags & (byte)CpuFlags.Zero) > 0)
            sb.Append("Z");
        else
            sb.Append(" ");

        if ((flags & (byte)CpuFlags.Carry) > 0)
            sb.Append("C");
        else
            sb.Append(" ");

        sb.Append("]");

        return sb.ToString();
    }

    [Flags]
    public enum CpuFlags : byte
    {
        None = 0,
        Carry = 1,
        Zero = 2,
        InterruptDisable = 4,
        Decimal = 8,
        Break = 16,
        Unused = 32,
        Overflow = 64,
        Negative = 128
    }

    public static class AddressModes
    {
        public enum AddressMode
        {
            Implied,
            Accumulator,
            Immediate,
            Absolute,
            XIndexAbsolute,
            YIndexAbsolute,
            AbsoluteIndirect,
            AbsoluteXIndexIndirect,
            ZeroPage,
            XIndexedZeroPage,
            YIndexedZeroPage,
            ZeroPageIndirect,
            XIndexZeroPageIndirect,
            ZeroPageIndirectYIndexed,
            Relative,
            ZeroPageRelative
        }

        public static string GetModeText(AddressMode addressMode, int value) => addressMode switch
        {
            AddressMode.Implied => "",
            AddressMode.Accumulator => "",
            AddressMode.Immediate => $"#${value & 0xff:X2}",
            AddressMode.Absolute => $"${value:X4}",
            AddressMode.XIndexAbsolute => $"${value:X4}, x",
            AddressMode.YIndexAbsolute => $"${value:X4}, y",
            AddressMode.AbsoluteIndirect => $"(${value:X4})",
            AddressMode.AbsoluteXIndexIndirect => $"(${value:X4}, x)",
            AddressMode.ZeroPage => $"${value & 0xff:X2}",
            AddressMode.XIndexedZeroPage => $"${value & 0xff:X2}, x",
            AddressMode.YIndexedZeroPage => $"${value & 0xff:X2}, y",
            AddressMode.ZeroPageIndirect => $"(${value & 0xff:X2})",
            AddressMode.XIndexZeroPageIndirect => $"(${value & 0xff:X2}, x)",
            AddressMode.ZeroPageIndirectYIndexed => $"(${value & 0xff:X2}), y",
            AddressMode.Relative => $"${value & 0xff:X2}",
            AddressMode.ZeroPageRelative => $"${value & 0xff:X2}",
            _ => "??"
        };
    }

    public static class OpCodes
    {
        public static (string OpCode, AddressModes.AddressMode AddressMode) GetOpcode(int code) => code switch
        {
            0x69 => ("ADC", AddressMode.Immediate),
            0x6D => ("ADC", AddressMode.Absolute),
            0x7D => ("ADC", AddressMode.XIndexAbsolute),
            0x79 => ("ADC", AddressMode.YIndexAbsolute),
            0x65 => ("ADC", AddressMode.ZeroPage),
            0x75 => ("ADC", AddressMode.XIndexedZeroPage),
            0x72 => ("ADC", AddressMode.ZeroPageIndirect),
            0x61 => ("ADC", AddressMode.XIndexZeroPageIndirect),
            0x71 => ("ADC", AddressMode.ZeroPageIndirectYIndexed),
            0x29 => ("AND", AddressMode.Immediate),
            0x2D => ("AND", AddressMode.Absolute),
            0x3D => ("AND", AddressMode.XIndexAbsolute),
            0x39 => ("AND", AddressMode.YIndexAbsolute),
            0x25 => ("AND", AddressMode.ZeroPage),
            0x35 => ("AND", AddressMode.XIndexedZeroPage),
            0x32 => ("AND", AddressMode.ZeroPageIndirect),
            0x21 => ("AND", AddressMode.XIndexZeroPageIndirect),
            0x31 => ("AND", AddressMode.ZeroPageIndirectYIndexed),
            0x0A => ("ASL", AddressMode.Accumulator),
            0x0E => ("ASL", AddressMode.Absolute),
            0x1E => ("ASL", AddressMode.XIndexAbsolute),
            0x6 => ("ASL", AddressMode.ZeroPage),
            0x16 => ("ASL", AddressMode.XIndexedZeroPage),
            0x0F => ("BBR0", AddressMode.ZeroPageRelative),
            0x1F => ("BBR1", AddressMode.ZeroPageRelative),
            0x2F => ("BBR2", AddressMode.ZeroPageRelative),
            0x3F => ("BBR3", AddressMode.ZeroPageRelative),
            0x4F => ("BBR4", AddressMode.ZeroPageRelative),
            0x5F => ("BBR5", AddressMode.ZeroPageRelative),
            0x6F => ("BBR6", AddressMode.ZeroPageRelative),
            0x7F => ("BBR7", AddressMode.ZeroPageRelative),
            0x8F => ("BBS0", AddressMode.ZeroPageRelative),
            0x9F => ("BBS1", AddressMode.ZeroPageRelative),
            0xAF => ("BBS2", AddressMode.ZeroPageRelative),
            0xBF => ("BBS3", AddressMode.ZeroPageRelative),
            0xCF => ("BBS4", AddressMode.ZeroPageRelative),
            0xDF => ("BBS5", AddressMode.ZeroPageRelative),
            0xEF => ("BBS6", AddressMode.ZeroPageRelative),
            0xFF => ("BBS7", AddressMode.ZeroPageRelative),
            0x90 => ("BCC", AddressMode.Relative),
            0xB0 => ("BCS", AddressMode.Relative),
            0xF0 => ("BEQ", AddressMode.Relative),
            0x89 => ("BIT", AddressMode.Immediate),
            0x2C => ("BIT", AddressMode.Absolute),
            0x3C => ("BIT", AddressMode.XIndexAbsolute),
            0x24 => ("BIT", AddressMode.ZeroPage),
            0x34 => ("BIT", AddressMode.XIndexedZeroPage),
            0x30 => ("BMI", AddressMode.Relative),
            0xD0 => ("BNE", AddressMode.Relative),
            0x10 => ("BPL", AddressMode.Relative),
            0x80 => ("BRA", AddressMode.Relative),
            0x0 => ("BRK", AddressMode.Implied),
            0x50 => ("BVC", AddressMode.Relative),
            0x70 => ("BVS", AddressMode.Relative),
            0x18 => ("CLC", AddressMode.Implied),
            0xD8 => ("CLD", AddressMode.Implied),
            0x58 => ("CLI", AddressMode.Implied),
            0xB8 => ("CLV", AddressMode.Implied),
            0xC9 => ("CMP", AddressMode.Immediate),
            0xCD => ("CMP", AddressMode.Absolute),
            0xDD => ("CMP", AddressMode.XIndexAbsolute),
            0xD9 => ("CMP", AddressMode.YIndexAbsolute),
            0xC5 => ("CMP", AddressMode.ZeroPage),
            0xD5 => ("CMP", AddressMode.XIndexedZeroPage),
            0xD2 => ("CMP", AddressMode.ZeroPageIndirect),
            0xC1 => ("CMP", AddressMode.XIndexZeroPageIndirect),
            0xD1 => ("CMP", AddressMode.ZeroPageIndirectYIndexed),
            0xE0 => ("CPX", AddressMode.Immediate),
            0xEC => ("CPX", AddressMode.Absolute),
            0xE4 => ("CPX", AddressMode.ZeroPage),
            0xC0 => ("CPY", AddressMode.Immediate),
            0xCC => ("CPY", AddressMode.Absolute),
            0xC4 => ("CPY", AddressMode.ZeroPage),
            0x3A => ("DEC", AddressMode.Accumulator),
            0xCE => ("DEC", AddressMode.Absolute),
            0xDE => ("DEC", AddressMode.XIndexAbsolute),
            0xC6 => ("DEC", AddressMode.ZeroPage),
            0xD6 => ("DEC", AddressMode.XIndexedZeroPage),
            0xCA => ("DEX", AddressMode.Implied),
            0x88 => ("DEY", AddressMode.Implied),
            0x49 => ("EOR", AddressMode.Immediate),
            0x4D => ("EOR", AddressMode.Absolute),
            0x5D => ("EOR", AddressMode.XIndexAbsolute),
            0x59 => ("EOR", AddressMode.YIndexAbsolute),
            0x45 => ("EOR", AddressMode.ZeroPage),
            0x55 => ("EOR", AddressMode.XIndexedZeroPage),
            0x52 => ("EOR", AddressMode.ZeroPageIndirect),
            0x41 => ("EOR", AddressMode.XIndexZeroPageIndirect),
            0x51 => ("EOR", AddressMode.ZeroPageIndirectYIndexed),
            0x1A => ("INC", AddressMode.Accumulator),
            0xEE => ("INC", AddressMode.Absolute),
            0xFE => ("INC", AddressMode.XIndexAbsolute),
            0xE6 => ("INC", AddressMode.ZeroPage),
            0xF6 => ("INC", AddressMode.XIndexedZeroPage),
            0xE8 => ("INX", AddressMode.Implied),
            0xC8 => ("INY", AddressMode.Implied),
            0x4C => ("JMP", AddressMode.Absolute),
            0x6C => ("JMP", AddressMode.AbsoluteIndirect),
            0x7C => ("JMP", AddressMode.AbsoluteXIndexIndirect),
            0x20 => ("JSR", AddressMode.Absolute),
            0xA9 => ("LDA", AddressMode.Immediate),
            0xAD => ("LDA", AddressMode.Absolute),
            0xBD => ("LDA", AddressMode.XIndexAbsolute),
            0xB9 => ("LDA", AddressMode.YIndexAbsolute),
            0xA5 => ("LDA", AddressMode.ZeroPage),
            0xB5 => ("LDA", AddressMode.XIndexedZeroPage),
            0xB2 => ("LDA", AddressMode.ZeroPageIndirect),
            0xA1 => ("LDA", AddressMode.XIndexZeroPageIndirect),
            0xB1 => ("LDA", AddressMode.ZeroPageIndirectYIndexed),
            0xA2 => ("LDX", AddressMode.Immediate),
            0xAE => ("LDX", AddressMode.Absolute),
            0xBE => ("LDX", AddressMode.YIndexAbsolute),
            0xA6 => ("LDX", AddressMode.ZeroPage),
            0xB6 => ("LDX", AddressMode.YIndexedZeroPage),
            0xA0 => ("LDY", AddressMode.Immediate),
            0xAC => ("LDY", AddressMode.Absolute),
            0xBC => ("LDY", AddressMode.XIndexAbsolute),
            0xA4 => ("LDY", AddressMode.ZeroPage),
            0xB4 => ("LDY", AddressMode.XIndexedZeroPage),
            0x4A => ("LSR", AddressMode.Accumulator),
            0x4E => ("LSR", AddressMode.Absolute),
            0x5E => ("LSR", AddressMode.XIndexAbsolute),
            0x46 => ("LSR", AddressMode.ZeroPage),
            0x56 => ("LSR", AddressMode.XIndexedZeroPage),
            0xEA => ("NOP", AddressMode.Implied),
            0x9 => ("ORA", AddressMode.Immediate),
            0x0D => ("ORA", AddressMode.Absolute),
            0x1D => ("ORA", AddressMode.XIndexAbsolute),
            0x19 => ("ORA", AddressMode.YIndexAbsolute),
            0x5 => ("ORA", AddressMode.ZeroPage),
            0x15 => ("ORA", AddressMode.XIndexedZeroPage),
            0x12 => ("ORA", AddressMode.ZeroPageIndirect),
            0x1 => ("ORA", AddressMode.XIndexZeroPageIndirect),
            0x11 => ("ORA", AddressMode.ZeroPageIndirectYIndexed),
            0x48 => ("PHA", AddressMode.Implied),
            0x8 => ("PHP", AddressMode.Implied),
            0xDA => ("PHX", AddressMode.Implied),
            0x5A => ("PHY", AddressMode.Implied),
            0x68 => ("PLA", AddressMode.Implied),
            0x28 => ("PLP", AddressMode.Implied),
            0xFA => ("PLX", AddressMode.Implied),
            0x7A => ("PLY", AddressMode.Implied),
            0x7 => ("RMB0", AddressMode.ZeroPage),
            0x17 => ("RMB1", AddressMode.ZeroPage),
            0x27 => ("RMB2", AddressMode.ZeroPage),
            0x37 => ("RMB3", AddressMode.ZeroPage),
            0x47 => ("RMB4", AddressMode.ZeroPage),
            0x57 => ("RMB5", AddressMode.ZeroPage),
            0x67 => ("RMB6", AddressMode.ZeroPage),
            0x77 => ("RMB7", AddressMode.ZeroPage),
            0x2A => ("ROL", AddressMode.Accumulator),
            0x2E => ("ROL", AddressMode.Absolute),
            0x3E => ("ROL", AddressMode.XIndexAbsolute),
            0x26 => ("ROL", AddressMode.ZeroPage),
            0x36 => ("ROL", AddressMode.XIndexedZeroPage),
            0x6A => ("ROR", AddressMode.Accumulator),
            0x6E => ("ROR", AddressMode.Absolute),
            0x7E => ("ROR", AddressMode.XIndexAbsolute),
            0x66 => ("ROR", AddressMode.ZeroPage),
            0x76 => ("ROR", AddressMode.XIndexedZeroPage),
            0x40 => ("RTI", AddressMode.Implied),
            0x60 => ("RTS", AddressMode.Implied),
            0xE9 => ("SBC", AddressMode.Immediate),
            0xED => ("SBC", AddressMode.Absolute),
            0xFD => ("SBC", AddressMode.XIndexAbsolute),
            0xF9 => ("SBC", AddressMode.YIndexAbsolute),
            0xE5 => ("SBC", AddressMode.ZeroPage),
            0xF5 => ("SBC", AddressMode.XIndexedZeroPage),
            0xF2 => ("SBC", AddressMode.ZeroPageIndirect),
            0xE1 => ("SBC", AddressMode.XIndexZeroPageIndirect),
            0xF1 => ("SBC", AddressMode.ZeroPageIndirectYIndexed),
            0x38 => ("SEC", AddressMode.Implied),
            0xF8 => ("SED", AddressMode.Implied),
            0x78 => ("SEI", AddressMode.Implied),
            0x87 => ("SMB0", AddressMode.ZeroPage),
            0x97 => ("SMB1", AddressMode.ZeroPage),
            0xA7 => ("SMB2", AddressMode.ZeroPage),
            0xB7 => ("SMB3", AddressMode.ZeroPage),
            0xC7 => ("SMB4", AddressMode.ZeroPage),
            0xD7 => ("SMB5", AddressMode.ZeroPage),
            0xE7 => ("SMB6", AddressMode.ZeroPage),
            0xF7 => ("SMB7", AddressMode.ZeroPage),
            0x8D => ("STA", AddressMode.Absolute),
            0x9D => ("STA", AddressMode.XIndexAbsolute),
            0x99 => ("STA", AddressMode.YIndexAbsolute),
            0x85 => ("STA", AddressMode.ZeroPage),
            0x95 => ("STA", AddressMode.XIndexedZeroPage),
            0x92 => ("STA", AddressMode.ZeroPageIndirect),
            0x81 => ("STA", AddressMode.XIndexZeroPageIndirect),
            0x91 => ("STA", AddressMode.ZeroPageIndirectYIndexed),
            0xDB => ("STP", AddressMode.Implied),
            0x8E => ("STX", AddressMode.Absolute),
            0x86 => ("STX", AddressMode.ZeroPage),
            0x96 => ("STX", AddressMode.YIndexedZeroPage),
            0x8C => ("STY", AddressMode.Absolute),
            0x84 => ("STY", AddressMode.ZeroPage),
            0x94 => ("STY", AddressMode.XIndexedZeroPage),
            0x9C => ("STZ", AddressMode.Absolute),
            0x9E => ("STZ", AddressMode.XIndexAbsolute),
            0x64 => ("STZ", AddressMode.ZeroPage),
            0x74 => ("STZ", AddressMode.XIndexedZeroPage),
            0xAA => ("TAX", AddressMode.Implied),
            0xA8 => ("TAY", AddressMode.Implied),
            0x1C => ("TRB", AddressMode.Absolute),
            0x14 => ("TRB", AddressMode.ZeroPage),
            0x0C => ("TSB", AddressMode.Absolute),
            0x4 => ("TSB", AddressMode.ZeroPage),
            0xBA => ("TSX", AddressMode.Implied),
            0x8A => ("TXA", AddressMode.Implied),
            0x9A => ("TXS", AddressMode.Implied),
            0x98 => ("TYA", AddressMode.Implied),
            0xCB => ("WAI", AddressMode.Implied),
            _ => ("", (AddressModes.AddressMode)(-1))
        };
    }
}
