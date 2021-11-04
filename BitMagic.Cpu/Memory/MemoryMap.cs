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
        private readonly int[] _startAddress;

        public MemoryMap(int size, IEnumerable<IMemory> blocks)
        {
            Length = size;
            _memoryMap = blocks.ToArray();
            _startAddress = new int[_memoryMap.Length];
            var pos = 0;

            for(var i = 0; i < _memoryMap.Length; i++)
            {
                _startAddress[i] = pos;
                pos += _memoryMap[i].Length;
            }

            if (pos != size)
                throw new Exception($"Size {size} is different to blocks provided {pos}");
        }

        public byte GetByte(int address)
        {
            int pos = 0;
            for(var i = 1; i < _memoryMap.Length; i++)
            {
                if (_startAddress[i] < address)
                {
                    return _memoryMap[i - 1].GetByte(address - pos);
                }
                pos += _startAddress[i];
            }

            return _memoryMap[_memoryMap.Length - 1].GetByte(address - pos);
        }

        public void SetByte(int address, byte value)
        {
            int pos = 0;
            for (var i = 1; i < _memoryMap.Length; i++)
            {
                if (_startAddress[i] < address)
                {
                    _memoryMap[i - 1].SetByte(address - pos, value);
                    return;
                }
                pos += _startAddress[i];
            }

            _memoryMap[_memoryMap.Length - 1].SetByte(address - pos, value);
        }

        public byte GetDebugByte(int address) => GetByte(address);
        public void SetDebugByte(int address, byte value) => SetByte(address, value);
    }
}
