using BitMagic.Common;

namespace BitMagic.Compiler.Exceptions
{
    public class CompilerVerbException : CompilerSourceException
    {
        public CompilerVerbException(SourceFilePosition source, string message) : base(source, message)
        {
        }
    }

}
