using BitMagic.Common;
using BitMagic.Cpu;
using BitMagic.Emulator.Gl;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace BitMagic.Emulation
{
    public class Emulator
    {
        private IMachine _machine { get; }
        private Project _project;

        public Emulator(Project project)
        {
            _machine = project.Machine;
            _project = project;
        }

        public void LoadPrg()
        {
            if (_project.ProgFile.Contents == null)
                throw new ArgumentNullException(nameof(_project.ProgFile.Contents));

            var address = _project.ProgFile.Contents[0] + (_project.ProgFile.Contents[1] << 8);

            for (var i = 2; i < _project.ProgFile.Contents.Length; i++)
            {
                _machine.Memory.SetByte(address++, _project.ProgFile.Contents[i]);
            }
        }

        public void Emulate(int startAddress)
        {
            _machine.Cpu.SetProgramCounter(startAddress);

            // Hack until we have a kernel
            var cpu = _machine.Cpu as WDC65c02;
            cpu.InterruptAddress = 0xfffe;

            var machineRunner = new MachineRunner(_project.Machine.Cpu.Frequency, CpuFunc, _project.Machine.Display, _project.Machine.Cpu);

            machineRunner.Start();

            EmulatorWindow.Run(_project.Machine.Display);

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

            while (true)
            {
                var ticks = 0;

                if (_machine.Cpu.HasInterrupt)
                    _machine.Cpu.HandleInterrupt(_machine.Memory);

                while (ticks < targetTicks)
                {
                    ticks += _machine.Cpu.ClockTick(_machine.Memory, (_project.Options.VerboseDebugging & ApplicationPart.Emulator) > 0);
                }

                if (releaseVideo)
                {
                    for (var i = 0; i < runner.DisplayEvents.Length; i++)
                    {
                        runner.DisplayStart[i].Set();
                    }

                    WaitHandle.WaitAll(runner.DisplayEvents);
                }

                runner.CpuTicks += ticks;

                if (frameDone)
                {
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
