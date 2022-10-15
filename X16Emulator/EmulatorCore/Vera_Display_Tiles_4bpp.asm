writepixel_4bpp_normal macro bitmask, bitshift, outputoffset, pixeloffset, width
local zero_pallette
	pixel_&pixeloffset&_&width&:
	mov r13, rbx		; use r13b to write to the buffer
	and r13, bitmask	; mask colour index whic his at the top
	if bitshift ne 0
	shr r13, bitshift	; shift to value
	endif
	test r13, r13
	je zero_pallette
	add r13, rax		; add offset
zero_pallette:
	mov byte ptr [rsi + r15 + outputoffset], r13b
	add rsi, 1
endm

layer0_4bpp_til_x_render proc
	push r12
	push r11
	add r12w, word ptr [rdx].state.layer0_vscroll
	add r11w, word ptr [rdx].state.layer0_hscroll
	mov r13d, dword ptr [rdx].state.layer0_mapAddress
	mov r14d, dword ptr [rdx].state.layer0_tileAddress

	get_tile_definition_layer0
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
	qword pixel_4_8
	qword pixel_5_8
	qword pixel_6_8
	qword pixel_7_8

	writepixel_4bpp_normal 0000000f0h, 04, BUFFER_LAYER0, 0, 8
	writepixel_4bpp_normal 00000000fh, 00, BUFFER_LAYER0, 1, 8
	writepixel_4bpp_normal 00000f000h, 12, BUFFER_LAYER0, 2, 8
	writepixel_4bpp_normal 000000f00h, 08 ,BUFFER_LAYER0, 3, 8
	writepixel_4bpp_normal 000f00000h, 20, BUFFER_LAYER0, 4, 8
	writepixel_4bpp_normal 0000f0000h, 16, BUFFER_LAYER0, 5, 8
	writepixel_4bpp_normal 0f0000000h, 28, BUFFER_LAYER0, 6, 8
	writepixel_4bpp_normal 00f000000h, 24, BUFFER_LAYER0, 7, 8

	mov rax, r10 ; count till next update requirement
	xor rax, r14 ; tile mask to invert
	add rax, 1
	
	pop r11
	pop r12

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
	qword pixel_4_9
	qword pixel_5_9
	qword pixel_6_9
	qword pixel_7_9

	writepixel_4bpp_normal 00f000000h, 24, BUFFER_LAYER0, 0, 9
	writepixel_4bpp_normal 0f0000000h, 28, BUFFER_LAYER0, 1, 9
	writepixel_4bpp_normal 0000f0000h, 16, BUFFER_LAYER0, 2, 9
	writepixel_4bpp_normal 000f00000h, 20, BUFFER_LAYER0, 3, 9
	writepixel_4bpp_normal 000000f00h, 08 ,BUFFER_LAYER0, 4, 9
	writepixel_4bpp_normal 00000f000h, 12, BUFFER_LAYER0, 5, 9
	writepixel_4bpp_normal 00000000fh, 00, BUFFER_LAYER0, 6, 9
	writepixel_4bpp_normal 0000000f0h, 04, BUFFER_LAYER0, 7, 9

	mov rax, r10 ; count till next update requirement
	xor rax, r14 ; tile mask to invert
	add rax, 1
	
	pop r11
	pop r12

	jmp layer0_render_done

layer0_4bpp_til_x_render endp

layer1_4bpp_til_x_render proc
	push r12
	push r11
	add r12w, word ptr [rdx].state.layer1_vscroll
	add r11w, word ptr [rdx].state.layer1_hscroll
	mov r13d, dword ptr [rdx].state.layer1_mapAddress
	mov r14d, dword ptr [rdx].state.layer1_tileAddress

	get_tile_definition_layer0
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
	qword pixel_4_8
	qword pixel_5_8
	qword pixel_6_8
	qword pixel_7_8

	writepixel_4bpp_normal 0000000f0h, 04, BUFFER_LAYER1, 0, 8
	writepixel_4bpp_normal 00000000fh, 00, BUFFER_LAYER1, 1, 8
	writepixel_4bpp_normal 00000f000h, 12, BUFFER_LAYER1, 2, 8
	writepixel_4bpp_normal 000000f00h, 08 ,BUFFER_LAYER1, 3, 8
	writepixel_4bpp_normal 000f00000h, 20, BUFFER_LAYER1, 4, 8
	writepixel_4bpp_normal 0000f0000h, 16, BUFFER_LAYER1, 5, 8
	writepixel_4bpp_normal 0f0000000h, 28, BUFFER_LAYER1, 6, 8
	writepixel_4bpp_normal 00f000000h, 24, BUFFER_LAYER1, 7, 8

	mov rax, r10 ; count till next update requirement
	xor rax, r14 ; tile mask to invert
	add rax, 1
	
	pop r11
	pop r12

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
	qword pixel_4_9
	qword pixel_5_9
	qword pixel_6_9
	qword pixel_7_9

	writepixel_4bpp_normal 00f000000h, 24, BUFFER_LAYER1, 0, 9
	writepixel_4bpp_normal 0f0000000h, 28, BUFFER_LAYER1, 1, 9
	writepixel_4bpp_normal 0000f0000h, 16, BUFFER_LAYER1, 2, 9
	writepixel_4bpp_normal 000f00000h, 20, BUFFER_LAYER1, 3, 9
	writepixel_4bpp_normal 000000f00h, 08 ,BUFFER_LAYER1, 4, 9
	writepixel_4bpp_normal 00000f000h, 12, BUFFER_LAYER1, 5, 9
	writepixel_4bpp_normal 00000000fh, 00, BUFFER_LAYER1, 6, 9
	writepixel_4bpp_normal 0000000f0h, 04, BUFFER_LAYER1, 7, 9

	mov rax, r10 ; count till next update requirement
	xor rax, r14 ; tile mask to invert
	add rax, 1
	
	pop r11
	pop r12

	jmp layer0_render_done

layer1_4bpp_til_x_render endp
