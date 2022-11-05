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

.code
; ----------------------------------------------------
; VERA
; ----------------------------------------------------

; Cpu emulation:
; rax  : scratch
; rbx  : scratch
; rcx  : scratch
; rdx  : state object 
; rdi  : current memory contexth
; rsi  : scratch
; r8b  : a
; r9b  : x
; r10b : y
; r11w : PC
; r12  : scratch
; r13  : scratch / use to indicate vera data0 or 1 read
; r14  : Clock Ticks
; r15  : Flags

include Constants.inc
include State.asm
include Vera_Display.asm

vera_setaddress_0 macro 
	local search_loop, match, not_negative

	xor r12, r12						; use r12 to store if decr should be set
	mov rax, [rdx].state.data0_step

	test rax, rax
	jns not_negative

	mov r12b, 1000b
	neg rax

not_negative:

	xor rbx, rbx
	mov rsi, vera_step_table

search_loop:
	cmp ax, word ptr [rsi+rbx]
	je match
	add rbx, 2
	cmp rbx, 20h
	jne search_loop

match:
	and rbx, 11110b						; mask off index step (nof 0x0t as we x2 this value earlier)
	shl rbx, 4+8+8-1					; shift to the correct position for the registers
	mov rdi, [rdx].state.data0_address
	or rdi, rbx
	
	mov rsi, [rdx].state.memory_ptr
	mov [rsi+ADDRx_L], di

	shr rdi, 16
	or dil, r12b						; or on the DECR bit
	mov [rsi+ADDRx_H], dil
endm

vera_setaddress_1 macro 
	local search_loop, match

	xor r12, r12						; use r12 to store if decr should be set
	mov rax, [rdx].state.data1_step

	test rax, rax
	jns not_negative

	mov r12b, 1000b
	neg rax

not_negative:

	xor rbx, rbx
	mov rsi, vera_step_table

search_loop:
	cmp ax, word ptr [rsi+rbx]
	je match
	add rbx, 2
	cmp rbx, 20h
	jne search_loop

match:
	and rbx, 11110b
	shl rbx, 4+8+8-1
	mov rdi, [rdx].state.data1_address
	or rdi, rbx
	
	mov rsi, [rdx].state.memory_ptr
	mov [rsi+ADDRx_L], di

	shr rdi, 16
	or dil, r12b						; or on the DECR bit
	mov [rsi+ADDRx_H], dil
endm

set_layer0_jump macro
	mov rsi, [rdx].state.memory_ptr
	movzx rax, byte ptr [rsi + L0_CONFIG]
	and rax, 11110011b
	movzx rbx, byte ptr [rsi + L0_TILEBASE]
	and rbx, 011b
	shl rbx, 2
	or rax, rbx
	lea rbx, get_tile_definition_jump
	mov rax, qword ptr [rbx + rax * 8]
	mov qword ptr [rdx].state.layer0_jmp, rax
endm

set_layer1_jump macro
	mov rsi, [rdx].state.memory_ptr
	movzx rax, byte ptr [rsi + L1_CONFIG]
	and rax, 11110011b
	movzx rbx, byte ptr [rsi + L1_TILEBASE]
	and rbx, 011b
	shl rbx, 2
	or rax, rbx
	lea rbx, get_tile_definition_jump
	mov rax, qword ptr [rbx + rax * 8]
	mov qword ptr [rdx].state.layer1_jmp, rax
endm

layer0_tileshifts macro
	; Multipliers
	; Y tile height
	lea rax, vera_map_shift_table
	movzx rbx, byte ptr [rdx].state.layer0_mapWidth
	mov bx, word ptr [rax + rbx * 2]
	mov word ptr [rdx].state.layer0_map_hshift, bx

	movzx rbx, byte ptr [rdx].state.layer0_mapHeight
	mov bx, word ptr [rax + rbx * 2]
	mov word ptr [rdx].state.layer0_map_vshift, bx

	lea rax, vera_tile_shift_table
	movzx rbx, byte ptr [rdx].state.layer0_tileWidth
	mov bx, word ptr [rax + rbx * 2]
	mov word ptr [rdx].state.layer0_tile_hshift, bx

	movzx rbx, byte ptr [rdx].state.layer0_tileHeight
	mov bx, word ptr [rax + rbx * 2]
	mov word ptr [rdx].state.layer0_tile_vshift, bx

	xor rbx, rbx
	xor rax, rax
endm

layer1_tileshifts macro
	; Multipliers
	; Y tile height
	lea rax, vera_map_shift_table
	movzx rbx, byte ptr [rdx].state.layer1_mapWidth
	mov bx, word ptr [rax + rbx * 2]
	mov word ptr [rdx].state.layer1_map_hshift, bx

	movzx rbx, byte ptr [rdx].state.layer1_mapHeight
	mov bx, word ptr [rax + rbx * 2]
	mov word ptr [rdx].state.layer1_map_vshift, bx

	lea rax, vera_tile_shift_table
	movzx rbx, byte ptr [rdx].state.layer1_tileWidth
	mov bx, word ptr [rax + rbx * 2]
	mov word ptr [rdx].state.layer1_tile_hshift, bx

	movzx rbx, byte ptr [rdx].state.layer1_tileHeight
	mov bx, word ptr [rax + rbx * 2]
	mov word ptr [rdx].state.layer1_tile_vshift, bx

	xor rbx, rbx
	xor rax, rax
endm

; initialise colours etc
; rdx points to cpu state
vera_init proc
	;
	; Set to be drawing, as we'll start at 0,0
	;
	mov byte ptr [rdx].state.drawing, 1
	mov word ptr [rdx].state.layer0_next_render, 1
	mov word ptr [rdx].state.layer1_next_render, 1
	;mov dword ptr [rdx].state.buffer_render_position, 010000000000b

	;
	; DATA0\1
	;
	mov rsi, [rdx].state.memory_ptr
	mov rdi, [rdx].state.vram_ptr
	
	mov rax, [rdx].state.data0_address
	mov bl, byte ptr [rdi+rax]
	mov byte ptr [rsi+DATA0], bl

	mov rax, [rdx].state.data1_address
	mov bl, byte ptr [rdi+rax]
	mov byte ptr [rsi+DATA1], bl

	;
	; AddrSel CTRL + ADDR_x
	;
	cmp [rdx].state.addrsel, 0
	jne set_address1

	; Set Address 0 - init to ctrl as 0
	vera_setaddress_0
	mov byte ptr [rsi+CTRL], 0
	jmp addr_done

set_address1:
	vera_setaddress_1
	mov byte ptr [rsi+CTRL], 1
	
addr_done:

	;
	; DcSel CTRL
	;
	mov r13b, [rdx].state.dcsel
	shl r13b, 1
	or byte ptr [rsi+CTRL], r13b

	;
	; DC_xxx + DC_ Video Settings
	;
	test r13b, r13b
	jnz set_dc1

	xor rax, rax
	mov al, byte ptr [rdx].state.sprite_enable
	shl rax, 6
	mov bl, byte ptr [rdx].state.layer1_enable
	shl rbx, 5
	or rax, rbx
	mov bl, byte ptr [rdx].state.layer0_enable
	shl rbx, 4
	or rax, rbx
	mov byte ptr [rsi+DC_VIDEO], al

	mov eax, dword ptr [rdx].state.dc_hscale
	shr rax, 9
	mov byte ptr [rsi+DC_HSCALE], al

	mov eax, dword ptr [rdx].state.dc_vscale
	shr rax, 9
	mov byte ptr [rsi+DC_VSCALE], al

	mov al, byte ptr [rdx].state.dc_border
	mov byte ptr [rsi+DC_BORDER], al

	jmp dc_done

set_dc1:

	mov al, byte ptr [rdx].state.dc_hstart
	shr ax, 2
	mov byte ptr [rsi+DC_HSTART], al

	mov al, byte ptr [rdx].state.dc_hstop
	shr ax, 2
	mov byte ptr [rsi+DC_HSTOP], al

	mov al, byte ptr [rdx].state.dc_vstart
	shr ax, 1
	mov byte ptr [rsi+DC_VSTART], al

	mov al, byte ptr [rdx].state.dc_vstop
	shr ax, 1
	mov byte ptr [rsi+DC_VSTOP], al

dc_done:

	;
	; Layer 0
	;

	; Config

	xor rax, rax
	mov al, byte ptr [rdx].state.layer0_mapHeight
	and rax, 00000011b
	shl rax, 6
	mov bl, byte ptr [rdx].state.layer0_mapWidth
	and rbx, 00000011b
	shl rbx, 4
	or al, bl
	mov bl, byte ptr [rdx].state.layer0_bitmapMode
	and rbx, 00000001b
	shl rbx, 2
	or al, bl
	mov bl, byte ptr [rdx].state.layer0_colourDepth
	and rbx, 00000011b
	or al, bl

	mov byte ptr [rsi+L0_CONFIG], al
	and al, 00001111b
	lea rbx, layer0_render_jump
	mov rbx, qword ptr [rbx + rax * 8]
	mov qword ptr [rdx].state.layer0_rtn, rbx

	;mov word ptr [rdx].state.layer0_config, ax

	; Map Base Address
	mov eax, dword ptr [rdx].state.layer0_mapAddress
	shr rax, 9
	mov byte ptr [rsi+L0_MAPBASE], al

	; Tile Base Address + Tile Height\Width
	mov eax, dword ptr [rdx].state.layer0_tileAddress
	shr rax, 9
	and rax, 11111100b
	mov bl, byte ptr [rdx].state.layer0_tileHeight
	and bl, 00000001b
	shl bl, 1
	or al, bl
	mov bl, byte ptr [rdx].state.layer0_tileWidth
	and bl, 00000001b
	or al, bl
	mov byte ptr [rsi+L0_TILEBASE], al

	layer0_tileshifts

	; HScroll
	mov ax, word ptr [rdx].state.layer0_hscroll
	and ax, 0fffh
	mov byte ptr [rsi+L0_HSCROLL_L], al
	mov byte ptr [rsi+L0_HSCROLL_H], ah

	; VScroll
	mov ax, word ptr [rdx].state.layer0_vscroll
	and ax, 0fffh
	mov byte ptr [rsi+L0_VSCROLL_L], al
	mov byte ptr [rsi+L0_VSCROLL_H], ah

	;
	; Layer 1
	;

	; Config

	xor rax, rax
	mov al, byte ptr [rdx].state.layer1_mapHeight
	and rax, 00000011b
	shl rax, 6
	mov bl, byte ptr [rdx].state.layer1_mapWidth
	and rbx, 00000011b
	shl rbx, 4
	or al, bl
	mov bl, byte ptr [rdx].state.layer1_bitmapMode
	and rbx, 00000001b
	shl rbx, 2
	or al, bl
	mov bl, byte ptr [rdx].state.layer1_colourDepth
	and rbx, 00000011b
	or al, bl

	mov byte ptr [rsi+L1_CONFIG], al
	and al, 00001111b
	lea rbx, layer1_render_jump
	mov rbx, qword ptr [rbx + rax * 8]
	mov qword ptr [rdx].state.layer1_rtn, rbx
	;mov word ptr [rdx].state.layer1_config, ax

	; Map Base Address
	mov eax, dword ptr [rdx].state.layer1_mapAddress
	shr rax, 9
	mov byte ptr [rsi+L1_MAPBASE], al

	; Tile Base Address + Tile Height\Width
	mov eax, dword ptr [rdx].state.layer1_tileAddress
	shr rax, 9
	and rax, 11111100b
	mov bl, byte ptr [rdx].state.layer1_tileHeight
	and bl, 00000001b
	shl bl, 1
	or al, bl
	mov bl, byte ptr [rdx].state.layer1_tileWidth
	and bl, 00000001b
	or al, bl
	mov byte ptr [rsi+L1_TILEBASE], al

	layer1_tileshifts

	; HScroll
	mov ax, word ptr [rdx].state.layer1_hscroll
	and ax, 0fffh
	mov byte ptr [rsi+L1_HSCROLL_L], al
	mov byte ptr [rsi+L1_HSCROLL_H], ah

	; VScroll
	mov ax, word ptr [rdx].state.layer1_vscroll
	and ax, 0fffh
	mov byte ptr [rsi+L1_VSCROLL_L], al
	mov byte ptr [rsi+L1_VSCROLL_H], ah

	; Interrupt Flags
	mov al, byte ptr [rdx].state.interrupt_vsync
	mov bl, byte ptr [rdx].state.interrupt_line
	shl bl, 1
	or al, bl
	mov bl, byte ptr [rdx].state.interrupt_spcol
	shl bl, 2
	or al, bl
	mov bl, byte ptr [rdx].state.interrupt_aflow
	shl bl, 3
	or al, bl
	mov bx, word ptr [rdx].state.interrupt_linenum

	mov byte ptr [rsi+IRQLINE_L], bl

	and bx, 100h
	shr bx, 1
	or al, bl
	mov byte ptr [rsi+IEN], al

	; Layer 0 jump
	set_layer0_jump

	; Layer 1 jump
	set_layer1_jump

	jmp vera_initialise_palette
vera_init endp

;
; rdi			address
; rax			base address
; r13b			value
;
; Todo, add PSG\Sprite changes if reqd
;
vera_dataupdate_stuctures macro
	local skip, xx_red
	push rsi
	cmp rdi, 1fa00h
	jb skip

	cmp rdi, 1fbffh
	ja skip

	push rax

	mov rsi, [rdx].state.palette_ptr
	mov rax, rdi
	sub rax, 01fa00h
	and rax, 0fffeh							; take off the low bit, as we want the colour index
	mov ecx, [rsi + rax * 2]

	bt rdi, 0
	jc xx_red

	; r13 is GB
	and rcx, 0ff0000ffh						; take GB from current colour
	
	mov r12, r13
	and r12, 00fh							; Isolate B
	shl r12, 16
	or rcx, r12								; or in first nibble
	shl r12, 4
	or rcx, r12								; or in second nibble
	
	mov r12, r13
	and r12, 0f0h							; Isolate G
	shl r12, 4		
	or rcx, r12								; or in first nibble
	shl r12, 4
	or rcx, r12								; or in second nibble

	mov dword ptr [rsi + rax * 2], ecx		; persist
	pop rax
	jmp skip
xx_red:
	; r13 is xR
	
	and rcx, 0ffffff00h						; take R from current colour

	mov r12, r13
	and r12, 00fh
	or rcx, r12								; or in first nibble
	shl r12, 4
	or rcx, r12								; or in second nibble

	mov dword ptr [rsi + rax * 2], ecx
	pop rax
skip:
	pop rsi
endm

; rbx			address
; [rsi+rbx]		output location in main memory
; 
; should only be called if data0\data1 is read\written.

vera_dataaccess_body macro doublestep, write_value

	mov rax, [rdx].state.vram_ptr				; get value from vram
	push rsi
	mov rsi, [rdx].state.memory_ptr				; we need memory context;

	cmp rbx, DATA0
	jne step_data1

	mov rdi, [rdx].state.data0_address

	if write_value eq 1 and doublestep eq 0
		;call vera_render_display
		mov r13b, byte ptr [rsi+rbx]			; get value that has been written
		mov byte ptr [rax+rdi], r13b			; store in vram
		vera_dataupdate_stuctures
	endif

	add rdi, [rdx].state.data0_step
	and rdi, 1ffffh								; mask off high bits so we wrap

	if doublestep eq 1
		if write_value eq 1
			;call vera_render_display
			mov r13b, byte ptr [rsi+rbx]			; get value that has been written
			mov byte ptr [rax+rdi], r13b			; store in vram
			vera_dataupdate_stuctures
		endif

		add rdi, [rdx].state.data0_step			; perform second step
		and rdi, 1ffffh							; mask off high bits so we wrap
	endif

	mov [rdx].state.data0_address, rdi

	mov r13b, byte ptr [rax+rdi]
	mov [rsi+rbx], r13b						; store in ram

	xor r13, r13								; clear r13b, as we use this to detect if we need to call vera

	cmp [rdx].state.addrsel, 0
	je set_data0_address
	pop rsi
	ret

set_data0_address:
		
	mov rsi, [rdx].state.memory_ptr
	mov [rsi+ADDRx_L], di

	shr rdi, 16
	mov al, [rsi+ADDRx_H]						; Add on stepping nibble
	and al, 0f8h								; mask off what isnt changable
	or dil, al
	mov [rsi+ADDRx_H], dil

	xor r13, r13								; clear r13b, as we use this to detect if we need to call vera
	pop rsi
	ret

step_data1:
	mov rdi, [rdx].state.data1_address

	if write_value eq 1 and doublestep eq 0
		;call vera_render_display
		mov r13b, byte ptr [rsi+rbx]			; get value that has been written
		mov byte ptr [rax+rdi], r13b			; store in vram
		vera_dataupdate_stuctures
	endif

	add rdi, [rdx].state.data1_step
	and rdi, 1ffffh								; mask off high bits so we wrap

	if doublestep eq 1
		if write_value eq 1
			;call vera_render_display
			mov r13b, byte ptr [rsi+rbx]		; get value that has been written
			mov byte ptr [rax+rdi], r13b		; store in vram
			vera_dataupdate_stuctures
		endif

		add rdi, [rdx].state.data1_step
		and rdi, 1ffffh							; mask off high bits so we wrap
	endif

	mov [rdx].state.data1_address, rdi

	mov r13b, byte ptr [rax+rdi]
	mov [rsi+rbx], r13b							; store in ram

	xor r13, r13								; clear r13b, as we use this to detect if we need to call vera

	cmp [rdx].state.addrsel, 1
	je set_data1_address
	pop rsi
	ret

set_data1_address:
		
	mov rsi, [rdx].state.memory_ptr
	mov [rsi+ADDRx_L], di

	shr rdi, 16
	mov al, [rsi+ADDRx_H]						; Add on stepping nibble
	and al, 0f8h								; mask off what isnt changable
	or dil, al
	mov [rsi+ADDRx_H], dil

	pop rsi
	ret
endm

;
; VERA post read side effect macros
;

vera_afterread proc
	;dec r13
	vera_dataaccess_body 0, 0
vera_afterread endp

; eg inc, asl
vera_afterreadwrite proc
	;dec r13
	vera_dataaccess_body 1, 1
vera_afterreadwrite endp

;
; Update procs for vera registers
;

vera_update_notimplemented proc
	ret
vera_update_notimplemented endp

vera_update_data proc
	vera_dataaccess_body 0, 1
vera_update_data endp

vera_update_addrl proc	
	mov r13b, byte ptr [rsi+rbx]
	cmp byte ptr [rdx].state.addrsel, 0

	jnz write_data1
	mov byte ptr [rdx].state.data0_address, r13b
	ret
write_data1:
	mov byte ptr [rdx].state.data1_address, r13b
	ret
vera_update_addrl endp

vera_update_addrm proc	
	mov r13b, byte ptr [rsi+rbx]
	cmp byte ptr [rdx].state.addrsel, 0

	jnz write_data1
	mov byte ptr [rdx].state.data0_address + 1, r13b
	ret
write_data1:
	mov byte ptr [rdx].state.data1_address + 1, r13b
	ret
vera_update_addrm endp

vera_update_addrh proc	
	mov r13b, byte ptr [rsi+rbx]						; value that has been written
	and r13, 11111001b
	mov byte ptr [rsi+rbx], r13b						; write back masked value
	cmp byte ptr [rdx].state.addrsel, 0					; data 0 or 1?

	jnz write_data1

	; Top address bit
	xor r12, r12
	bt r13w, 0											; check bit 0, if set then set r12b and move to data address
	setc r12b
	mov byte ptr [rdx].state.data0_address + 2, r12b

	; Index
	mov r12, r13
	and r12, 11110000b									; mask off the index
	shr r12, 3											; index in the table, not 4 as its a word
	mov rax, vera_step_table
	mov r12w, word ptr [rax + r12]						; get value from table

	bt r13w, 3											; check DECR
	jnc no_decr_0
	neg r12
	
no_decr_0:
	mov qword ptr [rdx].state.data0_step, r12
	ret

write_data1:
	; Top address bit
	xor r12, r12
	bt r13w, 0											; check bit 0, if set then set r12b and move to data address
	setc r12b
	mov byte ptr [rdx].state.data1_address + 2, r12b

	; Index
	mov r12, r13
	and r12, 11110000b									; mask off the index
	shr r12, 3											; index in the table, not 4 as its a word
	mov rax, vera_step_table
	mov r12w, word ptr [rax + r12]						; get value from table

	bt r13w, 3											; check DECR
	jnc no_decr_1
	neg r12
	
no_decr_1:
	mov qword ptr [rdx].state.data1_step, r12
	ret
vera_update_addrh endp

vera_update_ctrl proc
	; todo: Handle reset
	mov r13b, byte ptr [rsi+rbx]						; value that has been written
	and r13b, 00000011b
	mov byte ptr [rsi+rbx], r13b

	; Addrsel
	xor r12, r12
	bt r13w, 0
	jc set_addr

	mov byte ptr [rdx].state.addrsel, 0
	vera_setaddress_0
	jmp addr_done

set_addr:
	mov byte ptr [rdx].state.addrsel, 1
	vera_setaddress_1

addr_done:

	xor rax, rax

	bt r13w, 1
	jc set_dcsel

	mov byte ptr [rdx].state.dcsel, 0
	
	xor r12, r12
	mov al, byte ptr [rdx].state.sprite_enable
	shl rax, 6
	mov r12b, byte ptr [rdx].state.layer1_enable
	shl r12b, 5
	or rax, r12
	mov r12b, byte ptr [rdx].state.layer0_enable
	shl r12b, 4
	or rax, r12
	mov byte ptr [rsi+DC_VIDEO], al

	mov al, byte ptr [rdx].state.dc_hscale
	mov byte ptr [rsi+DC_HSCALE], al

	mov al, byte ptr [rdx].state.dc_vscale
	mov byte ptr [rsi+DC_VSCALE], al

	mov al, byte ptr [rdx].state.dc_border
	mov byte ptr [rsi+DC_BORDER], al

	ret
set_dcsel:
	mov byte ptr [rdx].state.dcsel, 1

	mov al, byte ptr [rdx].state.dc_hstart
	shr ax, 2
	mov byte ptr [rsi+DC_HSTART], al

	mov al, byte ptr [rdx].state.dc_hstop
	shr ax, 2
	mov byte ptr [rsi+DC_HSTOP], al

	mov al, byte ptr [rdx].state.dc_vstart
	shr ax, 1
	mov byte ptr [rsi+DC_VSTART], al

	mov al, byte ptr [rdx].state.dc_vstop
	shr ax, 1
	mov byte ptr [rsi+DC_VSTOP], al

	ret
vera_update_ctrl endp

vera_update_ien proc
	mov r13b, byte ptr [rsi+rbx]
	and r13b, 10001111b					; mask off unused bits
	mov byte ptr [rsi+rbx], r13b

	xor rax, rax
	bt r13, 0
	setc al
	mov byte ptr [rdx].state.interrupt_vsync, al
	
	xor rax, rax
	bt r13, 1
	setc al
	mov byte ptr [rdx].state.interrupt_line, al

	xor rax, rax
	bt r13, 2
	setc al
	mov byte ptr [rdx].state.interrupt_spcol, al

	xor rax, rax
	bt r13, 3
	setc al
	mov byte ptr [rdx].state.interrupt_aflow, al

	xor rax, rax
	bt r13, 7
	setc al
	mov byte ptr [rdx].state.interrupt_linenum + 1, al

	mov r13b, byte ptr [rdx].state.interrupt_spcol_hit
	shl r13, 1
	or r13b, byte ptr [rdx].state.interrupt_line_hit
	shl r13, 1
	or r13b, byte ptr [rdx].state.interrupt_vsync_hit

	xor rax, rax
	mov bl, byte ptr [rsi+IEN]
	and r13b, bl
	setnz al

	mov byte ptr [rdx].state.interrupt, al

	ret
vera_update_ien endp

vera_update_irqline_l proc
	mov r13b, byte ptr [rsi+rbx]
	mov byte ptr [rdx].state.interrupt_linenum, r13b

	ret
vera_update_irqline_l endp

vera_update_9f29 proc
	movzx r13, byte ptr [rsi+rbx]
	cmp byte ptr [rdx].state.dcsel, 0
	jnz dcsel_set

	and r13, 01110111b
	mov byte ptr [rsi+rbx], r13b

	xor rax, rax
	bt r13, 4
	setc al
	mov byte ptr [rdx].state.layer0_enable, al 

	xor rax, rax
	bt r13, 5
	setc al
	mov byte ptr [rdx].state.layer1_enable, al 

	xor rax, rax
	bt r13, 6
	setc al
	mov byte ptr [rdx].state.sprite_enable, al 

	ret
dcsel_set:
	shl r13, 2
	mov word ptr [rdx].state.dc_hstart, r13w

	ret
vera_update_9f29 endp

vera_update_9f2a proc
	movzx r13, byte ptr [rsi+rbx]
	cmp byte ptr [rdx].state.dcsel, 0
	jnz dcsel_set

	shl r13, 9
	mov dword ptr [rdx].state.dc_hscale, r13d
	ret
dcsel_set:
	shl r13, 2
	mov word ptr [rdx].state.dc_hstop, r13w
	ret
vera_update_9f2a endp

vera_update_9f2b proc
	movzx r13, byte ptr [rsi+rbx]
	cmp byte ptr [rdx].state.dcsel, 0
	jnz dcsel_set
	shl r13, 9
	mov dword ptr [rdx].state.dc_vscale, r13d
	ret
dcsel_set:
	shl r13, 1
	mov word ptr [rdx].state.dc_vstart, r13w
	ret
vera_update_9f2b endp

vera_update_9f2c proc
	movzx r13, byte ptr [rsi+rbx]
	cmp byte ptr [rdx].state.dcsel, 0
	jnz dcsel_set
	mov byte ptr [rdx].state.dc_border, r13b
	ret
dcsel_set:
	shl r13, 1
	mov word ptr [rdx].state.dc_vstop, r13w
	ret
vera_update_9f2c endp

;
; Layer 0 Config
;

vera_update_l0config proc
	movzx r13, byte ptr [rsi+rbx]
	lea rbx, vera_map_shift_table

	mov rax, r13
	and rax, 00000011b
	mov byte ptr [rdx].state.layer0_colourDepth, al

	mov rax, r13
	and rax, 00000100b
	shr rax, 2
	mov byte ptr [rdx].state.layer0_bitmapMode, al

	mov rax, r13
	and rax, 00110000b
	shr rax, 4
	mov byte ptr [rdx].state.layer0_mapWidth, al

	mov ax, word ptr [rbx + rax * 2]
	mov word ptr [rdx].state.layer0_map_vshift, ax

	mov rax, r13
	and rax, 11000000b
	shr rax, 6
	mov byte ptr [rdx].state.layer0_mapHeight, al

	mov ax, [rbx + rax * 2]
	mov word ptr [rdx].state.layer0_map_hshift, ax

	and r13, 00001111b
	lea rbx, layer0_render_jump
	mov rbx, qword ptr [rbx + r13 * 8]
	mov qword ptr [rdx].state.layer0_rtn, rbx

	;mov word ptr [rdx].state.layer0_config, r13w

	set_layer0_jump

	ret
vera_update_l0config endp

vera_update_l0mapbase proc
	movzx r13, byte ptr [rsi+rbx]

	shl r13, 9
	mov dword ptr [rdx].state.layer0_mapAddress, r13d

	ret
vera_update_l0mapbase endp

vera_update_l0tilebase proc
	movzx r13, byte ptr [rsi+rbx]
	lea rbx, vera_tile_shift_table

	mov rax, r13
	and rax, 00000001b
	mov byte ptr [rdx].state.layer0_tileWidth, al

	mov ax, word ptr [rbx + rax * 2]
	mov word ptr [rdx].state.layer0_tile_vshift, ax

	mov rax, r13
	and rax, 00000010b
	shr rax, 1
	mov byte ptr [rdx].state.layer0_tileHeight, al

	mov ax, word ptr [rbx + rax * 2]
	mov word ptr [rdx].state.layer0_tile_hshift, ax

	and r13, 11111100b
	shl r13, 9											; not 11, as we're shifted by 2 bits already
	mov dword ptr [rdx].state.layer0_tileAddress, r13d

	set_layer0_jump

	ret
vera_update_l0tilebase endp

vera_update_l0hscroll_l proc
	mov r13b, byte ptr [rsi+rbx]
	mov byte ptr [rdx].state.layer0_hscroll, r13b

	ret
vera_update_l0hscroll_l endp

vera_update_l0hscroll_h proc
	mov r13b, byte ptr [rsi+rbx]
	and r13b, 0fh
	mov byte ptr [rsi+rbx], r13b
	mov byte ptr [rdx].state.layer0_hscroll + 1, r13b

	ret
vera_update_l0hscroll_h endp

vera_update_l0vscroll_l proc
	mov r13b, byte ptr [rsi+rbx]
	mov byte ptr [rdx].state.layer0_vscroll, r13b

	ret
vera_update_l0vscroll_l endp

vera_update_l0vscroll_h proc
	mov r13b, byte ptr [rsi+rbx]
	and r13b, 0fh
	mov byte ptr [rsi+rbx], r13b
	mov byte ptr [rdx].state.layer0_vscroll + 1, r13b

	ret
vera_update_l0vscroll_h endp
;
; Layer 1 Config
;

vera_update_l1config proc
	movzx r13, byte ptr [rsi+rbx]
	lea rbx, vera_map_shift_table

	mov rax, r13
	and rax, 00000011b
	mov byte ptr [rdx].state.layer1_colourDepth, al

	mov rax, r13
	and rax, 00000100b
	shr rax, 2
	mov byte ptr [rdx].state.layer1_bitmapMode, al

	mov rax, r13
	and rax, 00110000b
	shr rax, 4
	mov byte ptr [rdx].state.layer1_mapWidth, al
	mov ax, [rbx + rax * 2]
	mov word ptr [rdx].state.layer1_map_vshift, ax

	mov rax, r13
	and rax, 11000000b
	shr rax, 6
	mov byte ptr [rdx].state.layer1_mapHeight, al
	
	mov ax, [rbx + rax * 2]
	mov word ptr [rdx].state.layer1_map_hshift, ax

	and r13, 00001111b
	lea rbx, layer1_render_jump
	mov rbx, qword ptr [rbx + r13 * 8]
	mov qword ptr [rdx].state.layer1_rtn, rbx

	;mov word ptr [rdx].state.layer1_config, r13w

	set_layer1_jump
	
	ret
vera_update_l1config endp

vera_update_l1mapbase proc
	movzx r13, byte ptr [rsi+rbx]

	shl r13, 9
	mov dword ptr [rdx].state.layer1_mapAddress, r13d

	ret
vera_update_l1mapbase endp

vera_update_l1tilebase proc
	movzx r13, byte ptr [rsi+rbx]
	lea rbx, vera_tile_shift_table

	mov rax, r13
	and rax, 00000001b
	mov byte ptr [rdx].state.layer1_tileWidth, al

	mov ax, word ptr [rbx + rax * 2]
	mov word ptr [rdx].state.layer1_tile_vshift, ax


	mov rax, r13
	and rax, 00000010b
	shr rax, 1
	mov byte ptr [rdx].state.layer1_tileHeight, al

	mov ax, word ptr [rbx + rax * 2]
	mov word ptr [rdx].state.layer1_tile_hshift, ax


	and r13, 11111100b
	shl r13, 9											; not 11, as we're shifted by 2 bits already
	mov dword ptr [rdx].state.layer1_tileAddress, r13d

	set_layer1_jump

	ret
vera_update_l1tilebase endp

vera_update_l1hscroll_l proc
	mov r13b, byte ptr [rsi+rbx]
	mov byte ptr [rdx].state.layer1_hscroll, r13b

	ret
vera_update_l1hscroll_l endp

vera_update_l1hscroll_h proc
	mov r13b, byte ptr [rsi+rbx]
	and r13b, 0fh
	mov byte ptr [rsi+rbx], r13b
	mov byte ptr [rdx].state.layer1_hscroll + 1, r13b

	ret
vera_update_l1hscroll_h endp

vera_update_l1vscroll_l proc
	mov r13b, byte ptr [rsi+rbx]
	mov byte ptr [rdx].state.layer1_vscroll, r13b

	ret
vera_update_l1vscroll_l endp

vera_update_l1vscroll_h proc
	mov r13b, byte ptr [rsi+rbx]
	and r13b, 0fh
	mov byte ptr [rsi+rbx], r13b
	mov byte ptr [rdx].state.layer1_vscroll + 1, r13b

	ret
vera_update_l1vscroll_h endp

; Todo: reconstruct ISR
vera_update_isr proc
	mov r13b, byte ptr [rsi+rbx]

	bt r13, 0
	jnc check_line

	mov [rdx].state.interrupt_vsync_hit, 0

check_line:
	bt r13, 1
	jnc check_spcol

	mov [rdx].state.interrupt_line_hit, 0

check_spcol:
	bt r13, 2
	jnc construct_isr

	mov [rdx].state.interrupt_spcol_hit, 0

construct_isr:
	mov r13b, byte ptr [rdx].state.interrupt_spcol_hit
	shl r13, 1
	or r13b, byte ptr [rdx].state.interrupt_line_hit
	shl r13, 1
	or r13b, byte ptr [rdx].state.interrupt_vsync_hit
	mov byte ptr [rsi+rbx], r13b

	xor rax, rax
	mov bl, byte ptr [rsi+IEN]
	and r13b, bl

	setnz al

	mov byte ptr [rdx].state.interrupt, al

	ret
vera_update_isr endp

.data

vera_step_table:
	dw 0, 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 40, 80, 160, 320, 640

vera_tile_shift_table:
	dw 3, 4
vera_map_shift_table:
	dw 5, 6, 7, 8

.code
