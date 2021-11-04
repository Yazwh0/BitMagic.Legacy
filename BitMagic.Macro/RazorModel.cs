using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Macro
{
    public class RazorModel : RazorEngineTemplateBase
    {
        public string Bytes(IEnumerable<byte> bytes, int width = 16)
        {
            StringBuilder sb = new StringBuilder();
            var cnt = 0;
            var first = true;
            foreach (var i in bytes)
            {
                if (first)
                {
                    sb.Append(".byte\t");
                    first = false;
                }
                else
                {
                    sb.Append(", ");
                }

                sb.Append($"${i:X2}");
                cnt++;
                if (cnt == width)
                {
                    sb.AppendLine();
                    cnt = 0;
                    first = true;
                } 
            }
            sb.AppendLine();
            return sb.ToString();
        }

        public string Words(IEnumerable<ushort> words, int width = 16)
        {
            StringBuilder sb = new StringBuilder();
            var cnt = 0;
            var first = true;
            foreach (var i in words)
            {
                if (first)
                {
                    sb.Append(".word\t");
                    first = false;
                }
                else
                {
                    sb.Append(", ");
                }

                sb.Append($"${i:X4}");
                cnt++;
                if (cnt == width)
                {
                    sb.AppendLine();
                    cnt = 0;
                    first = true;
                }
            }
            sb.AppendLine();
            return sb.ToString();
        }

        public string Words(IEnumerable<short> words, int width = 16)
        {
            StringBuilder sb = new StringBuilder();
            var cnt = 0;
            var first = true;
            foreach (var i in words)
            {
                if (first)
                {
                    sb.Append(".word\t");
                    first = false;
                }
                else
                {
                    sb.Append(", ");
                }

                sb.Append($"${i:X4}");
                cnt++;
                if (cnt == width)
                {
                    sb.AppendLine();
                    cnt = 0;
                    first = true;
                }
            }
            sb.AppendLine();
            return sb.ToString();
        }

        //    .byte $0C, $08              ; $080C - pointer to next line of BASIC code
        //    .byte $0A, $00              ; 2-byte line number($000A = 10)
        //    .byte $9E                   ; SYS BASIC token
        //    .byte $20                   ; [space]
        //    .byte $32, $30, $36, $34    ; $32="2",$30="0",$36="6",$34="4"
        //    .byte $00                   ; End of Line
        //    .byte $00, $00              ; This is address $080C containing
        //                                ; 2-byte pointer to next line of BASIC code
        //                                ; ($0000 = end of program)
        //    .byte $00, $00              ; Padding so code starts at $0810
        public string X16Header() => Bytes(new byte[] { 0x0c, 0x08, 0x0a, 0x00, 0x9e, 0x20, 0x32, 0x30, 0x36, 0x34, 0x00, 0x00, 0x00, 0x00, 0x00 });
    }
}
