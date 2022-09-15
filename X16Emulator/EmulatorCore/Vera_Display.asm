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
; r10  : should display table ptr?
; r11  : x
; r12  : y
; r13  : scratch
; r14  : scratch
; r15  : buffer_render_output




SCREEN_WIDTH		equ 800
SCREEN_HEIGHT		equ 525
SCREEN_DOTS			equ SCREEN_WIDTH * SCREEN_HEIGHT

; Screen is RGBA so * 4.
SCREEN_BUFFER_SIZE	equ SCREEN_DOTS * 4
BACKGROUND			equ 0
SPRITE_L1			equ SCREEN_BUFFER_SIZE
LAYER0				equ SCREEN_BUFFER_SIZE * 2
SPRITE_L2			equ SCREEN_BUFFER_SIZE * 3
LAYER1				equ SCREEN_BUFFER_SIZE * 4
SPRITE_L3			equ SCREEN_BUFFER_SIZE * 5

VISIBLE_WIDTH		equ 640
VISIBLE_HEIGHT		equ 480

VBLANK				equ 480

; Buffer is colour index. one line being rendered, the other being output
BUFFER_SIZE			equ 1024 * 2			; use 1024, so we can toggle high bit to switch
BUFFER_SPRITE_L1	equ 0
BUFFER_LAYER0		equ BUFFER_SIZE
BUFFER_SPRITE_L2	equ BUFFER_SIZE * 2
BUFFER_LAYER1		equ BUFFER_SIZE * 3
BUFFER_SPRITE_L3	equ BUFFER_SIZE * 4

;
; Render the rest of the display, only gets called on vsync
;
vera_render_display proc
	push rax
	mov rax, [rdx].state.vera_clock
	mov [rdx].state.vera_clock, r14				; store vera clock
	mov rcx, r14								; Cpu Clock ticks
	sub rcx, rax								; Take off vera ticks for the number of cpu ticks we need to process

	jnz change									; if nothing to do, leave
	pop rax
	ret

change:
	push rsi
	push rdi
	push r8
	push r9
	push r10
	push rbx 
	push r11
	push r12
	push r15

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

	mov rsi, [rdx].state.display_buffer_ptr
	mov rdi, [rdx].state.display_ptr
	mov r8, [rdx].state.palette_ptr
	mov r9d, [rdx].state.display_position
	mov r11w, [rdx].state.display_x
	mov r12w, [rdx].state.display_y
	mov r15d, [rdx].state.buffer_output_position

	lea r10, should_display_table

display_loop:

	; check if we're in the visible area as a trivial skip
	mov al, byte ptr [r10 + r9]
	test al, al
	jz display_skip

	movzx rax, word ptr [rdx].state.dc_vstart
	cmp r12, rax
	jl draw_border

	mov ax, word ptr [rdx].state.dc_vstop
	cmp r12w, ax
	jge draw_border

	mov ax, word ptr [rdx].state.dc_hstart
	cmp r11w, ax
	jl draw_border

	mov ax, word ptr [rdx].state.dc_hstop
	cmp r11w, ax
	jl draw_pixel

draw_border:
	movzx rax, [rdx].state.dc_border
	mov ebx, dword ptr [r8 + rax * 4]
	mov [rdi + r9 * 4 + BACKGROUND], ebx

	jmp draw_complete

draw_pixel:
	; background
	mov ebx, dword ptr [r8]
	mov [rdi + r9 * 4 + BACKGROUND], ebx

	;
	; layer 0
	;
	mov al, byte ptr [rdx].state.layer0_enable
	test al, al
	jz layer0_notenabled

	mov al, [rsi + r15 + BUFFER_LAYER0]			; read the colour index from the buffer
	mov ebx, dword ptr [r8 + rax * 4]
	mov [rdi + r9 * 4 + LAYER0], ebx
	jmp layer0_done

layer0_notenabled:
	xor rbx, rbx								; if layer is not enabled, write a transparent pixel
	mov [rdi + r9 * 4 + LAYER0], ebx	
layer0_done:

	;
	; layer 1
	;
	mov al, byte ptr [rdx].state.layer1_enable
	test al, al
	jz layer1_notenabled

	mov al, [rsi + r15 + BUFFER_LAYER1]			; read the colour index from the buffer
	mov ebx, dword ptr [r8 + rax * 4]
	mov [rdi + r9 * 4 + LAYER1], ebx
	jmp layer1_done

layer1_notenabled:
	xor rbx, rbx								; if layer is not enabled, write a transparent pixel
	mov [rdi + r9 * 4 + LAYER1], ebx	
layer1_done:



draw_complete:
	; buffer is 1024 wide, but the is display is 800.
	; need to ignore the top bit, then test vs 800. If we've hit, flip the top bit and remove the count
	add r15, 1
	mov rax, r15
	and rax, 011111111111b	; dont consider the top bit
	cmp rax, SCREEN_WIDTH
	jne buffer_reset_skip
	xor r15, 010000000000b	; flip top bit
	and r15, 010000000000b	; and clear count

buffer_reset_skip:
	xor r15, 010000000000b ; flip top bit, so rendering can compare
	;
	; Render next lines
	;

	; Layer 0
	movzx rax, word ptr [rdx].state.layer0_next_render
	sub rax, 1
	mov rax, 1
	jnz layer0_render_done

	; use config to jump to the correct renderer.
	mov ax, word ptr [rdx].state.layer0_config
	lea rbx, layer0_render_jump
	jmp qword ptr [rbx + rax * 8]	; jump to dislpay code


layer0_render_done::
	mov word ptr [rdx].state.layer0_next_render, ax




	xor r15, 010000000000b ; flip top bit back

check_bounds:
	add r11, 1
	cmp r11, VISIBLE_WIDTH
	jne display_skip
	xor r11, r11
	add r12, 1


display_skip:
	add r9, 1
	cmp r9, SCREEN_DOTS
	jne no_reset
	xor r9, r9
	xor r11, r11
	xor r12, r12

no_reset:
	dec rcx
	jnz display_loop

done:
	mov dword ptr [rdx].state.display_position, r9d
	mov word ptr [rdx].state.display_x, r11w
	mov word ptr [rdx].state.display_y, r12w

	pop r15
	pop r12
	pop r11
	pop rbx 
	pop r10
	pop r9
	pop r8
	pop rdi
	pop rsi
	pop rax
	ret
vera_render_display endp


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

;
; Renderers
;

;
; Layer 0
;

layer0_1bpp_til_x_render proc
	; + y / 8/16(tileheight) * mapWidth -- shr 3 or 4 and then shl 6, 7, 8 or 9
	; + x * 8/16(tilewidth)             -- shl 3, 4
	; mod mapHeight
	; * 2
	push rcx
	push rsi

	mov rax, r12										; y
	mov ecx, dword ptr [rdx].state.layer0_tile_vshift
	shr rax, cl											; / tile height
	mov ecx, dword ptr [rdx].state.layer0_map_hshift
	shl rax, cl											; * map width
	mov rbx, r11
	mov ecx, dword ptr [rdx].state.layer0_tile_hshift
	shr rbx, cl											; / tile width
	add rax, rbx			
	shl rax, 1											; * 2

	mov ebx, dword ptr [rdx].state.layer0_mapAddress
	mov rsi, [rdx].state.vram_ptr
	add rax, rbx
	mov rax, [rsi + rax]							; now has tile number

	pop rsi
	pop rcx

	jmp layer0_render_done
layer0_1bpp_til_x_render endp


mode_notsupported:
	mov ax, 1 ; count till next udpate requirement
	jmp layer0_render_done

layer0_render_jump:
	layer0_1bpp_til_x qword layer0_1bpp_til_x_render
	layer0_2bpp_til_x qword mode_notsupported
	layer0_4bpp_til_x qword mode_notsupported
	layer0_8bpp_til_x qword mode_notsupported
	layer0_1bpp_bit_x qword mode_notsupported
	layer0_2bpp_bit_x qword mode_notsupported
	layer0_4bpp_bit_x qword mode_notsupported
	layer0_8bpp_bit_x qword mode_notsupported
	layer0_1bpp_til_t qword mode_notsupported
	layer0_2bpp_til_t qword mode_notsupported
	layer0_4bpp_til_t qword mode_notsupported
	layer0_8bpp_til_t qword mode_notsupported
	layer0_1bpp_bit_t qword mode_notsupported
	layer0_2bpp_bit_t qword mode_notsupported
	layer0_4bpp_bit_t qword mode_notsupported
	layer0_8bpp_bit_t qword mode_notsupported

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
