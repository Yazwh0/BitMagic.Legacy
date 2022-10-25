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

	; interrupt
	movzx rax, [rdx].state.via_interrupt_timer1
	shl rax, 6	

	movzx rbx, [rdx].state.via_interrupt_timer2
	shl rbx, 5
	or rax, rbx

	movzx rbx, [rdx].state.via_interrupt_cb1
	shl rbx, 4
	or rax, rbx

	movzx rbx, [rdx].state.via_interrupt_cb2
	shl rbx, 3
	or rax, rbx

	movzx rbx, [rdx].state.via_interrupt_shiftregister
	shl rbx, 2
	or rax, rbx

	movzx rbx, [rdx].state.via_interrupt_ca1
	shl rbx, 1
	or rax, rbx

	movzx rbx, [rdx].state.via_interrupt_ca2
	or rax, rbx

	mov byte ptr [rsi + V_IER], al

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

	movzx r13, byte ptr [rdx].state.via_interrupt_timer1
	cmp r13, 1
	jne skip_timer1_set
	mov cl, 1
	skip_timer1_set:
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

	movzx r13, byte ptr [rdx].state.via_interrupt_timer2
	cmp r13, 1
	jne skip_timer2_set
	mov cl, 1
	skip_timer2_set:
	or rdi, 00100000b
	mov byte ptr [rdx].state.via_timer2_running, 0	; clear running flag

	no_timer2_interrupt:
	movzx rax, word ptr [rsi + V_T2_L]
	sub ax, r12w

	timer2_not_zero:
	mov word ptr [rsi + V_T2_L], ax

	timer2_alldone:

	mov byte ptr [rdx].state.nmi, cl	; set nmi

	mov rax, 80h
	test rcx, rcx
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

via_timer1_counter_h proc
	mov r13b, byte ptr [rsi+rbx]		; copy value to latch
	mov byte ptr [rsi+V_T1L_H], r13b

	mov r13w, word ptr [rsi+V_T1L_L]	; on high byte set, restart counter from latch
	mov word ptr [rsi+V_T1_L], r13w	

	mov r13b, byte ptr [rsi+V_IFR]		; set timer 1 bit on IFR
	and r13b, 10111111b
	mov byte ptr [rsi+V_IFR], r13b

	mov byte ptr [rdx].state.via_timer1_running, 1	; mark as running
	ret
via_timer1_counter_h endp

via_timer1_latch_l proc
	ret
via_timer1_latch_l endp

via_timer1_latch_h proc	
	; does not copy the value, but does set the IFR flag
	mov r13b, byte ptr [rsi+V_IFR]		; set timer 1 bit on IFR
	and r13b, 10111111b
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
	xor r13, 0ffh						; invert
	or r13, 80h							; turn on bit 7, as we preserve this
	and r12, r13						; and with original value. should clear bits that were written

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

	; now set flags
	xor r13, r13
	test r12, 00000001b
	setnz r13b
	mov byte ptr [rdx].state.via_interrupt_ca2, r13b

	xor r13, r13
	test r12, 00000010b
	setnz r13b
	mov byte ptr [rdx].state.via_interrupt_ca1, r13b

	xor r13, r13
	test r12, 00000100b
	setnz r13b
	mov byte ptr [rdx].state.via_interrupt_shiftregister, r13b

	xor r13, r13
	test r12, 00001000b
	setnz r13b
	mov byte ptr [rdx].state.via_interrupt_cb2, r13b

	xor r13, r13
	test r12, 00010000b
	setnz r13b
	mov byte ptr [rdx].state.via_interrupt_cb1, r13b

	xor r13, r13
	test r12, 00100000b
	setnz r13b
	mov byte ptr [rdx].state.via_interrupt_timer2, r13b

	xor r13, r13
	test r12, 01000000b
	setnz r13b
	mov byte ptr [rdx].state.via_interrupt_timer1, r13b

	ret

unset:
	xor r13, 0ffh
	and r12, r13
	and r12, 7fh
	or r12, 80h		; bit 7 is always set
	mov byte ptr [rsi+rbx], r12b

	; now set flags
	xor r13, r13
	test r12, 00000001b
	setnz r13b
	mov byte ptr [rdx].state.via_interrupt_ca2, r13b

	xor r13, r13
	test r12, 00000010b
	setnz r13b
	mov byte ptr [rdx].state.via_interrupt_ca1, r13b

	xor r13, r13
	test r12, 00000100b
	setnz r13b
	mov byte ptr [rdx].state.via_interrupt_shiftregister, r13b

	xor r13, r13
	test r12, 00001000b
	setnz r13b
	mov byte ptr [rdx].state.via_interrupt_cb2, r13b

	xor r13, r13
	test r12, 00010000b
	setnz r13b
	mov byte ptr [rdx].state.via_interrupt_cb1, r13b

	xor r13, r13
	test r12, 00100000b
	setnz r13b
	mov byte ptr [rdx].state.via_interrupt_timer2, r13b

	xor r13, r13
	test r12, 01000000b
	setnz r13b
	mov byte ptr [rdx].state.via_interrupt_timer1, r13b

	ret
via_ier endp