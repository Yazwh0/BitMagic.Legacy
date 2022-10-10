writepixel_2bpp_normal macro bitmask, bitshift, outputoffset, pixeloffset, width
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

layer0_2bpp_til_x_render proc
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

	cmp r13, 16
	je pixel_16

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

	writepixel_2bpp_normal 000c0h, 06, BUFFER_LAYER0, 0, 8
	writepixel_2bpp_normal 00030h, 04, BUFFER_LAYER0, 1, 8
	writepixel_2bpp_normal 0000ch, 02, BUFFER_LAYER0, 2, 8
	writepixel_2bpp_normal 00003h, 00 ,BUFFER_LAYER0, 3, 8
	writepixel_2bpp_normal 0c000h, 14, BUFFER_LAYER0, 4, 8
	writepixel_2bpp_normal 03000h, 12, BUFFER_LAYER0, 5, 8
	writepixel_2bpp_normal 00c00h, 10, BUFFER_LAYER0, 6, 8
	writepixel_2bpp_normal 00300h, 08, BUFFER_LAYER0, 7, 8


	mov rax, r10 ; count till next update requirement
	xor rax, r14 ; tile mask to invert
	add rax, 1
	
	pop r11
	pop r12

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

	writepixel_2bpp_normal 000c0h, 06, BUFFER_LAYER0, 0, 16
	writepixel_2bpp_normal 00030h, 04, BUFFER_LAYER0, 1, 16
	writepixel_2bpp_normal 0000ch, 02, BUFFER_LAYER0, 2, 16
	writepixel_2bpp_normal 00003h, 00 ,BUFFER_LAYER0, 3, 16
	writepixel_2bpp_normal 0c000h, 14, BUFFER_LAYER0, 4, 16
	writepixel_2bpp_normal 03000h, 12, BUFFER_LAYER0, 5, 16
	writepixel_2bpp_normal 00c00h, 10, BUFFER_LAYER0, 6, 16
	writepixel_2bpp_normal 00300h, 08, BUFFER_LAYER0, 7, 16

	writepixel_2bpp_normal 000c00000h, 06+16, BUFFER_LAYER0, 8, 16
	writepixel_2bpp_normal 000300000h, 04+16, BUFFER_LAYER0, 9, 16
	writepixel_2bpp_normal 0000c0000h, 02+16, BUFFER_LAYER0, 10, 16
	writepixel_2bpp_normal 000030000h, 00+16 ,BUFFER_LAYER0, 11, 16
	writepixel_2bpp_normal 0c0000000h, 14+16, BUFFER_LAYER0, 12, 16
	writepixel_2bpp_normal 030000000h, 12+16, BUFFER_LAYER0, 13, 16
	writepixel_2bpp_normal 00c000000h, 10+16, BUFFER_LAYER0, 14, 16
	writepixel_2bpp_normal 003000000h, 08+16, BUFFER_LAYER0, 15, 16

	mov rax, r10 ; count till next update requirement
	xor rax, r14
	add rax, 1
	
	pop r11
	pop r12

	jmp layer0_render_done

; ----------------------------------------------------------------------------------
flipped:
	shr rax, 12		; rax is now pallette offset
	shl rax, 4		; * 16

	cmp r13, 16
	je pixel_16_f

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

	writepixel_2bpp_normal 00300h, 08, BUFFER_LAYER0, 0, 9
	writepixel_2bpp_normal 00c00h, 10, BUFFER_LAYER0, 1, 9
	writepixel_2bpp_normal 03000h, 12, BUFFER_LAYER0, 2, 9
	writepixel_2bpp_normal 0c000h, 14, BUFFER_LAYER0, 3, 9
	writepixel_2bpp_normal 00003h, 00 ,BUFFER_LAYER0, 4, 9
	writepixel_2bpp_normal 0000ch, 02, BUFFER_LAYER0, 5, 9
	writepixel_2bpp_normal 00030h, 04, BUFFER_LAYER0, 6, 9
	writepixel_2bpp_normal 000c0h, 06, BUFFER_LAYER0, 7, 9

	mov rax, r10 ; count till next update requirement
	xor rax, r14 ; tile mask to invert
	add rax, 1
	
	pop r11
	pop r12

	jmp layer0_render_done

pixel_16_f:
	lea r13, pixel_jump_16_f
	jmp qword ptr [r13 + r10 * 8]

pixel_jump_16_f:
	qword pixel_0_17
	qword pixel_1_17
	qword pixel_2_17
	qword pixel_3_17
	qword pixel_4_17
	qword pixel_5_17
	qword pixel_6_17
	qword pixel_7_17
	qword pixel_8_17
	qword pixel_9_17
	qword pixel_10_17
	qword pixel_11_17
	qword pixel_12_17
	qword pixel_13_17
	qword pixel_14_17
	qword pixel_15_17
	
	writepixel_2bpp_normal 003000000h, 08+16, BUFFER_LAYER0, 0, 17
	writepixel_2bpp_normal 00c000000h, 10+16, BUFFER_LAYER0, 1, 17
	writepixel_2bpp_normal 030000000h, 12+16, BUFFER_LAYER0, 2, 17
	writepixel_2bpp_normal 0c0000000h, 14+16, BUFFER_LAYER0, 3, 17
	writepixel_2bpp_normal 000030000h, 00+16 ,BUFFER_LAYER0, 4, 17
	writepixel_2bpp_normal 0000c0000h, 02+16, BUFFER_LAYER0, 5, 17
	writepixel_2bpp_normal 000300000h, 04+16, BUFFER_LAYER0, 6, 17
	writepixel_2bpp_normal 000c00000h, 06+16, BUFFER_LAYER0, 7, 17

	writepixel_2bpp_normal 00300h, 08, BUFFER_LAYER0, 8, 17
	writepixel_2bpp_normal 00c00h, 10, BUFFER_LAYER0, 9, 17
	writepixel_2bpp_normal 03000h, 12, BUFFER_LAYER0, 10, 17
	writepixel_2bpp_normal 0c000h, 14, BUFFER_LAYER0, 11, 17
	writepixel_2bpp_normal 00003h, 00 ,BUFFER_LAYER0, 12, 17
	writepixel_2bpp_normal 0000ch, 02, BUFFER_LAYER0, 13, 17
	writepixel_2bpp_normal 00030h, 04, BUFFER_LAYER0, 14, 17
	writepixel_2bpp_normal 000c0h, 06, BUFFER_LAYER0, 15, 17


	mov rax, r10 ; count till next update requirement
	xor rax, r14
	add rax, 1
	
	pop r11
	pop r12

	jmp layer0_render_done

layer0_2bpp_til_x_render endp

layer1_2bpp_til_x_render proc
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

	cmp r13, 16
	je pixel_16

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

	writepixel_2bpp_normal 000c0h, 06, BUFFER_LAYER1, 0, 8
	writepixel_2bpp_normal 00030h, 04, BUFFER_LAYER1, 1, 8
	writepixel_2bpp_normal 0000ch, 02, BUFFER_LAYER1, 2, 8
	writepixel_2bpp_normal 00003h, 00 ,BUFFER_LAYER1, 3, 8
	writepixel_2bpp_normal 0c000h, 14, BUFFER_LAYER1, 4, 8
	writepixel_2bpp_normal 03000h, 12, BUFFER_LAYER1, 5, 8
	writepixel_2bpp_normal 00c00h, 10, BUFFER_LAYER1, 6, 8
	writepixel_2bpp_normal 00300h, 08, BUFFER_LAYER1, 7, 8


	mov rax, r10 ; count till next update requirement
	xor rax, r14 ; tile mask to invert
	add rax, 1
	
	pop r11
	pop r12

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

	writepixel_2bpp_normal 000c0h, 06, BUFFER_LAYER1, 0, 16
	writepixel_2bpp_normal 00030h, 04, BUFFER_LAYER1, 1, 16
	writepixel_2bpp_normal 0000ch, 02, BUFFER_LAYER1, 2, 16
	writepixel_2bpp_normal 00003h, 00 ,BUFFER_LAYER1, 3, 16
	writepixel_2bpp_normal 0c000h, 14, BUFFER_LAYER1, 4, 16
	writepixel_2bpp_normal 03000h, 12, BUFFER_LAYER1, 5, 16
	writepixel_2bpp_normal 00c00h, 10, BUFFER_LAYER1, 6, 16
	writepixel_2bpp_normal 00300h, 08, BUFFER_LAYER1, 7, 16

	writepixel_2bpp_normal 000c00000h, 06+16, BUFFER_LAYER1, 8, 16
	writepixel_2bpp_normal 000300000h, 04+16, BUFFER_LAYER1, 9, 16
	writepixel_2bpp_normal 0000c0000h, 02+16, BUFFER_LAYER1, 10, 16
	writepixel_2bpp_normal 000030000h, 00+16 ,BUFFER_LAYER1, 11, 16
	writepixel_2bpp_normal 0c0000000h, 14+16, BUFFER_LAYER1, 12, 16
	writepixel_2bpp_normal 030000000h, 12+16, BUFFER_LAYER1, 13, 16
	writepixel_2bpp_normal 00c000000h, 10+16, BUFFER_LAYER1, 14, 16
	writepixel_2bpp_normal 003000000h, 08+16, BUFFER_LAYER1, 15, 16

	mov rax, r10 ; count till next update requirement
	xor rax, r14
	add rax, 1
	
	pop r11
	pop r12

	jmp layer1_render_done

; ----------------------------------------------------------------------------------
flipped:
	shr rax, 12		; rax is now pallette offset
	shl rax, 4		; * 16

	cmp r13, 16
	je pixel_16_f

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

	writepixel_2bpp_normal 00300h, 08, BUFFER_LAYER1, 0, 9
	writepixel_2bpp_normal 00c00h, 10, BUFFER_LAYER1, 1, 9
	writepixel_2bpp_normal 03000h, 12, BUFFER_LAYER1, 2, 9
	writepixel_2bpp_normal 0c000h, 14, BUFFER_LAYER1, 3, 9
	writepixel_2bpp_normal 00003h, 00 ,BUFFER_LAYER1, 4, 9
	writepixel_2bpp_normal 0000ch, 02, BUFFER_LAYER1, 5, 9
	writepixel_2bpp_normal 00030h, 04, BUFFER_LAYER1, 6, 9
	writepixel_2bpp_normal 000c0h, 06, BUFFER_LAYER1, 7, 9

	mov rax, r10 ; count till next update requirement
	xor rax, r14 ; tile mask to invert
	add rax, 1
	
	pop r11
	pop r12

	jmp layer1_render_done

pixel_16_f:
	lea r13, pixel_jump_16_f
	jmp qword ptr [r13 + r10 * 8]

pixel_jump_16_f:
	qword pixel_0_17
	qword pixel_1_17
	qword pixel_2_17
	qword pixel_3_17
	qword pixel_4_17
	qword pixel_5_17
	qword pixel_6_17
	qword pixel_7_17
	qword pixel_8_17
	qword pixel_9_17
	qword pixel_10_17
	qword pixel_11_17
	qword pixel_12_17
	qword pixel_13_17
	qword pixel_14_17
	qword pixel_15_17

	writepixel_2bpp_normal 003000000h, 08+16, BUFFER_LAYER1, 0, 17
	writepixel_2bpp_normal 00c000000h, 10+16, BUFFER_LAYER1, 1, 17
	writepixel_2bpp_normal 030000000h, 12+16, BUFFER_LAYER1, 2, 17
	writepixel_2bpp_normal 0c0000000h, 14+16, BUFFER_LAYER1, 3, 17
	writepixel_2bpp_normal 000030000h, 00+16 ,BUFFER_LAYER1, 4, 17
	writepixel_2bpp_normal 0000c0000h, 02+16, BUFFER_LAYER1, 5, 17
	writepixel_2bpp_normal 000300000h, 04+16, BUFFER_LAYER1, 6, 17
	writepixel_2bpp_normal 000c00000h, 06+16, BUFFER_LAYER1, 7, 17

	writepixel_2bpp_normal 00300h, 08, BUFFER_LAYER1, 8, 17
	writepixel_2bpp_normal 00c00h, 10, BUFFER_LAYER1, 9, 17
	writepixel_2bpp_normal 03000h, 12, BUFFER_LAYER1, 10, 17
	writepixel_2bpp_normal 0c000h, 14, BUFFER_LAYER1, 11, 17
	writepixel_2bpp_normal 00003h, 00 ,BUFFER_LAYER1, 12, 17
	writepixel_2bpp_normal 0000ch, 02, BUFFER_LAYER1, 13, 17
	writepixel_2bpp_normal 00030h, 04, BUFFER_LAYER1, 14, 17
	writepixel_2bpp_normal 000c0h, 06, BUFFER_LAYER1, 15, 17

	mov rax, r10 ; count till next update requirement
	xor rax, r14
	add rax, 1
	
	pop r11
	pop r12

	jmp layer1_render_done

layer1_2bpp_til_x_render endp