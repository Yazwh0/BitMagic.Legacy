using System.Collections.Generic;

namespace BitMagic.Common
{
    public interface IMachine
    {
        string Name { get; }
        int Version { get; }
        ICpu Cpu { get; }
    }

    public interface IMachineEmulator : IMachine
    {
        IMemory Memory { get; }
        new ICpuEmulator Cpu { get; }
        IDisplay Display { get; }
        IVariables Variables { get; }
        void SetRom(byte[] rom);
        void Build();
        bool Initialised { get; }
    }

    public interface IVariables
    {
        IReadOnlyDictionary<string, int> Values { get; }
    }
}
