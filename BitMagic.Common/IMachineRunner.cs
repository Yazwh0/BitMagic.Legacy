using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Common
{
    public interface IMachineRunner
    {
        Task SignalAndWait();
        int TickDelta { get; } // number of cp
        double DeltaFrequency { get; }
    }
}
