;    Copyright (C) 2022 BJ

;    This program is free software: you can redistribute it and/or modify
;    it under the terms of the GNU General Public License as published by
;    the Free Software Foundation, either version 3 of the License, or
;    (at your option) any later version.

;    This program is distributed in the hope that it will be useful,
;    but WITHOUT ANY WARRANTY; without even the implied warranty of
;    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
;    GNU General Public License for more details.

;    You should have received a copy of the GNU General Public License
;    along with this program.  If not, see https://www.gnu.org/licenses/.

.CODE
ALIGN 16

register_a				equ 0
register_x				equ 4
register_y				equ 8
register_pc				equ 12
stackpointer			equ 16
flags_decimal			equ 20
flags_break				equ 21
flags_overflow			equ 22
flags_negative			equ 23
clock					equ 24 ; needs to be aligned, so but flags above

flags_carry				equ 32
flags_zero				equ 33
flags_interruptDisable	equ 34


; -----------------------------
; Set flags
; -----------------------------

update_nz_flags macro

	test al, al
	lahf
	mov r15, rax

endm

update_nz_flags_a macro
	test r8b, r8b
	lahf
	mov r15, rax
endm

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
	ror rax, 8
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

	; !!!
	;add byte ptr [rcx + 01h], 1
	

;	mov byte ptr [rcx + 0810h], 0adh	; LDA zp
;	mov byte ptr [rcx + 0811h], 000h	; $400
;	mov byte ptr [rcx + 0812h], 004h	; 

;	mov byte ptr [rcx + 0812h], 085h	; STA zp
;	mov byte ptr [rcx + 0813h], 010h	; $10

;	mov byte ptr [rcx + 0813h], 0dbh	; stp


main_loop:
	xor rax, rax
	mov al, [rcx+r11]			; Get opcode
	add r11w, 1						; PC+1
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

read_zp_rbx macro
	xor rbx, rbx
	mov bl, [rcx+r11]	; Get 8bit value in memory.
endm

read_zpx_rbx macro
	xor rbx, rbx
	mov bl, [rcx+r11]	; Get 8bit value in memory.
	add bl, r9b			; Add X
endm

read_zpy_rbx macro
	xor rbx, rbx
	mov bl, [rcx+r11]	; Get 8bit value in memory.
	add bl, r10b		; Add Y
endm

read_abs_rbx macro
	xor rbx, rbx
	mov bx, [rcx+r11]	; Get 16bit value in memory.
endm

read_absx_rbx macro
	xor rbx, rbx
	mov bx, [rcx+r11]	; Get 16bit value in memory.
	add bx, r9w			; Add X
endm

read_absx_rbx_pagepenalty macro
	local no_overflow
	xor rbx, rbx
	mov bx, [rcx+r11]	; Get 16bit value in memory.
	add bl, r9b			; Add X
	jnc no_overflow
	add bh, 1			; Add high bit
	add r14, 1			; Add cycle penatly
	;clc					; Clear carry

no_overflow:
endm

read_absy_rbx macro
	xor rbx, rbx
	mov bx, [rcx+r11]	; Get 16bit value in memory.
	add bx, r10w		; Add Y
endm

read_absy_rbx_pagepenalty macro
	local no_overflow
	xor rbx, rbx
	mov bx, [rcx+r11]	; Get 16bit value in memory.
	add bl, r10b		; Add Y
	jnc no_overflow
	add bh, 1			; Add high bit
	add r14, 1			; Add cycle penatly
	;clc					; Clear carry

no_overflow:
endm

read_indx_rbx macro
	xor rbx, rbx
	mov bl, [rcx+r11]	; Address in ZP
	add bl, r9b			; Add on X. Byte operation so it wraps.
	mov bx, [rcx+rbx]	; Address at location
endm

read_indy_rbx_pagepenalty macro
	local no_overflow
	xor rbx, rbx
	mov bl, [rcx+r11]	; Address in ZP
	mov bx, [rcx+rbx]	; Address pointed at in ZP

	adc bl, r10b		; Add Y to the lower address byte
	jnc no_overflow
	add bh, 1			; Inc higher address byte
	add r14, 1			; Add clock cycle
	clc

no_overflow:
endm

read_indy_rbx macro
	local no_overflow
	xor rbx, rbx
	mov bl, [rcx+r11]	; Address in ZP
	mov bx, [rcx+rbx]	; Address pointed at in ZP
	add bl, r10b		; Add Y to the lower address byte
endm

read_indzp_rbx macro
	xor rbx, rbx
	mov bl, [rcx+r11]	; Address in ZP
	mov bx, [rcx+rbx]	; Address at location
endm

; -----------------------------
; Flags
; -----------------------------

read_flags_rax macro
	mov rax, r15	; move flags to rax
	sahf			; set eflags
endm

write_flags_r15 macro
	lahf			; move new flags to rax
	mov r15, rax	; store
endm

; we dont use stc, as its actually slower!
write_flags_r15_preservecarry macro
	lahf						; move new flags to rax
	and r15w, 0100h				; preserve carry		
	or r15w, ax					; store flags over (carry is always clear)
endm

write_flags_r15_setnegative macro
	lahf						; move new flags to rax
	or rax, 1000000000000000b	; set negative flag
	mov r15, rax				; store
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
	add r11w, 1
endm

write_zpx macro
	xor rbx, rbx
	mov bl, [rcx+r11]	; ZP address
	add bl, r9b			; Add X
	mov [rcx+rbx], al  
	add r11w, 1
endm

write_zpy macro
	xor rbx, rbx
	mov bl, [rcx+r11]	; ZP address
	add bl, r10b		; Add Y
	mov [rcx+rbx], al  
	add r11w, 1
endm

write_abs macro
	xor rbx, rbx		; Clear b
	mov bx, [rcx+r11]	; Get address
	mov [rcx+rbx], al	; Update

	add r11w, 2			; Increment PC
endm

write_absx macro
	xor rbx, rbx		; Clear b
	mov bx, [rcx+r11]	; Get Address
	add bx, r9w			; Add X
	mov [rcx+rbx], al	; Update

	add r11w, 2			; Increment PC
endm

write_absy macro
	xor rbx, rbx		; Clear b
	mov bx, [rcx+r11]	; Get Address
	add bx, r10w		; Add Y
	mov [rcx+rbx], al	; Update

	add r11w, 2			; Increment PC
endm

write_indx macro
	xor rbx, rbx		; Clear b
	mov bl, [rcx+r11]	; Get address
	add bl, r9b			; Add X, use bl so it wraps
	mov bx, [rcx+rbx]	; Get destination address
	mov [rcx+rbx], al	; Update

	add r11w, 1				; Increment PC
endm

write_indy macro
	xor rbx, rbx		; Clear b
	mov bl, [rcx+r11]	; Get address
	mov bx, [rcx+rbx]	; Get destination address
	add bx, r10w		; Add Y
	mov [rcx+rbx], al	; Update

	add r11w, 1				; Increment PC
endm

write_indzp macro
	xor rbx, rbx		; Clear b
	mov bl, [rcx+r11]	; Get address
	mov bx, [rcx+rbx]	; Get destination address
	mov [rcx+rbx], al	; Update

	add r11w, 1				; Increment PC
endm

; -----------------------------
; Op Codes
; -----------------------------
; No need to increment PC for opcode


; -----------------------------
; LDA
; -----------------------------

lda_body_end macro clock, pc
	test r8b, r8b
	lahf
	and r15w, 0100h			; preserve carry		
	or r15w, ax				; store flags over (carry is always clear)

	add r14, clock
	add r11w, pc

	jmp opcode_done
endm

lda_body macro clock, pc
	mov r8b, [rcx+rbx]
	lda_body_end clock, pc
endm

xA9_lda_imm PROC
	mov	r8b, [rcx+r11]
	lda_body_end 2, 1
xA9_lda_imm ENDP

xA5_lda_zp PROC
	read_zp_rbx
	lda_body 3, 1
xA5_lda_zp ENDP

xB5_lda_zpx PROC
	read_zpx_rbx
	lda_body 4, 1
xB5_lda_zpx endp

xAD_lda_abs proc
	read_abs_rbx
	lda_body 4, 2
xAD_lda_abs endp

xBD_lda_absx proc
	read_absx_rbx_pagepenalty
	lda_body 4, 2
xBD_lda_absx endp

xB9_lda_absy proc
	read_absy_rbx_pagepenalty
	lda_body 4, 2
xB9_lda_absy endp

xA1_lda_indx proc
	read_indx_rbx
	lda_body 6, 1
xA1_lda_indx endp

xB1_lda_indy proc
	read_indy_rbx_pagepenalty
	lda_body 5, 1
xB1_lda_indy endp

xB2_lda_indzp proc
	read_indzp_rbx
	lda_body 5, 1
xB2_lda_indzp endp


; -----------------------------
; LDX
; -----------------------------

ldx_body_end macro clock, pc
	test r9b, r9b
	write_flags_r15_preservecarry

	add r14, clock
	add r11w, pc

	jmp opcode_done
endm

ldx_body macro clock, pc
	mov r9b, [rcx+rbx]
	ldx_body_end clock, pc
endm

xA2_ldx_imm PROC
	mov	r9b, [rcx+r11]
	ldx_body_end 2, 1
xA2_ldx_imm ENDP

xA6_ldx_zp PROC
	read_zp_rbx
	ldx_body 3, 1
xA6_ldx_zp  ENDP

xB6_ldx_zpy PROC
	read_zpy_rbx
	ldx_body 4, 1
xB6_ldx_zpy endp

xAE_ldx_abs proc
	read_abs_rbx
	ldx_body 4, 2
xAE_ldx_abs endp

xBE_ldx_absy proc
	read_absy_rbx_pagepenalty
	ldx_body 4, 2
xBE_ldx_absy endp

; -----------------------------
; LDY
; -----------------------------

ldy_body_end macro clock, pc
	test r10b, r10b
	write_flags_r15_preservecarry

	add r14, clock
	add r11w, pc

	jmp opcode_done
endm

ldy_body macro clock, pc
	mov r10b, [rcx+rbx]
	ldy_body_end clock, pc
endm

xA0_ldy_imm PROC
	mov	r10b, [rcx+r11]
	ldy_body_end 2, 1
xA0_ldy_imm ENDP

xA4_ldy_zp PROC
	read_zp_rbx
	ldy_body 3, 1
xA4_ldy_zp  ENDP

xB4_ldy_zpx PROC
	read_zpx_rbx
	ldy_body 4, 1
xB4_ldy_zpx endp

xAC_ldy_abs proc
	read_abs_rbx
	ldy_body 4, 2
xAC_ldy_abs endp

xBC_ldy_absx proc
	read_absx_rbx_pagepenalty
	ldy_body 4, 2
xBC_ldy_absx endp


; -----------------------------
; STA
; -----------------------------

sta_body macro clock, pc
	mov byte ptr [rcx+rbx], r8b

	add r14, clock
	add r11w, pc			; add on PC

	jmp opcode_done
endm

x85_sta_zp proc	
	read_zp_rbx
	sta_body 3, 1
x85_sta_zp endp

x95_sta_zpx proc
	read_zpx_rbx
	sta_body 4, 1
x95_sta_zpx endp

x8D_sta_abs proc
	read_abs_rbx
	sta_body 4, 2
x8D_sta_abs endp

x9D_sta_absx proc
	read_absx_rbx
	sta_body 5, 2
x9D_sta_absx endp

x99_sta_absy proc
	read_absy_rbx
	sta_body 5, 2
x99_sta_absy endp

x81_sta_indx proc
	read_indx_rbx
	sta_body 6, 1
x81_sta_indx endp

x91_sta_indy proc
	read_indy_rbx
	sta_body 6, 1
x91_sta_indy endp

x92_sta_indzp proc
	read_indzp_rbx
	sta_body 5, 1
x92_sta_indzp endp

;
; STX
;

stx_body macro clock, pc
	mov byte ptr [rcx+rbx], r9b

	add r14, clock
	add r11w, pc			; add on PC

	jmp opcode_done
endm

x86_stx_zp proc
	read_zp_rbx
	stx_body 3, 1
x86_stx_zp endp

x96_stx_zpy proc
	read_zpy_rbx
	stx_body 4, 1
x96_stx_zpy endp

x8E_stx_abs proc
	read_abs_rbx
	stx_body 4, 2
x8E_stx_abs endp

;
; STY
;

sty_body macro clock, pc
	mov byte ptr [rcx+rbx], r10b

	add r14, clock
	add r11w, pc			; add on PC

	jmp opcode_done
endm

x84_sty_zp proc
	read_zp_rbx
	sty_body 3, 1
x84_sty_zp endp

x94_sty_zpx proc
	read_zpx_rbx
	sty_body 4, 1
x94_sty_zpx endp

x8C_sty_abs proc
	read_abs_rbx
	sty_body 4, 2
x8C_sty_abs endp

;
; STZ
;

stz_body macro clock, pc
	mov byte ptr [rcx+rbx], 0

	add r14, clock
	add r11w, pc			; add on PC

	jmp opcode_done
endm

x64_stz_zp proc
	read_zp_rbx
	stz_body 3, 1
x64_stz_zp endp

x74_stz_zpx proc
	read_zpx_rbx
	stz_body 4, 1
x74_stz_zpx endp

x9C_stz_abs proc
	read_abs_rbx
	stz_body 4, 2
x9C_stz_abs endp

x9E_stz_absx proc
	read_absx_rbx
	stz_body 5, 2
x9E_stz_absx endp

;
; INC\DEC
;

inc_body macro clock, pc
	inc byte ptr [rcx+rbx]
	write_flags_r15_preservecarry

	add r14, clock
	add r11w, pc			; add on PC

	jmp opcode_done
endm

dec_body macro clock, pc
	dec byte ptr [rcx+rbx]
	write_flags_r15_preservecarry

	add r14, clock
	add r11w, pc			; add on PC

	jmp opcode_done
endm

x1A_inc_a proc
	inc r8b
	write_flags_r15_preservecarry

	add r14, 2
	jmp opcode_done
x1A_inc_a endp

x3A_dec_a proc
	dec r8b
	write_flags_r15_preservecarry

	add r14, 2
	jmp opcode_done
x3A_dec_a endp


xEE_inc_abs proc
	read_abs_rbx
	inc_body 6, 2
xEE_inc_abs endp

xCE_dec_abs proc
	read_abs_rbx
	dec_body 6, 2
xCE_dec_abs endp


xFE_inc_absx proc
	read_absx_rbx
	inc_body 7, 2
xFE_inc_absx endp

xDE_dec_absx proc
	read_absx_rbx
	dec_body 7, 2
xDE_dec_absx endp


xE6_inc_zp proc
	read_zp_rbx
	inc_body 5, 1
xE6_inc_zp endp

xC6_dec_absx proc
	read_zp_rbx
	dec_body 5, 1
xC6_dec_absx endp


xF6_inc_zpx proc
	read_zpx_rbx
	inc_body 6, 1
xF6_inc_zpx endp

xD6_dec_zpx proc
	read_zpx_rbx
	dec_body 6, 1
xD6_dec_zpx endp

;
; INX\DEX
;

xE8_inx proc
	inc r9b
	write_flags_r15_preservecarry

	add r14, 2
	jmp opcode_done
xE8_inx endp

xCA_dex proc

	dec r9b
	write_flags_r15_preservecarry

	add r14, 2
	jmp opcode_done
xCA_dex endp

;
; INY\DEY
;

xC8_iny proc
	inc r10b
	write_flags_r15_preservecarry

	add r14, 2
	jmp opcode_done
xC8_iny endp

x88_dey proc
	dec r10b
	write_flags_r15_preservecarry

	add r14, 2
	jmp opcode_done
x88_dey endp

;
; Register Transfer
;

xAA_tax proc
	add r14, 2
	mov	r9, r8		; A -> X

	test r9b, r9b
	write_flags_r15_preservecarry

	jmp opcode_done
xAA_tax endp

x8A_txa proc
	add r14, 2
	mov	r8, r9		; X -> A

	test r8b, r8b
	write_flags_r15_preservecarry

	jmp opcode_done
x8A_txa endp

xA8_tay proc
	add r14, 2
	mov	r10, r8		; A -> Y

	test r10b, r10b
	write_flags_r15_preservecarry

	jmp opcode_done
xA8_tay endp

x98_tya proc
	add r14, 2
	mov	r8, r10		; Y -> A

	test r8b, r8b
	write_flags_r15_preservecarry

	jmp opcode_done
x98_tya endp

;
; Shifts
;

;
; ASL
;

x0A_asl_a proc

	mov rax, r15	; move flags to rax
	sahf			; set eflags

	sal r8b,1		; shift

	lahf			; move new flags to rax
	mov r15, rax	; store

	add r14, 2		; Clock

	jmp opcode_done	

x0A_asl_a endp

x0E_asl_abs proc

	xor rbx, rbx
	mov bx, [rcx+r11]	; Get 16bit value in memory.

	mov rax, r15		; move flags to rax
	sahf				; set eflags

	sal byte ptr [rcx+rbx],1		; shift

	lahf				; move new flags to rax
	mov r15, rax		; store

	add r11w, 2			; move PC on
	add r14, 6			; Clock

	jmp opcode_done	

x0E_asl_abs endp

x1E_asl_absx proc

	xor rbx, rbx
	mov bx, [rcx+r11]	; Get 16bit value in memory.
	add	bl, r9b			; Add X

	mov rax, r15		; move flags to rax
	sahf				; set eflags

	sal byte ptr [rcx+rbx],1		; shift

	lahf				; move new flags to rax
	mov r15, rax		; store

	add r11w, 2			; move PC on
	add r14, 7			; Clock

	jmp opcode_done	

x1E_asl_absx endp

x06_asl_zp proc

	xor rbx, rbx
	mov bl, [rcx+r11]	; Get 8bit value in memory.

	mov rax, r15		; move flags to rax
	sahf				; set eflags

	sal byte ptr [rcx+rbx],1		; shift

	lahf				; move new flags to rax
	mov r15, rax		; store

	add r11w, 1			; move PC on
	add r14, 5			; Clock

	jmp opcode_done	

x06_asl_zp endp

x16_asl_zpx proc

	xor rbx, rbx
	mov bl, [rcx+r11]	; Get 8bit value in memory.
	add	bl, r9b			; Add X

	mov rax, r15		; move flags to rax
	sahf				; set eflags

	sal byte ptr [rcx+rbx],1		; shift

	lahf				; move new flags to rax
	mov r15, rax		; store

	add r11w, 1			; move PC on
	add r14, 6			; Clock

	jmp opcode_done	

x16_asl_zpx endp

;
; LSR
;

x4A_lsr_a proc

	read_flags_rax

	sar r8b,1		; shift

	write_flags_r15

	add r14, 2		; Clock

	jmp opcode_done	

x4A_lsr_a endp

x4E_lsr_abs proc

	read_abs_rbx

	read_flags_rax

	sar byte ptr [rcx+rbx],1	; shift

	write_flags_r15

	add r14, 6					; Clock
	add r11w, 2			; add on PC

	jmp opcode_done	

x4E_lsr_abs endp

x5E_lsr_absx proc

	read_absx_rbx

	read_flags_rax

	sar byte ptr [rcx+rbx],1	; shift

	write_flags_r15

	add r14, 7					; Clock
	add r11w, 2			; add on PC

	jmp opcode_done	

x5E_lsr_absx endp

x46_lsr_zp proc

	read_zp_rbx

	read_flags_rax

	sar byte ptr [rcx+rbx],1	; shift

	write_flags_r15

	add r11w, 1			; add on PC
	add r14, 5					; Clock

	jmp opcode_done	

x46_lsr_zp endp

x56_lsr_zpx proc

	read_zpx_rbx

	read_flags_rax

	sar byte ptr [rcx+rbx],1	; shift

	write_flags_r15

	add r14, 6					; Clock
	add r11w, 1			; add on PC

	jmp opcode_done	

x56_lsr_zpx endp

;
; ROL
;

x2A_rol_a proc
	
	read_flags_rax

	jnc no_carry

	mov rbx, 1
no_carry:

	sal r8b,1		; shift

	write_flags_r15

	or r8, rbx

	add r14, 2		; Clock

	jmp opcode_done	

x2A_rol_a endp

x2E_rol_abs proc
	
	read_abs_rbx
	read_flags_rax

	jnc no_carry

	sal byte ptr [rcx+rbx], 1		; shift

	write_flags_r15

	or byte ptr [rcx+rbx], 1 ; add on carry

	add r14, 6		; Clock
	add r11w, 2			; add on PC
	jmp opcode_done	

no_carry:

	sal byte ptr [rcx+rbx],1		; shift

	write_flags_r15

	add r14, 6		; Clock
	add r11w, 2			; add on PC
	jmp opcode_done	

x2E_rol_abs endp

x3E_rol_absx proc
	
	read_absx_rbx
	read_flags_rax

	jnc no_carry

	sal byte ptr [rcx+rbx], 1	; shift

	write_flags_r15

	or byte ptr [rcx+rbx], 1	; add on carry

	add r14, 7					; Clock
	add r11w, 2					; add on PC

	jmp opcode_done	

no_carry:

	sal byte ptr [rcx+rbx],1		; shift

	write_flags_r15

	add r14, 7					; Clock
	add r11w, 2					; add on PC

	jmp opcode_done	

x3E_rol_absx endp

x26_rol_zp proc
	
	add r14, 5		; Clock
	read_zp_rbx

	read_flags_rax

	jnc no_carry

	sal byte ptr [rcx+rbx], 1		; shift

	write_flags_r15

	or byte ptr [rcx+rbx], 1 ; add on carry

	add r11w, 1			; add on PC
	jmp opcode_done	

no_carry:

	sal byte ptr [rcx+rbx],1		; shift

	write_flags_r15

	add r11w, 1			; add on PC
	jmp opcode_done	

x26_rol_zp endp

x36_rol_zpx proc
	
	read_zpx_rbx
	add r14, 6		; Clock

	read_flags_rax

	jnc no_carry

	sal byte ptr [rcx+rbx], 1		; shift

	write_flags_r15

	or byte ptr [rcx+rbx], 1 ; add on carry

	add r11w, 1			; add on PC
	jmp opcode_done	

no_carry:

	sal byte ptr [rcx+rbx],1		; shift

	write_flags_r15

	add r11w, 1			; add on PC
	jmp opcode_done	

x36_rol_zpx endp

;
; ROR
;

x6A_ror_a proc
	
	add r14, 2		; Clock

	read_flags_rax

	jnc no_carry

	shr r8b,1		; shift

	write_flags_r15_setnegative

	or r8, 10000000b	; Add on carry

	jmp opcode_done	

no_carry:

	shr r8b,1		; shift

	write_flags_r15

	jmp opcode_done	

x6A_ror_a endp

x6E_ror_abs proc
	
	read_abs_rbx
	add r14, 6		; Clock

	read_flags_rax

	jnc no_carry

	shr byte ptr [rcx+rbx], 1		; shift

	write_flags_r15_setnegative

	or byte ptr [rcx+rbx], 10000000b ; add on carry

	add r11w, 2			; add on PC
	jmp opcode_done	

no_carry:

	shr byte ptr [rcx+rbx],1		; shift

	write_flags_r15

	add r11w, 2			; add on PC
	jmp opcode_done	

x6E_ror_abs endp

x7E_ror_absx proc
	
	read_absx_rbx
	read_flags_rax

	jnc no_carry

	shr byte ptr [rcx+rbx], 1		; shift

	write_flags_r15_setnegative

	or byte ptr [rcx+rbx], 10000000b ; add on carry
	add r14, 7		; Clock
	add r11w, 2			; add on PC

	jmp opcode_done	

no_carry:

	shr byte ptr [rcx+rbx],1		; shift

	write_flags_r15

	add r14, 7		; Clock
	add r11w, 2			; add on PC

	jmp opcode_done	

x7E_ror_absx endp

x66_ror_zp proc
	
	add r14, 5		; Clock
	read_zp_rbx

	read_flags_rax

	jnc no_carry

	shr byte ptr [rcx+rbx], 1		; shift

	write_flags_r15_setnegative

	or byte ptr [rcx+rbx], 10000000b ; add on carry

	add r11w, 1			; add on PC
	jmp opcode_done	

no_carry:

	shr byte ptr [rcx+rbx],1		; shift

	write_flags_r15

	add r11w, 1			; add on PC
	jmp opcode_done	

x66_ror_zp endp

x76_ror_zpx proc
	
	read_zpx_rbx
	add r14, 6		; Clock

	read_flags_rax

	jnc no_carry

	shr byte ptr [rcx+rbx], 1		; shift

	write_flags_r15_setnegative

	or byte ptr [rcx+rbx], 10000000b ; add on carry

	add r11w, 1			; add on PC
	jmp opcode_done	

no_carry:

	shr byte ptr [rcx+rbx],1		; shift

	write_flags_r15

	add r11w, 1			; add on PC
	jmp opcode_done	

x76_ror_zpx endp

;
; AND
;

and_body_end macro clock, pc
	and r8b, [rcx+rbx]
	write_flags_r15_preservecarry
	
	add r14, clock		; Clock
	add r11w, pc			; add on PC
	jmp opcode_done	
endm

x29_and_imm proc
	and r8b, [rcx+r11]
	write_flags_r15_preservecarry

	add r14, 2		; Clock
	add r11w, 1		; PC
	jmp opcode_done	
x29_and_imm endp

x2D_and_abs proc
	read_abs_rbx
	and_body_end 4, 2
x2D_and_abs endp

x3D_and_absx proc
	read_absx_rbx_pagepenalty
	and_body_end 4, 2
x3D_and_absx endp

x39_and_absy proc
	read_absy_rbx_pagepenalty
	and_body_end 4, 2
x39_and_absy endp

x25_and_zp proc
	read_zp_rbx
	and_body_end 3, 1
x25_and_zp endp

x35_and_zpx proc
	read_zpx_rbx
	and_body_end 4, 1
x35_and_zpx endp

x32_and_indzp proc
	read_indzp_rbx
	and_body_end 5, 1
x32_and_indzp endp

x21_and_indx proc
	read_indx_rbx
	and_body_end 6, 1
x21_and_indx endp

x31_and_indy proc
	read_indy_rbx_pagepenalty
	and_body_end 5, 1
x31_and_indy endp

;
; EOR
;

eor_body_end macro clock, pc
	xor r8b, [rcx+rbx]
	write_flags_r15_preservecarry

	add r14, clock	
	add r11w, pc

	jmp opcode_done	
endm

x49_eor_imm proc
	xor r8b, [rcx+r11]
	write_flags_r15_preservecarry

	add r14, 2		; Clock
	add r11w, 1		; PC

	jmp opcode_done	
x49_eor_imm endp

x4D_eor_abs proc
	read_abs_rbx
	eor_body_end 4, 2
x4D_eor_abs endp

x5D_eor_absx proc
	read_absx_rbx_pagepenalty
	eor_body_end 4, 2
x5D_eor_absx endp

x59_eor_absy proc
	read_absy_rbx_pagepenalty
	eor_body_end 4, 2
x59_eor_absy endp

x45_eor_zp proc
	read_zp_rbx
	eor_body_end 3, 1
x45_eor_zp endp

x55_eor_zpx proc
	read_zpx_rbx
	eor_body_end 4, 1
x55_eor_zpx endp

x52_eor_indzp proc
	read_indzp_rbx
	eor_body_end 5, 1
x52_eor_indzp endp

x41_eor_indx proc
	read_indx_rbx
	eor_body_end 6, 1
x41_eor_indx endp

x51_eor_indy proc
	read_indy_rbx_pagepenalty
	eor_body_end 5, 1
x51_eor_indy endp

;
; OR
;

ora_body macro clock, pc
	or r8b, [rcx+rbx]
	write_flags_r15_preservecarry
	
	add r11w, pc			; add on PC
	add r14, clock		; Clock
	jmp opcode_done	
endm

x09_ora_imm proc
	or r8b, [rcx+r11]
	write_flags_r15_preservecarry

	add r11w, 1		; PC
	add r14, 2		; Clock
	jmp opcode_done	
x09_ora_imm endp

x0D_ora_abs proc
	read_abs_rbx
	ora_body 4, 2
x0D_ora_abs endp

x1D_ora_absx proc
	read_absx_rbx_pagepenalty
	ora_body 4, 2
x1D_ora_absx endp

x19_ora_absy proc
	read_absy_rbx_pagepenalty
	ora_body 4, 2
x19_ora_absy endp

x05_ora_zp proc
	read_zp_rbx
	ora_body 3, 1
x05_ora_zp endp

x15_ora_zpx proc
	read_zpx_rbx
	ora_body 4, 1
x15_ora_zpx endp

x12_ora_indzp proc
	read_indzp_rbx
	ora_body 5, 1
x12_ora_indzp endp

x01_ora_indx proc
	read_indx_rbx
	ora_body 6, 1
x01_ora_indx endp

x11_ora_indy proc
	read_indy_rbx_pagepenalty
	ora_body 5, 1
x11_ora_indy endp

;
; ADC
;
adc_body_end macro clock, pc
	write_flags_r15

	seto bl
	mov byte ptr [rdx+flags_overflow], bl

	add r14, clock			; Clock
	add r11w, pc			; add on PC
	jmp opcode_done	
endm

adc_body macro clock, pc
	read_flags_rax

	adc r8b, [rcx+rbx]

	adc_body_end clock, pc
endm

x69_adc_imm proc
	read_flags_rax

	adc r8b, [rcx+r11]

	adc_body_end 2, 1
x69_adc_imm endp

x6D_adc_abs proc
	read_abs_rbx
	adc_body 4, 2
x6D_adc_abs endp

x7D_adc_absx proc
	read_absx_rbx_pagepenalty
	adc_body 4, 2
x7D_adc_absx endp

x79_adc_absy proc
	read_absy_rbx_pagepenalty
	adc_body 4, 2
x79_adc_absy endp

x65_adc_zp proc
	read_zp_rbx
	adc_body 3, 1
x65_adc_zp endp

x75_adc_zpx proc
	read_zpx_rbx
	adc_body 4, 1
x75_adc_zpx endp

x72_adc_indzp proc
	read_indzp_rbx
	adc_body 5, 1
x72_adc_indzp endp

x61_adc_indx proc
	read_indx_rbx
	adc_body 6, 1
x61_adc_indx endp

x71_adc_indy proc
	read_indy_rbx_pagepenalty
	adc_body 5, 1
x71_adc_indy endp

;
; SBC
;

sbc_body_end macro clock, pc
	write_flags_r15

	seto bl
	mov byte ptr [rdx+flags_overflow], bl

	add r14, clock			; Clock
	add r11w, pc			; add on PC
	jmp opcode_done	
endm

sbc_body macro clock, pc
	read_flags_rax

	cmc
	sbb r8b, [rcx+rbx]
	cmc

	sbc_body_end clock, pc
endm

xE9_sbc_imm proc
	read_flags_rax

	cmc
	sbb r8b, [rcx+r11]
	cmc

	sbc_body_end 2, 1
xE9_sbc_imm endp

xED_sbc_abs proc
	read_abs_rbx
	sbc_body 4, 2
xED_sbc_abs endp

xFD_sbc_absx proc
	read_absx_rbx_pagepenalty
	sbc_body 4, 2
xFD_sbc_absx endp

xF9_sbc_absy proc
	read_absy_rbx_pagepenalty
	sbc_body 4, 2
xF9_sbc_absy endp

xE5_sbc_zp proc
	read_zp_rbx
	sbc_body 3, 1
xE5_sbc_zp endp

xF5_sbc_zpx proc
	read_zpx_rbx
	sbc_body 4, 1
xF5_sbc_zpx endp

xF2_sbc_indzp proc
	read_indzp_rbx
	sbc_body 5, 1
xF2_sbc_indzp endp

xE1_sbc_indx proc
	read_indx_rbx
	sbc_body 6, 1
xE1_sbc_indx endp

xF1_sbc_indy proc
	read_indy_rbx_pagepenalty
	sbc_body 5, 1
xF1_sbc_indy endp

;
; CMP
;

cmp_body_end macro clock, pc
	cmc
	write_flags_r15

	add r14, clock			; Clock
	add r11w, pc			; add on PC
	jmp opcode_done	
endm

cmp_body macro clock, pc
	cmp r8b, [rcx+rbx]
	cmp_body_end clock, pc
endm

xC9_cmp_imm proc
	cmp r8b, [rcx+r11]		
	cmp_body_end 2, 1
xC9_cmp_imm endp

xCD_cmp_abs proc
	read_abs_rbx
	cmp_body 4, 2
xCD_cmp_abs endp

xDD_cmp_absx proc
	read_absx_rbx_pagepenalty
	cmp_body 4, 2
xDD_cmp_absx endp

xD9_cmp_absy proc
	read_absy_rbx_pagepenalty
	cmp_body 4, 2
xD9_cmp_absy endp

xC5_cmp_zp proc
	read_zp_rbx
	cmp_body 3, 1
xC5_cmp_zp endp

xD5_cmp_zpx proc
	read_zpx_rbx
	cmp_body 4, 1
xD5_cmp_zpx endp

xD2_cmp_indzp proc
	read_indzp_rbx
	cmp_body 5, 1
xD2_cmp_indzp endp

xC1_sbc_indx proc
	read_indx_rbx
	cmp_body 6, 1
xC1_sbc_indx endp

xD1_cmp_indy proc
	read_indy_rbx_pagepenalty
	cmp_body 5, 1
xD1_cmp_indy endp

;
; CMPX
;

cmpx_body macro clock, pc
	cmp r9b, [rcx+rbx]
	cmp_body_end clock, pc
endm

xE0_cmpx_imm proc
	cmp r9b, [rcx+r11]		
	cmp_body_end 2, 1
xE0_cmpx_imm endp

xEC_cmpx_abs proc
	read_abs_rbx
	cmpx_body 4, 2
xEC_cmpx_abs endp

xE4_cmpx_zp proc
	read_zp_rbx
	cmpx_body 3, 1
xE4_cmpx_zp endp

;
; CMPY
;

cmpy_body macro clock, pc
	cmp r10b, [rcx+rbx]
	cmp_body_end clock, pc
endm

xC0_cmpy_imm proc
	cmp r10b, [rcx+r11]		
	cmp_body_end 2, 1
xC0_cmpy_imm endp

xCC_cmpy_abs proc
	read_abs_rbx
	cmpy_body 4, 2
xCC_cmpy_abs endp

xC4_cmpy_zp proc
	read_zp_rbx
	cmpy_body 3, 1
xC4_cmpy_zp endp

;
; Flag Modifiers
;

x18_clc proc
	;                |
	and r15w, 1111111011111111b

	add r14, 2			; Clock
	jmp opcode_done	
x18_clc endp

x38_sec proc
	;                |
	or r15w, 0000000100000000b

	add r14, 2			; Clock
	jmp opcode_done	
x38_sec endp

xD8_cld proc
	mov byte ptr [rdx+flags_decimal], 0
	add r14, 2			; Clock
	jmp opcode_done	
xD8_cld endp

xF8_sed proc
	mov byte ptr [rdx+flags_decimal], 1
	add r14, 2			; Clock
	jmp opcode_done	
xF8_sed endp

x58_cli proc
	mov byte ptr [rdx+flags_interruptDisable], 0
	add r14, 2			; Clock
	jmp opcode_done	
x58_cli endp

x78_sei proc
	mov byte ptr [rdx+flags_interruptDisable], 1
	add r14, 2			; Clock
	jmp opcode_done	
x78_sei endp

xB8_clv proc
	mov byte ptr [rdx+flags_overflow], 0
	add r14, 2			; Clock
	jmp opcode_done	
xB8_clv endp

;
; Branches
;

bra_perform_jump macro
	local page_change

	movsx bx, byte ptr [rcx+r11]	; Get value at PC and turn it into a 2byte signed value
	add r11w, 1						; move PC on -- all jumps are relative
	mov rax, r11					; store PC
	add r11w, bx
	
	mov rbx, r11
	cmp ah, bh						; test if the page has changed.
	jne page_change

	add r14, 3						; Clock

	jmp opcode_done	

page_change:						; page change as a 1 cycle penalty
	add r14, 4						; Clock
	jmp opcode_done

endm

bra_nojump macro
	add r14, 2			; Clock
	add r11w, 1			; move PC on

	jmp opcode_done	
endm

x80_bra proc
	bra_perform_jump
x80_bra endp

xD0_bne proc
	mov rax, r15	; move flags to rax
	sahf			; set eflags

	jnz branch
	bra_nojump

branch:
	bra_perform_jump

xD0_bne endp

xF0_beq proc
	mov rax, r15	; move flags to rax
	sahf			; set eflags

	jz branch
	bra_nojump

branch:
	bra_perform_jump
xF0_beq endp

x10_bpl proc
	mov rax, r15	; move flags to rax
	sahf			; set eflags

	jns branch
	bra_nojump

branch:
	bra_perform_jump
x10_bpl endp

x30_bmi proc
	mov rax, r15	; move flags to rax
	sahf			; set eflags

	js branch
	bra_nojump

branch:
	bra_perform_jump
x30_bmi endp

x90_bcc proc
	mov rax, r15	; move flags to rax
	sahf			; set eflags

	jnc branch
	bra_nojump

branch:
	bra_perform_jump
x90_bcc endp

xB0_bcs proc
	mov rax, r15	; move flags to rax
	sahf			; set eflags

	jc branch
	bra_nojump

branch:
	bra_perform_jump
xB0_bcs endp

x50_bvc proc
	mov al, [rdx+flags_overflow]
	test al, al
	jz branch
	bra_nojump

branch:
	bra_perform_jump
x50_bvc endp

x70_bvs proc
	mov al, [rdx+flags_overflow]
	test al, al
	jnz branch
	bra_nojump

branch:
	bra_perform_jump
x70_bvs endp

;
; BBR
;

bb_perform_jump macro
	local page_change

	movsx bx, byte ptr [rcx+r11+1]	; Get value at PC+1 and turn it into a 2byte signed value
	add r11w, 1						; move PC on -- all jumps are relative
	mov rax, r11					; store PC
	add r11w, bx
	
	mov rbx, r11
	cmp ah, bh						; test if the page has changed.
	jne page_change

	add r14, 6						; Clock

	jmp opcode_done	

page_change:						; page change as a 1 cycle penalty
	add r14, 7						; Clock
	jmp opcode_done

endm

bbr_body macro bitnumber
	read_zp_rbx
	mov al, byte ptr[rcx+rbx]
	bt ax, bitnumber
	jnc branch
	add r11w, 2						; move PC on
	add r14, 5						; Clock

	jmp opcode_done	
branch:
	bb_perform_jump
endm

x0F_bbr0 proc
	bbr_body 0
x0F_bbr0 endp

x1F_bbr1 proc
	bbr_body 1
x1F_bbr1 endp

x2F_bbr2 proc
	bbr_body 2
x2F_bbr2 endp

x3F_bbr3 proc
	bbr_body 3
x3F_bbr3 endp

x4F_bbr4 proc
	bbr_body 4
x4F_bbr4 endp

x5F_bbr5 proc
	bbr_body 5
x5F_bbr5 endp

x6F_bbr6 proc
	bbr_body 6
x6F_bbr6 endp

x7F_bbr7 proc
	bbr_body 7
x7F_bbr7 endp

;
; BBS
;


bbs_body macro bitnumber
	read_zp_rbx
	mov al, byte ptr[rcx+rbx]
	bt ax, bitnumber
	jc branch
	add r11w, 2						; move PC on
	add r14, 5						; Clock

	jmp opcode_done	
branch:
	bb_perform_jump
endm

x8F_bbs0 proc
	bbs_body 0
x8F_bbs0 endp

x9F_bbs1 proc
	bbs_body 1
x9F_bbs1 endp

xAF_bbs2 proc
	bbs_body 2
xAF_bbs2 endp

xBF_bbs3 proc
	bbs_body 3
xBF_bbs3 endp

xCF_bbs4 proc
	bbs_body 4
xCF_bbs4 endp

xDF_bbs5 proc
	bbs_body 5
xDF_bbs5 endp

xEF_bbs6 proc
	bbs_body 6
xEF_bbs6 endp

xFF_bbs7 proc
	bbs_body 7
xFF_bbs7 endp

;
; JMP
;

x4C_jmp_abs proc
	mov r11w, [rcx+r11]		; Get 16bit value in memory and set it to the PC

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
; Subroutines
;

x20_jsr proc
	mov rax, r11						; Get PC + 1 as the return address (to put address-1 on the stack)
	add rax, 1

	xor rbx, rbx

	mov ebx, [rdx+stackpointer]			; Get stack pointer
	mov [rcx+rbx], al					; Put PC Low byte on stack
	dec bl								; Move stack pointer on
	mov [rcx+rbx], ah					; Put PC High byte on stack
	dec bl								; Move stack pointer on (done twice for wrapping)

	mov byte ptr [rdx+stackpointer], bl	; Store stack pointer

	mov r11w, [rcx+r11]					; Get 16bit value in memory and set it to the PC

	add r14, 6							; Add cycles

	jmp opcode_done
x20_jsr endp

x60_rts proc
	xor rbx, rbx

	mov ebx, [rdx+stackpointer]			; Get stack pointer
	add bl, 1							; Move stack pointer on
	mov ah, [rcx+rbx]					; Get PC High byte on stack
	add bl, 1							; Move stack pointer on (done twice for wrapping)
	mov al, [rcx+rbx]					; Get PC Low byte on stack

	mov byte ptr [rdx+stackpointer], bl	; Store stack pointer

	add ax, 1							; Add on 1 for the next byte
	mov r11w, ax						; Set PC to destination

	add r14, 6							; Add cycles

	jmp opcode_done
x60_rts endp

;
; Stack
;

x48_pha proc
	xor rbx, rbx
	
	mov ebx, [rdx+stackpointer]			; Get stack pointer
	sub byte ptr [rdx+stackpointer], 1	; Decrement stack pointer
	mov [rcx+rbx], r8b					; Put A on stack
	
	add r14, 3							; Add cycles

	jmp opcode_done

x48_pha endp

x68_pla proc
	xor rbx, rbx

	add byte ptr [rdx+stackpointer], 1	; Increment stack pointer
	mov ebx, [rdx+stackpointer]			; Get stack pointer

	mov r8b, byte ptr [rcx+rbx] 		; Pull A from the stack
	test r8b, r8b
	write_flags_r15_preservecarry
	
	add r14, 4							; Add cycles

	jmp opcode_done
x68_pla endp

xDA_phx proc
	
	xor rbx, rbx
	
	mov ebx, [rdx+stackpointer]			; Get stack pointer
	mov [rcx+rbx], r9b					; Put X on stack
	dec byte ptr [rdx+stackpointer]		; Decrement stack pointer
	
	add r14, 3							; Add cycles

	jmp opcode_done

xDA_phx endp

xFA_plx proc
	xor rbx, rbx

	add byte ptr [rdx+stackpointer], 1	; Increment stack pointer
	mov ebx, [rdx+stackpointer]			; Get stack pointer

	mov r9b, byte ptr [rcx+rbx] 		; Pull X from the stack
	test r9b, r9b
	write_flags_r15_preservecarry
	
	add r14, 4							; Add cycles

	jmp opcode_done
xFA_plx endp

x5A_phy proc	
	xor rbx, rbx
	
	mov ebx, [rdx+stackpointer]			; Get stack pointer
	mov [rcx+rbx], r10b					; Put Y on stack
	dec byte ptr [rdx+stackpointer]		; Decrement stack pointer
	
	add r14, 3							; Add cycles

	jmp opcode_done
x5A_phy endp

x7A_ply proc
	xor rbx, rbx

	add byte ptr [rdx+stackpointer] ,1	; Increment stack pointer
	mov ebx, [rdx+stackpointer]			; Get stack pointer

	mov r10b, byte ptr [rcx+rbx] 		; Pull Y from the stack
	test r10b, r10b
	write_flags_r15_preservecarry
	
	add r14, 4							; Add cycles
	jmp opcode_done
x7A_ply endp

x9A_txs proc
	mov byte ptr [rdx+stackpointer], r9b ; move X to stack pointer
	add r14, 2							; Add cycles
	jmp opcode_done
x9A_txs endp

xBA_tsx proc
	mov r9b, byte ptr [rdx+stackpointer] ; move stack pointer to X

	test r9b, r9b
	write_flags_r15_preservecarry

	add r14, 2							; Add cycles
	jmp opcode_done	
xBA_tsx endp

x08_php proc
	mov	al, 00110000b ; bits that are always set

	; carry
	bt r15w, 0 +8
	jnc no_carry
	or al, 00000001b
no_carry:
	
	; zero
	bt r15w, 6 +8
	jnc no_zero
	or al, 00000010b
no_zero:

	; negative
	bt r15w, 7 +8
	jnc no_negative
	or al, 10000000b
no_negative:

	; interrupt disable
	mov bl, [rdx+flags_interruptDisable]
	test bl, 1
	jz no_interrupt
	or al, 00000100b
no_interrupt:

	; overflow
	mov bl, [rdx+flags_overflow]
	test bl, 1
	jz no_overflow
	or al, 01000000b
no_overflow:

	; decimal
	mov bl, [rdx+flags_decimal]
	test bl, 1
	jz no_decimal
	or al, 00001000b
no_decimal:

	xor rbx, rbx

	mov ebx, [rdx+stackpointer]			; Get stack pointer
	sub byte ptr [rdx+stackpointer], 1	; Increment stack pointer
	mov [rcx+rbx], al					; Put status on stack
	
	add r14, 3							; Add cycles

	jmp opcode_done

x08_php endp

x28_plp proc

	xor rbx, rbx
	
	add byte ptr [rdx+stackpointer], 1	; Decrement stack pointer
	mov ebx, [rdx+stackpointer]			; Get stack pointer
	mov al, [rcx+rbx]					; Get status from stack
	
	xor r15w, r15w

	; carry
	bt ax, 0
	jnc no_carry
	;                |
	or r15w, 0000000100000000b
no_carry:

	; zero
	bt ax, 1
	jnc no_zero
	;                |
	or r15w, 0100000000000000b
no_zero:

	; negative
	bt ax, 7
	jnc no_negative
	;                |
	or r15w, 1000000000000000b
no_negative:

	; interrupt disable
	bt ax, 2
	setc bl
	mov byte ptr [rdx+flags_interruptDisable], bl

	; overflow
	bt ax, 6
	setc bl
	mov byte ptr [rdx+flags_overflow], bl

	; decimal
	bt ax, 3
	setc bl
	mov byte ptr [rdx+flags_decimal], bl

	add r14, 4							; Add cycles

	jmp opcode_done

x28_plp endp

;
; BIT
;

bit_body_end macro clock, pc
	and bl, r8b				; cant just test, as we need to check bit 6 for overflow.
	test bl, bl				; sets zero and sign flags
	write_flags_r15_preservecarry

	bt bx, 6				; test overflow
	setc bl
	mov byte ptr [rdx+flags_overflow], bl
	
	add r14, clock			
	add r11w, pc			

	jmp opcode_done
endm

bit_body macro clock, pc
	mov bl, [rcx+rbx]
	bit_body_end clock, pc
endm

x89_bit_imm proc
	mov bl, [rcx+r11]
	bit_body_end 3, 1
x89_bit_imm endp

x2C_bit_abs proc
	read_abs_rbx
	bit_body 4, 2
x2C_bit_abs endp

x3C_bit_absx proc
	read_absx_rbx
	bit_body 4, 2
x3C_bit_absx endp

x24_bit_zp proc
	read_zp_rbx
	bit_body 3, 1
x24_bit_zp endp

x34_bit_zpx proc
	read_zpx_rbx
	bit_body 3, 1
x34_bit_zpx endp

;
; TRB
;

x1C_trb_abs proc
	read_abs_rbx
	mov al, r8b
	not al
	and byte ptr [rcx+rbx], al

	jz set_zero
	add r14, 6
	add r11w, 2
	jmp opcode_done

set_zero:
	;        NZ A P C
	or r15w, 0100000000000000b
	add r14, 6
	add r11w, 2
	jmp opcode_done
x1C_trb_abs endp

x14_trb_zp proc
	read_zp_rbx
	mov al, r8b
	not al
	and byte ptr [rcx+rbx], al

	jz set_zero
	add r14, 5
	add r11w, 1			
	jmp opcode_done

set_zero:
	;        NZ A P C
	or r15w, 0100000000000000b
	add r14, 5
	add r11w, 1			
	jmp opcode_done
x14_trb_zp endp

;
; TSB
;

x04_tsb_zp proc
	read_zp_rbx
	or byte ptr [rcx+rbx], r8b

	jz set_zero
	add r14, 5
	add r11w, 1
	jmp opcode_done

set_zero:
	;        NZ A P C
	or r15w, 0100000000000000b
	add r14, 5
	add r11w, 1
	jmp opcode_done
x04_tsb_zp endp

x0C_tsb_abs proc
	read_zp_rbx
	or byte ptr [rcx+rbx], r8b

	jz set_zero
	add r14, 6
	add r11w, 2
	jmp opcode_done

set_zero:
	;        NZ A P C
	or r15w, 0100000000000000b
	add r14, 6
	add r11w, 2
	jmp opcode_done
x0C_tsb_abs endp

;
; RMB
;

x07_rmb0 proc
	read_zp_rbx
	and byte ptr [rcx+rbx], 11111110b
	add r14, 5
	add r11w, 1
	jmp opcode_done
x07_rmb0 endp

x17_rmb1 proc
	read_zp_rbx
	and byte ptr [rcx+rbx], 11111101b
	add r14, 5
	add r11w, 1
	jmp opcode_done
x17_rmb1 endp

x27_rmb2 proc
	read_zp_rbx
	and byte ptr [rcx+rbx], 11111011b
	add r14, 5
	add r11w, 1
	jmp opcode_done
x27_rmb2 endp

x37_rmb3 proc
	read_zp_rbx
	and byte ptr [rcx+rbx], 11110111b
	add r14, 5
	add r11w, 1
	jmp opcode_done
x37_rmb3 endp

x47_rmb4 proc
	read_zp_rbx
	and byte ptr [rcx+rbx], 11101111b
	add r14, 5
	add r11w, 1
	jmp opcode_done
x47_rmb4 endp

x57_rmb5 proc
	read_zp_rbx
	and byte ptr [rcx+rbx], 11011111b
	add r14, 5
	add r11w, 1
	jmp opcode_done
x57_rmb5 endp

x67_rmb6 proc
	read_zp_rbx
	and byte ptr [rcx+rbx], 10111111b
	add r14, 5
	add r11w, 1
	jmp opcode_done
x67_rmb6 endp

x77_rmb7 proc
	read_zp_rbx
	and byte ptr [rcx+rbx], 01111111b
	add r14, 5
	add r11w, 1
	jmp opcode_done
x77_rmb7 endp

;
; SMB
;

x87_smb0 proc
	read_zp_rbx
	or byte ptr [rcx+rbx], 00000001b
	add r14, 5
	add r11w, 1
	jmp opcode_done
x87_smb0 endp

x97_smb1 proc
	read_zp_rbx
	or byte ptr [rcx+rbx], 00000010b
	add r14, 5
	add r11w, 1
	jmp opcode_done
x97_smb1 endp

xa7_smb2 proc
	read_zp_rbx
	or byte ptr [rcx+rbx], 00000100b
	add r14, 5
	add r11w, 1
	jmp opcode_done
xa7_smb2 endp

xb7_smb3 proc
	read_zp_rbx
	or byte ptr [rcx+rbx], 00001000b
	add r14, 5
	add r11w, 1
	jmp opcode_done
xb7_smb3 endp

xc7_smb4 proc
	read_zp_rbx
	or byte ptr [rcx+rbx], 00010000b
	add r14, 5
	add r11w, 1
	jmp opcode_done
xc7_smb4 endp

xd7_smb5 proc
	read_zp_rbx
	or byte ptr [rcx+rbx], 00100000b
	add r14, 5
	add r11w, 1
	jmp opcode_done
xd7_smb5 endp

xe7_smb6 proc
	read_zp_rbx
	or byte ptr [rcx+rbx], 01000000b
	add r14, 5
	add r11w, 1
	jmp opcode_done
xe7_smb6 endp

xf7_smb7 proc
	read_zp_rbx
	or byte ptr [rcx+rbx], 10000000b
	add r14, 5
	add r11w, 1
	jmp opcode_done
xf7_smb7 endp

;
; NOP
;

xEA_nop proc
	add r14, 2	; Clock	
	jmp opcode_done
xEA_nop endp

;
; Exit
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
; should have 76 free when done!
opcode_00	qword	noinstruction 	; $00
opcode_01	qword	x01_ora_indx 	; $01
opcode_02	qword	noinstruction 	; $02
opcode_03	qword	noinstruction 	; $03
opcode_04	qword	x04_tsb_zp	 	; $04
opcode_05	qword	x05_ora_zp	 	; $05
opcode_06	qword	x06_asl_zp	 	; $06
opcode_07	qword	x07_rmb0	 	; $07
opcode_08	qword	x08_php		 	; $08
opcode_09	qword	x09_ora_imm	 	; $09
opcode_0A	qword	x0A_asl_a	 	; $0A
opcode_0B	qword	noinstruction 	; $0B
opcode_0C	qword	x0C_tsb_abs 	; $0C
opcode_0D	qword	x0D_ora_abs	 	; $0D
opcode_0E	qword	x0E_asl_abs	 	; $0E
opcode_0F	qword	x0f_bbr0	 	; $0F
opcode_10	qword	x10_bpl		 	; $10
opcode_11	qword	x11_ora_indy 	; $11
opcode_12	qword	x12_ora_indzp 	; $12
opcode_13	qword	noinstruction 	; $13
opcode_14	qword	x14_trb_zp	 	; $14
opcode_15	qword	x15_ora_zpx	 	; $15
opcode_16	qword	x16_asl_zpx	 	; $16
opcode_17	qword	x17_rmb1	 	; $17
opcode_18	qword	x18_clc		 	; $18
opcode_19	qword	x19_ora_absy 	; $19
opcode_1A	qword	x1A_inc_a	 	; $1A
opcode_1B	qword	noinstruction 	; $1B
opcode_1C	qword	x1C_trb_abs	 	; $1C
opcode_1D	qword	x1D_ora_absx 	; $1D
opcode_1E	qword	x1E_asl_absx 	; $1E
opcode_1F	qword	x1f_bbr1	 	; $1F
opcode_20	qword	x20_jsr		 	; $20
opcode_21	qword	x21_and_indx 	; $21
opcode_22	qword	noinstruction 	; $22
opcode_23	qword	noinstruction 	; $23
opcode_24	qword	x24_bit_zp	 	; $24
opcode_25	qword	x25_and_zp	 	; $25
opcode_26	qword	x26_rol_zp	 	; $26
opcode_27	qword	x27_rmb2	 	; $27
opcode_28	qword	x28_plp		 	; $28
opcode_29	qword	x29_and_imm 	; $29
opcode_2A	qword	x2A_rol_a	 	; $2A
opcode_2B	qword	noinstruction 	; $2B
opcode_2C	qword	x2C_bit_abs 	; $2C
opcode_2D	qword	x2D_and_abs 	; $2D
opcode_2E	qword	x2E_rol_abs 	; $2E
opcode_2F	qword	x2f_bbr2	 	; $2F
opcode_30	qword	x30_bmi		 	; $30
opcode_31	qword	x31_and_indy 	; $31
opcode_32	qword	x32_and_indzp 	; $32
opcode_33	qword	noinstruction 	; $33
opcode_34	qword	x34_bit_zpx 	; $34
opcode_35	qword	x35_and_zpx 	; $35
opcode_36	qword	x36_rol_zpx 	; $36
opcode_37	qword	x37_rmb3 		; $37
opcode_38	qword	x38_sec		 	; $38
opcode_39	qword	x39_and_absy 	; $39
opcode_3A	qword	x3A_dec_a	 	; $3A
opcode_3B	qword	noinstruction 	; $3B
opcode_3C	qword	x3C_bit_absx 	; $3C
opcode_3D	qword	x3D_and_absx 	; $3D
opcode_3E	qword	x3E_rol_absx 	; $3E
opcode_3F	qword	x3f_bbr3	 	; $3F
opcode_40	qword	noinstruction 	; $40
opcode_41	qword	x41_eor_indx 	; $41
opcode_42	qword	noinstruction 	; $42
opcode_43	qword	noinstruction 	; $43
opcode_44	qword	noinstruction 	; $44
opcode_45	qword	x45_eor_zp	 	; $45
opcode_46	qword	x46_lsr_zp	 	; $46
opcode_47	qword	x47_rmb4	 	; $47
opcode_48	qword	x48_pha		 	; $48
opcode_49	qword	x49_eor_imm 	; $49
opcode_4A	qword	x4A_lsr_a	 	; $4A
opcode_4B	qword	noinstruction 	; $4B
opcode_4C	qword	x4C_jmp_abs 	; $4C
opcode_4D	qword	x4D_eor_abs 	; $4D
opcode_4E	qword	x4E_lsr_abs 	; $4E
opcode_4F	qword	x4f_bbr4	 	; $4F
opcode_50	qword	x50_bvc		 	; $50
opcode_51	qword	x51_eor_indy 	; $51
opcode_52	qword	x52_eor_indzp 	; $52
opcode_53	qword	noinstruction 	; $53
opcode_54	qword	noinstruction 	; $54
opcode_55	qword	x55_eor_zpx 	; $55
opcode_56	qword	x56_lsr_zpx 	; $56
opcode_57	qword	x57_rmb5	 	; $57
opcode_58	qword	x58_cli		 	; $58
opcode_59	qword	x59_eor_absy 	; $59
opcode_5A	qword	x5A_phy		 	; $5A
opcode_5B	qword	noinstruction 	; $5B
opcode_5C	qword	noinstruction 	; $5C
opcode_5D	qword	x5D_eor_absx 	; $5D
opcode_5E	qword	x5E_lsr_absx 	; $5E
opcode_5F	qword	x5f_bbr5	 	; $5F
opcode_60	qword	x60_rts		 	; $60
opcode_61	qword	x61_adc_indx 	; $61
opcode_62	qword	noinstruction 	; $62
opcode_63	qword	noinstruction 	; $63
opcode_64	qword	x64_stz_zp	 	; $64
opcode_65	qword	x65_adc_zp	 	; $65
opcode_66	qword	x66_ror_zp	 	; $66
opcode_67	qword	x67_rmb6	 	; $67
opcode_68	qword	x68_pla		 	; $68
opcode_69	qword	x69_adc_imm 	; $69
opcode_6A	qword	x6A_ror_a	 	; $6A
opcode_6B	qword	noinstruction 	; $6B
opcode_6C	qword	x6C_jmp_ind 	; $6C
opcode_6D	qword	x6D_adc_abs 	; $6D
opcode_6E	qword	x6E_ror_abs 	; $6E
opcode_6F	qword	x6f_bbr6	 	; $6F
opcode_70	qword	x70_bvs		 	; $70
opcode_71	qword	x71_adc_indy 	; $71
opcode_72	qword	x72_adc_indzp 	; $72
opcode_73	qword	noinstruction 	; $73
opcode_74	qword	x74_stz_zpx 	; $74
opcode_75	qword	x75_adc_zpx 	; $75
opcode_76	qword	x76_ror_zpx 	; $76
opcode_77	qword	x77_rmb7	 	; $77
opcode_78	qword	x78_sei		 	; $78
opcode_79	qword	x79_adc_absy 	; $79
opcode_7A	qword	x7A_ply		 	; $7A
opcode_7B	qword	noinstruction 	; $7B
opcode_7C	qword	x7C_jmp_absx 	; $7C
opcode_7D	qword	x7D_adc_absx 	; $7D
opcode_7E	qword	x7E_ror_absx 	; $7E
opcode_7F	qword	x7f_bbr7	 	; $7F
opcode_80	qword	x80_bra		 	; $80
opcode_81	qword	x81_sta_indx 	; $81
opcode_82	qword	noinstruction 	; $82
opcode_83	qword	noinstruction 	; $83
opcode_84	qword	x84_sty_zp	 	; $84
opcode_85	qword	x85_sta_zp	 	; $85
opcode_86	qword	x86_stx_zp	 	; $86
opcode_87	qword	x87_smb0	 	; $87
opcode_88	qword	x88_dey		 	; $88
opcode_89	qword	x89_bit_imm 	; $89
opcode_8A	qword	x8A_txa		 	; $8A
opcode_8B	qword	noinstruction 	; $8B
opcode_8C	qword	x8C_sty_abs 	; $8C
opcode_8D	qword	x8D_sta_abs 	; $8D
opcode_8E	qword	x8E_stx_abs 	; $8E
opcode_8F	qword	x8F_bbs0	 	; $8F
opcode_90	qword	x90_bcc		 	; $90
opcode_91	qword	x91_sta_indy 	; $91
opcode_92	qword	x92_sta_indzp 	; $92
opcode_93	qword	noinstruction 	; $93
opcode_94	qword	x94_sty_zpx 	; $94
opcode_95	qword	x95_sta_zpx 	; $95
opcode_96	qword	x96_stx_zpy 	; $96
opcode_97	qword	x97_smb1	 	; $97
opcode_98	qword	x98_tya		 	; $98
opcode_99	qword	x99_sta_absy 	; $99
opcode_9A	qword	x9A_txs		 	; $9A
opcode_9B	qword	noinstruction 	; $9B
opcode_9C	qword	x9C_stz_abs 	; $9C
opcode_9D	qword	x9D_sta_absx 	; $9D
opcode_9E	qword	x9E_stz_absx 	; $9E
opcode_9F	qword	x9F_bbs1	 	; $9F
opcode_A0	qword	xA0_ldy_imm 	; $A0
opcode_A1	qword	xA1_lda_indx 	; $A1
opcode_A2	qword	xA2_ldx_imm 	; $A2
opcode_A3	qword	noinstruction 	; $A3
opcode_A4	qword	xA4_ldy_zp	 	; $A4
opcode_A5	qword	xA5_lda_zp	 	; $A5
opcode_A6	qword	xA6_ldx_zp	 	; $A6
opcode_A7	qword	xa7_smb2	 	; $A7
opcode_A8	qword	xA8_tay		 	; $A8
opcode_A9	qword	xA9_lda_imm 	; $A9
opcode_AA	qword	xAA_tax		 	; $AA
opcode_AB	qword	noinstruction 	; $AB
opcode_AC	qword	xAC_ldy_abs 	; $AC
opcode_AD	qword	xAD_lda_abs 	; $AD
opcode_AE	qword	xAE_ldx_abs 	; $AE
opcode_AF	qword	xAF_bbs2	 	; $AF
opcode_B0	qword	xB0_bcs		 	; $B0
opcode_B1	qword	xB1_lda_indy 	; $B1
opcode_B2	qword	xB2_lda_indzp 	; $B2
opcode_B3	qword	noinstruction 	; $B3
opcode_B4	qword	xB4_ldy_zpx 	; $B4
opcode_B5	qword	xB5_lda_zpx 	; $B5
opcode_B6	qword	xB6_ldx_zpy 	; $B6
opcode_B7	qword	xb7_smb3	 	; $B7
opcode_B8	qword	xB8_clv		 	; $B8
opcode_B9	qword	xB9_lda_absy 	; $B9
opcode_BA	qword	xBA_tsx		 	; $BA
opcode_BB	qword	noinstruction 	; $BB
opcode_BC	qword	xBC_ldy_absx 	; $BC
opcode_BD	qword	xBD_lda_absx 	; $BD
opcode_BE	qword	xBE_ldx_absy 	; $BE
opcode_BF	qword	xBF_bbs3 		; $BF
opcode_C0	qword	xC0_cmpy_imm 	; $C0
opcode_C1	qword	xC1_sbc_indx 	; $C1
opcode_C2	qword	noinstruction 	; $C2
opcode_C3	qword	noinstruction 	; $C3
opcode_C4	qword	xC4_cmpy_zp 	; $C4
opcode_C5	qword	xC5_cmp_zp	 	; $C5
opcode_C6	qword	xC6_dec_absx 	; $C6
opcode_C7	qword	xc7_smb4	 	; $C7
opcode_C8	qword	xC8_iny			; $C8
opcode_C9	qword	xC9_cmp_imm 	; $C9
opcode_CA	qword	xCA_dex		 	; $CA
opcode_CB	qword	noinstruction 	; $CB
opcode_CC	qword	xCC_cmpy_abs 	; $CC
opcode_CD	qword	xCD_cmp_abs 	; $CD
opcode_CE	qword	xCE_dec_abs 	; $CE
opcode_CF	qword	xCF_bbs4 	 	; $CF
opcode_D0	qword	xD0_bne		 	; $D0
opcode_D1	qword	xD1_cmp_indy 	; $D1
opcode_D2	qword	xD2_cmp_indzp 	; $D2
opcode_D3	qword	noinstruction 	; $D3
opcode_D4	qword	noinstruction 	; $D4
opcode_D5	qword	xD5_cmp_zpx 	; $D5
opcode_D6	qword	xD6_dec_zpx 	; $D6
opcode_D7	qword	xd7_smb5 		; $D7
opcode_D8	qword	xD8_cld		 	; $D8
opcode_D9	qword	xD9_cmp_absy 	; $D9
opcode_DA	qword	xDA_phx		 	; $DA
opcode_DB	qword	xDB_stp		 	; $DB
opcode_DC	qword	noinstruction 	; $DC
opcode_DD	qword	xDD_cmp_absx 	; $DD
opcode_DE	qword	xDE_dec_absx 	; $DE
opcode_DF	qword	xDF_bbs5	 	; $DF
opcode_E0	qword	xE0_cmpx_imm 	; $E0
opcode_E1	qword	xE1_sbc_indx 	; $E1
opcode_E2	qword	noinstruction 	; $E2
opcode_E3	qword	noinstruction 	; $E3
opcode_E4	qword	xE4_cmpx_zp 	; $E4
opcode_E5	qword	xE5_sbc_zp	 	; $E5
opcode_E6	qword	xE6_inc_zp	 	; $E6
opcode_E7	qword	xe7_smb6	 	; $E7
opcode_E8	qword	xE8_inx	 		; $E8
opcode_E9	qword	xE9_sbc_imm 	; $E9
opcode_EA	qword	xEA_nop		 	; $EA
opcode_EB	qword	noinstruction 	; $EB
opcode_EC	qword	xEC_cmpx_abs 	; $EC
opcode_ED	qword	xED_sbc_abs 	; $ED
opcode_EE	qword	xEE_inc_abs 	; $EE
opcode_EF	qword	xEF_bbs6 	; $EF
opcode_F0	qword	xF0_beq		 	; $F0
opcode_F1	qword	xF1_sbc_indy 	; $F1
opcode_F2	qword	xF2_sbc_indzp 	; $F2
opcode_F3	qword	noinstruction 	; $F3
opcode_F4	qword	noinstruction 	; $F4
opcode_F5	qword	xF5_sbc_zpx 	; $F5
opcode_F6	qword	xF6_inc_zpx 	; $F6
opcode_F7	qword	xf7_smb7	 	; $F7
opcode_F8	qword	xF8_sed		 	; $F8
opcode_F9	qword	xF9_sbc_absy 	; $F9
opcode_FA	qword	xFA_plx		 	; $FA
opcode_FB	qword	noinstruction 	; $FB
opcode_FC	qword	noinstruction 	; $FC
opcode_FD	qword	xFD_sbc_absx 	; $FD
opcode_FE	qword	xFE_inc_absx 	; $FE
opcode_FF	qword	xFF_bbs7	 	; $FF



END