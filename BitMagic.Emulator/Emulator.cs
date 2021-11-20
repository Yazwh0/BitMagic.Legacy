using BitMagic.Common;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;

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

            var machineRunner = new MachineRunner(_project.Machine.Cpu.Frequency);

            machineRunner.SetCpu(CpuFunc);
            machineRunner.SetDisplay(_project.Machine.Display);

            machineRunner.Start();

            machineRunner.MainWindow.Run(_project.Machine.Display);

            machineRunner.Stop();
        }

        internal async void CpuFunc(object? r)
        {
            var runner = r as MachineRunner;

            if (runner == null)
                throw new ArgumentException("r is not a machine runner.");

            int ticks = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (true || ticks < _machine.Cpu.Frequency)
            {
                ticks += _machine.Cpu.ClockTick(_machine.Memory, (_project.Options.VerboseDebugging & ApplicationPart.Emulator) > 0);

                await runner.Latch.ControlComplete();
            }

            stopwatch.Stop();
            Console.Write($"Running for: {stopwatch.Elapsed:s\\.fffff}s");
        }
    }
}
