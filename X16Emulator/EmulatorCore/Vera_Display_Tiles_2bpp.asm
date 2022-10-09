layer0_2bpp_til_x_render proc
	push r12
	push r11
	add r12w, word ptr [rdx].state.layer0_vscroll
	add r11w, word ptr [rdx].state.layer0_hscroll
	mov r13d, dword ptr [rdx].state.layer0_mapAddress
	mov r14d, dword ptr [rdx].state.layer0_tileAddress

	get_tile_definition_layer0


	
	pop r11
	pop r12

	jmp layer0_render_done
layer0_2bpp_til_x_render endp