namespace BitMagic.Compiler.Exceptions
{
    public class CompilerFileNotFound : CompilerException
    {
        public string Filename { get; }

        public CompilerFileNotFound(string filename) : base("File not found.")
        {
            Filename = filename;
        }

        public override string ErrorDetail => $"'{Filename}'";
    }

}
