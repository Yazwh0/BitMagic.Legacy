using BitMagic.Common;
using BitMagic.Emulator.Gl;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BitMagic.Emulation
{
    internal class MachineRunner : IMachineRunner
    {
        public class Worker
        {
            public Action<object?> WorkerAction { get; }

            public Thread? WorkerThread { get; set; }
            public string Name { get; }

            public Worker(Action<object?> action, string name)
            {
                WorkerAction = action;
                Name = name;
            }

            public void Start(MachineRunner parent)
            {
                WorkerThread = new Thread(() => { WorkerAction(parent); });
                WorkerThread.Name = Name;
                WorkerThread.Priority = ThreadPriority.Highest;
                WorkerThread.IsBackground = true;

                WorkerThread.Start();
            }
        }

        public Worker[] DisplayWorkers { get; } 
        public Worker CpuWorker { get; }

        public AutoResetEvent[] DisplayEvents { get; }
        public AutoResetEvent[] DisplayStart { get; }

        public Thread? Controller { get; internal set;  }
        public IDisplay Display { get; set; }

        public int CpuTicks { get; internal set; }

        public double CpuFrequency { get; init; }

        public ICpu Cpu { get; }

        public MachineRunner(double deltaHtz, Action<object?> cpuThread, IDisplay display, ICpu cpu)
        {
            CpuFrequency = deltaHtz;
            Display = display;
            Cpu = cpu;
            int j = 0;

            DisplayEvents = new AutoResetEvent[Display.DisplayThreads.Length];
            DisplayStart = new AutoResetEvent[Display.DisplayThreads.Length];

            for (var i = 0; i < DisplayEvents.Length; i++)
            {
                DisplayEvents[i] = new AutoResetEvent(false);
                DisplayStart[i] = new AutoResetEvent(false);
            }

            DisplayWorkers = Display.DisplayThreads.Select(i => new Worker(i, $"display {j++}")).ToArray();

            CpuWorker = new Worker(cpuThread, "cpu");
        }

        public void Start()
        {
            CpuWorker.Start(this);

            foreach (var worker in DisplayWorkers)
                worker.Start(this);

        }

        public void Stop()
        {
            // ??
        }

        public (bool framedone, int nextCpuTick, bool releaseVideo) IncrementDisplay() => Display.IncrementDisplay(this);
    }
}
