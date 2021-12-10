using BitMagic.Common;
using System.Collections.Generic;
using System.Linq;

namespace BitMagic.Cpu.Memory
{
    public class Banked : NormalMemory
    {
        private IMemoryBlockMap _currentBank;
        private IMemoryBlockMap[] _banks;
        private int _index;

        public Banked(string name, int length, IEnumerable<IMemoryBlockMap> banks) : base(name, length)
        {
            _index = 0;
            _banks = banks.ToArray();
            _currentBank = _banks[0];            
        }

        public override void Init(IMemoryBlockMap memory, int startAddress)
        {
            base.Init(memory, startAddress);

            for (var i = 0; i < Length; i++)
            {
                Memory!.Memory[StartAddress + i] = _currentBank.Memory[i];
                Memory!.ReadNotification[StartAddress + i] = _currentBank.ReadNotification[i];
                Memory!.WriteNotification[StartAddress + i] = _currentBank.WriteNotification[i];
            }
        }

        public int BankIndex { 
            get => _index;
            set { 
                _index = value;
                _currentBank = _banks[0];
            }
        }

        public void Switch(int address, byte value)
        {
            Memory!.Memory[address] = value;

            _index = value;
            _currentBank = _banks[_index];
            
            // todo: block copy
            for(var i = 0; i < Length; i++)
            {
                Memory!.Memory[StartAddress + i] = _currentBank.Memory[i];
                Memory!.ReadNotification[StartAddress + i] = _currentBank.ReadNotification[i];
                Memory!.WriteNotification[StartAddress + i] = _currentBank.WriteNotification[i];
            }
        }
    }
}
