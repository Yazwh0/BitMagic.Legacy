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

writepixel_8bpp_bitmap macro pixeloutput, outputoffset
	local skip
	xor r12, r12
	movzx r13, bl
	cmp r13, 15
	cmovg r12, r11		; clear offsett if >15
	add r13, r12		; add offset

	cmp r13b, 12
	jne skip
	mov r13b, r13b
	skip:

	mov byte ptr [rsi + r15 + pixeloutput + outputoffset], r13b
	shr ebx, 8
endm

layer0_8bpp_bmp_render proc
	; ebx contains the pixel data - 32bits worth
	; r15 is the buffer position
	mov rsi, [rdx].state.memory_ptr
	movzx r11, byte ptr [rsi + L0_HSCROLL_H]		; get offset
	shl r11, 4

	mov rsi, [rdx].state.display_buffer_ptr

	writepixel_8bpp_bitmap 00, BUFFER_LAYER0
	writepixel_8bpp_bitmap 01, BUFFER_LAYER0
	writepixel_8bpp_bitmap 02, BUFFER_LAYER0
	writepixel_8bpp_bitmap 03, BUFFER_LAYER0

	mov rax, 4

	jmp layer0_render_done

layer0_8bpp_bmp_render endp

layer1_8bpp_bmp_render proc
	; ebx contains the pixel data - 32bits worth
	; r15 is the buffer position
	mov rsi, [rdx].state.memory_ptr
	movzx r11, byte ptr [rsi + L1_HSCROLL_H]		; get offset
	shl r11, 4

	mov rsi, [rdx].state.display_buffer_ptr

	writepixel_8bpp_bitmap 00, BUFFER_LAYER1
	writepixel_8bpp_bitmap 01, BUFFER_LAYER1
	writepixel_8bpp_bitmap 02, BUFFER_LAYER1
	writepixel_8bpp_bitmap 03, BUFFER_LAYER1

	mov rax, 4

	jmp layer1_render_done

layer1_8bpp_bmp_render endp