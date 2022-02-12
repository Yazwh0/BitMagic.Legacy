
namespace Vera;

public static class VideoMemory
{
    private static byte _zpWorkAddress = 0;
    private static bool _copyWorkAddressSet = false;

    public static void SetCopyZpWordAddress(byte zpWordAddress)
    {
        if (!_copyWorkAddressSet) 
        {
            _zpWorkAddress = zpWordAddress;      
            _copyWorkAddressSet = true;
        }
        else if (_zpWorkAddress != zpWordAddress)
        {
            throw new Exception("Copy called with an zp address that is not the same as a previous call.");
        }
    }

    public static void Copy(int source, int dest, int count, byte zpWordAddress)
    {
        SetCopyZpWordAddress(zpWordAddress);
        
        Copy(source, dest, count);
    }

    public static void Copy(object source, int dest, int count)
    {
        if (!_copyWorkAddressSet)
        {
            throw new Exception("Copy called but zpWordAddress is not set.");
        }

        int addressPart = (dest & 0xf0000) >> 16;
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #{addressPart + 0x10}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta ADDRx_H");

        addressPart = (dest & 0xff00) >> 8; 

        if (addressPart == 0)
        {
BitMagic.AsmTemplate.Template.WriteLiteral($@"stz ADDRx_M");
        } 
        else 
        {
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #{addressPart}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta ADDRx_M");
        }

        addressPart = dest & 0xff;

        if (addressPart == 0)
        {
BitMagic.AsmTemplate.Template.WriteLiteral($@"stz ADDRx_L");
        } 
        else 
        {
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #{addressPart}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta ADDRx_L");
        }

BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #<{source}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta {_zpWorkAddress}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #>{source}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta {_zpWorkAddress}+1");

BitMagic.AsmTemplate.Template.WriteLiteral($@"ldx #>{count}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ldy #<{count}");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sty :CopyProcScope:CopyToVram:end_check+1");

BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr :CopyProcScope:CopyToVram");
    }

    public static void CopyProc(byte zpWordAddress)
    {
        SetCopyZpWordAddress(zpWordAddress);
        
        CopyProc();
    }

    public static void CopyProc()
    {
        if (!_copyWorkAddressSet)
        {
            throw new Exception("CopyProc called but zpWordAddress is not set.");
        }
BitMagic.AsmTemplate.Template.WriteLiteral($@".scope CopyProcScope");
BitMagic.AsmTemplate.Template.WriteLiteral($@".proc CopyToVram");

BitMagic.AsmTemplate.Template.WriteLiteral($@";sty end_check+1");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ldy #0");

BitMagic.AsmTemplate.Template.WriteLiteral($@".loop:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda ({_zpWorkAddress}), y");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta DATA0");
BitMagic.AsmTemplate.Template.WriteLiteral($@"iny");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bne loop");

BitMagic.AsmTemplate.Template.WriteLiteral($@"inc {_zpWorkAddress}+1");
BitMagic.AsmTemplate.Template.WriteLiteral($@"dex");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bne loop");

BitMagic.AsmTemplate.Template.WriteLiteral($@".remain_loop:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda ({_zpWorkAddress}), y");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta DATA0");
BitMagic.AsmTemplate.Template.WriteLiteral($@"iny");
BitMagic.AsmTemplate.Template.WriteLiteral($@".end_check:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"cpy #$aa ; gets updated");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bne remain_loop");

BitMagic.AsmTemplate.Template.WriteLiteral($@"rts");
BitMagic.AsmTemplate.Template.WriteLiteral($@".endproc");
BitMagic.AsmTemplate.Template.WriteLiteral($@".endscope");
    }
}
