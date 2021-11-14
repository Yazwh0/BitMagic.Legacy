using BitMagic.Common;
using System;
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
            var ticks = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (ticks < _machine.Cpu.Frequency)
            {
                ticks += _machine.Cpu.ClockTick(_machine.Memory, (_project.Options.VerboseDebugging & ApplicationPart.Emulator) > 0);
            }

            stopwatch.Stop();
            Console.Write($"{stopwatch.Elapsed:s\\.fffff}s");
        }
    }
}
