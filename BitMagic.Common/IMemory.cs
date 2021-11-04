using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Common
{
    public interface IMemory
    {
        public int Length { get; }
        public byte GetByte(int address);
        void SetByte(int address, byte value);
        public byte GetDebugByte(int address);
        void SetDebugByte(int address, byte value);
    }
}
