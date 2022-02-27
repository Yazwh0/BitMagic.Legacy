using Mega65Parser;
using System.Text;
namespace Mega65Processor;

public static class CpuDocumentationGenerator
{
    public static void Output(Parser parser)
    {
        var instructions = parser.Instructions.GroupBy(i => i.Code).OrderBy(i => i.Key).ToArray();
        var sb = new StringBuilder();
BitMagic.AsmTemplate.Template.WriteLiteral($@"# {parser.ChipName} Instructions");
BitMagic.AsmTemplate.Template.WriteLiteral($@"");
BitMagic.AsmTemplate.Template.WriteLiteral($@"## All Instructions");
BitMagic.AsmTemplate.Template.WriteLiteral($@"");
        int size = 16;
BitMagic.AsmTemplate.Template.WriteLiteral($@"| |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |");
BitMagic.AsmTemplate.Template.WriteLiteral($@"| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |");
        for(var i = 0; i < instructions.Length; i += size)
        {
            var thisLine = instructions.Skip(i).Take(size);
            sb.Clear();
            sb.Append("|");
            foreach(var item in thisLine.Select(i => i.Key))
            {
                sb.Append($" [{item}](#{item}) |");
            }
BitMagic.AsmTemplate.Template.WriteLiteral($@"{sb}");
        }
        foreach(var group in instructions)
        {
            var explanation = parser.Explanation[group.Key];
BitMagic.AsmTemplate.Template.WriteLiteral($@"");
BitMagic.AsmTemplate.Template.WriteLiteral($@"## {group.Key}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"");
BitMagic.AsmTemplate.Template.WriteLiteral($@"**{explanation.Description}**\");
            foreach(var exp in explanation.Explanations) {
BitMagic.AsmTemplate.Template.WriteLiteral($@"{exp}\");
            }
BitMagic.AsmTemplate.Template.WriteLiteral($@"Flags: {explanation.Flags}");
BitMagic.AsmTemplate.Template.WriteLiteral($@" | Mode | Syntax | Hex | Len |");
BitMagic.AsmTemplate.Template.WriteLiteral($@" | --- | --- | --- | --- |");
            foreach(var ip in parser.Instructions.
                    Where(i => i.Code == group.Key).
                    Select(i => (Instruction: i, Description: parser.ParametersOrder[i.Parameters])).
                    OrderBy(ip => ip.Description.Order))
            {
BitMagic.AsmTemplate.Template.WriteLiteral($@" | {ip.Description.Name} | {ip.Instruction.Code} {ip.Instruction.Parameters} | ${ip.Instruction.OpCode:X2} | {ip.Description.ByteCount + 1} |");
            }
            if (!string.IsNullOrWhiteSpace(explanation.ExplanationText))
            {
BitMagic.AsmTemplate.Template.WriteLiteral($@"");
BitMagic.AsmTemplate.Template.WriteLiteral($@"{explanation.ExplanationText}");
            }
        }
    }
}

