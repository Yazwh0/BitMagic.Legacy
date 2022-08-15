.code

; Cpu emulation:
; rax  : scratch
; rbx  : scratch
; rcx  : current memory context
; rdx  : state object 
; rsi  : scratch
; rdi  : scratch
; r8b  : a
; r9b  : x
; r10b : y
; r11w : PC
; r12  : scratch
; r13  : scratch / use to indicate vera data0 or 1 read
; r14  : Clock Ticks
; r15  : Flags

include vera_constants.inc


vera_state struct
	vram_ptr		qword ?
	data0_address	qword ?
	data1_address	qword ?

	data0_step		qword ?
	data1_step		qword ?


vera_state ends


; initialise colours etc
vera_init proc

	
	ret
vera_init endp


; rbx			address read from
; [rcx+rbx]		output location in main memory
; 
; should only be called if data0\data1 is read from.
vera_afterread proc

	;cmp rbx, DATA0
	jne step_data1

	;mov rax, [rdx].state.vera_ptr
	mov rsi, [rax].vera_state.data0_address
	add rsi, [rax].vera_state.data0_step
	and rsi, 1ffffh								; mask off high bits so we wrap
	mov [rax].vera_state.data0_address, rsi

	mov rax, [rax].vera_state.vram_ptr
	mov r13b, byte ptr [rax+rsi]
	mov [rcx+rbx], r13b

	ret
step_data1:



	ret
vera_afterread endp

vera_afterreadwrite proc
vera_afterreadwrite endp

vera_afterwrite proc
vera_afterwrite endp

.data

vera_step_table:
	dw 0, 1, 2, 4 ,8, 16, 32, 64, 128, 256, 512, 40, 80, 160, 320, 640

.code

end