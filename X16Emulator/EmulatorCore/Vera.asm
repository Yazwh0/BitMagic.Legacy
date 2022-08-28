.code
; ----------------------------------------------------
; VERA
; ----------------------------------------------------


; Cpu emulation:
; rax  : scratch
; rbx  : scratch
; rcx  : current memory context
; rdx  : state object 
; rsi  : scratch
; rdi  : scratch
; r8b  : a
; r9b  : x
; r10b : y
; r11w : PC
; r12  : scratch
; r13  : scratch / use to indicate vera data0 or 1 read
; r14  : Clock Ticks
; r15  : Flags

include vera_constants.inc
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
	mov rcx, vera_step_table

search_loop:
	cmp ax, word ptr [rcx+rbx]
	je match
	add rbx, 2
	cmp rbx, 20h
	jne search_loop

match:
	and rbx, 11110b						; mask off index step (nof 0x0t as we x2 this value earlier)
	shl rbx, 4+8+8-1					; shift to the correct position for the registers
	mov rsi, [rdx].state.data0_address
	or rsi, rbx
	
	mov rcx, [rdx].state.memory_ptr
	mov [rcx+ADDRx_L], si

	shr rsi, 16
	or sil, r12b						; or on the DECR bit
	mov [rcx+ADDRx_H], sil
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
	mov rcx, vera_step_table

search_loop:
	cmp ax, word ptr [rcx+rbx]
	je match
	add rbx, 2
	cmp rbx, 20h
	jne search_loop

match:
	and rbx, 11110b
	shl rbx, 4+8+8-1
	mov rsi, [rdx].state.data1_address
	or rsi, rbx
	
	mov rcx, [rdx].state.memory_ptr
	mov [rcx+ADDRx_L], si

	shr rsi, 16
	or sil, r12b						; or on the DECR bit
	mov [rcx+ADDRx_H], sil
endm

; initialise colours etc
; rdx points to cpu state
vera_init proc
	
	;
	; DATA0\1
	;
	mov rcx, [rdx].state.memory_ptr
	mov rsi, [rdx].state.vram_ptr
	
	mov rax, [rdx].state.data0_address
	mov bl, byte ptr [rsi+rax]
	mov byte ptr [rcx+DATA0], bl

	mov rax, [rdx].state.data1_address
	mov bl, byte ptr [rsi+rax]
	mov byte ptr [rcx+DATA1], bl

	;
	; AddrSel CTRL + ADDR_x
	;
	cmp [rdx].state.addrsel, 0
	jne set_address1

	; Set Address 0 - init to ctrl as 0
	vera_setaddress_0
	mov byte ptr [rcx+CTRL], 0
	jmp addr_done

set_address1:
	vera_setaddress_1
	mov byte ptr [rcx+CTRL], 1
	
addr_done:

	;
	; DcSel CTRL
	;
	mov r13b, [rdx].state.dcsel
	shl r13b, 1
	or byte ptr [rcx+CTRL], r13b

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
	mov byte ptr [rcx+DC_VIDEO], al

	mov al, byte ptr [rdx].state.dc_hscale
	mov byte ptr [rcx+DC_HSCALE], al

	mov al, byte ptr [rdx].state.dc_vscale
	mov byte ptr [rcx+DC_VSCALE], al

	mov al, byte ptr [rdx].state.dc_border
	mov byte ptr [rcx+DC_BORDER], al

	jmp dc_done

set_dc1:

	mov al, byte ptr [rdx].state.dc_hstart
	shr ax, 2
	mov byte ptr [rcx+DC_HSTART], al

	mov al, byte ptr [rdx].state.dc_hstop
	shr ax, 2
	mov byte ptr [rcx+DC_HSTOP], al

	mov al, byte ptr [rdx].state.dc_vstart
	shr ax, 1
	mov byte ptr [rcx+DC_VSTART], al

	mov al, byte ptr [rdx].state.dc_vstop
	shr ax, 1
	mov byte ptr [rcx+DC_VSTOP], al

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

	mov byte ptr [rcx+L0_CONFIG], al

	; Map Base Address
	mov eax, dword ptr [rdx].state.layer0_mapAddress
	shr rax, 9
	mov byte ptr [rcx+L0_MAPBASE], al

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
	mov byte ptr [rcx+L0_TILEBASE], al

	; HScroll
	mov ax, word ptr [rdx].state.layer0_hscroll
	and ax, 0fffh
	mov byte ptr [rcx+L0_HSCROLL_L], al
	mov byte ptr [rcx+L0_HSCROLL_H], ah

	; VScroll
	mov ax, word ptr [rdx].state.layer0_vscroll
	and ax, 0fffh
	mov byte ptr [rcx+L0_VSCROLL_L], al
	mov byte ptr [rcx+L0_VSCROLL_H], ah

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

	mov byte ptr [rcx+L1_CONFIG], al

	; Map Base Address
	mov eax, dword ptr [rdx].state.layer1_mapAddress
	shr rax, 9
	mov byte ptr [rcx+L1_MAPBASE], al

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
	mov byte ptr [rcx+L1_TILEBASE], al

	; HScroll
	mov ax, word ptr [rdx].state.layer1_hscroll
	and ax, 0fffh
	mov byte ptr [rcx+L1_HSCROLL_L], al
	mov byte ptr [rcx+L1_HSCROLL_H], ah

	; VScroll
	mov ax, word ptr [rdx].state.layer1_vscroll
	and ax, 0fffh
	mov byte ptr [rcx+L1_VSCROLL_L], al
	mov byte ptr [rcx+L1_VSCROLL_H], ah

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

	mov byte ptr [rcx+IRQLINE_L], bl

	and bx, 100h
	shr bx, 1
	or al, bl
	mov byte ptr [rcx+IEN], al

	ret
vera_init endp

; rbx			address
; [rcx+rbx]		output location in main memory
; 
; should only be called if data0\data1 is read\written.

vera_dataaccess_body macro doublestep, write_value

	mov rax, [rdx].state.vram_ptr				; get value from vram

	cmp rbx, DATA0
	jne step_data1

	mov rsi, [rdx].state.data0_address

	if write_value eq 1 and doublestep eq 0
		mov r13b, byte ptr [rcx+rbx]			; get value that has been written
		mov byte ptr [rax+rsi], r13b			; store in vram
	endif

	add rsi, [rdx].state.data0_step
	and rsi, 1ffffh								; mask off high bits so we wrap

	if doublestep eq 1
		if write_value eq 1
			mov r13b, byte ptr [rcx+rbx]			; get value that has been written
			mov byte ptr [rax+rsi], r13b			; store in vram
		endif

		add rsi, [rdx].state.data0_step			; perform second step
		and rsi, 1ffffh							; mask off high bits so we wrap
	endif

	mov [rdx].state.data0_address, rsi

	mov r13b, byte ptr [rax+rsi]
	mov [rcx+rbx], r13b						; store in ram

	xor r13, r13								; clear r13b, as we use this to detect if we need to call vera

	cmp [rdx].state.addrsel, 0
	je set_data0_address
	ret

set_data0_address:
		
	mov rcx, [rdx].state.memory_ptr
	mov [rcx+ADDRx_L], si

	shr rsi, 16
	mov al, [rcx+ADDRx_H]						; Add on stepping nibble
	and al, 0f8h								; mask off what isnt changable
	or sil, al
	mov [rcx+ADDRx_H], sil

	xor r13, r13								; clear r13b, as we use this to detect if we need to call vera
	ret

step_data1:
	mov rsi, [rdx].state.data1_address

	if write_value eq 1 and doublestep eq 0
		mov r13b, byte ptr [rcx+rbx]			; get value that has been written
		mov byte ptr [rax+rsi], r13b			; store in vram
	endif

	add rsi, [rdx].state.data1_step
	and rsi, 1ffffh								; mask off high bits so we wrap

	if doublestep eq 1
		if write_value eq 1
			mov r13b, byte ptr [rcx+rbx]		; get value that has been written
			mov byte ptr [rax+rsi], r13b		; store in vram
		endif

		add rsi, [rdx].state.data1_step
		and rsi, 1ffffh							; mask off high bits so we wrap
	endif

	mov [rdx].state.data1_address, rsi

	mov r13b, byte ptr [rax+rsi]
	mov [rcx+rbx], r13b							; store in ram

	xor r13, r13								; clear r13b, as we use this to detect if we need to call vera

	cmp [rdx].state.addrsel, 1
	je set_data1_address
	ret

set_data1_address:
		
	mov rcx, [rdx].state.memory_ptr
	mov [rcx+ADDRx_L], si

	shr rsi, 16
	mov al, [rcx+ADDRx_H]						; Add on stepping nibble
	and al, 0f8h								; mask off what isnt changable
	or sil, al
	mov [rcx+ADDRx_H], sil

	ret
endm

vera_afterread proc
	dec r13
	vera_dataaccess_body 0, 0
vera_afterread endp

; eg inc, asl
vera_afterreadwrite proc
	dec r13
	vera_dataaccess_body 1, 1
vera_afterreadwrite endp

vera_afterwrite proc
	dec r13
	lea rax, vera_registers
	jmp qword ptr [rax + r13 * 8]
	;vera_dataaccess_body 0, 1
vera_afterwrite endp

vera_update_notimplemented proc
	ret
vera_update_notimplemented endp

vera_update_data proc
	vera_dataaccess_body 0, 1
vera_update_data endp

vera_update_addrl proc	
	mov r13b, byte ptr [rcx+rbx]
	cmp byte ptr [rdx].state.addrsel, 0

	jnz write_data1
	mov byte ptr [rdx].state.data0_address, r13b
	ret
write_data1:
	mov byte ptr [rdx].state.data1_address, r13b
	ret
vera_update_addrl endp

vera_update_addrm proc	
	mov r13b, byte ptr [rcx+rbx]
	cmp byte ptr [rdx].state.addrsel, 0

	jnz write_data1
	mov byte ptr [rdx].state.data0_address + 1, r13b
	ret
write_data1:
	mov byte ptr [rdx].state.data1_address + 1, r13b
	ret
vera_update_addrm endp

vera_update_addrh proc	
	mov r13b, byte ptr [rcx+rbx]						; value that has been written
	and r13, 11111001b
	mov byte ptr [rcx+rbx], r13b						; write back masked value
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
	mov r13b, byte ptr [rcx+rbx]						; value that has been written
	and r13b, 00000011b
	mov byte ptr [rcx+rbx], r13b

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
	mov byte ptr [rcx+DC_VIDEO], al

	mov al, byte ptr [rdx].state.dc_hscale
	mov byte ptr [rcx+DC_HSCALE], al

	mov al, byte ptr [rdx].state.dc_vscale
	mov byte ptr [rcx+DC_VSCALE], al

	mov al, byte ptr [rdx].state.dc_border
	mov byte ptr [rcx+DC_BORDER], al

	ret
set_dcsel:
	mov byte ptr [rdx].state.dcsel, 1

	mov al, byte ptr [rdx].state.dc_hstart
	shr ax, 2
	mov byte ptr [rcx+DC_HSTART], al

	mov al, byte ptr [rdx].state.dc_hstop
	shr ax, 2
	mov byte ptr [rcx+DC_HSTOP], al

	mov al, byte ptr [rdx].state.dc_vstart
	shr ax, 1
	mov byte ptr [rcx+DC_VSTART], al

	mov al, byte ptr [rdx].state.dc_vstop
	shr ax, 1
	mov byte ptr [rcx+DC_VSTOP], al

	ret
vera_update_ctrl endp

vera_update_ien proc
	mov r13b, byte ptr [rcx+rbx]
	and r13b, 10001111b					; mask off unused bits
	mov byte ptr [rcx+rbx], r13b

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

	ret
vera_update_ien endp

vera_update_irqline_l proc
	mov r13b, byte ptr [rcx+rbx]
	mov byte ptr [rdx].state.interrupt_linenum, r13b

	ret
vera_update_irqline_l endp

vera_update_9f29 proc
	movzx r13, byte ptr [rcx+rbx]
	cmp byte ptr [rdx].state.dcsel, 0
	jnz dcsel_set

	and r13, 01110111b
	mov byte ptr [rcx+rbx], r13b

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
	movzx r13, byte ptr [rcx+rbx]
	cmp byte ptr [rdx].state.dcsel, 0
	jnz dcsel_set
	mov byte ptr [rdx].state.dc_hscale, r13b
	ret
dcsel_set:
	shl r13, 2
	mov word ptr [rdx].state.dc_hstop, r13w
	ret
vera_update_9f2a endp

vera_update_9f2b proc
	movzx r13, byte ptr [rcx+rbx]
	cmp byte ptr [rdx].state.dcsel, 0
	jnz dcsel_set
	mov byte ptr [rdx].state.dc_vscale, r13b
	ret
dcsel_set:
	shl r13, 1
	mov word ptr [rdx].state.dc_vstart, r13w
	ret
vera_update_9f2b endp

vera_update_9f2c proc
	movzx r13, byte ptr [rcx+rbx]
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
	movzx r13, byte ptr [rcx+rbx]

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

	mov rax, r13
	and rax, 11000000b
	shr rax, 6
	mov byte ptr [rdx].state.layer0_mapHeight, al

	ret
vera_update_l0config endp

vera_update_l0mapbase proc
	movzx r13, byte ptr [rcx+rbx]

	shl r13, 9
	mov dword ptr [rdx].state.layer0_mapAddress, r13d

	ret
vera_update_l0mapbase endp

vera_update_l0tilebase proc
	movzx r13, byte ptr [rcx+rbx]

	mov rax, r13
	and rax, 00000001b
	mov byte ptr [rdx].state.layer0_tileWidth, al

	mov rax, r13
	and rax, 00000010b
	shr rax, 1
	mov byte ptr [rdx].state.layer0_tileHeight, al

	and r13, 11111100b
	shl r13, 9											; not 11, as we're shifted by 2 bits already
	mov dword ptr [rdx].state.layer0_tileAddress, r13d

	ret
vera_update_l0tilebase endp

vera_update_l0hscroll_l proc
	mov r13b, byte ptr [rcx+rbx]
	mov byte ptr [rdx].state.layer0_hscroll, r13b

	ret
vera_update_l0hscroll_l endp

vera_update_l0hscroll_h proc
	mov r13b, byte ptr [rcx+rbx]
	and r13b, 0fh
	mov byte ptr [rcx+rbx], r13b
	mov byte ptr [rdx].state.layer0_hscroll + 1, r13b

	ret
vera_update_l0hscroll_h endp

vera_update_l0vscroll_l proc
	mov r13b, byte ptr [rcx+rbx]
	mov byte ptr [rdx].state.layer0_vscroll, r13b

	ret
vera_update_l0vscroll_l endp

vera_update_l0vscroll_h proc
	mov r13b, byte ptr [rcx+rbx]
	and r13b, 0fh
	mov byte ptr [rcx+rbx], r13b
	mov byte ptr [rdx].state.layer0_vscroll + 1, r13b

	ret
vera_update_l0vscroll_h endp
;
; Layer 1 Config
;

vera_update_l1config proc
	movzx r13, byte ptr [rcx+rbx]

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

	mov rax, r13
	and rax, 11000000b
	shr rax, 6
	mov byte ptr [rdx].state.layer1_mapHeight, al

	ret
vera_update_l1config endp

vera_update_l1mapbase proc
	movzx r13, byte ptr [rcx+rbx]

	shl r13, 9
	mov dword ptr [rdx].state.layer1_mapAddress, r13d

	ret
vera_update_l1mapbase endp

vera_update_l1tilebase proc
	movzx r13, byte ptr [rcx+rbx]

	mov rax, r13
	and rax, 00000001b
	mov byte ptr [rdx].state.layer1_tileWidth, al

	mov rax, r13
	and rax, 00000010b
	shr rax, 1
	mov byte ptr [rdx].state.layer1_tileHeight, al

	and r13, 11111100b
	shl r13, 9											; not 11, as we're shifted by 2 bits already
	mov dword ptr [rdx].state.layer1_tileAddress, r13d

	ret
vera_update_l1tilebase endp

vera_update_l1hscroll_l proc
	mov r13b, byte ptr [rcx+rbx]
	mov byte ptr [rdx].state.layer1_hscroll, r13b

	ret
vera_update_l1hscroll_l endp

vera_update_l1hscroll_h proc
	mov r13b, byte ptr [rcx+rbx]
	and r13b, 0fh
	mov byte ptr [rcx+rbx], r13b
	mov byte ptr [rdx].state.layer1_hscroll + 1, r13b

	ret
vera_update_l1hscroll_h endp

vera_update_l1vscroll_l proc
	mov r13b, byte ptr [rcx+rbx]
	mov byte ptr [rdx].state.layer1_vscroll, r13b

	ret
vera_update_l1vscroll_l endp

vera_update_l1vscroll_h proc
	mov r13b, byte ptr [rcx+rbx]
	and r13b, 0fh
	mov byte ptr [rcx+rbx], r13b
	mov byte ptr [rdx].state.layer1_vscroll + 1, r13b

	ret
vera_update_l1vscroll_h endp

vera_registers:
	vera_9f20 qword vera_update_addrl
	vera_9f21 qword vera_update_addrm
	vera_9f22 qword vera_update_addrh
	vera_9f23 qword vera_update_data
	vera_9f24 qword vera_update_data
	vera_9f25 qword vera_update_ctrl
	vera_9f26 qword vera_update_ien
	vera_9f27 qword vera_update_notimplemented
	vera_9f28 qword vera_update_irqline_l
	vera_9f29 qword vera_update_9f29
	vera_9f2a qword vera_update_9f2a
	vera_9f2b qword vera_update_9f2b
	vera_9f2c qword vera_update_9f2c
	vera_9f2d qword vera_update_l0config
	vera_9f2e qword vera_update_l0mapbase
	vera_9f2f qword vera_update_l0tilebase
	vera_9f30 qword vera_update_l0hscroll_l
	vera_9f31 qword vera_update_l0hscroll_h
	vera_9f32 qword vera_update_l0vscroll_l
	vera_9f33 qword vera_update_l0vscroll_h
	vera_9f34 qword vera_update_l1config
	vera_9f35 qword vera_update_l1mapbase
	vera_9f36 qword vera_update_l1tilebase
	vera_9f37 qword vera_update_l1hscroll_l
	vera_9f38 qword vera_update_l1hscroll_h
	vera_9f39 qword vera_update_l1vscroll_l
	vera_9f3a qword vera_update_l1vscroll_h
	vera_9f3b qword vera_update_notimplemented
	vera_9f3c qword vera_update_notimplemented
	vera_9f3d qword vera_update_notimplemented
	vera_9f3e qword vera_update_notimplemented
	vera_9f3f qword vera_update_notimplemented

;include Vera_Display.asm

.data

vera_step_table:
	dw 0, 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 40, 80, 160, 320, 640

.code
