using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BitMagic.Common
{
    public class CompileResult
    {
        public string[] Warnings { get; init; }
        public Dictionary<string, NamedStream> Data { get; init; }
        public Project Project { get; set; }

        public CompileResult(IEnumerable<string> warnings, Dictionary<string, NamedStream> result, Project project)
        { 
            Warnings = warnings.ToArray();
            Data = result;
            Project = project;
        }
    }

    public class NamedStream : MemoryStream
    {
        public string SegmentName { get; set; }
        public string FileName { get; set; }

        public NamedStream(string name, string fileName, byte[] data) : base(data, false)
        {
            SegmentName = name;
            FileName = fileName;
        }
    }
}
