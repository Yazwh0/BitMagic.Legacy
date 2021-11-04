using BitMagic.Common;
using System.Collections.Generic;
using System.Linq;

namespace BitMagic.Cpu.Memory
{
    public class Banked : IMemory
    {
        public int Length { get; }

        private IMemory _currentBank;
        private IMemory[] _banks;
        private int _index;

        public Banked(int size, IEnumerable<IMemory> banks)
        {
            _index = 0;
            Length = size;
            _banks = banks.ToArray();
            _currentBank = _banks[0];            
        }

        public int BankIndex { 
            get => _index;
            set { 
                _index = value;
                _currentBank = _banks[0];
            }
        }

        public byte GetByte(int address) => _currentBank.GetByte(address);
        public void SetByte(int address, byte value) => _currentBank.SetByte(address, value);
        public byte GetDebugByte(int address) => GetByte(address);
        public void SetDebugByte(int address, byte value) => SetByte(address, value);
    }
}
