using BitMagic.Common;
using BitMagic.Cpu;
using BitMagic.Emulator.Gl;
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

        public Emulator(Project project)
        {
            if (project.MachineEnumalator == null)
                throw new ArgumentException(nameof(project.Machine));

            _machine = project.MachineEnumalator;
            _project = project;
        }

        public void LoadPrg()
        {
            if (_project.OutputFile.Contents == null)
                throw new ArgumentNullException(nameof(_project.OutputFile.Contents));

            var address = _project.OutputFile.Contents[0] + (_project.OutputFile.Contents[1] << 8);

            if (!string.IsNullOrWhiteSpace(_project.OutputFile.Filename) && !_project.OutputFile.Filename.EndsWith(".prg", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Output filename must be .prg if loading into the emulator.");
            }

            for (var i = 2; i < _project.OutputFile.Contents.Length; i++)
            {
                _machine.Memory.SetByte(address++, _project.OutputFile.Contents[i]);
            }
        }

        public void Emulate()
        {
            _machine.Cpu.Reset();

            var machineRunner = new MachineRunner(_machine.Cpu.Frequency, CpuFunc, _machine.Display, _machine.Cpu);

            if (_project.MachineEnumalator == null)
                throw new NullReferenceException("MachineEmulator is null");

            machineRunner.Start();

            EmulatorWindow.Run(_project.MachineEnumalator.Display);

            machineRunner.Stop();
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
                    ticks += _machine.Cpu.ClockTick(_machine.Memory, debugging);
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

                runner.CpuTicks += ticks;

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
