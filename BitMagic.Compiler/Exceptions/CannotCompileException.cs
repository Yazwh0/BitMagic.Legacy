using BitMagic.Common;
using BitMagic.Compiler.Exceptions;

namespace BitMagic.Compiler.Exceptions
{
    public class CannotCompileException : CompilerException
    {
        public IOutputData Line { get; }

        public CannotCompileException(IOutputData line, string message) : base(message)
        {
            Line = line;
        }

        public override string ErrorDetail => Line.Source.ToString();
    }

}
