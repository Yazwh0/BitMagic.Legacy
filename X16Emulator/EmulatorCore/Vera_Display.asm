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
; rsi  : memory context (changable)
; rdi  : display
; r8   : palette
; r9   : output offset
; r10  : scratch
; r11  : x
; r12  : y
; r13  : scratch
; r14  : scratch
; r15  : buffer render output position for rendering in\out of buffer




SCREEN_WIDTH		equ 800
SCREEN_HEIGHT		equ 525
SCREEN_DOTS			equ SCREEN_WIDTH * SCREEN_HEIGHT
RENDER_RESET		equ SCREEN_DOTS - 800

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
BUFFER_SIZE			equ 2048 * 2			; use 2048, so we can toggle high bit to switch, also needs to be wide enough for scaling of $ff
BUFFER_SPRITE_L1	equ 0
BUFFER_LAYER0		equ BUFFER_SIZE
BUFFER_SPRITE_L2	equ BUFFER_SIZE * 2
BUFFER_LAYER1		equ BUFFER_SIZE * 3
BUFFER_SPRITE_L3	equ BUFFER_SIZE * 4

;
; Render the rest of the display, only gets called on vsync
;
vera_render_display proc
	; update display
	mov byte ptr cl, [rdx].state.headless
	test cl, cl
	jz not_headless
	ret
not_headless:

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
	push r13
	push r14
	push r15

	;mov rax, rcx								; keep hold of base ticks

	lea rcx, [rcx+rcx*4]						; * 5
	lea rcx, [rcx+rcx*4]						; * 5 = *25

	;shl rcx, 3
	;mov rbx, rcx
	;add rcx, rbx								; now * 3
	;add rcx, rbx								
	;add rcx, rax								; now * 3.125

	movzx rax, [rdx].state.display_carry		; Get carry, and add it on
	add rcx, rax

	mov rax, rcx								; Store to trim
	and al, 111b
	mov byte ptr [rdx].state.display_carry, al	; save carry
	shr rcx, 3									; /8 (round), rcx now contains the steps

	mov rsi, [rdx].state.display_buffer_ptr
	mov rdi, [rdx].state.display_ptr
	mov r8, [rdx].state.palette_ptr
	mov r9d, [rdx].state.display_position
	mov r11w, [rdx].state.display_x
	mov r15d, [rdx].state.buffer_render_position

	; this also gets set at the end of the display loop
	movzx r12, word ptr [rdx].state.display_y

display_loop:
	;
	; BORDER and VISIBLE CHECK
	;
	; needs actual x, y coordinates

	movzx r12, word ptr [rdx].state.display_y
	movzx r11, word ptr [rdx].state.display_x


	; check if we're in the visible area as a trivial skip
	lea r10, should_display_table
	movzx rax, byte ptr [r10 + r9]
	test rax, rax
	jz render_complete

	cmp rax, 2
	jg do_render

	push rax

	; r12 gets set at the end of the display loop
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

	jmp draw_end

;
; VIDEO Output
;
; Needs scaled x, buffer position (r15)
;
; Todo: USE SCALED X
;
draw_pixel:

	;mov r12d, dword ptr [rdx].state.scale_y
	;shr r12, 16
	mov r11d, dword ptr [rdx].state.scale_x ; should have high bit set for indexing into the buffer
	shr r11, 16

	mov rsi, [rdx].state.display_buffer_ptr

	; background
	mov ebx, dword ptr [r8]
	mov [rdi + r9 * 4 + BACKGROUND], ebx

	;
	; layer 0
	;
	mov al, byte ptr [rdx].state.layer0_enable
	test al, al
	jz layer0_notenabled

	movzx rax, byte ptr [rsi + r11 + BUFFER_LAYER0]			; read the colour index from the buffer
	xor rbx, rbx
	test rax, rax
	cmovnz ebx, dword ptr [r8 + rax * 4]		; get colour from palette
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

	movzx rax, byte ptr [rsi + r11 + BUFFER_LAYER1]			; read the colour index from the buffer
	xor rbx, rbx
	test rax, rax
	cmovnz ebx, dword ptr [r8 + rax * 4]
	mov [rdi + r9 * 4 + LAYER1], ebx
	jmp layer1_done

layer1_notenabled:
	xor rbx, rbx								; if layer is not enabled, write a transparent pixel
	mov [rdi + r9 * 4 + LAYER1], ebx	
layer1_done:

	; step x on, and store
	mov r11d, dword ptr [rdx].state.scale_x
	mov eax, [rdx].state.dc_hscale
	add r11, rax
	mov dword ptr [rdx].state.scale_x, r11d

;
;	Display complete.
;
draw_end:

	pop rax
	cmp rax, 2
	jl render_complete_visible

;
; RENDERING TO BUFFER
;
; Needs act x, scaled y + 1 line from VIDEO
;
do_render:
	movzx r11, word ptr [rdx].state.display_x

	; set r12 to scaled y and add a line
	mov r12d, dword ptr [rdx].state.scale_y
	shr r12, 16							; adjust to actual value

	;
	; Render next lines
	;

	;
	; Layer 0
	;
	movzx rax, word ptr [rdx].state.layer0_next_render
	sub rax, 1
	jnz layer0_render_done

	; use config to jump to the correct renderer.
	movzx rax, word ptr [rdx].state.layer0_config
	lea rbx, layer0_render_jump
	jmp qword ptr [rbx + rax * 8]	; jump to dislpay code

layer0_render_done::
	mov word ptr [rdx].state.layer0_next_render, ax

	;
	; Layer 1
	;
	movzx rax, word ptr [rdx].state.layer1_next_render
	sub rax, 1
	jnz layer1_render_done

	; use config to jump to the correct renderer.
	movzx rax, word ptr [rdx].state.layer1_config
	lea rbx, layer1_render_jump
	jmp qword ptr [rbx + rax * 8]	; jump to dislpay code

layer1_render_done::
	mov word ptr [rdx].state.layer1_next_render, ax

; ------------------------------------------------
; end of rendering
; ------------------------------------------------
render_complete_visible:	; arrives here if the video wrote data
	; buffer is 2048 wide, but the is display is 640.
	; need to ignore the top bit, then test vs 640. If we've hit, flip the top bit and remove the count
	add r15, 1
	mov rax, r15
	and rax, 0011111111111b	; dont consider the top bit
	cmp rax, VISIBLE_WIDTH
	jne not_next_line
	
	; Add 1 to act y
	movzx r12, word ptr [rdx].state.display_y
	add r12, 1
	mov word ptr [rdx].state.display_y, r12w

	; next line, reset counters
	xor r15, 0100000000000b	; flip top bit
	and r15, 0100000000000b	; and clear count

	xor r11, r11
	mov word ptr [rdx].state.display_x, r11w	; zero

	mov r11, r15
	xor r11, 0100000000000b ; flip top bit back
	shl r11, 16				; adjust to the fixed point number
	mov dword ptr [rdx].state.scale_x, r11d		; zero


	; Add line to scaled y
	mov r12d, dword ptr [rdx].state.scale_y
	mov eax, [rdx].state.dc_vscale
	add r12, rax
	mov [rdx].state.scale_y, r12d		

	mov word ptr [rdx].state.layer0_next_render, 1	; next pixel forces a draw
	mov word ptr [rdx].state.layer1_next_render, 1

	jmp render_complete

not_next_line:
	mov word ptr [rdx].state.display_x, ax

	; step step onto scaled x
	;mov r11d, dword ptr [rdx].state.scale_x
	;mov eax, [rdx].state.dc_hscale
	;add r11, rax
	;mov [rdx].state.scale_x, r11d


; --------------------------------------------------------------------------
render_complete:			; arrives here if we're outside the visible area
	add r9, 1
	cmp r9, SCREEN_DOTS
	jne not_end_of_screen

	xor r9, r9
	xor r11, r11
	xor r12, r12
	mov word ptr [rdx].state.display_x, r11w
	mov word ptr [rdx].state.display_y, r12w
	jmp no_scale_reset

not_end_of_screen:
	cmp r9, RENDER_RESET
	jne no_scale_reset
	mov dword ptr [rdx].state.scale_y, 0				; reset scaled value 
	xor r15, r15										; reset buffer pointer
	mov word ptr [rdx].state.layer0_next_render, 1	; next pixel forces a draw
	mov word ptr [rdx].state.layer1_next_render, 1

no_scale_reset:
	dec rcx
	jnz display_loop

done:
	mov dword ptr [rdx].state.display_position, r9d
	;mov word ptr [rdx].state.display_x, r11w
	mov dword ptr [rdx].state.buffer_render_position, r15d

	pop r15
	pop r14
	pop r13
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

; Todo, move this jump calc to state so its calculated when the config changes

get_tile_definition_layer0 macro
	mov rsi, [rdx].state.memory_ptr
	movzx rax, byte ptr [rsi + L0_CONFIG]
	and rax, 11110011b
	movzx rbx, byte ptr [rsi + L0_TILEBASE]
	and rbx, 011b
	shl rbx, 2
	or rax, rbx
	lea rbx, get_tile_definition_jump
	call qword ptr [rbx + rax * 8]
endm

get_tile_definition_layer1 macro
	mov rsi, [rdx].state.memory_ptr
	movzx rax, byte ptr [rsi + L1_CONFIG]
	and rax, 11110011b
	movzx rbx, byte ptr [rsi + L1_TILEBASE]
	and rbx, 011b
	shl rbx, 2
	or rax, rbx
	lea rbx, get_tile_definition_jump
	call qword ptr [rbx + rax * 8]
endm

include Vera_Display_Tiles_1bpp.asm
include Vera_Display_Tiles_2bpp.asm
include Vera_Display_Tiles_4bpp.asm
include Vera_Display_Tiles_8bpp.asm

; macros to find the current tile definition, returns data in ax. 
; expects:
; r13: current layer map address
; r14: current layer tile address
; rsi: vram
; r11: x
; r12: y
; returns
; ax  : tile information
; ebx : tile data
; r10 : x position through tile
; r13 : width
; r14 : x mask

get_tile_definition macro map_height, map_width, tile_height, tile_width, colour_depth
	local m_height_px, m_width_px, t_height_px, t_width_px, t_colour_size, t_size_shift, t_tile_mask, t_multiplier, t_colour_mask, t_tile_shift, t_tile_x_mask, t_height_invert_mask
	mov rsi, [rdx].state.vram_ptr

	mov rax, r12					; y
	shr rax, tile_height + 3		; / tile height
	shl rax, map_width + 5			; * map width

	;xor rbx, rbx
	mov rbx, r11					; x
	shr rbx, tile_width + 3			; / tile width
	add rax, rbx			

	; constrain map to height
	; this needs to consider map_height and map_width
	if map_height eq 0				
		m_height_px equ 32
	endif
	if map_height eq 1
		m_height_px equ 64
	endif
	if map_height eq 2
		m_height_px equ 128
	endif
	if map_height eq 3
		m_height_px equ 256
	endif

	if map_width eq 0				
		m_width_px equ 32
	endif
	if map_width eq 1
		m_width_px equ 64
	endif
	if map_width eq 2
		m_width_px equ 128
	endif
	if map_width eq 3
		m_width_px equ 256
	endif

	and rax, (m_height_px * m_width_px) - 1

	shl rax, 1						; * 2

	add rax, r13					; vram address
	movzx rax, word ptr [rsi + rax]	; now has tile number (ah) and data (al)

	; ---------------------------------------------------------------
	if tile_height eq 0
		t_height_px equ 8
		t_height_invert_mask equ 0111b
	endif
	if tile_height eq 1
		t_height_px equ 16
		t_height_invert_mask equ 01111b
	endif
	if tile_width eq 0
		t_width_px equ 8
		t_multiplier equ 1
	endif
	if tile_width eq 1
		t_width_px equ 16
		t_multiplier equ 2
	endif
	if colour_depth eq 0
		t_colour_size equ 8		
	endif
	if colour_depth eq 1
		t_colour_size equ 4
	endif
	if colour_depth eq 2
		t_colour_size equ 2
	endif
	if colour_depth eq 3
		t_colour_size equ 1
	endif
	if t_height_px * t_width_px / t_colour_size eq 8
		t_size_shift equ 3
	endif
	if t_height_px * t_width_px / t_colour_size eq 16
		t_size_shift equ 4
	endif
	if t_height_px * t_width_px / t_colour_size eq 32
		t_size_shift equ 5
	endif
	if t_height_px * t_width_px / t_colour_size eq 64
		t_size_shift equ 6
	endif
	if t_height_px * t_width_px / t_colour_size eq 128
		t_size_shift equ 7
	endif
	if t_height_px * t_width_px / t_colour_size eq 256
		t_size_shift equ 8
	endif
	t_tile_mask equ t_height_px - 1
	t_tile_shift equ (t_multiplier - 1) + colour_depth
	t_tile_x_mask equ t_width_px-1

	xor rbx, rbx

	if colour_depth eq 0
	mov bl, al								; get tile number
	else
	mov bx, ax
	and bx, 01111111111b					; mask off tile index
	endif

	; find dword in memory that is being rendered
	mov r10, r11							; store x for later

	; r12 is y, need to convert it to where the line starts in memory
	if colour_depth ne 0

		test eax, 800h							; check if flipped
		jz no_v_flip

		xor r12, t_height_invert_mask			; inverts the y position

		if tile_width eq 1
		if colour_depth eq 2
		xor r11, 01000b							; flip bit to invert, its masked later
		endif
		endif

		if tile_width eq 0
		endif

	no_v_flip:
	endif
	and r12, t_tile_mask					; mask for tile height, so now line within tile
	shl r12, t_tile_shift					; adjust to width of line to get offset address

	shl rbx, t_size_shift					; rbx is now the address
	or rbx, r12								; adjust to the line offset
	add rbx, r14							; add to tile base address
	
	; find offset of current x, we saved x into r10 earlier for this
	mov r14, t_tile_x_mask
	and r10, r14							; return pixels

	if tile_width eq 1
	if colour_depth eq 2					; 4bpp
	and r11, 01000b							; mask x position
	shr r11, 1								; adjust to the actual address
	add rbx, r11
	endif
	endif

	mov ebx, dword ptr [rsi + rbx]			; set ebx 32bits worth of values
	mov r13, t_width_px						; return width

	; now rbx has 32bit from tile data location
	ret
endm


mode_layer0_notsupported proc
	mov ax, 8 ; count till next update requirement
	jmp layer0_render_done
mode_layer0_notsupported endp

mode_layer1_notsupported proc
	mov ax, 8 ; count till next update requirement
	jmp layer1_render_done
mode_layer1_notsupported endp

layer0_render_jump:
	layer0_1bpp_til_x qword layer0_1bpp_til_x_render
	layer0_2bpp_til_x qword layer0_2bpp_til_x_render
	layer0_4bpp_til_x qword layer0_4bpp_til_x_render
	layer0_8bpp_til_x qword mode_layer0_notsupported
	layer0_1bpp_bit_x qword mode_layer0_notsupported
	layer0_2bpp_bit_x qword mode_layer0_notsupported
	layer0_4bpp_bit_x qword mode_layer0_notsupported
	layer0_8bpp_bit_x qword mode_layer0_notsupported
	layer0_1bpp_til_t qword layer0_1bpp_til_t_render
	layer0_2bpp_til_t qword layer0_2bpp_til_x_render
	layer0_4bpp_til_t qword layer0_4bpp_til_x_render
	layer0_8bpp_til_t qword mode_layer0_notsupported
	layer0_1bpp_bit_t qword mode_layer0_notsupported
	layer0_2bpp_bit_t qword mode_layer0_notsupported
	layer0_4bpp_bit_t qword mode_layer0_notsupported
	layer0_8bpp_bit_t qword mode_layer0_notsupported

layer1_render_jump:
	layer1_1bpp_til_x qword layer1_1bpp_til_x_render
	layer1_2bpp_til_x qword layer1_2bpp_til_x_render
	layer1_4bpp_til_x qword mode_layer1_notsupported
	layer1_8bpp_til_x qword mode_layer1_notsupported
	layer1_1bpp_bit_x qword mode_layer1_notsupported
	layer1_2bpp_bit_x qword mode_layer1_notsupported
	layer1_4bpp_bit_x qword mode_layer1_notsupported
	layer1_8bpp_bit_x qword mode_layer1_notsupported
	layer1_1bpp_til_t qword layer1_1bpp_til_t_render
	layer1_2bpp_til_t qword layer1_2bpp_til_x_render
	layer1_4bpp_til_t qword mode_layer1_notsupported
	layer1_8bpp_til_t qword mode_layer1_notsupported
	layer1_1bpp_bit_t qword mode_layer1_notsupported
	layer1_2bpp_bit_t qword mode_layer1_notsupported
	layer1_4bpp_bit_t qword mode_layer1_notsupported
	layer1_8bpp_bit_t qword mode_layer1_notsupported

tile_definition_proc macro _map_height, _map_width, _tile_height, _tile_width, _colour_depth, _tile_definition_count
tile_definition_&_tile_definition_count& proc
	get_tile_definition _map_height, _map_width, _tile_height, _tile_width, _colour_depth
tile_definition_&_tile_definition_count& endp
endm

tile_definition_proc 0, 0, 0, 0, 0, 0
tile_definition_proc 0, 0, 0, 0, 1, 1
tile_definition_proc 0, 0, 0, 0, 2, 2
tile_definition_proc 0, 0, 0, 0, 3, 3
tile_definition_proc 0, 0, 0, 1, 0, 4
tile_definition_proc 0, 0, 0, 1, 1, 5
tile_definition_proc 0, 0, 0, 1, 2, 6
tile_definition_proc 0, 0, 0, 1, 3, 7
tile_definition_proc 0, 0, 1, 0, 0, 8
tile_definition_proc 0, 0, 1, 0, 1, 9
tile_definition_proc 0, 0, 1, 0, 2, 10
tile_definition_proc 0, 0, 1, 0, 3, 11
tile_definition_proc 0, 0, 1, 1, 0, 12
tile_definition_proc 0, 0, 1, 1, 1, 13
tile_definition_proc 0, 0, 1, 1, 2, 14
tile_definition_proc 0, 0, 1, 1, 3, 15
tile_definition_proc 0, 1, 0, 0, 0, 16
tile_definition_proc 0, 1, 0, 0, 1, 17
tile_definition_proc 0, 1, 0, 0, 2, 18
tile_definition_proc 0, 1, 0, 0, 3, 19
tile_definition_proc 0, 1, 0, 1, 0, 20
tile_definition_proc 0, 1, 0, 1, 1, 21
tile_definition_proc 0, 1, 0, 1, 2, 22
tile_definition_proc 0, 1, 0, 1, 3, 23
tile_definition_proc 0, 1, 1, 0, 0, 24
tile_definition_proc 0, 1, 1, 0, 1, 25
tile_definition_proc 0, 1, 1, 0, 2, 26
tile_definition_proc 0, 1, 1, 0, 3, 27
tile_definition_proc 0, 1, 1, 1, 0, 28
tile_definition_proc 0, 1, 1, 1, 1, 29
tile_definition_proc 0, 1, 1, 1, 2, 30
tile_definition_proc 0, 1, 1, 1, 3, 31
tile_definition_proc 0, 2, 0, 0, 0, 32
tile_definition_proc 0, 2, 0, 0, 1, 33
tile_definition_proc 0, 2, 0, 0, 2, 34
tile_definition_proc 0, 2, 0, 0, 3, 35
tile_definition_proc 0, 2, 0, 1, 0, 36
tile_definition_proc 0, 2, 0, 1, 1, 37
tile_definition_proc 0, 2, 0, 1, 2, 38
tile_definition_proc 0, 2, 0, 1, 3, 39
tile_definition_proc 0, 2, 1, 0, 0, 40
tile_definition_proc 0, 2, 1, 0, 1, 41
tile_definition_proc 0, 2, 1, 0, 2, 42
tile_definition_proc 0, 2, 1, 0, 3, 43
tile_definition_proc 0, 2, 1, 1, 0, 44
tile_definition_proc 0, 2, 1, 1, 1, 45
tile_definition_proc 0, 2, 1, 1, 2, 46
tile_definition_proc 0, 2, 1, 1, 3, 47
tile_definition_proc 0, 3, 0, 0, 0, 48
tile_definition_proc 0, 3, 0, 0, 1, 49
tile_definition_proc 0, 3, 0, 0, 2, 50
tile_definition_proc 0, 3, 0, 0, 3, 51
tile_definition_proc 0, 3, 0, 1, 0, 52
tile_definition_proc 0, 3, 0, 1, 1, 53
tile_definition_proc 0, 3, 0, 1, 2, 54
tile_definition_proc 0, 3, 0, 1, 3, 55
tile_definition_proc 0, 3, 1, 0, 0, 56
tile_definition_proc 0, 3, 1, 0, 1, 57
tile_definition_proc 0, 3, 1, 0, 2, 58
tile_definition_proc 0, 3, 1, 0, 3, 59
tile_definition_proc 0, 3, 1, 1, 0, 60
tile_definition_proc 0, 3, 1, 1, 1, 61
tile_definition_proc 0, 3, 1, 1, 2, 62
tile_definition_proc 0, 3, 1, 1, 3, 63
tile_definition_proc 1, 0, 0, 0, 0, 64
tile_definition_proc 1, 0, 0, 0, 1, 65
tile_definition_proc 1, 0, 0, 0, 2, 66
tile_definition_proc 1, 0, 0, 0, 3, 67
tile_definition_proc 1, 0, 0, 1, 0, 68
tile_definition_proc 1, 0, 0, 1, 1, 69
tile_definition_proc 1, 0, 0, 1, 2, 70
tile_definition_proc 1, 0, 0, 1, 3, 71
tile_definition_proc 1, 0, 1, 0, 0, 72
tile_definition_proc 1, 0, 1, 0, 1, 73
tile_definition_proc 1, 0, 1, 0, 2, 74
tile_definition_proc 1, 0, 1, 0, 3, 75
tile_definition_proc 1, 0, 1, 1, 0, 76
tile_definition_proc 1, 0, 1, 1, 1, 77
tile_definition_proc 1, 0, 1, 1, 2, 78
tile_definition_proc 1, 0, 1, 1, 3, 79
tile_definition_proc 1, 1, 0, 0, 0, 80
tile_definition_proc 1, 1, 0, 0, 1, 81
tile_definition_proc 1, 1, 0, 0, 2, 82
tile_definition_proc 1, 1, 0, 0, 3, 83
tile_definition_proc 1, 1, 0, 1, 0, 84
tile_definition_proc 1, 1, 0, 1, 1, 85
tile_definition_proc 1, 1, 0, 1, 2, 86
tile_definition_proc 1, 1, 0, 1, 3, 87
tile_definition_proc 1, 1, 1, 0, 0, 88
tile_definition_proc 1, 1, 1, 0, 1, 89
tile_definition_proc 1, 1, 1, 0, 2, 90
tile_definition_proc 1, 1, 1, 0, 3, 91
tile_definition_proc 1, 1, 1, 1, 0, 92
tile_definition_proc 1, 1, 1, 1, 1, 93
tile_definition_proc 1, 1, 1, 1, 2, 94
tile_definition_proc 1, 1, 1, 1, 3, 95
tile_definition_proc 1, 2, 0, 0, 0, 96
tile_definition_proc 1, 2, 0, 0, 1, 97
tile_definition_proc 1, 2, 0, 0, 2, 98
tile_definition_proc 1, 2, 0, 0, 3, 99
tile_definition_proc 1, 2, 0, 1, 0, 100
tile_definition_proc 1, 2, 0, 1, 1, 101
tile_definition_proc 1, 2, 0, 1, 2, 102
tile_definition_proc 1, 2, 0, 1, 3, 103
tile_definition_proc 1, 2, 1, 0, 0, 104
tile_definition_proc 1, 2, 1, 0, 1, 105
tile_definition_proc 1, 2, 1, 0, 2, 106
tile_definition_proc 1, 2, 1, 0, 3, 107
tile_definition_proc 1, 2, 1, 1, 0, 108
tile_definition_proc 1, 2, 1, 1, 1, 109
tile_definition_proc 1, 2, 1, 1, 2, 110
tile_definition_proc 1, 2, 1, 1, 3, 111
tile_definition_proc 1, 3, 0, 0, 0, 112
tile_definition_proc 1, 3, 0, 0, 1, 113
tile_definition_proc 1, 3, 0, 0, 2, 114
tile_definition_proc 1, 3, 0, 0, 3, 115
tile_definition_proc 1, 3, 0, 1, 0, 116
tile_definition_proc 1, 3, 0, 1, 1, 117
tile_definition_proc 1, 3, 0, 1, 2, 118
tile_definition_proc 1, 3, 0, 1, 3, 119
tile_definition_proc 1, 3, 1, 0, 0, 120
tile_definition_proc 1, 3, 1, 0, 1, 121
tile_definition_proc 1, 3, 1, 0, 2, 122
tile_definition_proc 1, 3, 1, 0, 3, 123
tile_definition_proc 1, 3, 1, 1, 0, 124
tile_definition_proc 1, 3, 1, 1, 1, 125
tile_definition_proc 1, 3, 1, 1, 2, 126
tile_definition_proc 1, 3, 1, 1, 3, 127
tile_definition_proc 2, 0, 0, 0, 0, 128
tile_definition_proc 2, 0, 0, 0, 1, 129
tile_definition_proc 2, 0, 0, 0, 2, 130
tile_definition_proc 2, 0, 0, 0, 3, 131
tile_definition_proc 2, 0, 0, 1, 0, 132
tile_definition_proc 2, 0, 0, 1, 1, 133
tile_definition_proc 2, 0, 0, 1, 2, 134
tile_definition_proc 2, 0, 0, 1, 3, 135
tile_definition_proc 2, 0, 1, 0, 0, 136
tile_definition_proc 2, 0, 1, 0, 1, 137
tile_definition_proc 2, 0, 1, 0, 2, 138
tile_definition_proc 2, 0, 1, 0, 3, 139
tile_definition_proc 2, 0, 1, 1, 0, 140
tile_definition_proc 2, 0, 1, 1, 1, 141
tile_definition_proc 2, 0, 1, 1, 2, 142
tile_definition_proc 2, 0, 1, 1, 3, 143
tile_definition_proc 2, 1, 0, 0, 0, 144
tile_definition_proc 2, 1, 0, 0, 1, 145
tile_definition_proc 2, 1, 0, 0, 2, 146
tile_definition_proc 2, 1, 0, 0, 3, 147
tile_definition_proc 2, 1, 0, 1, 0, 148
tile_definition_proc 2, 1, 0, 1, 1, 149
tile_definition_proc 2, 1, 0, 1, 2, 150
tile_definition_proc 2, 1, 0, 1, 3, 151
tile_definition_proc 2, 1, 1, 0, 0, 152
tile_definition_proc 2, 1, 1, 0, 1, 153
tile_definition_proc 2, 1, 1, 0, 2, 154
tile_definition_proc 2, 1, 1, 0, 3, 155
tile_definition_proc 2, 1, 1, 1, 0, 156
tile_definition_proc 2, 1, 1, 1, 1, 157
tile_definition_proc 2, 1, 1, 1, 2, 158
tile_definition_proc 2, 1, 1, 1, 3, 159
tile_definition_proc 2, 2, 0, 0, 0, 160
tile_definition_proc 2, 2, 0, 0, 1, 161
tile_definition_proc 2, 2, 0, 0, 2, 162
tile_definition_proc 2, 2, 0, 0, 3, 163
tile_definition_proc 2, 2, 0, 1, 0, 164
tile_definition_proc 2, 2, 0, 1, 1, 165
tile_definition_proc 2, 2, 0, 1, 2, 166
tile_definition_proc 2, 2, 0, 1, 3, 167
tile_definition_proc 2, 2, 1, 0, 0, 168
tile_definition_proc 2, 2, 1, 0, 1, 169
tile_definition_proc 2, 2, 1, 0, 2, 170
tile_definition_proc 2, 2, 1, 0, 3, 171
tile_definition_proc 2, 2, 1, 1, 0, 172
tile_definition_proc 2, 2, 1, 1, 1, 173
tile_definition_proc 2, 2, 1, 1, 2, 174
tile_definition_proc 2, 2, 1, 1, 3, 175
tile_definition_proc 2, 3, 0, 0, 0, 176
tile_definition_proc 2, 3, 0, 0, 1, 177
tile_definition_proc 2, 3, 0, 0, 2, 178
tile_definition_proc 2, 3, 0, 0, 3, 179
tile_definition_proc 2, 3, 0, 1, 0, 180
tile_definition_proc 2, 3, 0, 1, 1, 181
tile_definition_proc 2, 3, 0, 1, 2, 182
tile_definition_proc 2, 3, 0, 1, 3, 183
tile_definition_proc 2, 3, 1, 0, 0, 184
tile_definition_proc 2, 3, 1, 0, 1, 185
tile_definition_proc 2, 3, 1, 0, 2, 186
tile_definition_proc 2, 3, 1, 0, 3, 187
tile_definition_proc 2, 3, 1, 1, 0, 188
tile_definition_proc 2, 3, 1, 1, 1, 189
tile_definition_proc 2, 3, 1, 1, 2, 190
tile_definition_proc 2, 3, 1, 1, 3, 191
tile_definition_proc 3, 0, 0, 0, 0, 192
tile_definition_proc 3, 0, 0, 0, 1, 193
tile_definition_proc 3, 0, 0, 0, 2, 194
tile_definition_proc 3, 0, 0, 0, 3, 195
tile_definition_proc 3, 0, 0, 1, 0, 196
tile_definition_proc 3, 0, 0, 1, 1, 197
tile_definition_proc 3, 0, 0, 1, 2, 198
tile_definition_proc 3, 0, 0, 1, 3, 199
tile_definition_proc 3, 0, 1, 0, 0, 200
tile_definition_proc 3, 0, 1, 0, 1, 201
tile_definition_proc 3, 0, 1, 0, 2, 202
tile_definition_proc 3, 0, 1, 0, 3, 203
tile_definition_proc 3, 0, 1, 1, 0, 204
tile_definition_proc 3, 0, 1, 1, 1, 205
tile_definition_proc 3, 0, 1, 1, 2, 206
tile_definition_proc 3, 0, 1, 1, 3, 207
tile_definition_proc 3, 1, 0, 0, 0, 208
tile_definition_proc 3, 1, 0, 0, 1, 209
tile_definition_proc 3, 1, 0, 0, 2, 210
tile_definition_proc 3, 1, 0, 0, 3, 211
tile_definition_proc 3, 1, 0, 1, 0, 212
tile_definition_proc 3, 1, 0, 1, 1, 213
tile_definition_proc 3, 1, 0, 1, 2, 214
tile_definition_proc 3, 1, 0, 1, 3, 215
tile_definition_proc 3, 1, 1, 0, 0, 216
tile_definition_proc 3, 1, 1, 0, 1, 217
tile_definition_proc 3, 1, 1, 0, 2, 218
tile_definition_proc 3, 1, 1, 0, 3, 219
tile_definition_proc 3, 1, 1, 1, 0, 220
tile_definition_proc 3, 1, 1, 1, 1, 221
tile_definition_proc 3, 1, 1, 1, 2, 222
tile_definition_proc 3, 1, 1, 1, 3, 223
tile_definition_proc 3, 2, 0, 0, 0, 224
tile_definition_proc 3, 2, 0, 0, 1, 225
tile_definition_proc 3, 2, 0, 0, 2, 226
tile_definition_proc 3, 2, 0, 0, 3, 227
tile_definition_proc 3, 2, 0, 1, 0, 228
tile_definition_proc 3, 2, 0, 1, 1, 229
tile_definition_proc 3, 2, 0, 1, 2, 230
tile_definition_proc 3, 2, 0, 1, 3, 231
tile_definition_proc 3, 2, 1, 0, 0, 232
tile_definition_proc 3, 2, 1, 0, 1, 233
tile_definition_proc 3, 2, 1, 0, 2, 234
tile_definition_proc 3, 2, 1, 0, 3, 235
tile_definition_proc 3, 2, 1, 1, 0, 236
tile_definition_proc 3, 2, 1, 1, 1, 237
tile_definition_proc 3, 2, 1, 1, 2, 238
tile_definition_proc 3, 2, 1, 1, 3, 239
tile_definition_proc 3, 3, 0, 0, 0, 240
tile_definition_proc 3, 3, 0, 0, 1, 241
tile_definition_proc 3, 3, 0, 0, 2, 242
tile_definition_proc 3, 3, 0, 0, 3, 243
tile_definition_proc 3, 3, 0, 1, 0, 244
tile_definition_proc 3, 3, 0, 1, 1, 245
tile_definition_proc 3, 3, 0, 1, 2, 246
tile_definition_proc 3, 3, 0, 1, 3, 247
tile_definition_proc 3, 3, 1, 0, 0, 248
tile_definition_proc 3, 3, 1, 0, 1, 249
tile_definition_proc 3, 3, 1, 0, 2, 250
tile_definition_proc 3, 3, 1, 0, 3, 251
tile_definition_proc 3, 3, 1, 1, 0, 252
tile_definition_proc 3, 3, 1, 1, 1, 253
tile_definition_proc 3, 3, 1, 1, 2, 254
tile_definition_proc 3, 3, 1, 1, 3, 255

get_tile_definition_jump:
	qword tile_definition_0
	qword tile_definition_1
	qword tile_definition_2
	qword tile_definition_3
	qword tile_definition_4
	qword tile_definition_5
	qword tile_definition_6
	qword tile_definition_7
	qword tile_definition_8
	qword tile_definition_9
	qword tile_definition_10
	qword tile_definition_11
	qword tile_definition_12
	qword tile_definition_13
	qword tile_definition_14
	qword tile_definition_15
	qword tile_definition_16
	qword tile_definition_17
	qword tile_definition_18
	qword tile_definition_19
	qword tile_definition_20
	qword tile_definition_21
	qword tile_definition_22
	qword tile_definition_23
	qword tile_definition_24
	qword tile_definition_25
	qword tile_definition_26
	qword tile_definition_27
	qword tile_definition_28
	qword tile_definition_29
	qword tile_definition_30
	qword tile_definition_31
	qword tile_definition_32
	qword tile_definition_33
	qword tile_definition_34
	qword tile_definition_35
	qword tile_definition_36
	qword tile_definition_37
	qword tile_definition_38
	qword tile_definition_39
	qword tile_definition_40
	qword tile_definition_41
	qword tile_definition_42
	qword tile_definition_43
	qword tile_definition_44
	qword tile_definition_45
	qword tile_definition_46
	qword tile_definition_47
	qword tile_definition_48
	qword tile_definition_49
	qword tile_definition_50
	qword tile_definition_51
	qword tile_definition_52
	qword tile_definition_53
	qword tile_definition_54
	qword tile_definition_55
	qword tile_definition_56
	qword tile_definition_57
	qword tile_definition_58
	qword tile_definition_59
	qword tile_definition_60
	qword tile_definition_61
	qword tile_definition_62
	qword tile_definition_63
	qword tile_definition_64
	qword tile_definition_65
	qword tile_definition_66
	qword tile_definition_67
	qword tile_definition_68
	qword tile_definition_69
	qword tile_definition_70
	qword tile_definition_71
	qword tile_definition_72
	qword tile_definition_73
	qword tile_definition_74
	qword tile_definition_75
	qword tile_definition_76
	qword tile_definition_77
	qword tile_definition_78
	qword tile_definition_79
	qword tile_definition_80
	qword tile_definition_81
	qword tile_definition_82
	qword tile_definition_83
	qword tile_definition_84
	qword tile_definition_85
	qword tile_definition_86
	qword tile_definition_87
	qword tile_definition_88
	qword tile_definition_89
	qword tile_definition_90
	qword tile_definition_91
	qword tile_definition_92
	qword tile_definition_93
	qword tile_definition_94
	qword tile_definition_95
	qword tile_definition_96
	qword tile_definition_97
	qword tile_definition_98
	qword tile_definition_99
	qword tile_definition_100
	qword tile_definition_101
	qword tile_definition_102
	qword tile_definition_103
	qword tile_definition_104
	qword tile_definition_105
	qword tile_definition_106
	qword tile_definition_107
	qword tile_definition_108
	qword tile_definition_109
	qword tile_definition_110
	qword tile_definition_111
	qword tile_definition_112
	qword tile_definition_113
	qword tile_definition_114
	qword tile_definition_115
	qword tile_definition_116
	qword tile_definition_117
	qword tile_definition_118
	qword tile_definition_119
	qword tile_definition_120
	qword tile_definition_121
	qword tile_definition_122
	qword tile_definition_123
	qword tile_definition_124
	qword tile_definition_125
	qword tile_definition_126
	qword tile_definition_127
	qword tile_definition_128
	qword tile_definition_129
	qword tile_definition_130
	qword tile_definition_131
	qword tile_definition_132
	qword tile_definition_133
	qword tile_definition_134
	qword tile_definition_135
	qword tile_definition_136
	qword tile_definition_137
	qword tile_definition_138
	qword tile_definition_139
	qword tile_definition_140
	qword tile_definition_141
	qword tile_definition_142
	qword tile_definition_143
	qword tile_definition_144
	qword tile_definition_145
	qword tile_definition_146
	qword tile_definition_147
	qword tile_definition_148
	qword tile_definition_149
	qword tile_definition_150
	qword tile_definition_151
	qword tile_definition_152
	qword tile_definition_153
	qword tile_definition_154
	qword tile_definition_155
	qword tile_definition_156
	qword tile_definition_157
	qword tile_definition_158
	qword tile_definition_159
	qword tile_definition_160
	qword tile_definition_161
	qword tile_definition_162
	qword tile_definition_163
	qword tile_definition_164
	qword tile_definition_165
	qword tile_definition_166
	qword tile_definition_167
	qword tile_definition_168
	qword tile_definition_169
	qword tile_definition_170
	qword tile_definition_171
	qword tile_definition_172
	qword tile_definition_173
	qword tile_definition_174
	qword tile_definition_175
	qword tile_definition_176
	qword tile_definition_177
	qword tile_definition_178
	qword tile_definition_179
	qword tile_definition_180
	qword tile_definition_181
	qword tile_definition_182
	qword tile_definition_183
	qword tile_definition_184
	qword tile_definition_185
	qword tile_definition_186
	qword tile_definition_187
	qword tile_definition_188
	qword tile_definition_189
	qword tile_definition_190
	qword tile_definition_191
	qword tile_definition_192
	qword tile_definition_193
	qword tile_definition_194
	qword tile_definition_195
	qword tile_definition_196
	qword tile_definition_197
	qword tile_definition_198
	qword tile_definition_199
	qword tile_definition_200
	qword tile_definition_201
	qword tile_definition_202
	qword tile_definition_203
	qword tile_definition_204
	qword tile_definition_205
	qword tile_definition_206
	qword tile_definition_207
	qword tile_definition_208
	qword tile_definition_209
	qword tile_definition_210
	qword tile_definition_211
	qword tile_definition_212
	qword tile_definition_213
	qword tile_definition_214
	qword tile_definition_215
	qword tile_definition_216
	qword tile_definition_217
	qword tile_definition_218
	qword tile_definition_219
	qword tile_definition_220
	qword tile_definition_221
	qword tile_definition_222
	qword tile_definition_223
	qword tile_definition_224
	qword tile_definition_225
	qword tile_definition_226
	qword tile_definition_227
	qword tile_definition_228
	qword tile_definition_229
	qword tile_definition_230
	qword tile_definition_231
	qword tile_definition_232
	qword tile_definition_233
	qword tile_definition_234
	qword tile_definition_235
	qword tile_definition_236
	qword tile_definition_237
	qword tile_definition_238
	qword tile_definition_239
	qword tile_definition_240
	qword tile_definition_241
	qword tile_definition_242
	qword tile_definition_243
	qword tile_definition_244
	qword tile_definition_245
	qword tile_definition_246
	qword tile_definition_247
	qword tile_definition_248
	qword tile_definition_249
	qword tile_definition_250
	qword tile_definition_251
	qword tile_definition_252
	qword tile_definition_253
	qword tile_definition_254
	qword tile_definition_255



;map_height=0
;map_width=0
;tile_height=0
;tile_width=0
;colour_depth=0
;tile_definition_count=0

;rept 4								; map_height
;	map_width=0
;	rept 4							; map_width
;		tile_height=0
;		rept 2						; tile_height
;			tile_width=0
;			rept 2					; tile_width
;				colour_depth=0
;				rept 4				; colour_depth
					;tile_definition_proc map_height, map_width, tile_height, tile_width, colour_depth, tile_definition_count

;					colour_depth=colour_depth+1
;					tile_definition_count=tile_definition_count+1
;				endm
;				tile_width=tile_width+1
;			endm
;			tile_height=tile_height+1
;		endm
;		map_width=map_width+1
;	endm	
;	map_height=map_height+1
;endm





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
; 0 - dont render
; 1 - pull from buffer
; 2 - normal
; 3 - render to buffer
REPT 479
	REPT 640
		db 2
	ENDM
	REPT 160
		db 0
	ENDM
ENDM

; last visible line, dont render to the buffer
REPT 640
	db 1
ENDM
REPT 160
	db 0
ENDM
; 480 lines done, end of visible area

REPT 44
	REPT 800
		db 0
	ENDM
ENDM

; last line, render to the buffer
REPT 640
	db 3
ENDM
REPT 160
	db 0
ENDM

.code
