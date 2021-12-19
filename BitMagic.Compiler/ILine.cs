using System.Collections.Generic;

namespace BitMagic.Compiler
{
    public interface ILine
    {
        byte[] Data { get; }
        string OriginalText { get; }
        int LineNumber { get; }
        int Address { get; }
        bool RequiresReval { get; }
        List<string> RequiresRevalNames { get; }
        void ProcessParts(bool finalParse);
        void WriteToConsole();
    }
}
