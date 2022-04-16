using BitMagic.Common;

namespace BitMagic.Compiler.Exceptions
{
    public abstract class CompilerSourceException : CompilerException
    {
        public SourceFilePosition SourceFile { get; }

        public CompilerSourceException(SourceFilePosition source, string message) : base(message)
        {
            SourceFile = source;
        }

        public override string ErrorDetail => SourceFile.ToString();
    }

}
