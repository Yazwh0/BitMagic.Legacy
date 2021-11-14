using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Common
{
    public interface IMemory
    {
        int Length { get; }
        byte GetByte(int address);
        void SetByte(int address, byte value);
        byte GetDebugByte(int address);
        void SetDebugByte(int address, byte value);
    }
}
