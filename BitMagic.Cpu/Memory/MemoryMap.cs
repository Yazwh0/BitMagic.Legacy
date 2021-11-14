using BitMagic.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BitMagic.Cpu.Memory
{
    public class MemoryMap : IMemory
    {
        public int Length { get; }
        private readonly IMemory[] _memoryMap;

        private readonly (IMemory Memory, int Offset)[] _lookup;

        public MemoryMap(int size, IEnumerable<IMemory> blocks)
        {
            Length = size;
            _memoryMap = blocks.ToArray();
            var pos = 0;

            for(var i = 0; i < _memoryMap.Length; i++)
            {   pos += _memoryMap[i].Length;
            }

            if (pos != size)
                throw new Exception($"Size {size} is different to blocks provided {pos}");

            _lookup = new (IMemory, int)[size];
            var idx = 0;
            var current = _memoryMap[idx];
            pos = 0;
            for(var i = 0; i < size; i++)
            {
                _lookup[i] = (current, pos++);
                if (pos > current.Length)
                {
                    pos = 0;
                    idx++;
                    current = _memoryMap[idx];
                }
            }
        }

        public byte GetByte(int address)
        {
            var (memory, offset) = _lookup[address];
            return memory.GetByte(offset);
        }

        public void SetByte(int address, byte value)
        {
            var (memory, offset) = _lookup[address];
            memory.SetByte(offset, value);
            return;
        }

        public byte GetDebugByte(int address) => GetByte(address);
        public void SetDebugByte(int address, byte value) => SetByte(address, value);
    }
}
