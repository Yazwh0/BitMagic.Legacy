using BitMagic.Common;
using BitMagic.Cpu;
using BitMagic.Emulator.Gl;
using BitMagic.Machines;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace BitMagic.Emulation
{
    public class Emulator
    {
        private IMachineEmulator _machine { get; }
        private Project _project;

        public IMachineEmulator Machine => _machine;

        public Emulator(Project project)
        {
            if (project.MachineEmulator == null)
                throw new ArgumentException(nameof(project.Machine));

            _machine = project.MachineEmulator;
            _project = project;
        }

        public Emulator(CompileResult result, Machine machine, byte[] Rom)
        {
            _machine = MachineFactory.GetMachine(machine) as IMachineEmulator ?? throw new Exception($"{machine} is not an IMachineEmulator");
            _machine.SetRom(Rom);
            _machine.Build();

            _project = new Project();
            _project.OutputFile.Contents = result.Data["Main"].ToArray();
            _project.RomFile.Contents = Rom;
            _project.Machine = _machine;
        }

        public void LoadPrg()
        {
            if (_project.OutputFile.Contents == null)
                throw new ArgumentNullException(nameof(_project.OutputFile.Contents));

            if (!string.IsNullOrWhiteSpace(_project.OutputFile.Filename) && !_project.OutputFile.Filename.EndsWith(".prg", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Output filename must be .prg if loading into the emulator.");
            }

            LoadPrg(_project.OutputFile.Contents);
        }

        public void LoadPrg(byte[] data)
        {
            var address = data[0] + (data[1] << 8);

            for (var i = 2; i < data.Length; i++)
            {
                _machine.Memory.SetByte(address++, data[i]);
            }
        }

        public IMachineRunner Emulate(bool headless = false, Func<IMachineRunner, bool>? exitCheck = null)
        {
            _machine.Cpu.Reset();

            var machineRunner = new MachineRunner(_machine.Cpu.Frequency, CpuFunc, _machine.Display, _machine.Cpu, exitCheck);

            if (_project.MachineEmulator == null)
                throw new NullReferenceException("MachineEmulator is null");

            machineRunner.Start();

            if (!headless)
                EmulatorWindow.Run(_project.MachineEmulator.Display);
            else
                machineRunner.WaitForCompletion();

            return machineRunner;
        }

        internal void CpuFunc(object? r)
        {
            if (r is not MachineRunner runner)
                throw new ArgumentException("r is not a machine runner.");

            Stopwatch timer = new Stopwatch();
            timer.Start();
            int frames = 0;

            var targetTicks = 0;
            var frameDone = false;
            var totalTicks = 0;
            bool releaseVideo = true;
            bool debugging = (_project.Options.VerboseDebugging & ApplicationPart.Emulator) > 0;

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            while (true)
            {
                var ticks = 0;

                if (_machine.Cpu.HasInterrupt)
                    _machine.Cpu.HandleInterrupt(_machine.Memory);

                while (ticks < targetTicks)
                {
                    var thisTicks = _machine.Cpu.ClockTick(_machine.Memory, debugging);
                    ticks += thisTicks;
                    runner.CpuTicks += thisTicks;

                    if (runner.ExitCheck != null)
                    {
                        if (runner.ExitCheck(runner))
                            return;
                    }
                }

                if (releaseVideo)
                {
                    runner.PreRender();

                    for (var i = 0; i < runner.DisplayEvents.Length; i++)
                    {
                        runner.Display.DisplayHold[i] = false;
                        //runner.DisplayStart[i].Set();
                    }
                    while (runner.Display.DisplayHold.Any(i => i == false)) { }
                    //WaitHandle.WaitAll(runner.DisplayEvents);
                }

                if (frameDone)
                {
                    // wait until the stopwatch is at 1/60th of a second.

/*                    while (stopWatch.ElapsedMilliseconds < 16)
                    {
                    }

                    stopWatch.Reset();*/

                    totalTicks += runner.CpuTicks;
                    runner.CpuTicks = 0;
                    // trigger image upload and wait for next frame
                    EmulatorWindow.SetRequireUpdate();
                    frames++;
                    if (frames == 60)
                    {
                        frames = 0;
                        Console.WriteLine($"{timer.Elapsed:s\\.fff}s - {totalTicks / 60} ticks");
                        timer.Restart();
                        totalTicks = 0;
                    }
                }

                (frameDone, targetTicks, releaseVideo) = runner.IncrementDisplay();
            }
        }
    }
}
