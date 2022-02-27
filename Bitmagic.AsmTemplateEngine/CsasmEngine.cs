using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BitMagic.AsmTemplateEngine
{
    public static class CsasmEngine
    {
        public static ITemplateEngine CreateEngine() => TemplateEngineBuilder
                .As("csasm")
                // 6502
                .WithUnderlying(new Regex(@"^\s*(?<line>((?i)(adc|and|asl|bcc|bcs|beq|bit|bmi|bne|bpl|brk|bvc|bvs|clc|cld|cli|clv|cmp|cpx|cpy|dec|dex|dey|eor|inc|inx|iny|jmp|jsr|lda|ldx|ldy|lsr|nop|ora|pha|php|pla|plp|rol|ror|rti|rts|sbc|sec|sed|sei|sta|stx|sty|stp|tax|tay|tsx|txa|txs|tya)(\s+.*|)))$", RegexOptions.Compiled))
                // 65c02
                .WithUnderlying(new Regex(@"^\s*(?<line>((?i)(bra|phx|phy|plx|ply|stz|trb|tsb|bbr0|bbr1|bbr2|bbr3|bbr4|bbr5|bbr6|bbr7|bbs0|bbs1|bbs2|bbs3|bbs4|bbs5|bbs6|bbs7|rmb0|rmb1|rmb2|rmb3|rmb4|rmb5|rmb6|rmb7|smb0|smb1|smb2|smb3|smb4|smb5|smb6|smb7)(\s+.*|)))$", RegexOptions.Compiled))
                // bmasm lines, anything that starts with a . or a ;
                .WithUnderlying(new Regex(@"^\s*(?<line>([\.;].*))$", RegexOptions.Compiled))
                // imbedded csharp, eg lda @( csharp_variable ) - https://stackoverflow.com/questions/17003799/what-are-regular-expression-balancing-groups
                .WithCSharpInline(new Regex(@"(?<csharp>(@[^\s](?:[^\(\)]|(?<open>\()|(?<-open>\)))+(?(open)(?!))\)))", RegexOptions.Compiled), new Regex(@"(@\()(?<csharp>(.*))(\))", RegexOptions.Compiled))
                .Build();
    }
}
