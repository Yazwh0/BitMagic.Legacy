using BitMagic.Common;
using BitMagic.Compiler.Exceptions;

namespace BitMagic.Compiler.Exceptions
{
    public class CompilerBranchToFarException : CompilerException
    {
        public IOutputData Line { get; }

        public CompilerBranchToFarException(IOutputData line, string message) : base(message)
        {
            Line = line;
        }

        public override string ErrorDetail => Line.Source.ToString();
    }

}
