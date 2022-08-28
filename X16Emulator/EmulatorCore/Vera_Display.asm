.code
include State.asm

; rax  : scratch
; rbx  : scratch
; rcx  : current memory context (vram)
; rdx  : state object 
; rsi  : scratch
; rdi  : display
; r8b  : scratch
; r9b  : scratch
; r10b : scratch
; r11w : scratch
; r12  : scratch
; r13  : scratch
; r14  : Clock Ticks
; r15  : Flags

vera_render_display proc
	push r8
	push r9
	push r10
	push r11

	mov rcx, [rdx].state.vram_ptr
	mov rdi, [rdx].state.display_ptr

	pop r11
	pop r10
	pop r9
	pop r8

	jmp main_loop

vera_render_display endp

