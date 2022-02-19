using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BitMagic.Common
{
    public interface IMachineRunner
    {
       // Task SignalAndWait();
        int CpuTicks { get; } // number of cpu ticks this frame
        double CpuFrequency { get; }
        (bool framedone, int nextCpuTick, bool releaseVideo) IncrementDisplay();
        AutoResetEvent[] DisplayEvents { get; }
        AutoResetEvent[] DisplayStart { get; }
        ICpuEmulator Cpu { get; }
    }
}
