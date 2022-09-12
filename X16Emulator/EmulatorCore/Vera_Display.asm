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

.code
include State.asm

; rax  : scratch
; rbx  : scratch
; rcx  : loop target
; rdx  : state object 
; rsi  : vram
; rdi  : display
; r8   : palette
; r9   : output offset
; r10  : should display
; r11  : scratch
; r12  : scratch
; r13  : scratch
; r14  : scratch
; r15  : scratch




SCREEN_WIDTH	equ 800
SCREEN_HEIGHT	equ 525
SCREEN_DOTS		equ SCREEN_WIDTH * SCREEN_HEIGHT

BACKGROUND		equ 0
SPRITE_L1		equ SCREEN_DOTS
LAYER0			equ SCREEN_DOTS*2
SPRITE_L2		equ SCREEN_DOTS*3
LAYER1			equ SCREEN_DOTS*4
SPRITE_L3		equ SCREEN_DOTS*5

VISIBLE_WIDTH	equ 640
VISIBLE_HEIGHT	equ 480

VBLANK			equ 480

;
; Render the rest of the display, only gets called on vsync
;
vera_render_display proc
	mov rax, [rdx].state.vera_clock
	mov [rdx].state.vera_clock, r14				; store vera clock
	mov rcx, r14								; Cpu Clock ticks
	sub rcx, rax								; Take off vera ticks for the number of cpu ticks we need to process

	jnz change									; if nothing to do, leave
	ret

change:
	push rsi
	push rdi
	push r8
	push r9
	push r10

	mov rax, rcx								; keep hold of base ticks
	shl rcx, 3
	mov rbx, rcx
	add rcx, rbx								; now * 3
	add rcx, rbx								
	add rcx, rax								; now * 3.125


	movzx rax, [rdx].state.display_carry		; Get carry, and add it on
	add rcx, rax

	mov rax, rcx								; Store to trim
	and al, 111b
	mov byte ptr [rdx].state.display_carry, al	; save carry
	shr rcx, 3									; round, rcx now contains the steps

	mov rsi, [rdx].state.vram_ptr
	mov rdi, [rdx].state.display_ptr
	mov r8, [rdx].state.palette_ptr
	mov r9d, [rdx].state.display_position

	lea r10, should_display_table

display_loop:
	mov al, byte ptr [r10 + r9]
	test al, al
	jz display_skip

	; background
	mov ebx, dword ptr [r8]
	mov [rdi + r9 * 4 + BACKGROUND], ebx




display_skip:
	add r9, 1
	cmp r9, SCREEN_DOTS
	jne no_reset
	xor r9, r9

no_reset:
	dec rcx
	jnz display_loop

done:
	mov dword ptr [rdx].state.display_position, r9d

	pop r10
	pop r9
	pop r8
	pop rdi
	pop rsi
	ret
vera_render_display endp

; Render the display to the CPU clock
vera_render_display_cpu proc
	ret
vera_render_display_cpu endp


vera_render_display_old proc

	push r8
	push r9
	push r10
	push r11

	mov rax, [rdx].state.vera_clock
	mov [rdx].state.vera_clock, r14		; store vera clock
	mov rcx, r14						; Cpu Clock ticks
	sub rcx, rax						; Take off vera ticks for the number of cpu ticks we need to process

	and rax, 0111b						; mask off start value
	shl rax, 3							; multiply up
	add rax, rcx						; add on steps
	lea r11, cpu_step_table
	movzx rcx, byte ptr [r11 + rax]
		
	movzx r10, byte ptr [rdx].state.drawing
	mov rsi, [rdx].state.vram_ptr
	mov rdi, [rdx].state.display_ptr
	mov r11d, dword ptr [rdx].state.display_position
;	movzx r12, word ptr [rdx].state.display_x
;	movzx r13, word ptr [rdx].state.display_y

display_loop:
	
	; do render
	test r10, r10
	jz render_done

	; pixel is in visible window



	add r11, 1											; move pixel ptr on

render_done:
	; coords
	add r12, 1

	;cmp r12, VISIBLE_WIDTH
	;jl loop_end

	;cmp r12, SCREEN_WIDTH
	;je next_line

	;xor r10, r10										; we're outside the bounds, stop drawing

loop_end:
	loop display_loop

	mov [rdx].state.display_position, r11d
;	mov [rdx].state.display_x, r12w
;	mov [rdx].state.display_y, r13w

	mov byte ptr [rdx].state.drawing, r10b

	xor r13, r13 ; r13 needs to be cleared for the CPU emulation

	pop r11
	pop r10
	pop r9
	pop r8

	jmp display_done

next_line:
	xor r12, r12
	add r13, 1	; no need to check for max lines, as we do that with the beam check.
	cmp r13, SCREEN_HEIGHT
	jne not_new_screen

	xor r11, r11
	xor r12, r12
	xor r13, r13

not_new_screen:
	mov r10, 1											; new line, start drawing, gets changed if vsync

	; check line IRQ
	cmp byte ptr [rdx].state.interrupt_line, 0			; have a line interrupt?
	je line_irqskip

	cmp r13w, word ptr [rdx].state.interrupt_linenum	; are we on the line?
	jne line_irqskip

	cmp byte ptr [rdx].state.interrupt_line_hit, 0		; has it been set and not cleared?
	jne line_irqskip

	mov rbx, [rdx].state.memory_ptr		
	or byte ptr [rbx+ISR], 2							; set bit in memory
	mov byte ptr [rdx].state.interrupt_line_hit, 1		; record that its been hit

	mov byte ptr [rdx].state.interrupt, 1				; cpu interrupt

line_irqskip:
	; Check vblank IRQ
	cmp r13, VBLANK
	jne loop_end

	xor r10, r10										; vblank means we're out of bounds, stop drawing

	cmp byte ptr [rdx].state.interrupt_vsync, 0			; have a vsync interrupt?
	je loop_end

	cmp byte ptr [rdx].state.interrupt_vsync_hit, 0		; has it been set and not cleared?
	jne loop_end

	mov rbx, [rdx].state.memory_ptr
	or byte ptr [rbx+ISR], 1
	mov byte ptr [rdx].state.interrupt_vsync_hit, 1		; record that its been hit

	mov byte ptr [rdx].state.interrupt, 1

vera_render_display_old endp



vera_initialise_palette proc	
	xor rax, rax
	mov rsi, [rdx].state.palette_ptr
	lea rcx, vera_default_palette

create_argb_palette:
	; need to go from xRBG
	; to xxBBGGRR as its little endian
	mov r9, 0ff000000h
	mov word ptr bx, [rcx + rax * 2]

	; Red
	mov r8, rbx
	and r8, 0f00h
	shr r8, 8
	or r9, r8

	shl r8, 4
	or r9, r8

	; Green
	mov r8, rbx
	and r8, 000fh
	shl r8, 16
	or r9, r8

	shl r8, 4
	or r9, r8

	;Blue
	mov r8, rbx
	and r8, 00f0h
	shl r8, 4
	or r9, r8

	shl r8, 4
	or r9, r8

	mov dword ptr [rsi + rax * 4], r9d

	add rax, 1
	cmp rax, 100h
	jne create_argb_palette

	ret
vera_initialise_palette endp

.data

vera_default_palette:
; display here as xRGB, but written to memory as little endian, so GBxR
dw 0000h, 0fffh, 0800h, 0afeh, 0c4ch, 00c5h, 000ah, 0ee7h, 0d85h, 0640h, 0f77h, 0333h, 0777h, 0af6h, 008fh, 0bbbh
dw 0000h, 0111h, 0222h, 0333h, 0444h, 0555h, 0666h, 0777h, 0888h, 0999h, 0aaah, 0bbbh, 0ccch, 0dddh, 0eeeh, 0fffh
dw 0211h, 0433h, 0644h, 0866h, 0a88h, 0c99h, 0fbbh, 0211h, 0422h, 0633h, 0844h, 0a55h, 0c66h, 0f77h, 0200h, 0411h
dw 0611h, 0822h, 0a22h, 0c33h, 0f33h, 0200h, 0400h, 0600h, 0800h, 0a00h, 0c00h, 0f00h, 0221h, 0443h, 0664h, 0886h
dw 0aa8h, 0cc9h, 0febh, 0211h, 0432h, 0653h, 0874h, 0a95h, 0cb6h, 0fd7h, 0210h, 0431h, 0651h, 0862h, 0a82h, 0ca3h
dw 0fc3h, 0210h, 0430h, 0640h, 0860h, 0a80h, 0c90h, 0fb0h, 0121h, 0343h, 0564h, 0786h, 09a8h, 0bc9h, 0dfbh, 0121h
dw 0342h, 0463h, 0684h, 08a5h, 09c6h, 0bf7h, 0120h, 0241h, 0461h, 0582h, 06a2h, 08c3h, 09f3h, 0120h, 0240h, 0360h
dw 0480h, 05a0h, 06c0h, 07f0h, 0121h, 0343h, 0465h, 0686h, 08a8h, 09cah, 0bfch, 0121h, 0242h, 0364h, 0485h, 05a6h
dw 06c8h, 07f9h, 0020h, 0141h, 0162h, 0283h, 02a4h, 03c5h, 03f6h, 0020h, 0041h, 0061h, 0082h, 00a2h, 00c3h, 00f3h
dw 0122h, 0344h, 0466h, 0688h, 08aah, 09cch, 0bffh, 0122h, 0244h, 0366h, 0488h, 05aah, 06cch, 07ffh, 0022h, 0144h
dw 0166h, 0288h, 02aah, 03cch, 03ffh, 0022h, 0044h, 0066h, 0088h, 00aah, 00cch, 00ffh, 0112h, 0334h, 0456h, 0668h
dw 088ah, 09ach, 0bcfh, 0112h, 0224h, 0346h, 0458h, 056ah, 068ch, 079fh, 0002h, 0114h, 0126h, 0238h, 024ah, 035ch
dw 036fh, 0002h, 0014h, 0016h, 0028h, 002ah, 003ch, 003fh, 0112h, 0334h, 0546h, 0768h, 098ah, 0b9ch, 0dbfh, 0112h
dw 0324h, 0436h, 0648h, 085ah, 096ch, 0b7fh, 0102h, 0214h, 0416h, 0528h, 062ah, 083ch, 093fh, 0102h, 0204h, 0306h
dw 0408h, 050ah, 060ch, 070fh, 0212h, 0434h, 0646h, 0868h, 0a8ah, 0c9ch, 0fbeh, 0211h, 0423h, 0635h, 0847h, 0a59h
dw 0c6bh, 0f7dh, 0201h, 0413h, 0615h, 0826h, 0a28h, 0c3ah, 0f3ch, 0201h, 0403h, 0604h, 0806h, 0a08h, 0c09h, 0f0bh

display_step real8 3.1500000001f ; add small error, so we dont under count as 3.15 is actually 3.1499999999
display_reset real8 0.00001	     ; reset if under this value to account for the error added above

cpu_step_table:
db 0, 3, 6, 9, 12, 15, 18, 21
db 0, 3, 6, 9, 12, 15, 18, 22
db 0, 3, 6, 9, 12, 15, 19, 22
db 0, 3, 6, 9, 12, 16, 19, 22
db 0, 3, 6, 9, 13, 16, 19, 22
db 0, 3, 6, 10, 13, 16, 19, 22
db 0, 3, 7, 10, 13, 16, 19, 22
db 0, 4, 7, 10, 13, 16, 19, 22

should_display_table:
REPT 480
	REPT 640
		db 1
	ENDM
	REPT 800-640
		db 0
	ENDM
ENDM
REPT 525-480
	REPT 800
		db 0
	ENDM
ENDM

.code
