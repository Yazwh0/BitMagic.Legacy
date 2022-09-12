
state struct 
	memory_ptr				qword ?
	rom_ptr					qword ?
	rambank_ptr				qword ?
	display_ptr				qword ?

	; Vera
	vram_ptr				qword ?
	palette_ptr				qword ? 

	data0_address			qword ?
	data1_address			qword ?
	data0_step				qword ?
	data1_step				qword ?


	clock					qword ?
	vera_clock				qword ?
	register_pc				word ?
	stackpointer			word ?
	register_a				byte ?
	register_x				byte ?
	register_y				byte ?
	flags_decimal			byte ?
	flags_break				byte ?
	flags_overflow			byte ?
	flags_negative			byte ?
	flags_carry				byte ?
	flags_zero				byte ?
	flags_interruptDisable	byte ?

	interrupt				byte ?

	; Vera
	addrsel					byte ?
	dcsel					byte ?
	dc_hscale				byte ?
	dc_vscale				byte ?
	dc_border				byte ?
	dc_hstart				word ?
	dc_hstop				word ?
	dc_vstart				word ?
	dc_vstop				word ?

	sprite_enable			byte ?
	layer0_enable			byte ?
	layer1_enable			byte ?

	display_carry			byte ?
	;_padding				byte ?

	layer0_mapAddress		dword ?
	layer0_tileAddress		dword ?
	layer0_hscroll			word ?
	layer0_vscroll			word ?
	layer0_mapHeight		byte ?
	layer0_mapWidth			byte ?
	layer0_bitmapMode		byte ?
	layer0_colourDepth		byte ?
	layer0_tileHeight		byte ?
	layer0_tileWidth		byte ?

	cpu_waiting				byte ?
;	_padding1				byte ?
	_padding2				byte ?


	layer1_mapAddress		dword ?
	layer1_tileAddress		dword ?
	layer1_hscroll			word ?
	layer1_vscroll			word ?
	layer1_mapHeight		byte ?
	layer1_mapWidth			byte ?
	layer1_bitmapMode		byte ?
	layer1_colourDepth		byte ?
	layer1_tileHeight		byte ?
	layer1_tileWidth		byte ?

	interrupt_linenum		word ?
	interrupt_aflow			byte ?
	interrupt_spcol			byte ?
	interrupt_line			byte ?
	interrupt_vsync			byte ?

	interrupt_line_hit		byte ?
	interrupt_vsync_hit		byte ?
	interrupt_spcol_hit		byte ?
	drawing					byte ?

	; Rendering
	display_position		dword ?
;	display_x				word ?
;	display_y				word ?
state ends

