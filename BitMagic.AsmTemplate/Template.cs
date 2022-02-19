using BitMagic.Common;
using BitMagic.Machines;
using System.Text;

namespace BitMagic.AsmTemplate
{
    public static class Template
    {
        internal static StringBuilder _output = new();

        public static void WriteLiteral(string literal)
        {
            _output.AppendLine(literal);
        }

        public static void StartProject(Project project)
        {
            BitMagicHelper.Machine = project.Machine;
            _output.Clear();
        }

        public static new string ToString => _output.ToString();
    }
}