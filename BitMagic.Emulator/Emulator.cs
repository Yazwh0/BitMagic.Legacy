using BitMagic.Common;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
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
            CreateWindow();

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

        public void CreateWindow()
        {
            var window = Window.Create(WindowOptions.Default);
            GL? gl = null;

            window.Size = new Silk.NET.Maths.Vector2D<int> { X = 640*2, Y = 480*2 };
            window.Title = "BitMagic!";
            window.WindowBorder = WindowBorder.Fixed;

            window.Load += () =>
            {
                gl = window.CreateOpenGL();
                gl.Viewport(window.Size);
            };

            window.Render += delta =>
            {
                if (gl == null) throw new ArgumentNullException(nameof(gl));

                gl.ClearColor(0, 0, .1f, 1);
                gl.Clear(ClearBufferMask.ColorBufferBit);
            };

            window.Closing += () =>
            {
                gl?.Dispose();
            };

            window.Run();

        }
    }
}
