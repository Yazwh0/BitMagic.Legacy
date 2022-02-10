using System.IO.Compression;
using System.IO;

namespace Compression;

public static class Deflator
{
    public static byte[] Deflate(byte[] input)
    {
        using var outputStream = new MemoryStream();
        using var compressedStream = new DeflateStream(outputStream, CompressionLevel.SmallestSize);

        compressedStream.Write(input, 0, input.Length);
        compressedStream.Flush();
        compressedStream.Close();

        var toReturn = outputStream.ToArray();

        return toReturn;
    }
}

public static partial class Inflator
{
    public static byte Source {get; internal set;} // needed until we get a better .const parser

    public static void SetSourceZp(byte zpAddress)
    {
        Source = zpAddress;
BitMagic.AsmTemplate.Template.WriteLiteral($@".scope Inflate");
BitMagic.AsmTemplate.Template.WriteLiteral($@".const inflateZp = {zpAddress}");
BitMagic.AsmTemplate.Template.WriteLiteral($@".endscope");
    }

    public static void InflateToRam(string sourceLabel, ushort destinationAddr)
    {
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #<{sourceLabel}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta inflateZp");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #>{sourceLabel}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta inflateZp+1");

BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #<{destinationAddr}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta inflateZp+2");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #>{destinationAddr}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta inflateZp+3");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr :Inflate:inflate_to_ram");
    }

    public static void InflateToVram(string sourceLabel, uint destinationAddr)
    {
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #<{sourceLabel}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta inflateZp");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #>{sourceLabel}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta inflateZp+1");

BitMagic.AsmTemplate.Template.WriteLiteral($@"stz CTRL");

BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #<{destinationAddr}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta ADDRx_L");

BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #>{destinationAddr}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta ADDRx_M");

BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #^{destinationAddr}+$10");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta ADDRx_H");

BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr :Inflate:inflate_to_vram");
    }

    public static void DefineScratchArea()
    {
BitMagic.AsmTemplate.Template.WriteLiteral($@".scope Inflate");
BitMagic.AsmTemplate.Template.WriteLiteral($@".literalSymbolCodeLength:        ");
BitMagic.AsmTemplate.Template.WriteLiteral($@".pad $100");

BitMagic.AsmTemplate.Template.WriteLiteral($@".controlSymbolCodeLength:");
BitMagic.AsmTemplate.Template.WriteLiteral($@".pad {1+29+2 + 30}");


BitMagic.AsmTemplate.Template.WriteLiteral($@".nBitCode_clearFrom:");
BitMagic.AsmTemplate.Template.WriteLiteral($@".nBitCode_literalCount:");
BitMagic.AsmTemplate.Template.WriteLiteral($@".pad 32");

BitMagic.AsmTemplate.Template.WriteLiteral($@".nBitCode_controlCount:");
BitMagic.AsmTemplate.Template.WriteLiteral($@".pad 32");

BitMagic.AsmTemplate.Template.WriteLiteral($@".nBitCode_literalOffset:");
BitMagic.AsmTemplate.Template.WriteLiteral($@".pad 32");

BitMagic.AsmTemplate.Template.WriteLiteral($@".nBitCode_controlOffset:");
BitMagic.AsmTemplate.Template.WriteLiteral($@".pad 32");

BitMagic.AsmTemplate.Template.WriteLiteral($@".allLiteralsCodeLength:");
BitMagic.AsmTemplate.Template.WriteLiteral($@".pad 1");

BitMagic.AsmTemplate.Template.WriteLiteral($@".codeToLiteralSymbol:");
BitMagic.AsmTemplate.Template.WriteLiteral($@".pad $100");

BitMagic.AsmTemplate.Template.WriteLiteral($@".codeToControlSymbol:");
BitMagic.AsmTemplate.Template.WriteLiteral($@".pad {1+29+2 + 30}");
BitMagic.AsmTemplate.Template.WriteLiteral($@".endscope");
    }
}

