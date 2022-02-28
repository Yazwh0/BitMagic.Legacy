
using BitMagic.AsmTemplate;
using Mega65Parser;
using Mega65Processor;

var p = new Parser(@"D:\Documents\Source\mega65-user-guide\instruction_sets");

var chipName = "4510";

Template.StartProject();
p.Parse(chipName);
CpuDocumentationGenerator.Output(p);

File.WriteAllText(Path.Combine(@"D:\Documents\Source\BitMagic\Documentation\InstructionSets", chipName + "_instructions.md"), Template.ToString);

chipName = "45GS02";
p = new Parser(@"D:\Documents\Source\mega65-user-guide\instruction_sets");
Template.StartProject();
p.Parse(chipName);
CpuDocumentationGenerator.Output(p);

File.WriteAllText(Path.Combine(@"D:\Documents\Source\BitMagic\Documentation\InstructionSets", chipName + "_instructions.md"), Template.ToString);
