using BitMagic.Common;

namespace BitMagic.Compiler.Warnings
{
    public class UnmatchedEndProcWarning : CompilerWarning
    {
        public int LineNumber { get; }
        public string FileName { get; }
        public UnmatchedEndProcWarning(SourceFilePosition source)
        {
            LineNumber = source.LineNumber;
            FileName = source.Name;
        }
        public override string ToString() => $"Unmateched endproc on line {LineNumber} in file '{FileName}'.";
    }

    public class EndProcOnAnonymousWarning : CompilerWarning
    {
        public int LineNumber { get; }
        public string FileName { get; }
        public EndProcOnAnonymousWarning(SourceFilePosition source)
        {
            LineNumber = source.LineNumber;
            FileName = source.Name;
        }
        public override string ToString() => $"Endproc trying to end system scope block. Too many endprocs? on line {LineNumber} in file '{FileName}'.";
    }
}
