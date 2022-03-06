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
BitMagic.AsmTemplate.Template.WriteLiteral($@"## Credits and License");
BitMagic.AsmTemplate.Template.WriteLiteral($@"");
BitMagic.AsmTemplate.Template.WriteLiteral($@"All text derived from the [Mega65 User Guide](https://github.com/MEGA65/mega65-user-guide). Copyright 2019-2022 by Paul Gardner-Stephen, the MEGA Museum of Electronic Games & Art e.V., and contributors.");
BitMagic.AsmTemplate.Template.WriteLiteral($@"");
BitMagic.AsmTemplate.Template.WriteLiteral($@"This reference guide is made available under the GNU Free Documentation License");
BitMagic.AsmTemplate.Template.WriteLiteral($@"v1.3, or later, if desired. This means that you are free to modify, reproduce and redistribute this reference guide, subject to certain conditions. The full text of the GNU");
BitMagic.AsmTemplate.Template.WriteLiteral($@"Free Documentation License v1.3 can be found at [https://www.gnu.org/licenses/fdl-1.3.en.html](https://www.gnu.org/licenses/fdl-1.3.en.html).");
BitMagic.AsmTemplate.Template.WriteLiteral($@"");
BitMagic.AsmTemplate.Template.WriteLiteral($@"## All Instructions");
BitMagic.AsmTemplate.Template.WriteLiteral($@"");
        int size = 12;
BitMagic.AsmTemplate.Template.WriteLiteral($@"| |  |  |  |  |  |  |  |  |  |  |  |");
BitMagic.AsmTemplate.Template.WriteLiteral($@"| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |");
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
        var cycleNotes = new List<Char>();
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
BitMagic.AsmTemplate.Template.WriteLiteral($@" | Mode | Syntax | Hex | Len | Cycles |");
BitMagic.AsmTemplate.Template.WriteLiteral($@" | --- | --- | --- | --- | --- |");
            cycleNotes.Clear();
            foreach(var ip in parser.Instructions.
                    Where(i => i.Code == group.Key).
                    Select(i => (Instruction: i, Description: parser.ParametersOrder[i.Parameters])).
                    OrderBy(ip => ip.Description.Order))
            {
                string cycles = ip.Instruction.Cycles != 0 ? ip.Instruction.Cycles.ToString() : "";
BitMagic.AsmTemplate.Template.WriteLiteral($@" | {ip.Description.Name} | {ip.Instruction.Code} {ip.Instruction.ParametersBase} | {ip.Instruction.OpCodeDisplay()} | {ip.Description.ByteCount + ip.Instruction.InstructionLength()} | {cycles}<sup>{ip.Instruction.CycleNotesDisplay}</sup> |");
                cycleNotes.AddRange(ip.Instruction.CycleNotes);
            }
            if (cycleNotes.Any())
            {
BitMagic.AsmTemplate.Template.WriteLiteral($@"");
                var notes = cycleNotes.Distinct().OrderBy(c => c).ToArray();
                for(var i = 0; i < notes.Length; i++)
                {
                    if (i == notes.Length - 1)
                    {
BitMagic.AsmTemplate.Template.WriteLiteral($@" <sup>{notes[i]}</sup> {parser.CycleNotes[notes[i]]}");
                    } else {
BitMagic.AsmTemplate.Template.WriteLiteral($@" <sup>{notes[i]}</sup> {parser.CycleNotes[notes[i]]}\");
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(explanation.ExplanationText))
            {
BitMagic.AsmTemplate.Template.WriteLiteral($@"");
BitMagic.AsmTemplate.Template.WriteLiteral($@"{explanation.ExplanationText}");
            }
        }        
    }
}

