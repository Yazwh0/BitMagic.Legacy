namespace Compression;

public static partial class Inflator 
{
    public static void InflateToVramCode()
    {
BitMagic.AsmTemplate.Template.WriteLiteral($@"; inflate taken from https://github.com/pfusik/zlib6502/blob/master/inflate.asx");
BitMagic.AsmTemplate.Template.WriteLiteral($@".scope Inflate");

BitMagic.AsmTemplate.Template.WriteLiteral($@".proc inflate_to_vram");
BitMagic.AsmTemplate.Template.WriteLiteral($@".const inputPointer                    = {Source}");
BitMagic.AsmTemplate.Template.WriteLiteral($@";x.const outputPointer                   = {Source+2}");
BitMagic.AsmTemplate.Template.WriteLiteral($@".const getBit_buffer                   = {Source+4}");

BitMagic.AsmTemplate.Template.WriteLiteral($@".const getBits_base                    = {Source+5}  ; 1 byte");
BitMagic.AsmTemplate.Template.WriteLiteral($@".const inflateStored_pageCounter       = {Source+5}  ; 1 byte");

BitMagic.AsmTemplate.Template.WriteLiteral($@";x.const inflateCodes_sourcePointer      = {Source+6}  ; 2 bytes");
BitMagic.AsmTemplate.Template.WriteLiteral($@".const inflateDynamic_symbol           = {Source+6}  ; 1 byte");
BitMagic.AsmTemplate.Template.WriteLiteral($@".const inflateDynamic_lastLength       = {Source+7}  ; 1 byte");
BitMagic.AsmTemplate.Template.WriteLiteral($@".const inflateDynamic_tempCodes        = {Source+7}  ; 1 byte");

BitMagic.AsmTemplate.Template.WriteLiteral($@".const inflateCodes_lengthMinus2       = {Source+8}  ; 1 byte");
BitMagic.AsmTemplate.Template.WriteLiteral($@".const inflateDynamic_allCodes         = {Source+8}  ; 1 byte");

BitMagic.AsmTemplate.Template.WriteLiteral($@".const inflateDynamic_primaryCodes     = {Source+9}  ; 1 byte");

BitMagic.AsmTemplate.Template.WriteLiteral($@".const inflateCodes_lookback           = {Source+6} ; 2 bytes");
BitMagic.AsmTemplate.Template.WriteLiteral($@".const inflateCodes_vramAddr           = {Source+10} ; 3 bytes");

BitMagic.AsmTemplate.Template.WriteLiteral($@".const GET_1_BIT                       =	$81");
BitMagic.AsmTemplate.Template.WriteLiteral($@".const GET_2_BITS                      =	$82");
BitMagic.AsmTemplate.Template.WriteLiteral($@".const GET_3_BITS                      =	$84");
BitMagic.AsmTemplate.Template.WriteLiteral($@".const GET_4_BITS                      =	$88");
BitMagic.AsmTemplate.Template.WriteLiteral($@".const GET_5_BITS                      =	$90");
BitMagic.AsmTemplate.Template.WriteLiteral($@".const GET_6_BITS                      =	$a0");
BitMagic.AsmTemplate.Template.WriteLiteral($@".const GET_7_BITS                      =	$c0   ");

BitMagic.AsmTemplate.Template.WriteLiteral($@".const TREE_SIZE                       =	16");
BitMagic.AsmTemplate.Template.WriteLiteral($@".const PRIMARY_TREE                    =	0");
BitMagic.AsmTemplate.Template.WriteLiteral($@".const DISTANCE_TREE                   =	16");

BitMagic.AsmTemplate.Template.WriteLiteral($@".const LENGTH_SYMBOLS                  =	{1+29+2}");
BitMagic.AsmTemplate.Template.WriteLiteral($@".const DISTANCE_SYMBOLS                =	30");
BitMagic.AsmTemplate.Template.WriteLiteral($@".const CONTROL_SYMBOLS                 =	{1+29+2 + 30} ;LENGTH_SYMBOLS+DISTANCE_SYMBOLS");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; Uncompress DEFLATE stream starting from the address stored in inputPointer");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; to the memory starting from the address stored in outputPointer");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ldy #0");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sty getBit_buffer");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; Get a bit of EOF and two bits of block type");
BitMagic.AsmTemplate.Template.WriteLiteral($@".inflate_blockLoop:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sty	getBits_base");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; injected background stuff");

BitMagic.AsmTemplate.Template.WriteLiteral($@"lda	#GET_3_BITS");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	getBits");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lsr");
BitMagic.AsmTemplate.Template.WriteLiteral($@"php");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bne	inflateCompressed");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; Copy uncompressed block");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sty	getBit_buffer  ; ignore bits until byte boundary");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	getWord        ; skip the length we don't need");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	getWord        ; get the one's complement length");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta	inflateStored_pageCounter");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcs	inflateStored_firstByte");

BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateStored_copyByte:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	getByte");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	storeByte");
            
BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateStored_firstByte:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"inx");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bne	inflateStored_copyByte");
BitMagic.AsmTemplate.Template.WriteLiteral($@"inc	inflateStored_pageCounter");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bne	inflateStored_copyByte");

BitMagic.AsmTemplate.Template.WriteLiteral($@".inflate_nextBlock:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"plp");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcc	inflate_blockLoop");
BitMagic.AsmTemplate.Template.WriteLiteral($@"rts");

BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateCompressed:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; A=1: fixed block, initialize with fixed codes");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; A=2: dynamic block, start by clearing all code lengths");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; A=3: invalid compressed data, not handled in this routine");
BitMagic.AsmTemplate.Template.WriteLiteral($@"eor	#2");

BitMagic.AsmTemplate.Template.WriteLiteral($@";	ldy	#0");
BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateCompressed_setCodeLengths:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"tax");
BitMagic.AsmTemplate.Template.WriteLiteral($@"beq	inflateCompressed_setLiteralCodeLength");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; fixed Huffman literal codes:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; :144 dta 8");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; :112 dta 9");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda	#4");
BitMagic.AsmTemplate.Template.WriteLiteral($@"cpy	#144");
BitMagic.AsmTemplate.Template.WriteLiteral($@"rol	");

BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateCompressed_setLiteralCodeLength:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta	literalSymbolCodeLength,y");
BitMagic.AsmTemplate.Template.WriteLiteral($@"beq	inflateCompressed_setControlCodeLength");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; fixed Huffman control codes:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; :24  dta 7");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; :6   dta 8");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; :2   dta 8 ; meaningless codes");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; :30  dta 5+DISTANCE_TREE");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda	#5+DISTANCE_TREE");
BitMagic.AsmTemplate.Template.WriteLiteral($@"cpy	#LENGTH_SYMBOLS");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcs	inflateCompressed_setControlCodeLength");
BitMagic.AsmTemplate.Template.WriteLiteral($@"cpy	#24");
BitMagic.AsmTemplate.Template.WriteLiteral($@"adc	#2-DISTANCE_TREE");

BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateCompressed_setControlCodeLength:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"cpy	#CONTROL_SYMBOLS");

BitMagic.AsmTemplate.Template.WriteLiteral($@";scs:sta	controlSymbolCodeLength,y");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcs skip_5");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta	controlSymbolCodeLength,y");
BitMagic.AsmTemplate.Template.WriteLiteral($@".skip_5:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"iny");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bne	inflateCompressed_setCodeLengths");

BitMagic.AsmTemplate.Template.WriteLiteral($@"tax");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bne	inflateCodes");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; Decompress a block reading Huffman trees first");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; Build the tree for temporary codes");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	buildTempHuffmanTree");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; Use temporary codes to get lengths of literal/length and distance codes");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	ldx	#0");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	sec");
BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateDynamic_decodeLength:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; C=1: literal codes");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; C=0: control codes");
BitMagic.AsmTemplate.Template.WriteLiteral($@"stx	inflateDynamic_symbol");
BitMagic.AsmTemplate.Template.WriteLiteral($@"php");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; Fetch a temporary code");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	fetchPrimaryCode");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; Temporary code 0..15: put this length");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bpl	inflateDynamic_verbatimLength");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; Temporary code 16: repeat last length 3 + getBits(2) times");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; Temporary code 17: put zero length 3 + getBits(3) times");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; Temporary code 18: put zero length 11 + getBits(7) times");
BitMagic.AsmTemplate.Template.WriteLiteral($@"tax");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	getBits");
BitMagic.AsmTemplate.Template.WriteLiteral($@"cpx	#GET_3_BITS");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcc	inflateDynamic_repeatLast");

BitMagic.AsmTemplate.Template.WriteLiteral($@"beq skip_1");
BitMagic.AsmTemplate.Template.WriteLiteral($@";seq:adc	#7");
BitMagic.AsmTemplate.Template.WriteLiteral($@"adc	#7");
BitMagic.AsmTemplate.Template.WriteLiteral($@".skip_1:");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	ldy	#0");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sty	inflateDynamic_lastLength");

BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateDynamic_repeatLast:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"tay");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda	inflateDynamic_lastLength");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; iny:iny");
BitMagic.AsmTemplate.Template.WriteLiteral($@"iny");
BitMagic.AsmTemplate.Template.WriteLiteral($@"iny");

BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateDynamic_verbatimLength:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"iny");
BitMagic.AsmTemplate.Template.WriteLiteral($@"plp");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ldx	inflateDynamic_symbol");

BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateDynamic_storeLength:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcc	inflateDynamic_controlSymbolCodeLength");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta	literalSymbolCodeLength,x ; +");
BitMagic.AsmTemplate.Template.WriteLiteral($@"inx");
BitMagic.AsmTemplate.Template.WriteLiteral($@"cpx	#1");

BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateDynamic_storeNext:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"dey");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bne	inflateDynamic_storeLength");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta	inflateDynamic_lastLength");
BitMagic.AsmTemplate.Template.WriteLiteral($@"beq	inflateDynamic_decodeLength");

BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateDynamic_controlSymbolCodeLength:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"cpx	inflateDynamic_primaryCodes");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcc	inflateDynamic_storeControl");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; the code lengths we skip here were zero-initialized");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; in inflateCompressed_setControlCodeLength");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bne skip_2");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ldx	#LENGTH_SYMBOLS");
BitMagic.AsmTemplate.Template.WriteLiteral($@".skip_2:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ora	#DISTANCE_TREE");

BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateDynamic_storeControl:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta	controlSymbolCodeLength,x ; +");
BitMagic.AsmTemplate.Template.WriteLiteral($@"inx");
BitMagic.AsmTemplate.Template.WriteLiteral($@"cpx	inflateDynamic_allCodes");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcc	inflateDynamic_storeNext");
BitMagic.AsmTemplate.Template.WriteLiteral($@"dey");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; Decompress a block");
BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateCodes:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	buildHuffmanTree");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	jmp	inflateCodes_loop");
BitMagic.AsmTemplate.Template.WriteLiteral($@"beq	inflateCodes_loop");

BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateCodes_literal:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	storeByte");

BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateCodes_loop:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	fetchPrimaryCode");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcc	inflateCodes_literal");
BitMagic.AsmTemplate.Template.WriteLiteral($@"beq	inflate_nextBlock");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; Copy sequence from look-behind buffer");

BitMagic.AsmTemplate.Template.WriteLiteral($@"sty	getBits_base");
BitMagic.AsmTemplate.Template.WriteLiteral($@"cmp	#9");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcc	inflateCodes_setSequenceLength");
BitMagic.AsmTemplate.Template.WriteLiteral($@"tya");

BitMagic.AsmTemplate.Template.WriteLiteral($@"cpx	#1+28");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcs	inflateCodes_setSequenceLength");
BitMagic.AsmTemplate.Template.WriteLiteral($@"dex");
BitMagic.AsmTemplate.Template.WriteLiteral($@"txa");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lsr	");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ror	getBits_base");
BitMagic.AsmTemplate.Template.WriteLiteral($@"inc	getBits_base");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lsr	");
BitMagic.AsmTemplate.Template.WriteLiteral($@"rol	getBits_base");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	getAMinus1BitsMax8");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	sec");
BitMagic.AsmTemplate.Template.WriteLiteral($@"adc	#0");

BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateCodes_setSequenceLength:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta	inflateCodes_lengthMinus2");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ldx	#DISTANCE_TREE");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	fetchCode");
BitMagic.AsmTemplate.Template.WriteLiteral($@"cmp	#4");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcc	inflateCodes_setOffsetLowByte");
BitMagic.AsmTemplate.Template.WriteLiteral($@"inc	getBits_base");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lsr	");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	getAMinus1BitsMax8");

BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateCodes_setOffsetLowByte:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta inflateCodes_lookback");

BitMagic.AsmTemplate.Template.WriteLiteral($@"lda	getBits_base");
BitMagic.AsmTemplate.Template.WriteLiteral($@"cpx	#10");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcc	inflateCodes_setOffsetHighByte");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda	getNPlus1Bits_mask-10,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	getBits");
BitMagic.AsmTemplate.Template.WriteLiteral($@"clc");

BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateCodes_setOffsetHighByte:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta inflateCodes_lookback+1");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; copy addr from data0 to data1 minus inflateCodes_lookback ");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; already in data0");
BitMagic.AsmTemplate.Template.WriteLiteral($@"clc");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda ADDRx_L");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sbc inflateCodes_lookback");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta inflateCodes_vramAddr");

BitMagic.AsmTemplate.Template.WriteLiteral($@"lda ADDRx_M");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sbc inflateCodes_lookback+1");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta inflateCodes_vramAddr+1");

BitMagic.AsmTemplate.Template.WriteLiteral($@"lda ADDRx_H");
BitMagic.AsmTemplate.Template.WriteLiteral($@"and #$0f");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sbc #0");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta inflateCodes_vramAddr+2");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; set data1 addr");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #01");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta CTRL");

BitMagic.AsmTemplate.Template.WriteLiteral($@"lda inflateCodes_vramAddr");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta ADDRx_L");

BitMagic.AsmTemplate.Template.WriteLiteral($@"lda inflateCodes_vramAddr+1");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta ADDRx_M");

BitMagic.AsmTemplate.Template.WriteLiteral($@"lda inflateCodes_vramAddr+2");
BitMagic.AsmTemplate.Template.WriteLiteral($@"and #$0f");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ora #$10");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta ADDRx_H");
            
BitMagic.AsmTemplate.Template.WriteLiteral($@"clc");

BitMagic.AsmTemplate.Template.WriteLiteral($@"stz CTRL            ");
            
BitMagic.AsmTemplate.Template.WriteLiteral($@"; done");

BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	copyByte");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	copyByte");

BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateCodes_copyByte:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	copyByte");
BitMagic.AsmTemplate.Template.WriteLiteral($@"dec	inflateCodes_lengthMinus2");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bne	inflateCodes_copyByte");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	jmp	inflateCodes_loop");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jmp	inflateCodes_loop");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; Get dynamic block header and use it to build the temporary tree");
BitMagic.AsmTemplate.Template.WriteLiteral($@".buildTempHuffmanTree:");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	ldy	#0");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; numberOfPrimaryCodes = 257 + getBits(5)");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; numberOfDistanceCodes = 1 + getBits(5)");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; numberOfTemporaryCodes = 4 + getBits(4)");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ldx	#3");
BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateDynamic_getHeader:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda	inflateDynamic_headerBits-1,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	getBits");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	sec");
BitMagic.AsmTemplate.Template.WriteLiteral($@"adc	inflateDynamic_headerBase-1,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta	inflateDynamic_tempCodes-1,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@"dex");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bne	inflateDynamic_getHeader");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; Get lengths of temporary codes in the order stored in inflateDynamic_tempSymbols");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	ldx	#0");
BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateDynamic_getTempCodeLengths:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda	#GET_3_BITS");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	getBits");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ldy	inflateDynamic_tempSymbols,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta	literalSymbolCodeLength,y");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ldy	#0");
BitMagic.AsmTemplate.Template.WriteLiteral($@"inx");
BitMagic.AsmTemplate.Template.WriteLiteral($@"cpx	inflateDynamic_tempCodes");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcc	inflateDynamic_getTempCodeLengths");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; Build Huffman trees basing on code lengths (in bits)");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; stored in the *SymbolCodeLength arrays");
BitMagic.AsmTemplate.Template.WriteLiteral($@".buildHuffmanTree:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; Clear nBitCode_literalCount, nBitCode_controlCount");
BitMagic.AsmTemplate.Template.WriteLiteral($@"tya");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	lda	#0");
BitMagic.AsmTemplate.Template.WriteLiteral($@";sta:rne	nBitCode_clearFrom,y+");
BitMagic.AsmTemplate.Template.WriteLiteral($@".loop_1:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta nBitCode_clearFrom,y");
BitMagic.AsmTemplate.Template.WriteLiteral($@"iny");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bne loop_1");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; Count number of codes of each length");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	ldy	#0");

BitMagic.AsmTemplate.Template.WriteLiteral($@".buildHuffmanTree_countCodeLengths:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ldx	literalSymbolCodeLength,y");
BitMagic.AsmTemplate.Template.WriteLiteral($@"inc	nBitCode_literalCount,x");

BitMagic.AsmTemplate.Template.WriteLiteral($@"bne skip_4");
BitMagic.AsmTemplate.Template.WriteLiteral($@";sne:stx	allLiteralsCodeLength");
BitMagic.AsmTemplate.Template.WriteLiteral($@"stx	allLiteralsCodeLength");
BitMagic.AsmTemplate.Template.WriteLiteral($@".skip_4:");

BitMagic.AsmTemplate.Template.WriteLiteral($@"cpy	#CONTROL_SYMBOLS");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcs	buildHuffmanTree_noControlSymbol");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ldx	controlSymbolCodeLength,y");
BitMagic.AsmTemplate.Template.WriteLiteral($@"inc	nBitCode_controlCount,x");

BitMagic.AsmTemplate.Template.WriteLiteral($@".buildHuffmanTree_noControlSymbol:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"iny");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bne	buildHuffmanTree_countCodeLengths");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; Calculate offsets of symbols sorted by code length");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	lda	#0");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ldx	#-4*TREE_SIZE");

BitMagic.AsmTemplate.Template.WriteLiteral($@".buildHuffmanTree_calculateOffsets:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; !problem!");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta	nBitCode_literalOffset+64-$100,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@";add	nBitCode_literalCount+4*TREE_SIZE-$100,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@"clc");
BitMagic.AsmTemplate.Template.WriteLiteral($@"adc	nBitCode_literalCount+64-$100,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@"inx");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bne	buildHuffmanTree_calculateOffsets");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; Put symbols in their place in the sorted array");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	ldy	#0");

BitMagic.AsmTemplate.Template.WriteLiteral($@".buildHuffmanTree_assignCode:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"tya");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ldx	literalSymbolCodeLength,y");
BitMagic.AsmTemplate.Template.WriteLiteral($@";ldy:inc	nBitCode_literalOffset,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ldy	nBitCode_literalOffset,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@"inc	nBitCode_literalOffset,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta	codeToLiteralSymbol,y");
BitMagic.AsmTemplate.Template.WriteLiteral($@"tay");
BitMagic.AsmTemplate.Template.WriteLiteral($@"cpy	#CONTROL_SYMBOLS");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcs	buildHuffmanTree_noControlSymbol2");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ldx	controlSymbolCodeLength,y");
BitMagic.AsmTemplate.Template.WriteLiteral($@";ldy:inc	nBitCode_controlOffset,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ldy nBitCode_controlOffset,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@"inc	nBitCode_controlOffset,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta	codeToControlSymbol,y");
BitMagic.AsmTemplate.Template.WriteLiteral($@"tay");

BitMagic.AsmTemplate.Template.WriteLiteral($@".buildHuffmanTree_noControlSymbol2:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"iny");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bne	buildHuffmanTree_assignCode");
BitMagic.AsmTemplate.Template.WriteLiteral($@"rts");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; Read Huffman code using the primary tree");
BitMagic.AsmTemplate.Template.WriteLiteral($@".fetchPrimaryCode:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ldx	#PRIMARY_TREE");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; Read a code from input using the tree specified in X,");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; return low byte of this code in A,");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; return C flag reset for literal code, set for length code");
BitMagic.AsmTemplate.Template.WriteLiteral($@".fetchCode:");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	ldy	#0");
BitMagic.AsmTemplate.Template.WriteLiteral($@"tya");

BitMagic.AsmTemplate.Template.WriteLiteral($@".fetchCode_nextBit:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	getBit");
BitMagic.AsmTemplate.Template.WriteLiteral($@"rol	");
BitMagic.AsmTemplate.Template.WriteLiteral($@"inx");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcs	fetchCode_ge256");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; are all 256 literal codes of this length?");
BitMagic.AsmTemplate.Template.WriteLiteral($@"cpx	allLiteralsCodeLength");
BitMagic.AsmTemplate.Template.WriteLiteral($@"beq	fetchCode_allLiterals");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; is it literal code of length X?");
BitMagic.AsmTemplate.Template.WriteLiteral($@";sub	nBitCode_literalCount,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sec");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sbc	nBitCode_literalCount,x");

BitMagic.AsmTemplate.Template.WriteLiteral($@"bcs	fetchCode_notLiteral");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; literal code");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	clc");
BitMagic.AsmTemplate.Template.WriteLiteral($@"adc	nBitCode_literalOffset,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@"tax");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda	codeToLiteralSymbol,x");

BitMagic.AsmTemplate.Template.WriteLiteral($@".fetchCode_allLiterals:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"clc");
BitMagic.AsmTemplate.Template.WriteLiteral($@"rts");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; code >= 256, must be control");
BitMagic.AsmTemplate.Template.WriteLiteral($@".fetchCode_ge256:");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	sec");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sbc	nBitCode_literalCount,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sec");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; is it control code of length X?");
BitMagic.AsmTemplate.Template.WriteLiteral($@".fetchCode_notLiteral:");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	sec");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sbc	nBitCode_controlCount,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcs	fetchCode_nextBit");
BitMagic.AsmTemplate.Template.WriteLiteral($@"; control code");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	clc");
BitMagic.AsmTemplate.Template.WriteLiteral($@"adc	nBitCode_controlOffset,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@"tax");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda	codeToControlSymbol,x");
BitMagic.AsmTemplate.Template.WriteLiteral($@"and	#$1f	; make distance symbols zero-based");
BitMagic.AsmTemplate.Template.WriteLiteral($@"tax");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	sec");
BitMagic.AsmTemplate.Template.WriteLiteral($@"rts");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; Read A minus 1 bits, but no more than 8");
BitMagic.AsmTemplate.Template.WriteLiteral($@".getAMinus1BitsMax8:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"rol	getBits_base");
BitMagic.AsmTemplate.Template.WriteLiteral($@"tax");
BitMagic.AsmTemplate.Template.WriteLiteral($@"cmp	#9");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcs	getByte");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda	getNPlus1Bits_mask-2,x");

BitMagic.AsmTemplate.Template.WriteLiteral($@".getBits:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	getBits_loop");

BitMagic.AsmTemplate.Template.WriteLiteral($@".getBits_normalizeLoop:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lsr	getBits_base");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ror	");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcc	getBits_normalizeLoop");
BitMagic.AsmTemplate.Template.WriteLiteral($@"rts");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; Read 16 bits");
BitMagic.AsmTemplate.Template.WriteLiteral($@".getWord:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	getByte");
BitMagic.AsmTemplate.Template.WriteLiteral($@"tax");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; Read 8 bits");
BitMagic.AsmTemplate.Template.WriteLiteral($@".getByte:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda	#$80");

BitMagic.AsmTemplate.Template.WriteLiteral($@".getBits_loop:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"jsr	getBit");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ror	");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bcc	getBits_loop");
BitMagic.AsmTemplate.Template.WriteLiteral($@"rts");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; Read one bit, return in the C flag");
BitMagic.AsmTemplate.Template.WriteLiteral($@".getBit:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lsr	getBit_buffer");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bne	getBit_return");
BitMagic.AsmTemplate.Template.WriteLiteral($@"pha");
BitMagic.AsmTemplate.Template.WriteLiteral($@";	ldy	#0");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda	(inputPointer),y");

BitMagic.AsmTemplate.Template.WriteLiteral($@";inw	inputPointer");
BitMagic.AsmTemplate.Template.WriteLiteral($@"inc	inputPointer");
BitMagic.AsmTemplate.Template.WriteLiteral($@"bne skip_3");
BitMagic.AsmTemplate.Template.WriteLiteral($@"inc inputPointer+1");
BitMagic.AsmTemplate.Template.WriteLiteral($@".skip_3:");
        
BitMagic.AsmTemplate.Template.WriteLiteral($@"sec");
BitMagic.AsmTemplate.Template.WriteLiteral($@"ror");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta	getBit_buffer");
BitMagic.AsmTemplate.Template.WriteLiteral($@"pla");

BitMagic.AsmTemplate.Template.WriteLiteral($@".getBit_return:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"rts");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; Copy a previously written byte");
BitMagic.AsmTemplate.Template.WriteLiteral($@".copyByte:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #1");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta CTRL");

BitMagic.AsmTemplate.Template.WriteLiteral($@"inc ADDRx_L");
BitMagic.AsmTemplate.Template.WriteLiteral($@"dec ADDRx_L");

BitMagic.AsmTemplate.Template.WriteLiteral($@"stz CTRL");

BitMagic.AsmTemplate.Template.WriteLiteral($@"lda DATA1");
BitMagic.AsmTemplate.Template.WriteLiteral($@".skip:");

BitMagic.AsmTemplate.Template.WriteLiteral($@"; Write a byte");
BitMagic.AsmTemplate.Template.WriteLiteral($@".storeByte:");

BitMagic.AsmTemplate.Template.WriteLiteral($@"sta DATA0");

BitMagic.AsmTemplate.Template.WriteLiteral($@"lda ADDRx_H");
BitMagic.AsmTemplate.Template.WriteLiteral($@"pha");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda ADDRx_M");
BitMagic.AsmTemplate.Template.WriteLiteral($@"pha");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda ADDRx_L");
BitMagic.AsmTemplate.Template.WriteLiteral($@"pha");

BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #$01");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta ADDRx_H");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda #$fa");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta ADDRx_M");
BitMagic.AsmTemplate.Template.WriteLiteral($@"stz ADDRx_L");
BitMagic.AsmTemplate.Template.WriteLiteral($@"lda DATA0");
BitMagic.AsmTemplate.Template.WriteLiteral($@"eor #$0f");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta DATA0");

BitMagic.AsmTemplate.Template.WriteLiteral($@"pla");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta ADDRx_L");
BitMagic.AsmTemplate.Template.WriteLiteral($@"pla ");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta ADDRx_M");
BitMagic.AsmTemplate.Template.WriteLiteral($@"pla");
BitMagic.AsmTemplate.Template.WriteLiteral($@"sta ADDRx_H");

BitMagic.AsmTemplate.Template.WriteLiteral($@".storeByte_return:");
BitMagic.AsmTemplate.Template.WriteLiteral($@"rts            ");

BitMagic.AsmTemplate.Template.WriteLiteral($@".getNPlus1Bits_mask:");
BitMagic.AsmTemplate.Template.WriteLiteral($@".byte $81, $82, $84, $88, $90, $a0, $c0");

BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateDynamic_tempSymbols:");
BitMagic.AsmTemplate.Template.WriteLiteral($@".byte $82,$84,$c0,0,8,7,9,6,10,5,11,4,12,3,13,2,14,1,15");

BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateDynamic_headerBits:");
BitMagic.AsmTemplate.Template.WriteLiteral($@".byte $88, $90, $90");
BitMagic.AsmTemplate.Template.WriteLiteral($@".inflateDynamic_headerBase:");
BitMagic.AsmTemplate.Template.WriteLiteral($@".byte	3,{1+29+2},0");
        
BitMagic.AsmTemplate.Template.WriteLiteral($@".endproc        ");
BitMagic.AsmTemplate.Template.WriteLiteral($@".endscope");
    }

}
