using BitMagic.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Machines
{
    public enum Machine
    {
        CommanderX16R38,
        CommanderX16R39
    }

    public static class MachineFactory
    {
        public static IMachineEmulator? GetMachine(string name)
        {
            var machine = Enum.Parse<Machine>(name);
            return GetMachine(machine);
        }

        public static IMachineEmulator? GetMachine(Machine machine) => machine switch
        {
            Machine.CommanderX16R38 => new CommanderX16R38(),
            Machine.CommanderX16R39 => new CommanderX16R39(),
            _ => null
        };
    }

    public class NoMachine : IMachine
    {
        public string Name => "NoMachine";

        public int Version => 0;

        public ICpu Cpu => new NoCpu();
    }

    public class NoCpu : ICpu
    {
        public string Name => "NoCpu";

        public IEnumerable<ICpuOpCode> OpCodes => Array.Empty<ICpuOpCode>();
    }
}
