
writepixel_1bpp_bitmap macro pixeloutput, outputoffset
	xor r13, r13
	shr ebx, 1
	setc r13b
	mov byte ptr [rsi + r15 + pixeloutput + outputoffset], r13b
endm

layer0_1bpp_bmp_render proc
	; ebx contains the pixel data - 32bits worth
	; r15 is the buffer position
	mov rsi, [rdx].state.display_buffer_ptr

	writepixel_1bpp_bitmap 07, BUFFER_LAYER0	
	writepixel_1bpp_bitmap 06, BUFFER_LAYER0
	writepixel_1bpp_bitmap 05, BUFFER_LAYER0
	writepixel_1bpp_bitmap 04, BUFFER_LAYER0
	writepixel_1bpp_bitmap 03, BUFFER_LAYER0
	writepixel_1bpp_bitmap 02, BUFFER_LAYER0
	writepixel_1bpp_bitmap 01, BUFFER_LAYER0
	writepixel_1bpp_bitmap 00, BUFFER_LAYER0

	writepixel_1bpp_bitmap 15, BUFFER_LAYER0
	writepixel_1bpp_bitmap 14, BUFFER_LAYER0
	writepixel_1bpp_bitmap 13, BUFFER_LAYER0
	writepixel_1bpp_bitmap 12, BUFFER_LAYER0
	writepixel_1bpp_bitmap 11, BUFFER_LAYER0
	writepixel_1bpp_bitmap 10, BUFFER_LAYER0
	writepixel_1bpp_bitmap 09, BUFFER_LAYER0
	writepixel_1bpp_bitmap 08, BUFFER_LAYER0

	writepixel_1bpp_bitmap 23, BUFFER_LAYER0
	writepixel_1bpp_bitmap 22, BUFFER_LAYER0
	writepixel_1bpp_bitmap 21, BUFFER_LAYER0
	writepixel_1bpp_bitmap 20, BUFFER_LAYER0
	writepixel_1bpp_bitmap 19, BUFFER_LAYER0
	writepixel_1bpp_bitmap 18, BUFFER_LAYER0
	writepixel_1bpp_bitmap 17, BUFFER_LAYER0
	writepixel_1bpp_bitmap 16, BUFFER_LAYER0

	writepixel_1bpp_bitmap 31, BUFFER_LAYER0
	writepixel_1bpp_bitmap 30, BUFFER_LAYER0
	writepixel_1bpp_bitmap 29, BUFFER_LAYER0
	writepixel_1bpp_bitmap 28, BUFFER_LAYER0
	writepixel_1bpp_bitmap 27, BUFFER_LAYER0
	writepixel_1bpp_bitmap 26, BUFFER_LAYER0
	writepixel_1bpp_bitmap 25, BUFFER_LAYER0
	writepixel_1bpp_bitmap 24, BUFFER_LAYER0

	mov rax, 32
	jmp layer0_render_done

layer0_1bpp_bmp_render endp
