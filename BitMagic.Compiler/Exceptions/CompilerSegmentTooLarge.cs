namespace BitMagic.Compiler.Exceptions
{
    public class CompilerSegmentTooLarge : CompilerException
    {
        internal Segment Segment { get; }

        internal CompilerSegmentTooLarge(Segment segment) : base("Segment too large.")
        {
            Segment = segment;
        }

        public override string ErrorDetail => $"Maxsize is ${Segment.MaxSize:X4}, but the segment is ${Segment.Address - Segment.StartAddress:X4}";
    }

}
