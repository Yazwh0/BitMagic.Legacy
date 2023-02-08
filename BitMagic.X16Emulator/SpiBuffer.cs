using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BitMagic.X16Emulator.Emulator;

namespace BitMagic.X16Emulator
{
    public class SpiBuffer
    {
        private bool _running = true;

        public unsafe void RunSpiDisplay(ref CpuState state)
        {
            var lastPosition = state.SpiPosition;
            var buffer = new Span<byte>((void*)state.SpiHistoryPtr, 1024 * 2);
            while (_running || false)
            {
                var thisPosition = state.SpiPosition;

                if (thisPosition != lastPosition)
                {
                    while (thisPosition != lastPosition)
                    {
                        var val = buffer[(int)lastPosition];
                        switch (buffer[(int)lastPosition + 1])
                        {
                            case 0:
                                Console.WriteLine($"SPI  >> {val:X2}");
                                break;
                            case 1:
                                Console.Write($"{val:X2}, ");
                                //Console.WriteLine($"SPI  << {val:X2}");
                                break;
                            case 2:
                                Console.WriteLine($"CTRL >> Select: {val & 01:0} Slow Clock : {val >> 1 & 01:0} ");
                                break;
                        }

                        lastPosition += 2;
                        if (lastPosition >= 2048)
                            lastPosition = 0;
                    }
                }
                else
                    Thread.Sleep(1);
            }
        }

        public void Stop()
        {
            _running = false;
        }
    }
}
