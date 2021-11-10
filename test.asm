


.const data = $abcd

.scope main

; entrypoint
.byte	$0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00

.loop:
ldx 0x05
jsr set_white
dex
bne loop

.proc set_white
lda #<65520
sta data
lda #>65520
sta data
rts
.proc set_grey
lda #<34944
sta data
lda #>34944
sta data
rts

.sintable:
.byte	$80, $98, $AF, $C5, $DA, $EA, $F5, $FD, $00, $FD, $F6, $EB, $DA, $C7, $B2, $9A
.byte	$80, $67, $50, $3A, $25, $15, $0A, $02, $00, $02, $09, $14, $25, $38, $4D, $65


.datatable:
.byte	$00, $02, $04, $06

.somefile:
.byte	$2F, $2F, $2F, $2F, $2F, $2F, $2F, $2F, $2F, $2F

.word	$FFFF, $0000, $FF00
.word	$03E8, $FC18

.endscope
 
