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

state struct 
	memory_ptr				qword ?
	rom_ptr					qword ?
	rambank_ptr				qword ?
	display_ptr				qword ?
	display_buffer_ptr		qword ?
	history_ptr				qword ?

	; Vera
	vram_ptr				qword ?
	palette_ptr				qword ? 

	data0_address			qword ?
	data1_address			qword ?
	data0_step				qword ?
	data1_step				qword ?

	clock_previous			qword ?
	clock					qword ?
	vera_clock				qword ?

	history_pos				qword ?

	layer0_jmp				qword ?
	layer0_rtn				qword ?
	layer1_jmp				qword ?
	layer1_rtn				qword ?

	dc_hscale				dword ?
	dc_vscale				dword ?

	brk_causes_stop			dword ?

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
	nmi						byte ?
	nmi_previous			byte ?

	; Vera
	addrsel					byte ?
	dcsel					byte ?
	dc_border				byte ?

	dc_hstart				word ?
	dc_hstop				word ?
	dc_vstart				word ?
	dc_vstop				word ?

	sprite_enable			byte ?
	layer0_enable			byte ?
	layer1_enable			byte ?

	display_carry			byte ?

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
	headless				byte ?

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
	frame_count				dword ?
	buffer_render_position	dword ?
	buffer_output_position	dword ?
	scale_x					dword ?
	scale_y					dword ?
	display_x				word ?
	display_y				word ?

	display_dirty			byte ?
	render_ready			byte ?
	
	; Sprites
	sprite_wait				dword ?
	sprite_position			dword ?

	; Layer 0
	layer0_next_render		word ?
	layer0_tile_hshift		word ?
	layer0_tile_vshift		word ?
	layer0_map_hshift		word ?
	layer0_map_vshift		word ?
	
	; Layer 1
	layer1_next_render		word ?
	layer1_tile_hshift		word ?
	layer1_tile_vshift		word ?
	layer1_map_hshift		word ?
	layer1_map_vshift		word ?
	
	; VIA 1
	via_t1counter_latch		word ?
	via_t1counter_value		word ?
	via_t2counter_latch		word ?
	via_t2counter_value		word ?

	via_register_a_outvalue	byte ?
	via_register_a_invalue	byte ?
	; we just use whats in memory now
	;via_interrupt_timer1	byte ?
	;via_interrupt_timer2	byte ?
	;via_interrupt_cb1		byte ?
	;via_interrupt_cb2		byte ?
	;via_interrupt_shiftregister	byte ?
	;via_interrupt_ca1		byte ?
	;via_interrupt_ca2		byte ?

	via_timer1_continuous	byte ?
	via_timer1_pb7			byte ?
	via_timer1_running		byte ?

	via_timer2_pulsecount	byte ?
	via_timer2_running		byte ?

	; I2c
	;i2c_address				word ?  ; 7 bit address of destination
	;i2c_state				byte ?  ; enum of where we are in the message
	;i2c_sending				byte ?	
	;i2c_data				byte ?	; data that is being transmitted
	;i2c_pos					byte ?  ; position in that data
	;i2c_laststate			byte ?  ; clock and data on the previous obs


state ends

