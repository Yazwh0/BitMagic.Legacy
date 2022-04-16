using BitMagic.Common;
using BitMagic.Compiler.Exceptions;

namespace BitMagic.Compiler.Exceptions
{
    public class UnknownSymbolException : CompilerException
    {
        public IOutputData Line { get; }

        public UnknownSymbolException(IOutputData line, string message) : base(message)
        {
            Line = line;
        }

        public override string ErrorDetail => Line.Source.ToString();
    }

}
