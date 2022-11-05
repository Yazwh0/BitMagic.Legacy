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

;
; *** Known issues ***
; Via timing will not be perfect on a read
; because the value in memory is based upon the last update, not the time of the read.
;
include State.asm

via_init proc
	mov rsi, [rdx].state.memory_ptr

	; timers
	mov ax, word ptr [rdx].state.via_t1counter_value
	mov word ptr [rsi + V_T1_L], ax

	mov ax, word ptr [rdx].state.via_t1counter_latch
	mov word ptr [rsi + V_T1L_L], ax
	
	mov ax, word ptr [rdx].state.via_t2counter_value
	mov word ptr [rsi + V_T2_L], ax

	mov al, byte ptr [rsi + V_IER]
	or al, 080h
	mov byte ptr [rsi + V_IER], al
	
	mov al, byte ptr [rsi + V_IFR]
	mov bl, al
	or bl, 080h
	test al, al
	cmovne rax, rbx
	mov byte ptr [rsi + V_IFR], al

	mov al, byte ptr [rdx].state.via_register_a_outvalue
	mov byte ptr [rsi + V_PRA], al
	mov byte ptr [rsi + V_ORA], al

	ret
via_init endp

via_write_state proc
	mov rsi, [rdx].state.memory_ptr

	; timers
	mov ax, word ptr [rsi + V_T1_L]
	mov word ptr [rdx].state.via_t1counter_value, ax

	mov ax, word ptr [rsi + V_T1L_L]
	mov word ptr [rdx].state.via_t1counter_latch, ax
	
	mov ax, word ptr [rsi + V_T2_L]
	mov word ptr [rdx].state.via_t2counter_value, ax

	; dont get register in\out value, as its not stored in memory	

	ret
via_write_state endp

; todo: change to macro
; requires:
; r14 to be cpu clock
; rbx last cpu clock -- this doesn't appear to be true, need to get and keep this delta else where
; ----
; use rcx to keep track of if nmi is set
via_step proc	
	xor rbx, rbx
	movzx rcx, byte ptr [rdx].state.nmi
	mov byte ptr [rdx].state.nmi_previous, cl
	movzx rdi, byte ptr [rsi + V_IFR]

	mov r13, qword ptr [rdx].state.clock_previous
	mov r12, r14
	sub r12, r13						; now has the delta

	;
	; Timer 1
	;
	movzx r13, byte ptr [rdx].state.via_timer1_running
	cmp r13, 1
	jne no_timer1_interrupt


	movzx rax, word ptr [rsi + V_T1_L]
	sub ax, r12w

	jnc timer1_not_zero ; jump if carry not set.
	mov ax, word ptr [rsi + V_T1L_L]
	mov word ptr [rsi + V_T1_L], ax

	or rdi, 01000000b

	movzx rax, byte ptr [rdx].state.via_timer1_continuous
	test rax, rax
	jnz timer1_alldone

	mov byte ptr [rdx].state.via_timer1_running, 0	; clear running flag if we're in one-shot mode

	jmp timer1_alldone

	no_timer1_interrupt:
	movzx rax, word ptr [rsi + V_T1_L]
	sub ax, r12w

	timer1_not_zero:
	mov word ptr [rsi + V_T1_L], ax

	timer1_alldone:

	;
	; Timer 2
	;
	movzx r13, byte ptr [rdx].state.via_timer2_pulsecount ; if in pulse mode, we dont dec here
	test r13, r13
	jnz timer2_alldone
	

	movzx r13, byte ptr [rdx].state.via_timer2_running
	test r13, r13
	jz no_timer2_interrupt

	mov ax, word ptr [rsi + V_T2_L]
	sub ax, r12w

	jnc timer2_not_zero

	mov ax, word ptr [rdx].state.via_t2counter_latch
	mov word ptr [rsi + V_T1_L], ax

	or rdi, 00100000b

	mov byte ptr [rdx].state.via_timer2_running, 0	; clear running flag

	no_timer2_interrupt:
	movzx rax, word ptr [rsi + V_T2_L]
	sub ax, r12w

	timer2_not_zero:
	mov word ptr [rsi + V_T2_L], ax

	timer2_alldone:

	mov cl, byte ptr[rsi + V_IER]
	and cl, 7fh
	and cl, dil
	setnz cl							; change to 1 if non zero
	mov byte ptr [rdx].state.nmi, cl	; set nmi

	and dil, 7fh		; mask of irq bits
	mov rax, 80h
;	test rcx, rcx
	cmovz rax, rbx
	or dil, al

	mov byte ptr [rsi + V_IFR], dil		; mask on interrupt bit if nmi is high
	
	ret
via_step endp

; rbx: address that was updated
; rsi: memory
via_timer1_counter_l proc
	mov r13b, byte ptr [rsi+rbx]		; copy value to latch
	mov byte ptr [rsi+V_T1L_L], r13b
	mov byte ptr [rsi+rbx], r12b		; counter is not affected until high byte is changed
	ret
via_timer1_counter_l endp

; reading T1C_L resets t1 interrupt flag
via_timer1_counter_l_read proc
	mov al, byte ptr [rsi+V_IFR]
	and al, 00111111b
	mov bl, al
	or bl, 80h
	test al, al
	cmovne rax, rbx

	mov byte ptr [rsi+V_IFR], al
	ret
via_timer1_counter_l_read endp

via_timer1_counter_h proc
	mov r13b, byte ptr [rsi+rbx]		; copy value to latch
	mov byte ptr [rsi+V_T1L_H], r13b

	mov r13w, word ptr [rsi+V_T1L_L]	; on high byte set, restart counter from latch
	mov word ptr [rsi+V_T1_L], r13w	

	mov byte ptr [rdx].state.via_timer1_running, 1	; mark as running
	ret
via_timer1_counter_h endp

via_timer1_latch_l proc
	ret
via_timer1_latch_l endp

via_timer1_latch_h proc	
	; does not copy the value, but does set the IFR flag	
	mov r13b, byte ptr [rsi+V_IFR]		; set timer 1 bit on IFR
	and r13b, 00111111b
	mov bl, r13b
	or bl, 80h
	test r13b, r13b
	cmovne r13, rbx
	mov byte ptr [rsi+V_IFR], r13b

	ret
via_timer1_latch_h endp

via_timer2_latch_l proc
	mov r13b, byte ptr [rsi+rbx]
	mov byte ptr [rdx].state.via_t2counter_latch, r13b	; get value and store
	mov byte ptr [rsi+rbx], r12b				; counter is not affected
	ret
via_timer2_latch_l endp

via_timer2_latch_h proc
	mov r13b, byte ptr [rsi+rbx]
	mov byte ptr [rdx].state.via_t2counter_latch+1, r13b	; get value and store
	mov ax, word ptr [rdx].state.via_t2counter_latch
	mov word ptr [rsi+V_T2_L], ax							; copy value from latch to counter
	mov byte ptr [rdx].state.via_timer2_running, 1			; set running flag
	ret
via_timer2_latch_h endp

via_acl proc
	mov r13b, byte ptr [rsi+rbx]

	xor rax, rax
	test r13b, 01000000b
	setnz al
	mov byte ptr [rdx].state.via_timer1_continuous, al

	xor rax, rax
	test r13b, 10000000b
	setnz al
	mov byte ptr [rdx].state.via_timer1_pb7, al

	xor rax, rax
	test r13b, 00100000b
	setnz al
	mov byte ptr [rdx].state.via_timer2_pulsecount, al

	ret
via_acl endp

via_ifr proc
	movzx r13, byte ptr [rsi+rbx]		; get value written
	xor rdi, rdi
	xor r13, 0ffh						; invert
	mov rax, 80h						; turn on bit 7, as we mask this on if there is a value
	and r12, r13						; and with original value. should clear bits that were written
	cmovne rdi, rax						; set high bit if the and results in value
	or r12, rdi							; set high bit

	mov byte ptr [rsi+rbx], r12b		; write back
	ret
via_ifr endp

via_ier proc
	mov r13b, byte ptr [rsi+rbx]		; get value
	test r13, 10000000b
	jz unset

	or r12, r13
	and r12, 7fh
	or r12, 80h		; bit 7 is always set
	mov byte ptr [rsi+rbx], r12b

	ret

unset:
	xor r13, 0ffh
	and r12, r13
	and r12, 7fh
	or r12, 80h		; bit 7 is always set
	mov byte ptr [rsi+rbx], r12b

	ret
via_ier endp

; Serial Bus
via_prb proc
	mov r13b, byte ptr [rsi+rbx]
	mov byte ptr [rsi+rbx], 080h ; we dont support the serial bus yet, so this is a good default state, and lets the kernal proceed
	ret
via_prb endp

; I2C
; Data Direction Register 0: input, 1: output.
via_dra proc
	mov r13b, byte ptr [rsi+rbx]
	xor r13b, 0ffh						; invert

	mov r12b, byte ptr [rdx].state.via_register_a_outvalue
	or r12b, r13b						; set bits high that are output
	mov byte ptr [rsi+V_PRA], r12b
	mov byte ptr [rsi+V_ORA], r12b

	ret
via_dra endp

; writes to the registers are stored, but only show in memory depending on DRA (Data Direction Register)
via_pra proc
	mov r13b, byte ptr [rsi+rbx]
	mov byte ptr [rdx].state.via_register_a_outvalue, r13b	; store new value

	mov dil, byte ptr [rsi+V_DDRA]							
	and r13, rdi											; mask off bytes. 

	xor dil, 0ffh											; invert mask

	mov al, byte ptr [rdx].state.via_register_a_invalue		; get value in
	and rax, rdi

	or r13, rax												; merge two values and store
	mov byte ptr [rsi+rbx], r13b
	mov byte ptr [rsi+V_ORA], r13b

	ret
via_pra endp
