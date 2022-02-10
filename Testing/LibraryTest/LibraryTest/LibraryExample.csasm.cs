
public static class Library
{
    public static void UnrolledExample(int rollCount)
    {
BitMagic.AsmTemplate.Template.WriteLiteral($@"; Generated on {DateTime.Now.ToString("dd MMM yyyy HH:mm:ss")}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; Unrolled code, {rollCount} steps.");
        for (var i = 0; i < rollCount; i++)
        {
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #{i}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; etc");
        }
BitMagic.AsmTemplate.Template.WriteLiteral($@"rts");
BitMagic.AsmTemplate.Template.WriteLiteral($@"");
        BM.Bytes(new byte[] {1, 2, 3}); // will create ".byte $01, $02, $03"
    }
}

