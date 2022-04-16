using BitMagic.Common;

namespace BitMagic.Compiler.Exceptions
{
    public class CompilerLineException : CompilerException
    {
        public IOutputData Line { get; }

        public CompilerLineException(IOutputData line, string message) : base(message)
        {
            Line = line;
        }

        public override string ErrorDetail => Line.Source.ToString();
    }

}
