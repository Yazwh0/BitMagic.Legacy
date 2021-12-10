using BitMagic.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BitMagic.Cpu.Memory
{
    public class MemoryMap : IMemory, IMemoryBlockMap
    {
        public int StartAddress { get; }
        public int Length { get; }
        private readonly IMemoryBlock[] _memoryMap;

        //private readonly (IMemory Memory, int Offset)[] _lookup;

        public Memory<byte> Memory { get; }

        public readonly byte[] _memory;
        public Func<int, byte>[] _readNotification;
        public Action<int, byte>[] _writeNotification;

        byte[] IMemoryBlockMap.Memory => _memory;
        public Func<int, byte>[] ReadNotification => _readNotification;
        public Action<int, byte>[] WriteNotification => _writeNotification;


        public MemoryMap(int startAddress, int size, IEnumerable<IMemoryBlock> blocks, IEnumerable<(int address, Action<int, byte>)>? writeOverrides = null )
        {
            Length = size;

            _memory = new byte[size];
            _readNotification = new Func<int, byte>[size];
            _writeNotification = new Action<int, byte>[size];

            _memoryMap = blocks.ToArray();
            var pos = startAddress;

            for(var i = 0; i < _memoryMap.Length; i++)
            {
                _memoryMap[i].Init(this, pos);
                pos += _memoryMap[i].Length;
            }

            if (pos - startAddress != size)
                throw new Exception($"Size {size} is different to blocks provided {pos}");

            Memory = new Memory<byte>(_memory);
        }

        public byte GetByte(int address) {
            if (_readNotification[address] == null)
                return _memory[address];

            var value = _readNotification[address](address);
            _memory[address] = value;
            return value;
        }

        public void SetByte(int address, byte value)
        {
            if (_writeNotification[address] == null)
            {
                _memory[address] = value;
                return;
            }

            _writeNotification[address](address, value); // if there is a notification, it will update the memory
        }

        public byte PeekByte(int address) => _memory[address];
    }
}
