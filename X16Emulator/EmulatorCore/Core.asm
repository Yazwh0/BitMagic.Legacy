
.CODE
ALIGN 16

register_a				equ 0
register_x				equ 4
register_y				equ 8
register_pc				equ 12
clock					equ 16
flags_carry				equ 20
flags_zero				equ 21
flags_interruptDisable	equ 22
flags_decimal			equ 23
flags_break				equ 24
flags_overflow			equ 25
flags_negative			equ 26


; -----------------------------
; Set flags
; -----------------------------

update_nz_flags macro

	;popf
	;test al, al
	;pushf

	test al, al
	lahf
	mov r15, rax

endm

; todo: move overflow from flags to the state obj
write_state_obj macro	
	mov	[rdx+register_a], r8d		; a
	mov	[rdx+register_x], r9d		; x
	mov	[rdx+register_y], r10d		; y
	mov	[rdx+register_pc], r11d		; PC
	mov [rdx+clock], r14			; Clock

	; Flags
	; read from r15 directly

	; Carry
	mov rax, r15
	;        NZ A P C
	and rax, 0000000100000000b
	mov byte ptr [rdx+flags_carry], al

	; Zero
	mov rax, r15
	;        NZ A P C
	and rax, 0100000000000000b
	ror rax, 6+8
	mov byte ptr [rdx+flags_zero], al

	; Negative
	mov rax, r15
	;        NZ A P C
	and rax, 1000000000000000b
	ror rax, 7+8
	mov byte ptr [rdx+flags_negative], al

endm

read_state_obj macro
	local no_carry, no_zero, no_overflow, no_negative

	mov r8d, [rdx+register_a]		; a
	mov r9d, [rdx+register_x]		; x
	mov r10d, [rdx+register_y]		; y
	mov r11d, [rdx+register_pc]		; PC
	mov r14, [rdx+clock]			; Clock

	; Flags
	xor r15, r15 ; clear flags register
	
	; unnecesary now we clear
	;        NZ A P C
	;and r15, 0011111000000000b

	mov al, byte ptr [rdx+flags_carry]
	test al, al
	jz no_carry
	;       NZ A P C
	or r15, 0000000100000000b
no_carry:

	mov al, byte ptr [rdx+flags_zero]
	test al, al
	jz no_zero
	;       NZ A P C
	or r15, 0100000000000000b
no_zero:

	mov al, byte ptr [rdx+flags_negative]
	test al, al
	jz no_negative
	;       NZ A P C
	or r15, 1000000000000000b
no_negative:

endm

store_registers macro
	push rbx
	push rbp
	push rdi
	push rsi
	push r12
	push r13
	push r14
	push r15
endm

restore_registers macro
	pop r15
	pop r14
	pop r13
	pop r12
	pop rsi
	pop rdi
	pop rbp
	pop rbx
endm

; rax : scratch
; rbx : scratch
; rcx : memory
; rdx : state = {
;         a
;         x
;         y
;         PC
;         Flags }
; r8b  : a
; r9b  : x
; r10b : y
; r11w : PC
; r12w : Write Location
; r13w : Read Location
; r14  : Clock Ticks
; r15  : Flags

asm_func proc memory:QWORD, state:QWORD
	
	store_registers

	push rdx
	push rcx

	; see if lahf is supported. if not return -1.
	mov eax, 80000001h
    cpuid
    test ecx,1           ;Is bit 0 (the "LAHF-SAHF" bit) set?
    je not_supported     ; no, LAHF is not supported

	pop rcx
	pop rdx

	read_state_obj

;	mov byte ptr [rcx + 0810h], 0adh	; LDA zp
;	mov byte ptr [rcx + 0811h], 000h	; $400
;	mov byte ptr [rcx + 0812h], 004h	; 

;	mov byte ptr [rcx + 0812h], 085h	; STA zp
;	mov byte ptr [rcx + 0813h], 010h	; $10

;	mov byte ptr [rcx + 0813h], 0dbh	; stp


main_loop:
	xor rax, rax
	mov al, [rcx+r11]			; Get opcode
	inc r11						; PC+1
	lea rbx, opcode_00			; start of jump table

	jmp qword ptr [rbx + rax*8]	; jump to opcode

opcode_done::

	jmp main_loop

exit_loop: ; how do we get here?
	
	; return all ok
	write_state_obj
	mov rax, 00h

	restore_registers
	;leave masm adds this.
	ret

not_supported:
	mov rax, -1
	ret
asm_func ENDP


; -----------------------------
; Read Memory
; -----------------------------
; Put value into al
; and increment PC.

read_imm macro
	mov al, [rcx+r11]	; Get value at PC
	inc r11				; Inc PC for param
endm

read_zp macro
	xor rbx, rbx
	mov bl, [rcx+r11]	; Get address in ZP
	mov al, [rcx+rbx]   ; Get value in ZP
	inc r11				; Inc PC for param
endm

read_zpx macro
	xor rbx, rbx
	mov bl, [rcx+r11]	; Get address in ZP
	add bl, r9b			; Add X. Byte operation so it wraps.

	mov al, [rcx+rbx]   ; Get Value in ZP
	inc r11				; Inc PC for param
endm

read_zpy macro
	xor rbx, rbx
	mov bl, [rcx+r11]	; Get address in ZP
	add bl, r10b		; Add Y. Byte operation so it wraps.

	mov al, [rcx+rbx]   ; Get Value in ZP
	inc r11				; Inc PC for param
endm

read_abs macro
	xor rbx, rbx
	mov bx, [rcx+r11]	; Get 16bit value in memory.
	mov al, [rcx+rbx]	; Get value at that address

	inc r11				; Inc PC twice for param
	inc r11
endm

read_absx macro
	local no_overflow

	xor rbx, rbx	
	mov bx, [rcx+r11]	; Get 16bit address in memory.

	adc bl, r9b			; Add X to lower address byte
	jnc no_overflow
	inc bh				; Inc higher address byte
	inc r14				; Add clock cycle
	clc

no_overflow:
	mov al, [rcx+rbx]	; Get value in memory
	inc r11				; Inc PC for param
	inc r11
endm

read_absy macro
	local no_overflow

	xor rbx, rbx
	mov bx, [rcx+r11]	; Get 16bit address in memory

	adc bl, r10b		; Add Y to the lower address byte
	jnc no_overflow
	inc bh				; Inc higher address byte
	inc r14				; Add clock cycle
	clc

no_overflow:
	mov al, [rcx+rbx]	; Get value in memory
	inc r11				; Inc PC for param
	inc r11
endm

read_indx macro
	xor rbx, rbx
	mov bl, [rcx+r11]	; Address in ZP
	add bl, r9b			; Add on X. Byte operation so it wraps.
	mov bx, [rcx+rbx]	; Address at location
	mov al, [rcx+rbx]	; Final value
	inc r11				; Inc PC for param
endm

read_indy macro
	local no_overflow
	xor rbx, rbx
	mov bl, [rcx+r11]	; Address in ZP
	mov bx, [rcx+rbx]	; Address pointed at in ZP

	adc bl, r10b		; Add Y to the lower address byte
	jnc no_overflow
	inc bh				; Inc higher address byte
	inc r14				; Add clock cycle
	clc

no_overflow:
	mov al, [rcx+rbx]	; Final value	
	inc r11				; Inc PC for param
endm

; -----------------------------
; Write Memory
; -----------------------------
; Put al into memory
; and increment PC.

write_zp macro
	xor rbx, rbx
	mov bl, [rcx+r11]	; ZP address
	mov [rcx+rbx], al
	inc r11
endm

write_zpx macro
	xor rbx, rbx
	mov bl, [rcx+r11]	; ZP address
	add bl, r9b			; Add X
	mov [rcx+rbx], al  
	inc r11
endm

write_zpy macro
	xor rbx, rbx
	mov bl, [rcx+r11]	; ZP address
	add bl, r10b		; Add Y
	mov [rcx+rbx], al  
	inc r11
endm

write_abs macro
	xor rbx, rbx		; Clear b
	mov bx, [rcx+r11]	; Get address
	mov [rcx+rbx], al	; Update

	add r11, 2			; Increment PC twice
endm

write_absx macro
	xor rbx, rbx		; Clear b
	mov bx, [rcx+r11]	; Get Address
	add bx, r9w			; Add X
	mov [rcx+rbx], al	; Update

	add r11, 2			; Increment PC twice
endm

write_absy macro
	xor rbx, rbx		; Clear b
	mov bx, [rcx+r11]	; Get Address
	add bx, r10w		; Add Y
	mov [rcx+rbx], al	; Update

	add r11, 2			; Increment PC twice
endm

write_indx macro
	xor rbx, rbx		; Clear b
	mov bl, [rcx+r11]	; Get address
	add bl, r9b			; Add X, use bl so it wraps
	mov bx, [rcx+rbx]	; Get destination address
	mov [rcx+rbx], al	; Update

	inc r11				; Increment PC
endm

write_indy macro
	xor rbx, rbx		; Clear b
	mov bl, [rcx+r11]	; Get address
	mov bx, [rcx+rbx]	; Get destination address
	add bx, r10w		; Add Y
	mov [rcx+rbx], al	; Update

	inc r11				; Increment PC
endm

; -----------------------------
; Op Codes
; -----------------------------
; No need to increment PC for opcode


; -----------------------------
; LDA
; -----------------------------

xA9_lda_imm PROC

	read_imm
	mov	r8b, al
	update_nz_flags

	add r14, 2

	jmp opcode_done

xA9_lda_imm ENDP

xA5_lda_zp PROC
	
	read_zp
	mov r8b, al
	update_nz_flags

	add r14, 3

	jmp opcode_done

xA5_lda_zp ENDP

xB5_lda_zpx PROC

	read_zpx
	mov r8b, al
	update_nz_flags

	add r14, 4

	jmp opcode_done

xB5_lda_zpx endp

xAD_lda_abs proc

	read_abs
	mov r8b, al
	update_nz_flags

	add r14, 4

	jmp opcode_done

xAD_lda_abs endp

xBD_lda_absx proc

	read_absx
	mov r8b, al
	update_nz_flags

	add r14, 4

	jmp opcode_done

xBD_lda_absx endp

xB9_lda_absy proc

	read_absy
	mov r8b, al
	update_nz_flags

	add r14, 4

	jmp opcode_done

xB9_lda_absy endp

xA1_lda_indx proc

	read_indx
	mov r8b, al
	update_nz_flags

	add r14, 6

	jmp opcode_done

xA1_lda_indx endp

xB1_lda_indy proc
	read_indy
	mov r8b, al
	update_nz_flags

	add r14, 5

	jmp opcode_done

xB1_lda_indy endp


; -----------------------------
; LDX
; -----------------------------

xA2_ldx_imm PROC

	read_imm
	mov	r9b, al
	update_nz_flags

	add r14, 2

	jmp opcode_done

xA2_ldx_imm ENDP

xA6_ldx_zp PROC
	
	read_zp
	mov r9b, al
	update_nz_flags

	add r14, 3

	jmp opcode_done

xA6_ldx_zp  ENDP

xB6_ldx_zpy PROC

	read_zpy
	mov r9b, al
	update_nz_flags

	add r14, 4

	jmp opcode_done

xB6_ldx_zpy endp

xAE_ldx_abs proc

	read_abs
	mov r9b, al
	update_nz_flags

	add r14, 4

	jmp opcode_done

xAE_ldx_abs endp

xBE_ldx_absy proc

	read_absy
	mov r9b, al
	update_nz_flags

	add r14, 4

	jmp opcode_done

xBE_ldx_absy endp

; -----------------------------
; LDY
; -----------------------------

xA0_ldy_imm PROC

	read_imm
	mov	r10b, al
	update_nz_flags

	add r14, 2

	jmp opcode_done

xA0_ldy_imm ENDP

xA4_ldy_zp PROC
	
	read_zp
	mov r10b, al
	update_nz_flags

	add r14, 3

	jmp opcode_done

xA4_ldy_zp  ENDP

xB4_ldy_zpx PROC

	read_zpx
	mov r10b, al
	update_nz_flags

	add r14, 4

	jmp opcode_done

xB4_ldy_zpx endp

xAC_ldy_abs proc

	read_abs
	mov r10b, al
	update_nz_flags

	add r14, 4

	jmp opcode_done

xAC_ldy_abs endp

xBC_ldy_absx proc

	read_absx
	mov r10b, al
	update_nz_flags

	add r14, 4

	jmp opcode_done

xBC_ldy_absx endp


; -----------------------------
; STA
; -----------------------------

x85_sta_zp proc
	
	mov al, r8b
	write_zp

	add r14, 3

	jmp opcode_done

x85_sta_zp endp

x95_sta_zpx proc

	mov al, r8b
	write_zpx

	add r14, 4

	jmp opcode_done

x95_sta_zpx endp

x8D_sta_abs proc

	mov al, r8b
	write_abs

	add r14, 4

	jmp opcode_done

x8D_sta_abs endp

x9D_sta_absx proc

	mov al, r8b
	write_absx

	add r14,5

	jmp opcode_done

x9D_sta_absx endp

x99_sta_absy proc

	mov al, r8b
	write_absy

	add r14,5

	jmp opcode_done

x99_sta_absy endp

x81_sta_indx proc

	mov al, r8b
	write_indx

	add r14,6

	jmp opcode_done

x81_sta_indx endp

x91_sta_indy proc

	mov al, r8b
	write_indy

	add r14,6

	jmp opcode_done

x91_sta_indy endp

;
; STX
;

x86_stx_zp proc

	mov al, r9b
	write_zp

	add r14, 3

	jmp opcode_done

x86_stx_zp endp

x96_stx_zpy proc

	mov al, r9b
	write_zpy

	add r14, 4

	jmp opcode_done

x96_stx_zpy endp

x8E_stx_abs proc

	mov al, r9b
	write_abs

	add r14, 4

	jmp opcode_done

x8E_stx_abs endp

;
; STY
;

x84_sty_zp proc

	mov al, r10b
	write_zp

	add r14, 3

	jmp opcode_done

x84_sty_zp endp

x94_sty_zpx proc

	mov al, r10b
	write_zpx

	add r14, 4

	jmp opcode_done

x94_sty_zpx endp

x8C_sty_abs proc

	mov al, r10b
	write_abs

	add r14, 4

	jmp opcode_done

x8C_sty_abs endp

;
; STZ
;

x64_stz_zp proc
	
	xor rax, rax
	write_zp

	add r14, 3

	jmp opcode_done

x64_stz_zp endp

x74_stz_zpx proc

	xor rax, rax
	write_zpx

	add r14, 4

	jmp opcode_done

x74_stz_zpx endp

x9C_stz_abs proc

	xor rax, rax
	write_abs

	add r14, 4

	jmp opcode_done

x9C_stz_abs endp

x9E_stz_absx proc

	xor rax, rax
	write_absx

	add r14,5

	jmp opcode_done

x9E_stz_absx endp

;
; Register Flags
;

xE8_inx proc

	mov rax, r15	; move flags to rax
	sahf			; set eflags
	inc r9b
	test r9b, r9b	; this does blatt the overflow register. Is that correct? -- mabe move overflow out of flags?
	lahf			; move new flags to rax
	mov r15, rax	; store

	add r14, 2		; Clock

	jmp opcode_done		

xE8_inx endp

xCA_dex proc

	mov rax, r15	; move flags to rax
	sahf			; set eflags
	dec r9b
	test r9b, r9b	; this does blatt the overflow register. Is that correct? -- mabe move overflow out of flags?
	lahf			; move new flags to rax
	mov r15, rax	; store

	add r14, 2	; Clock

	jmp opcode_done		

xCA_dex endp

xC8_iny proc

	mov rax, r15	; move flags to rax
	sahf			; set eflags
	inc r10b
	test r10b, r10b ; this does blatt the overflow register. Is that correct? -- mabe move overflow out of flags?
	lahf			; move new flags to rax
	mov r15, rax	; store

	add r14, 2	; Clock

	jmp opcode_done		

xC8_iny endp

x88_dey proc

	mov rax, r15	; move flags to rax
	sahf			; set eflags
	dec r10b
	test r10b, r10b ; this does blatt the overflow register. Is that correct? -- mabe move overflow out of flags?
	lahf			; move new flags to rax
	mov r15, rax	; store

	add r14, 2	; Clock

	jmp opcode_done		

x88_dey endp

xAA_tax proc

	mov rax, r15	; move flags to rax
	sahf			; set eflags
	mov	r9, r8		; A -> X
	test r9b, r9b
	lahf			; move new flags to rax
	mov r15, rax	; store

	add r14, 2 ; Clock

	jmp opcode_done	

xAA_tax endp

x8A_txa proc

	mov rax, r15	; move flags to rax
	sahf			; set eflags
	mov	r8, r9		; X -> A
	test r8b, r8b
	lahf			; move new flags to rax
	mov r15, rax	; store

	add r14, 2 ; Clock

	jmp opcode_done	

x8A_txa endp

xA8_tay proc

	mov rax, r15	; move flags to rax
	sahf			; set eflags
	mov	r10, r8		; A -> Y
	test r10b, r10b
	lahf			; move new flags to rax
	mov r15, rax	; store

	add r14, 2 ; Clock

	jmp opcode_done	

xA8_tay endp

x98_tya proc

	mov rax, r15	; move flags to rax
	sahf			; set eflags
	mov	r8, r10		; Y -> A
	test r8b, r8b
	lahf			; move new flags to rax
	mov r15, rax	; store

	add r14, 2		; Clock

	jmp opcode_done	

x98_tya endp

;
; Branches
;

xD0_bne proc
	mov rax, r15	; move flags to rax
	sahf			; set eflags

	jnz is_zero

	add r14, 2		; Clock
	inc r11			; move PC on

	jmp opcode_done	

is_zero:
	movsx bx, byte ptr [rcx+r11]	; Get value at PC and turn it into a 2byte signed value
	inc r11							; move PC on -- all jumps are relative
	mov rax, r11					; store PC
	add r11w, bx
	
	add r14, 3						; Clock

	mov rbx, r11
	cmp ah, bh						; test if the page has changed.
	jne page_change

	jmp opcode_done	

page_change:						; page change as a 1 cycle penalty
	inc r14
	jmp opcode_done

xD0_bne endp

;
; JMP
;

x4C_jmp_abs proc

	mov r11w, [rcx+r11]		; Get 16bit value in memory and set it to the clock

	add r14, 3

	jmp opcode_done

x4C_jmp_abs endp

x6C_jmp_ind proc

	mov r11w, [rcx+r11]		; Get 16bit value in memory and set it to the clock
	mov r11w, [rcx+r11]		; Get 16bit value at the new memory position, and set the clock to the final value

	add r14, 5

	jmp opcode_done

x6C_jmp_ind endp

x7C_jmp_absx proc

	mov r11w, [rcx+r11]		; Get 16bit value in memory and set it to the clock
	add	r11, r9				; Add on X
	mov r11w, [rcx+r11]		; Get 16bit value at the new memory position, and set the clock to the final value

	add r14, 6

	jmp opcode_done

x7C_jmp_absx endp

;
;
;

xDB_stp proc

	add r14, 3	; Clock

	; return stp was hit.
	write_state_obj
	mov rax, 02h

	restore_registers

	leave
	ret

xDB_stp endp

noinstruction PROC

	; return error	
	write_state_obj
	mov rax, 01h

	restore_registers

	leave
	ret
	
noinstruction ENDP

.DATA

; all opcodes, in order of value starting with 0x00
opcode_00	qword	noinstruction 	; $00
opcode_01	qword	noinstruction 	; $01
opcode_02	qword	noinstruction 	; $02
opcode_03	qword	noinstruction 	; $03
opcode_04	qword	noinstruction 	; $04
opcode_05	qword	noinstruction 	; $05
opcode_06	qword	noinstruction 	; $06
opcode_07	qword	noinstruction 	; $07
opcode_08	qword	noinstruction 	; $08
opcode_09	qword	noinstruction 	; $09
opcode_0A	qword	noinstruction 	; $0A
opcode_0B	qword	noinstruction 	; $0B
opcode_0C	qword	noinstruction 	; $0C
opcode_0D	qword	noinstruction 	; $0D
opcode_0E	qword	noinstruction 	; $0E
opcode_0F	qword	noinstruction 	; $0F
opcode_10	qword	noinstruction 	; $10
opcode_11	qword	noinstruction 	; $11
opcode_12	qword	noinstruction 	; $12
opcode_13	qword	noinstruction 	; $13
opcode_14	qword	noinstruction 	; $14
opcode_15	qword	noinstruction 	; $15
opcode_16	qword	noinstruction 	; $16
opcode_17	qword	noinstruction 	; $17
opcode_18	qword	noinstruction 	; $18
opcode_19	qword	noinstruction 	; $19
opcode_1A	qword	noinstruction 	; $1A
opcode_1B	qword	noinstruction 	; $1B
opcode_1C	qword	noinstruction 	; $1C
opcode_1D	qword	noinstruction 	; $1D
opcode_1E	qword	noinstruction 	; $1E
opcode_1F	qword	noinstruction 	; $1F
opcode_20	qword	noinstruction 	; $20
opcode_21	qword	noinstruction 	; $21
opcode_22	qword	noinstruction 	; $22
opcode_23	qword	noinstruction 	; $23
opcode_24	qword	noinstruction 	; $24
opcode_25	qword	noinstruction 	; $25
opcode_26	qword	noinstruction 	; $26
opcode_27	qword	noinstruction 	; $27
opcode_28	qword	noinstruction 	; $28
opcode_29	qword	noinstruction 	; $29
opcode_2A	qword	noinstruction 	; $2A
opcode_2B	qword	noinstruction 	; $2B
opcode_2C	qword	noinstruction 	; $2C
opcode_2D	qword	noinstruction 	; $2D
opcode_2E	qword	noinstruction 	; $2E
opcode_2F	qword	noinstruction 	; $2F
opcode_30	qword	noinstruction 	; $30
opcode_31	qword	noinstruction 	; $31
opcode_32	qword	noinstruction 	; $32
opcode_33	qword	noinstruction 	; $33
opcode_34	qword	noinstruction 	; $34
opcode_35	qword	noinstruction 	; $35
opcode_36	qword	noinstruction 	; $36
opcode_37	qword	noinstruction 	; $37
opcode_38	qword	noinstruction 	; $38
opcode_39	qword	noinstruction 	; $39
opcode_3A	qword	noinstruction 	; $3A
opcode_3B	qword	noinstruction 	; $3B
opcode_3C	qword	noinstruction 	; $3C
opcode_3D	qword	noinstruction 	; $3D
opcode_3E	qword	noinstruction 	; $3E
opcode_3F	qword	noinstruction 	; $3F
opcode_40	qword	noinstruction 	; $40
opcode_41	qword	noinstruction 	; $41
opcode_42	qword	noinstruction 	; $42
opcode_43	qword	noinstruction 	; $43
opcode_44	qword	noinstruction 	; $44
opcode_45	qword	noinstruction 	; $45
opcode_46	qword	noinstruction 	; $46
opcode_47	qword	noinstruction 	; $47
opcode_48	qword	noinstruction 	; $48
opcode_49	qword	noinstruction 	; $49
opcode_4A	qword	noinstruction 	; $4A
opcode_4B	qword	noinstruction 	; $4B
opcode_4C	qword	x4C_jmp_abs 	; $4C
opcode_4D	qword	noinstruction 	; $4D
opcode_4E	qword	noinstruction 	; $4E
opcode_4F	qword	noinstruction 	; $4F
opcode_50	qword	noinstruction 	; $50
opcode_51	qword	noinstruction 	; $51
opcode_52	qword	noinstruction 	; $52
opcode_53	qword	noinstruction 	; $53
opcode_54	qword	noinstruction 	; $54
opcode_55	qword	noinstruction 	; $55
opcode_56	qword	noinstruction 	; $56
opcode_57	qword	noinstruction 	; $57
opcode_58	qword	noinstruction 	; $58
opcode_59	qword	noinstruction 	; $59
opcode_5A	qword	noinstruction 	; $5A
opcode_5B	qword	noinstruction 	; $5B
opcode_5C	qword	noinstruction 	; $5C
opcode_5D	qword	noinstruction 	; $5D
opcode_5E	qword	noinstruction 	; $5E
opcode_5F	qword	noinstruction 	; $5F
opcode_60	qword	noinstruction 	; $60
opcode_61	qword	noinstruction 	; $61
opcode_62	qword	noinstruction 	; $62
opcode_63	qword	noinstruction 	; $63
opcode_64	qword	x64_stz_zp	 	; $64
opcode_65	qword	noinstruction 	; $65
opcode_66	qword	noinstruction 	; $66
opcode_67	qword	noinstruction 	; $67
opcode_68	qword	noinstruction 	; $68
opcode_69	qword	noinstruction 	; $69
opcode_6A	qword	noinstruction 	; $6A
opcode_6B	qword	noinstruction 	; $6B
opcode_6C	qword	x6C_jmp_ind 	; $6C
opcode_6D	qword	noinstruction 	; $6D
opcode_6E	qword	noinstruction 	; $6E
opcode_6F	qword	noinstruction 	; $6F
opcode_70	qword	noinstruction 	; $70
opcode_71	qword	noinstruction 	; $71
opcode_72	qword	noinstruction 	; $72
opcode_73	qword	noinstruction 	; $73
opcode_74	qword	x74_stz_zpx 	; $74
opcode_75	qword	noinstruction 	; $75
opcode_76	qword	noinstruction 	; $76
opcode_77	qword	noinstruction 	; $77
opcode_78	qword	noinstruction 	; $78
opcode_79	qword	noinstruction 	; $79
opcode_7A	qword	noinstruction 	; $7A
opcode_7B	qword	noinstruction 	; $7B
opcode_7C	qword	x7C_jmp_absx 	; $7C
opcode_7D	qword	noinstruction 	; $7D
opcode_7E	qword	noinstruction 	; $7E
opcode_7F	qword	noinstruction 	; $7F
opcode_80	qword	noinstruction 	; $80
opcode_81	qword	x81_sta_indx 	; $81
opcode_82	qword	noinstruction 	; $82
opcode_83	qword	noinstruction 	; $83
opcode_84	qword	x84_sty_zp	 	; $84
opcode_85	qword	x85_sta_zp	 	; $85
opcode_86	qword	x86_stx_zp	 	; $86
opcode_87	qword	noinstruction 	; $87
opcode_88	qword	x88_dey		 	; $88
opcode_89	qword	noinstruction 	; $89
opcode_8A	qword	x8A_txa		 	; $8A
opcode_8B	qword	noinstruction 	; $8B
opcode_8C	qword	x8C_sty_abs 	; $8C
opcode_8D	qword	x8D_sta_abs 	; $8D
opcode_8E	qword	x8E_stx_abs 	; $8E
opcode_8F	qword	noinstruction 	; $8F
opcode_90	qword	noinstruction 	; $90
opcode_91	qword	x91_sta_indy 	; $91
opcode_92	qword	noinstruction 	; $92
opcode_93	qword	noinstruction 	; $93
opcode_94	qword	x94_sty_zpx 	; $94
opcode_95	qword	x95_sta_zpx 	; $95
opcode_96	qword	x96_stx_zpy 	; $96
opcode_97	qword	noinstruction 	; $97
opcode_98	qword	x98_tya		 	; $98
opcode_99	qword	x99_sta_absy 	; $99
opcode_9A	qword	noinstruction 	; $9A
opcode_9B	qword	noinstruction 	; $9B
opcode_9C	qword	x9C_stz_abs 	; $9C
opcode_9D	qword	x9D_sta_absx 	; $9D
opcode_9E	qword	x9E_stz_absx 	; $9E
opcode_9F	qword	noinstruction 	; $9F
opcode_A0	qword	xA0_ldy_imm 	; $A0
opcode_A1	qword	xA1_lda_indx 	; $A1
opcode_A2	qword	xA2_ldx_imm 	; $A2
opcode_A3	qword	noinstruction 	; $A3
opcode_A4	qword	xA4_ldy_zp	 	; $A4
opcode_A5	qword	xA5_lda_zp	 	; $A5
opcode_A6	qword	xA6_ldx_zp	 	; $A6
opcode_A7	qword	noinstruction 	; $A7
opcode_A8	qword	xA8_tay		 	; $A8
opcode_A9	qword	xA9_lda_imm 	; $A9
opcode_AA	qword	xAA_tax		 	; $AA
opcode_AB	qword	noinstruction 	; $AB
opcode_AC	qword	xAC_ldy_abs 	; $AC
opcode_AD	qword	xAD_lda_abs 	; $AD
opcode_AE	qword	xAE_ldx_abs 	; $AE
opcode_AF	qword	noinstruction 	; $AF
opcode_B0	qword	noinstruction 	; $B0
opcode_B1	qword	xB1_lda_indy 	; $B1
opcode_B2	qword	noinstruction 	; $B2
opcode_B3	qword	noinstruction 	; $B3
opcode_B4	qword	xB4_ldy_zpx 	; $B4
opcode_B5	qword	xB5_lda_zpx 	; $B5
opcode_B6	qword	xB6_ldx_zpy 	; $B6
opcode_B7	qword	noinstruction 	; $B7
opcode_B8	qword	noinstruction 	; $B8
opcode_B9	qword	xB9_lda_absy 	; $B9
opcode_BA	qword	noinstruction 	; $BA
opcode_BB	qword	noinstruction 	; $BB
opcode_BC	qword	xBC_ldy_absx 	; $BC
opcode_BD	qword	xBD_lda_absx 	; $BD
opcode_BE	qword	xBE_ldx_absy 	; $BE
opcode_BF	qword	noinstruction 	; $BF
opcode_C0	qword	noinstruction 	; $C0
opcode_C1	qword	noinstruction 	; $C1
opcode_C2	qword	noinstruction 	; $C2
opcode_C3	qword	noinstruction 	; $C3
opcode_C4	qword	noinstruction 	; $C4
opcode_C5	qword	noinstruction 	; $C5
opcode_C6	qword	noinstruction 	; $C6
opcode_C7	qword	noinstruction 	; $C7
opcode_C8	qword	xC8_iny			; $C8
opcode_C9	qword	noinstruction 	; $C9
opcode_CA	qword	xCA_dex		 	; $CA
opcode_CB	qword	noinstruction 	; $CB
opcode_CC	qword	noinstruction 	; $CC
opcode_CD	qword	noinstruction 	; $CD
opcode_CE	qword	noinstruction 	; $CE
opcode_CF	qword	noinstruction 	; $CF
opcode_D0	qword	xD0_bne		 	; $D0
opcode_D1	qword	noinstruction 	; $D1
opcode_D2	qword	noinstruction 	; $D2
opcode_D3	qword	noinstruction 	; $D3
opcode_D4	qword	noinstruction 	; $D4
opcode_D5	qword	noinstruction 	; $D5
opcode_D6	qword	noinstruction 	; $D6
opcode_D7	qword	noinstruction 	; $D7
opcode_D8	qword	noinstruction 	; $D8
opcode_D9	qword	noinstruction 	; $D9
opcode_DA	qword	noinstruction 	; $DA
opcode_DB	qword	xDB_stp		 	; $DB
opcode_DC	qword	noinstruction 	; $DC
opcode_DD	qword	noinstruction 	; $DD
opcode_DE	qword	noinstruction 	; $DE
opcode_DF	qword	noinstruction 	; $DF
opcode_E0	qword	noinstruction 	; $E0
opcode_E1	qword	noinstruction 	; $E1
opcode_E2	qword	noinstruction 	; $E2
opcode_E3	qword	noinstruction 	; $E3
opcode_E4	qword	noinstruction 	; $E4
opcode_E5	qword	noinstruction 	; $E5
opcode_E6	qword	noinstruction 	; $E6
opcode_E7	qword	noinstruction 	; $E7
opcode_E8	qword	xE8_inx	 		; $E8
opcode_E9	qword	noinstruction 	; $E9
opcode_EA	qword	noinstruction 	; $EA
opcode_EB	qword	noinstruction 	; $EB
opcode_EC	qword	noinstruction 	; $EC
opcode_ED	qword	noinstruction 	; $ED
opcode_EE	qword	noinstruction 	; $EE
opcode_EF	qword	noinstruction 	; $EF
opcode_F0	qword	noinstruction 	; $F0
opcode_F1	qword	noinstruction 	; $F1
opcode_F2	qword	noinstruction 	; $F2
opcode_F3	qword	noinstruction 	; $F3
opcode_F4	qword	noinstruction 	; $F4
opcode_F5	qword	noinstruction 	; $F5
opcode_F6	qword	noinstruction 	; $F6
opcode_F7	qword	noinstruction 	; $F7
opcode_F8	qword	noinstruction 	; $F8
opcode_F9	qword	noinstruction 	; $F9
opcode_FA	qword	noinstruction 	; $FA
opcode_FB	qword	noinstruction 	; $FB
opcode_FC	qword	noinstruction 	; $FC
opcode_FD	qword	noinstruction 	; $FD
opcode_FE	qword	noinstruction 	; $FE
opcode_FF	qword	noinstruction 	; $FF



END