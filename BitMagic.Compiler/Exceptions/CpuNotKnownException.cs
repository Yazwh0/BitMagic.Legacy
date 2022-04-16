using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Compiler.Exceptions
{
    public class CpuNotKnownException : CompilerException
    {
        public string CpuName { get; }

        public CpuNotKnownException(string name) : base("Cpu not known.")
        {
            CpuName = name;
        }

        public override string ErrorDetail => $"Unknown Cpu '{CpuName}'.";
    }
}
