using BitMagic.Common;

namespace BitMagic.Compiler.Exceptions
{
    public class CompilerUnknownOpcode : CompilerSourceException
    {
        public CompilerUnknownOpcode(SourceFilePosition source, string message) : base(source, message)
        {
        }
    }

}
