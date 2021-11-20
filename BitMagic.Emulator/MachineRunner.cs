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
        internal class Worker
        {
            public Action<object?> WorkerAction { get; }
            public Task? WorkerTask { get; set; }

            public Worker(Action<object?> action)
            {
                WorkerAction = action;
            }

            public void Start(MachineRunner parent)
            {
                WorkerTask = Task.Factory.StartNew(WorkerAction, parent, parent.TokenSource.Token, TaskCreationOptions.LongRunning | TaskCreationOptions.RunContinuationsAsynchronously, TaskScheduler.Default);
            }
        }

        public Worker[]? DisplayWorkers { get; set; } = null;
        public Worker? CpuWorker { get; set; } = null;

        public AsyncBarrierControl Latch { get; private set; } = new AsyncBarrierControl(1);

        internal CancellationTokenSource TokenSource { get; } = new CancellationTokenSource();

        public EmulatorWindow MainWindow { get; } = new EmulatorWindow();
        public uint TimeIndex { get; set; }

        public IDisplay? Display { get; set; }

        public int TickDelta { get; internal set; }

        public double DeltaFrequency { get; init; }

        public MachineRunner(double deltaHtz)
        {
            DeltaFrequency = deltaHtz;
        }

        public void SetCpu(Action<object?> work)
        {
            CpuWorker = new Worker(work);
        }

        public void SetDisplay(IDisplay display)
        {
            Display = display;
            DisplayWorkers = Display.DisplayThreads.Select(i => new Worker(i)).ToArray();
            Latch = new AsyncBarrierControl(DisplayWorkers.Length);
        }

        public void Start()
        {
            if (CpuWorker == null) throw new ArgumentException(nameof(CpuWorker));
            if (DisplayWorkers == null) throw new ArgumentException(nameof(DisplayWorkers));

            CpuWorker.Start(this);

            foreach (var worker in DisplayWorkers)
                worker.Start(this);
        }

        public void Stop()
        {
            TokenSource.Cancel();
        }

        public Task SignalAndWait() => Latch.SignalAndWait();
    }

}
