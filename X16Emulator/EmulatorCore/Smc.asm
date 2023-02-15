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


; read the state of the i2c bus and set state as required
; expects 
; rbx :- message (alternativley in i2c_transmit)
smc_process_message proc
	mov dword ptr [rdx].state.smc_offset, ebx

	cmp ebx, 7
	jg unknown_command

	lea rax, smc_commands
	jmp qword ptr [rax + rbx * 8]

smc_commands:
	qword smc_donothing		; 0
	qword smc_power			; 1
	qword smc_reset			; 2
	qword smc_nmibutton		; 3
	qword smc_powerled		; 4
	qword smc_activityled	; 5
	qword smc_donothing		; 6
	qword smc_keyboard		; 7

unknown_command:
	ret
smc_process_message endp

; return rbx as scancode
smc_set_next_write proc
	mov eax, dword ptr [rdx].state.smc_keyboard_readposition
	mov r13d, dword ptr [rdx].state.smc_keyboard_writeposition

	cmp eax, r13d
	je no_data

	mov rbx, qword ptr [rdx].state.smc_keyboard_ptr
	movzx rbx, byte ptr [rbx + rax]
	inc rax
	and rax, 16-1
	; dont save position, this is done post read.
	;mov dword ptr [rdx].state.smc_keyboard_readposition, eax

	xor r12, r12
	cmp eax, r13d	; check if we're the same now, if so there is no more darta
	sete r12b

	mov dword ptr [rdx].state.smc_keyboard_readnodata, r12d
	mov dword ptr [rdx].state.i2c_datatotransmit, 1

	ret
no_data:
	xor rbx, rbx
	mov dword ptr [rdx].state.smc_keyboard_readnodata, 1
	mov dword ptr [rdx].state.i2c_datatotransmit, 0
	ret
smc_set_next_write endp

smc_complete_write proc
	mov eax, dword ptr [rdx].state.smc_keyboard_readposition
	inc rax
	and rax, 16-1
	mov dword ptr [rdx].state.smc_keyboard_readposition, eax
	ret
smc_complete_write endp

smc_power proc
	mov dword ptr [rdx].state.control, 2	; todo: use smc_powerdown return code
	ret
smc_power endp

smc_reset proc
	ret
smc_reset endp

smc_nmibutton proc
	ret
smc_nmibutton endp

smc_powerled proc
	ret
smc_powerled endp

smc_activityled proc
	ret
smc_activityled endp

smc_keyboard proc
	ret
smc_keyboard endp

smc_donothing proc
	ret
smc_donothing endp
