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
; rbx last cpu clock

via_step proc
	mov r12, r14
	sub r12, rbx
	mov ax, word ptr [rsi + V_T1_L]
	sub ax, r12w
	mov word ptr [rsi + V_T1_L], ax

	mov ax, word ptr [rsi + V_T2_L]
	sub ax, r12w
	mov word ptr [rsi + V_T2_L], ax
	
	ret
via_step endp

; rbx: address that was updated
; rsi: memory
via_timer1_counter_l proc
	mov r13b, byte ptr [rsi+rbx]		; copy value to latch
	mov byte ptr [rsi+V_T1L_L], r13b
	mov byte ptr [rsi+rbx], r12b		; counter is preserved
	ret
via_timer1_counter_l endp

via_timer1_counter_h proc
	mov r13b, byte ptr [rsi+rbx]		; copy value to latch
	mov byte ptr [rsi+V_T1L_H], r13b

	mov r13w, word ptr [rsi+V_T1L_L]	; on high byte set, restart counter from latch
	mov word ptr [rsi+V_T1_L], r13w	
	ret
via_timer1_counter_h endp

via_timer1_latch_l proc
	ret
via_timer1_latch_l endp

via_timer1_latch_h proc
	ret
via_timer1_latch_h endp

via_acl proc
via_acl endp

via_ifr proc
	ret
via_ifr endp

via_ier proc
	ret
via_ier endp