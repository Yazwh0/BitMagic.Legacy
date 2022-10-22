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
; Pixel write macros
;

writepixel_1bpp_t256 macro bitmask, outputoffset, pixeloffset, width
pixel_&pixeloffset&_&width&:
	mov r13, rbx		; use r13b to write to the buffer
	and r13, bitmask
	cmovne r13, rax
	mov byte ptr [rsi + r15 + outputoffset], r13b
	add rsi, 1
endm

writepixel_1bpp_normal macro bitmask, outputoffset, pixeloffset, width
pixel_&pixeloffset&_&width&:
	mov r13, r11		; use r13b to write to the buffer
	test ebx, bitmask
	cmovne r13, rax
	mov byte ptr [rsi + r15 + outputoffset], r13b
	add rsi, 1
endm
;
; Layer 0
;

layer0_1bpp_til_x_render proc
	; ax now contains tile number and colour information
	; ebx now contains tile data
	; r10 is the number of pixels in ebx 

	; r15 is our buffer current position
	; need to fill the buffer with the colour indexes for each pixel
	mov rsi, [rdx].state.display_buffer_ptr

	mov r11, rax
	shr r11, 12			; r11b now contains the background colour index

	and rax, 0f00h		; al now contains the foreground colour index
	shr rax, 8

	cmp r13, 16
	je pixel_16

pixel_8:
	lea r13, pixel_jump_8
	jmp qword ptr [r13 + r10 * 8]

pixel_jump_8:
	qword pixel_0_8
	qword pixel_1_8
	qword pixel_2_8
	qword pixel_3_8
	qword pixel_4_8
	qword pixel_5_8
	qword pixel_6_8
	qword pixel_7_8
	
	writepixel_1bpp_normal 080h, BUFFER_LAYER0, 0, 8
	writepixel_1bpp_normal 040h, BUFFER_LAYER0, 1, 8
	writepixel_1bpp_normal 020h, BUFFER_LAYER0, 2, 8
	writepixel_1bpp_normal 010h, BUFFER_LAYER0, 3, 8
	writepixel_1bpp_normal 08h, BUFFER_LAYER0, 4, 8
	writepixel_1bpp_normal 04h, BUFFER_LAYER0, 5, 8
	writepixel_1bpp_normal 02h, BUFFER_LAYER0, 6, 8
	writepixel_1bpp_normal 01h, BUFFER_LAYER0, 7, 8

	xor r10, r14		; mask value
	lea rax, [r10+1]	; add 1 to complete count
	
	jmp layer0_render_done

pixel_16:
	lea r13, pixel_jump_16
	jmp qword ptr [r13 + r10 * 8]

pixel_jump_16:
	qword pixel_0_16
	qword pixel_1_16
	qword pixel_2_16
	qword pixel_3_16
	qword pixel_4_16
	qword pixel_5_16
	qword pixel_6_16
	qword pixel_7_16
	qword pixel_8_16
	qword pixel_9_16
	qword pixel_10_16
	qword pixel_11_16
	qword pixel_12_16
	qword pixel_13_16
	qword pixel_14_16
	qword pixel_15_16
	
	writepixel_1bpp_normal 080h, BUFFER_LAYER0, 0, 16
	writepixel_1bpp_normal 040h, BUFFER_LAYER0, 1, 16
	writepixel_1bpp_normal 020h, BUFFER_LAYER0, 2, 16
	writepixel_1bpp_normal 010h, BUFFER_LAYER0, 3, 16
	writepixel_1bpp_normal 08h, BUFFER_LAYER0, 4, 16
	writepixel_1bpp_normal 04h, BUFFER_LAYER0, 5, 16
	writepixel_1bpp_normal 02h, BUFFER_LAYER0, 6, 16
	writepixel_1bpp_normal 01h, BUFFER_LAYER0, 7, 16

	writepixel_1bpp_normal 08000h, BUFFER_LAYER0, 8, 16
	writepixel_1bpp_normal 04000h, BUFFER_LAYER0, 9, 16
	writepixel_1bpp_normal 02000h, BUFFER_LAYER0, 10, 16
	writepixel_1bpp_normal 01000h, BUFFER_LAYER0, 11, 16
	writepixel_1bpp_normal 0800h, BUFFER_LAYER0, 12, 16
	writepixel_1bpp_normal 0400h, BUFFER_LAYER0, 13, 16
	writepixel_1bpp_normal 0200h, BUFFER_LAYER0, 14, 16
	writepixel_1bpp_normal 0100h, BUFFER_LAYER0, 15, 16

	xor r10, r14		; mask value
	lea rax, [r10+1]	; add 1 to complete count
	
	jmp layer0_render_done
layer0_1bpp_til_x_render endp

layer0_1bpp_til_t_render proc
	; ax now contains tile number and colour information
	; ebx now contains tile data

	; r15 is our buffer current position
	; need to fill the buffer with the colour indexes for each pixel
	mov rsi, [rdx].state.display_buffer_ptr

	shr rax, 8			; use ah as the idnex
	
	cmp r13, 16
	je pixel_16

pixel_8:
	lea r13, pixel_jump_8
	jmp qword ptr [r13 + r10 * 8]

pixel_jump_8:
	qword pixel_0_8
	qword pixel_1_8
	qword pixel_2_8
	qword pixel_3_8
	qword pixel_4_8
	qword pixel_5_8
	qword pixel_6_8
	qword pixel_7_8
	
	writepixel_1bpp_t256 080h, BUFFER_LAYER0, 0, 8
	writepixel_1bpp_t256 040h, BUFFER_LAYER0, 1, 8
	writepixel_1bpp_t256 020h, BUFFER_LAYER0, 2, 8
	writepixel_1bpp_t256 010h, BUFFER_LAYER0, 3, 8
	writepixel_1bpp_t256 08h, BUFFER_LAYER0, 4, 8
	writepixel_1bpp_t256 04h, BUFFER_LAYER0, 5, 8
	writepixel_1bpp_t256 02h, BUFFER_LAYER0, 6, 8
	writepixel_1bpp_t256 01h, BUFFER_LAYER0, 7, 8

	xor r10, r14		; mask value
	lea rax, [r10+1]	; add 1 to complete count

	jmp layer0_render_done

pixel_16:
	lea r13, pixel_jump_16
	jmp qword ptr [r13 + r10 * 8]

pixel_jump_16:
	qword pixel_0_16
	qword pixel_1_16
	qword pixel_2_16
	qword pixel_3_16
	qword pixel_4_16
	qword pixel_5_16
	qword pixel_6_16
	qword pixel_7_16
	qword pixel_8_16
	qword pixel_9_16
	qword pixel_10_16
	qword pixel_11_16
	qword pixel_12_16
	qword pixel_13_16
	qword pixel_14_16
	qword pixel_15_16
	
	writepixel_1bpp_t256 080h, BUFFER_LAYER0, 0, 16
	writepixel_1bpp_t256 040h, BUFFER_LAYER0, 1, 16
	writepixel_1bpp_t256 020h, BUFFER_LAYER0, 2, 16
	writepixel_1bpp_t256 010h, BUFFER_LAYER0, 3, 16
	writepixel_1bpp_t256 08h, BUFFER_LAYER0, 4, 16
	writepixel_1bpp_t256 04h, BUFFER_LAYER0, 5, 16
	writepixel_1bpp_t256 02h, BUFFER_LAYER0, 6, 16
	writepixel_1bpp_t256 01h, BUFFER_LAYER0, 7, 16

	writepixel_1bpp_t256 08000h, BUFFER_LAYER0, 8, 16
	writepixel_1bpp_t256 04000h, BUFFER_LAYER0, 9, 16
	writepixel_1bpp_t256 02000h, BUFFER_LAYER0, 10, 16
	writepixel_1bpp_t256 01000h, BUFFER_LAYER0, 11, 16
	writepixel_1bpp_t256 0800h, BUFFER_LAYER0, 12, 16
	writepixel_1bpp_t256 0400h, BUFFER_LAYER0, 13, 16
	writepixel_1bpp_t256 0200h, BUFFER_LAYER0, 14, 16
	writepixel_1bpp_t256 0100h, BUFFER_LAYER0, 15, 16

	xor r10, r14		; mask value
	lea rax, [r10+1]	; add 1 to complete count

	jmp layer0_render_done
layer0_1bpp_til_t_render endp

;
; Layer 1
;

layer1_1bpp_til_x_render proc
	; ax now contains tile number
	; ebx now contains tile data
	; r10 is the number of pixels in ebx 

	; r15 is our buffer current position
	; need to fill the buffer with the colour indexes for each pixel
	mov rsi, [rdx].state.display_buffer_ptr

	mov r11, rax
	shr r11, 12			; r11b now contains the background colour index

	and rax, 0f00h		; al now contains the foreground colour index
	shr rax, 8

	cmp r13, 16
	je pixel_16

pixel_8:
	lea r13, pixel_jump_8
	jmp qword ptr [r13 + r10 * 8]

pixel_jump_8:
	qword pixel_0_8
	qword pixel_1_8
	qword pixel_2_8
	qword pixel_3_8
	qword pixel_4_8
	qword pixel_5_8
	qword pixel_6_8
	qword pixel_7_8

	writepixel_1bpp_normal 080h, BUFFER_LAYER1, 0, 8
	writepixel_1bpp_normal 040h, BUFFER_LAYER1, 1, 8
	writepixel_1bpp_normal 020h, BUFFER_LAYER1, 2, 8
	writepixel_1bpp_normal 010h, BUFFER_LAYER1, 3, 8
	writepixel_1bpp_normal 08h, BUFFER_LAYER1, 4, 8
	writepixel_1bpp_normal 04h, BUFFER_LAYER1, 5, 8
	writepixel_1bpp_normal 02h, BUFFER_LAYER1, 6, 8
	writepixel_1bpp_normal 01h, BUFFER_LAYER1, 7, 8

	xor r10, r14		; mask value
	lea rax, [r10+1]	; add 1 to complete count

	jmp layer1_render_done

pixel_16:
	lea r13, pixel_jump_16
	jmp qword ptr [r13 + r10 * 8]

pixel_jump_16:
	qword pixel_0_16
	qword pixel_1_16
	qword pixel_2_16
	qword pixel_3_16
	qword pixel_4_16
	qword pixel_5_16
	qword pixel_6_16
	qword pixel_7_16
	qword pixel_8_16
	qword pixel_9_16
	qword pixel_10_16
	qword pixel_11_16
	qword pixel_12_16
	qword pixel_13_16
	qword pixel_14_16
	qword pixel_15_16

	writepixel_1bpp_normal 080h, BUFFER_LAYER1, 0, 16
	writepixel_1bpp_normal 040h, BUFFER_LAYER1, 1, 16
	writepixel_1bpp_normal 020h, BUFFER_LAYER1, 2, 16
	writepixel_1bpp_normal 010h, BUFFER_LAYER1, 3, 16
	writepixel_1bpp_normal 08h, BUFFER_LAYER1, 4, 16
	writepixel_1bpp_normal 04h, BUFFER_LAYER1, 5, 16
	writepixel_1bpp_normal 02h, BUFFER_LAYER1, 6, 16
	writepixel_1bpp_normal 01h, BUFFER_LAYER1, 7, 16

	writepixel_1bpp_normal 08000h, BUFFER_LAYER1, 8, 16
	writepixel_1bpp_normal 04000h, BUFFER_LAYER1, 9, 16
	writepixel_1bpp_normal 02000h, BUFFER_LAYER1, 10, 16
	writepixel_1bpp_normal 01000h, BUFFER_LAYER1, 11, 16
	writepixel_1bpp_normal 0800h, BUFFER_LAYER1, 12, 16
	writepixel_1bpp_normal 0400h, BUFFER_LAYER1, 13, 16
	writepixel_1bpp_normal 0200h, BUFFER_LAYER1, 14, 16
	writepixel_1bpp_normal 0100h, BUFFER_LAYER1, 15, 16

	xor r10, r14		; mask value
	lea rax, [r10+1]	; add 1 to complete count

	jmp layer1_render_done
layer1_1bpp_til_x_render endp

layer1_1bpp_til_t_render proc
	; ax now contains tile number
	; ebx now contains tile data
	; r10 number of pixels
	; r14 is the mask to xor r10 on to get number of pixels

	; r15 is our buffer current position
	; need to fill the buffer with the colour indexes for each pixel
	mov rsi, [rdx].state.display_buffer_ptr

	shr rax, 8			; use ah as the index

	cmp r13, 16
	je pixel_16
	
pixel_8:
	lea r13, pixel_jump_8
	jmp qword ptr [r13 + r10 * 8]

pixel_jump_8:
	qword pixel_0_8
	qword pixel_1_8
	qword pixel_2_8
	qword pixel_3_8
	qword pixel_4_8
	qword pixel_5_8
	qword pixel_6_8
	qword pixel_7_8

	writepixel_1bpp_t256 080h, BUFFER_LAYER1, 0, 8
	writepixel_1bpp_t256 040h, BUFFER_LAYER1, 1, 8
	writepixel_1bpp_t256 020h, BUFFER_LAYER1, 2, 8
	writepixel_1bpp_t256 010h, BUFFER_LAYER1, 3, 8
	writepixel_1bpp_t256 08h, BUFFER_LAYER1, 4, 8
	writepixel_1bpp_t256 04h, BUFFER_LAYER1, 5, 8
	writepixel_1bpp_t256 02h, BUFFER_LAYER1, 6, 8
	writepixel_1bpp_t256 01h, BUFFER_LAYER1, 7, 8

	xor r10, r14		; mask value
	lea rax, [r10+1]	; add 1 to complete count

	jmp layer1_render_done

pixel_16:
	lea r13, pixel_jump_16
	jmp qword ptr [r13 + r10 * 8]

pixel_jump_16:
	qword pixel_0_16
	qword pixel_1_16
	qword pixel_2_16
	qword pixel_3_16
	qword pixel_4_16
	qword pixel_5_16
	qword pixel_6_16
	qword pixel_7_16
	qword pixel_8_16
	qword pixel_9_16
	qword pixel_10_16
	qword pixel_11_16
	qword pixel_12_16
	qword pixel_13_16
	qword pixel_14_16
	qword pixel_15_16

	writepixel_1bpp_t256 080h, BUFFER_LAYER1, 0, 16
	writepixel_1bpp_t256 040h, BUFFER_LAYER1, 1, 16
	writepixel_1bpp_t256 020h, BUFFER_LAYER1, 2, 16
	writepixel_1bpp_t256 010h, BUFFER_LAYER1, 3, 16
	writepixel_1bpp_t256 08h, BUFFER_LAYER1, 4, 16
	writepixel_1bpp_t256 04h, BUFFER_LAYER1, 5, 16
	writepixel_1bpp_t256 02h, BUFFER_LAYER1, 6, 16
	writepixel_1bpp_t256 01h, BUFFER_LAYER1, 7, 16

	writepixel_1bpp_t256 08000h, BUFFER_LAYER1, 8, 16
	writepixel_1bpp_t256 04000h, BUFFER_LAYER1, 9, 16
	writepixel_1bpp_t256 02000h, BUFFER_LAYER1, 10, 16
	writepixel_1bpp_t256 01000h, BUFFER_LAYER1, 11, 16
	writepixel_1bpp_t256 0800h, BUFFER_LAYER1, 12, 16
	writepixel_1bpp_t256 0400h, BUFFER_LAYER1, 13, 16
	writepixel_1bpp_t256 0200h, BUFFER_LAYER1, 14, 16
	writepixel_1bpp_t256 0100h, BUFFER_LAYER1, 15, 16

	xor r10, r14		; mask value
	lea rax, [r10+1]	; add 1 to complete count

	jmp layer1_render_done
layer1_1bpp_til_t_render endp