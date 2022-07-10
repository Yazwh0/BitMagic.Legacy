using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BigMagic_Emulator
{
    public enum EmulatorReturn
    {
        ExitCondition,
        UnknownOpCode,
        DebugOpCode
    }

    internal class Emulator
    {
        [DllImport(@"..\..\..\..\x64\Debug\EmulatorCode.dll")]
        private static extern int fnEmulatorCode(byte[] memory, ref State state);

        [StructLayout(LayoutKind.Sequential)]
        private struct State
        {
            int A;
            int X;
            int Y;
            int Pc;
            int Clock;
            bool Carry;
            bool Zero;
            bool interruptDisable;
            bool Decimal;
            bool BreakFlag;
            bool Overflow;
            bool Negative;
        }

        public EmulatorReturn Tests()
        {
            var memory = new byte[Int16.MaxValue];
            var state = new State();

            var returnCode = fnEmulatorCode(memory, ref state);

            return (EmulatorReturn)returnCode;
        }
    }
}
