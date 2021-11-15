using BitMagic.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BitMagic.Cpu
{
    public abstract class CpuOpCode : ICpuOpCode
    {
        internal abstract List<(byte OpCode, AccessMode Mode, int Timing)> OpCodes { get; }
        public virtual string Code => GetType().Name.ToUpper();
        public byte GetOpCode(AccessMode mode) => OpCodes.First(i => i.Mode == mode).OpCode;
        public abstract int Process(byte opCode, Func<(byte value, int timing, ushort pcStep)> GetValueAtPC, Func<(ushort address, int timing, ushort pcStep)> GetAddressAtPc, IMemory memory, I6502 cpu);
        public IEnumerable<AccessMode> Modes => OpCodes.Select(i => i.Mode);
    }
}
