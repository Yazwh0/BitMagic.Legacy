using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Compiler
{
    public record SourceFilePosition
    {
        public string Name = "";
        public int LineNumber;
        public string Source = "";

        public override string ToString() => $"{Name}:{LineNumber}\n{Source}";
    }
}
