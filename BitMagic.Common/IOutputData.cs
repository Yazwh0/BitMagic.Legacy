using System.Collections.Generic;

namespace BitMagic.Common
{
    public interface IOutputData
    {        
        byte[] Data { get; }
        //string OriginalText { get; }
        //int LineNumber { get; }
        int Address { get; }
        bool RequiresReval { get; }
        List<string> RequiresRevalNames { get; }
        void ProcessParts(bool finalParse);
        void WriteToConsole();
        SourceFilePosition Source { get; }
    }

    public record SourceFilePosition
    {
        public string Name = "";
        public int LineNumber;
        public string Source = "";

        public override string ToString() => $"{Name}:{LineNumber}\n{Source}";
    }
}
