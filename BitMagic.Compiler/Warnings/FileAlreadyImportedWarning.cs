using BitMagic.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Compiler.Warnings
{
    public class FileAlreadyImportedWarning : CompilerWarning
    {
        public int LineNumber { get; }
        public string FileName { get; }
        public string ImportName { get; }
        public FileAlreadyImportedWarning(SourceFilePosition source, string importName)
        {
            LineNumber = source.LineNumber;
            FileName = source.Name;
            ImportName = importName;
        }
        public override string ToString() => $"Already imported '{ImportName}' import is ignored, on line {LineNumber} in file '{FileName}'.";
    }
}
