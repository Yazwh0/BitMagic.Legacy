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

writepixel_8bpp_normal macro bitmask, bitshift, outputoffset, pixeloffset, width
local zero_pallette
	pixel_&pixeloffset&_&width&:
	mov r13, rbx		; use r13b to write to the buffer
	and r13, bitmask	; mask colour index whic his at the top
	if bitshift ne 0
	shr r13, bitshift	; shift to value
	endif
	lea r11, [r13-1]	; 0-16 -> -1-15 for pallette check
	lea r8, [r13 + rax]	; load r8 with the pallette if we apply offset
	cmp r11, 16			; check if offset is to be applied
	cmovb r13, r8

	mov byte ptr [rsi + r15 + outputoffset], r13b
	add rsi, 1
endm

layer0_8bpp_til_x_render proc
	; ax  : tile information
	; ebx : tile data
	; r10 : x position through tile
	; r13 : width
	; r14 : x mask

	; need to fill the buffer with the colour indexes for each pixel
	mov rsi, [rdx].state.display_buffer_ptr

	test rax, 400h
	jne flipped

	shr rax, 12		; rax is now pallette offset
	shl rax, 4		; * 16

	lea r13, pixel_jump_8
	jmp qword ptr [r13 + r10 * 8]

pixel_jump_8:
	qword pixel_0_8
	qword pixel_1_8
	qword pixel_2_8
	qword pixel_3_8

	writepixel_8bpp_normal 0000000ffh, 00, BUFFER_LAYER0, 0, 8
	writepixel_8bpp_normal 00000ff00h, 08, BUFFER_LAYER0, 1, 8
	writepixel_8bpp_normal 000ff0000h, 16 ,BUFFER_LAYER0, 2, 8
	writepixel_8bpp_normal 0ff000000h, 24, BUFFER_LAYER0, 3, 8

	xor r10, r14		; mask value
	lea rax, [r10+1]	; add 1 to complete count

	jmp layer0_render_done

; ----------------------------------------------------------------------------------
flipped:
	shr rax, 12		; rax is now pallette offset
	shl rax, 4		; * 16

	lea r13, pixel_jump_8_f
	jmp qword ptr [r13 + r10 * 8]

pixel_jump_8_f:
	qword pixel_0_9
	qword pixel_1_9
	qword pixel_2_9
	qword pixel_3_9

	writepixel_8bpp_normal 0ff000000h, 24, BUFFER_LAYER0, 0, 9
	writepixel_8bpp_normal 000ff0000h, 16 ,BUFFER_LAYER0, 1, 9
	writepixel_8bpp_normal 00000ff00h, 08, BUFFER_LAYER0, 2, 9
	writepixel_8bpp_normal 0000000ffh, 00, BUFFER_LAYER0, 3, 9

	xor r10, r14		; mask value
	lea rax, [r10+1]	; add 1 to complete count

	jmp layer0_render_done

layer0_8bpp_til_x_render endp

layer1_8bpp_til_x_render proc
	; ax now contains tile number and colour information
	; ebx now contains tile data
	; r10 is the number of pixels in ebx 

	; r15 is our buffer current position
	; need to fill the buffer with the colour indexes for each pixel
	mov rsi, [rdx].state.display_buffer_ptr

	test rax, 400h
	jne flipped

	shr rax, 12		; rax is now pallette offset
	shl rax, 4		; * 16

	lea r13, pixel_jump_8
	jmp qword ptr [r13 + r10 * 8]

pixel_jump_8:
	qword pixel_0_8
	qword pixel_1_8
	qword pixel_2_8
	qword pixel_3_8

	writepixel_8bpp_normal 0000000ffh, 00, BUFFER_LAYER1, 0, 8
	writepixel_8bpp_normal 00000ff00h, 08, BUFFER_LAYER1, 1, 8
	writepixel_8bpp_normal 000ff0000h, 16 ,BUFFER_LAYER1, 2, 8
	writepixel_8bpp_normal 0ff000000h, 24, BUFFER_LAYER1, 3, 8

	xor r10, r14		; mask value
	lea rax, [r10+1]	; add 1 to complete count
	
	jmp layer1_render_done

; ----------------------------------------------------------------------------------
flipped:
	shr rax, 12		; rax is now pallette offset
	shl rax, 4		; * 16

	lea r13, pixel_jump_8_f
	jmp qword ptr [r13 + r10 * 8]

pixel_jump_8_f:
	qword pixel_0_9
	qword pixel_1_9
	qword pixel_2_9
	qword pixel_3_9

	writepixel_8bpp_normal 0ff000000h, 24, BUFFER_LAYER1, 0, 9
	writepixel_8bpp_normal 000ff0000h, 16 ,BUFFER_LAYER1, 1, 9
	writepixel_8bpp_normal 00000ff00h, 08, BUFFER_LAYER1, 2, 9
	writepixel_8bpp_normal 0000000ffh, 00, BUFFER_LAYER1, 3, 9

	xor r10, r14		; mask value
	lea rax, [r10+1]	; add 1 to complete count
	
	jmp layer1_render_done

layer1_8bpp_til_x_render endp