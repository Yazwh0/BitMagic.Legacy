writepixel_8bpp_normal macro bitmask, bitshift, outputoffset, pixeloffset, width
local zero_pallette
	pixel_&pixeloffset&_&width&:
	mov r13, rbx		; use r13b to write to the buffer
	and r13, bitmask	; mask colour index whic his at the top
	if bitshift ne 0
	shr r13, bitshift	; shift to value
	endif
	test r13, r13
	je zero_pallette
	cmp r13, 16
	jge zero_pallette
	add r13, rax		; add offset
zero_pallette:
	mov byte ptr [rsi + r15 + outputoffset], r13b
	add rsi, 1
endm

layer0_8bpp_til_x_render proc
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

	writepixel_8bpp_normal 0000000ffh, 00, BUFFER_LAYER0, 0, 8
	writepixel_8bpp_normal 00000ff00h, 08, BUFFER_LAYER0, 1, 8
	writepixel_8bpp_normal 000ff0000h, 16 ,BUFFER_LAYER0, 2, 8
	writepixel_8bpp_normal 0ff000000h, 24, BUFFER_LAYER0, 3, 8

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

	writepixel_8bpp_normal 0ff000000h, 24, BUFFER_LAYER0, 0, 9
	writepixel_8bpp_normal 000ff0000h, 16 ,BUFFER_LAYER0, 1, 9
	writepixel_8bpp_normal 00000ff00h, 08, BUFFER_LAYER0, 2, 9
	writepixel_8bpp_normal 0000000ffh, 00, BUFFER_LAYER0, 3, 9

	mov rax, r10 ; count till next update requirement
	xor rax, r14 ; tile mask to invert
	add rax, 1
	
	pop r11
	pop r12

	jmp layer0_render_done

layer0_8bpp_til_x_render endp

layer1_8bpp_til_x_render proc
	push r12
	push r11
	add r12w, word ptr [rdx].state.layer1_vscroll
	add r11w, word ptr [rdx].state.layer1_hscroll
	mov r13d, dword ptr [rdx].state.layer1_mapAddress
	mov r14d, dword ptr [rdx].state.layer1_tileAddress

	get_tile_definition_layer1
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

	writepixel_8bpp_normal 0ff000000h, 24, BUFFER_LAYER1, 0, 9
	writepixel_8bpp_normal 000ff0000h, 16 ,BUFFER_LAYER1, 1, 9
	writepixel_8bpp_normal 00000ff00h, 08, BUFFER_LAYER1, 2, 9
	writepixel_8bpp_normal 0000000ffh, 00, BUFFER_LAYER1, 3, 9

	mov rax, r10 ; count till next update requirement
	xor rax, r14 ; tile mask to invert
	add rax, 1
	
	pop r11
	pop r12

	jmp layer1_render_done

layer1_8bpp_til_x_render endp