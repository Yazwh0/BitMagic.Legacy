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

SPI_PARAMETER_ERROR equ 01000000b
SPI_ADDRESS_ERROR	equ 00100000b
SPI_ERASE_SEQ_ERROR equ 00010000b
SPI_CRC_ERROR		equ 00001000b
SPI_ILLEGAL_COMMAND equ 00000100b
SPI_ERASE_RESET		equ 00000010b
SPI_IDLE_STATE		equ 00000001b

SPI_SELECT		equ 01b
SPI_SLOWCLOCK	equ 10b

SPI_DEFAULT_RESPONSE equ 0ffh

spi_log_fromsdcard macro
	;mov eax, dword ptr [rdx].state.spi_position
	;mov rbx, qword ptr [rdx].state.spi_history_ptr
	;mov byte ptr [rbx + rax+1], 1		; input (from sdcard)
	;mov byte ptr [rbx + rax], r13b
	;add eax, 2
	;and eax, 2048-1
	;mov dword ptr [rdx].state.spi_position, eax
endm

vera_update_spi_data proc
	movzx r13, byte ptr [rsi+SPI_DATA]

	; Logging
	;mov eax, dword ptr [rdx].state.spi_position
	;mov rbx, qword ptr [rdx].state.spi_history_ptr
	;mov byte ptr [rbx + rax+1], 0		; output (to sdcard)
	;mov byte ptr [rbx + rax], r13b
	;add eax, 2
	;and eax, 2048-1
	;mov dword ptr [rdx].state.spi_position, eax

	; no matter what, if there is no sdcard then we dont reply
	mov rax, qword ptr [rdx].state.sdcard_ptr
	test rax, rax
	jz dont_change_message

	; if select is 0 then ignore completely.
	mov eax, dword ptr [rdx].state.spi_chipselect
	test eax, eax
	jz dont_change_message

	; ignore all 0xff's if we have an empty receive buffer

	; do we have data?
	mov ebx, dword ptr [rdx].state.spi_receivecount
	test rbx, rbx
	jnz receiving_command

	; if no inbounddata, if 0xff then see if there is outbound data
	cmp r13b, 0ffh
	je spi_send_data

receiving_command:
	; shift and or on the new data, then check
	mov rax, qword ptr [rdx].state.spi_inbound_buffer_ptr
	mov byte ptr [rax + rbx], r13b

	inc ebx
	mov dword ptr [rdx].state.spi_receivecount, ebx

	mov r13d, dword ptr [rdx].state.spi_writeblock
	test r13d, r13d
	jnz writing_block

	; check command
	cmp ebx, 6
	je spi_handle_command

default_message:
	mov r13b, SPI_DEFAULT_RESPONSE
	mov byte ptr [rsi+SPI_DATA], r13b
	mov dword ptr [rdx].state.spi_previousvalue, r13d

	spi_log_fromsdcard

	ret

dont_change_message:
	mov r13d, dword ptr [rdx].state.spi_previousvalue
	mov byte ptr [rsi+SPI_DATA], r13b
	
	spi_log_fromsdcard

	ret

writing_block:
	cmp ebx, 515
	je spi_write_complete

	mov r13b, SPI_DEFAULT_RESPONSE
	mov byte ptr [rsi+SPI_DATA], r13b
	mov dword ptr [rdx].state.spi_previousvalue, r13d

	spi_log_fromsdcard

	ret
vera_update_spi_data endp

spi_do_nothing proc
	mov r13b, SPI_DEFAULT_RESPONSE
	mov byte ptr [rsi+SPI_DATA], r13b
	mov dword ptr [rdx].state.spi_previousvalue, r13d

	spi_log_fromsdcard

	ret
spi_do_nothing endp

spi_not_known proc
	mov r13b, SPI_DEFAULT_RESPONSE
	mov byte ptr [rsi+SPI_DATA], r13b
	mov dword ptr [rdx].state.spi_previousvalue, r13d

	spi_log_fromsdcard

	ret
spi_not_known endp

spi_send_data proc
	mov ebx, dword ptr [rdx].state.spi_sendlength
	test rbx, rbx
	jz spi_do_nothing

	; put data into reply
	mov ebx, dword ptr [rdx].state.spi_sendcount
	mov rax, qword ptr [rdx].state.spi_outbound_buffer_ptr
	movzx r13, byte ptr [rax + rbx]
	mov byte ptr [rsi+SPI_DATA], r13b
	mov dword ptr [rdx].state.spi_previousvalue, r13d

	inc rbx

	mov r13d, dword ptr [rdx].state.spi_sendlength


	xor rax, rax
	cmp r13d, ebx									; are we done?

	cmove rbx, rax									; if done clear sendlength and sendcount
	cmove r13, rax									

	mov dword ptr [rdx].state.spi_sendcount, ebx
	mov dword ptr [rdx].state.spi_sendlength, r13d


	movzx r13, byte ptr [rsi+SPI_DATA] ; for logging

	spi_log_fromsdcard

	ret
spi_send_data endp

spi_handle_command proc
	mov r13b, SPI_DEFAULT_RESPONSE
	mov byte ptr [rsi+SPI_DATA], r13b
	mov dword ptr [rdx].state.spi_previousvalue, r13d

	; clear receive count, so it can start again
	mov dword ptr [rdx].state.spi_receivecount, 0

	movzx rbx, byte ptr [rax] ; get command
	and rbx, 00111111b
	mov dword ptr [rdx].state.spi_previouscommand, ebx
	lea rdi, spi_command_table
	jmp qword ptr [rdi + rbx * 8]
spi_handle_command endp

spi_go_idle proc
	; place reset state into transmit buffer
	mov rax, qword ptr [rdx].state.spi_outbound_buffer_ptr
	mov byte ptr [rax], SPI_IDLE_STATE
	xor rax, rax
	mov dword ptr [rdx].state.spi_sendcount, eax
	inc eax
	mov dword ptr [rdx].state.spi_sendlength, eax
	mov dword ptr [rdx].state.spi_idle, eax

	spi_log_fromsdcard

	ret
spi_go_idle endp

spi_cmd8 proc
	; place reset state into transmit buffer
	mov rax, qword ptr [rdx].state.spi_outbound_buffer_ptr
	; magic values, for voltage etc, taken from the official emulator
	mov dword ptr [rax], 01000001h
	mov dword ptr [rax + 4], 0aah
	mov dword ptr [rdx].state.spi_sendcount, 0
	mov dword ptr [rdx].state.spi_sendlength, 5

	spi_log_fromsdcard

	ret
spi_cmd8 endp

spi_cmd55 proc	
	mov dword ptr [rdx].state.spi_commandnext, 1	; application specific command

	mov rax, qword ptr [rdx].state.spi_outbound_buffer_ptr
	mov ebx, dword ptr [rdx].state.spi_idle
	mov byte ptr [rax], bl
	mov dword ptr [rdx].state.spi_sendcount, 0
	mov dword ptr [rdx].state.spi_sendlength, 1

	spi_log_fromsdcard
	ret
spi_cmd55 endp

spi_acmd41 proc
	mov dword ptr [rdx].state.spi_commandnext, 1	; application specific command

	mov rax, qword ptr [rdx].state.spi_outbound_buffer_ptr
	mov dword ptr [rdx].state.spi_idle, 0			; no longer idle
	mov byte ptr [rax], 0
	mov dword ptr [rdx].state.spi_sendcount, 0
	mov dword ptr [rdx].state.spi_sendlength, 1
	mov dword ptr [rdx].state.spi_initialised, 1	; now initilised

	spi_log_fromsdcard

	ret
spi_acmd41 endp

spi_read_single_block proc
	; get requested block number
	mov rax, qword ptr [rdx].state.spi_inbound_buffer_ptr ; need to get the parameter

	xor rbx, rbx
	mov bl, byte ptr [rax + 1]
	shl rbx, 8
	mov bl, byte ptr [rax + 2]
	shl rbx, 8
	mov bl, byte ptr [rax + 3]
	shl rbx, 8
	mov bl, byte ptr [rax + 4]
	; ebx now contains the block number
	shl rbx, 9	; * 512 to get offset of the sd card

	; fill outbound buffer
	mov rax, qword ptr [rdx].state.spi_outbound_buffer_ptr
	mov rdi, qword ptr [rdx].state.sdcard_ptr

	; need to copy 512 bytes from sdcard rdi + rbx, to rax + 2
	; sd card is alligned, buffer isn't

	vmovdqa ymm0, ymmword ptr [rdi + rbx + 00h]
	vmovdqu ymmword ptr [rax + 2 + 00h], ymm0
	vmovdqa ymm1, ymmword ptr [rdi + rbx + 20h]
	vmovdqu ymmword ptr [rax + 2 + 20h], ymm1
	vmovdqa ymm2, ymmword ptr [rdi + rbx + 40h]
	vmovdqu ymmword ptr [rax + 2 + 40h], ymm2
	vmovdqa ymm3, ymmword ptr [rdi + rbx + 60h]
	vmovdqu ymmword ptr [rax + 2 + 60h], ymm3

	vmovdqa ymm4, ymmword ptr [rdi + rbx + 080h]
	vmovdqu ymmword ptr [rax + 2 + 080h], ymm4
	vmovdqa ymm5, ymmword ptr [rdi + rbx + 0a0h]
	vmovdqu ymmword ptr [rax + 2 + 0a0h], ymm5
	vmovdqa ymm6, ymmword ptr [rdi + rbx + 0c0h]
	vmovdqu ymmword ptr [rax + 2 + 0c0h], ymm6
	vmovdqa ymm7, ymmword ptr [rdi + rbx + 0e0h]
	vmovdqu ymmword ptr [rax + 2 + 0e0h], ymm7
	
	vmovdqa ymm8, ymmword ptr [rdi + rbx + 100h]
	vmovdqu ymmword ptr [rax + 2 + 100h], ymm8
	vmovdqa ymm9, ymmword ptr [rdi + rbx + 120h]
	vmovdqu ymmword ptr [rax + 2 + 120h], ymm9
	vmovdqa ymm10, ymmword ptr [rdi + rbx + 140h]
	vmovdqu ymmword ptr [rax + 2 + 140h], ymm10
	vmovdqa ymm11, ymmword ptr [rdi + rbx + 160h]
	vmovdqu ymmword ptr [rax + 2 + 160h], ymm11

	vmovdqa ymm12, ymmword ptr [rdi + rbx + 180h]
	vmovdqu ymmword ptr [rax + 2 + 180h], ymm12
	vmovdqa ymm13, ymmword ptr [rdi + rbx + 1a0h]
	vmovdqu ymmword ptr [rax + 2 + 1a0h], ymm13
	vmovdqa ymm14, ymmword ptr [rdi + rbx + 1c0h]
	vmovdqu ymmword ptr [rax + 2 + 1c0h], ymm14
	vmovdqa ymm15, ymmword ptr [rdi + rbx + 1e0h]
	vmovdqu ymmword ptr [rax + 2 + 1e0h], ymm15

	; header
	mov byte ptr [rax], 0
	mov byte ptr [rax+1], 0feh
	mov byte ptr [rax+512+2], 0			; crc bytes
	mov byte ptr [rax+512+2+1], 0

	mov dword ptr [rdx].state.spi_sendcount, 0
	mov dword ptr [rdx].state.spi_sendlength, 512 + 2 + 2

	spi_log_fromsdcard

	ret
spi_read_single_block endp

spi_write_single_block proc
	; get requested block number
	mov rax, qword ptr [rdx].state.spi_inbound_buffer_ptr ; need to get the parameter

	xor rbx, rbx
	mov bl, byte ptr [rax + 1]
	shl rbx, 8
	mov bl, byte ptr [rax + 2]
	shl rbx, 8
	mov bl, byte ptr [rax + 3]
	shl rbx, 8
	mov bl, byte ptr [rax + 4]
	; ebx now contains the block number
	shl rbx, 9	; * 512 to get offset of the sd card

	or rbx, 1	; set low bit so we know we're receiving. will remove before writing to sd card.

	mov dword ptr [rdx].state.spi_writeblock, ebx

	xor r13, r13
	mov byte ptr [rax], r13b
	mov dword ptr [rdx].state.spi_sendcount, r13d
	mov dword ptr [rdx].state.spi_sendlength, 1

	spi_log_fromsdcard

	ret
spi_write_single_block endp

spi_write_complete proc
	; check start block byte
	cmp byte ptr [rax], 0feh
	jne bad_data

	mov ebx, dword ptr [rdx].state.spi_previouscommand
	cmp ebx, 24
	jne bad_data

	; rax is the inbound data buffer
	mov rdi, qword ptr [rdx].state.sdcard_ptr
	mov ebx, dword ptr [rdx].state.spi_writeblock
	xor ebx, 1 ; remove the bit we set earlier
	
	; copying from buffer to sdcard
	; sd card is aligned, buffer isn't
	vmovdqu ymm0, ymmword ptr [rax + 1 + 00h]
	vmovdqa ymmword ptr [rdi + rbx + 00h], ymm0
	vmovdqu ymm1, ymmword ptr [rax + 1 + 20h]
	vmovdqa ymmword ptr [rdi + rbx + 20h], ymm1
	vmovdqu ymm2, ymmword ptr [rax + 1 + 40h]
	vmovdqa ymmword ptr [rdi + rbx + 40h], ymm2
	vmovdqu ymm3, ymmword ptr [rax + 1 + 60h]
	vmovdqa ymmword ptr [rdi + rbx + 60h], ymm3

	vmovdqu ymm4, ymmword ptr [rax + 1 + 080h]
	vmovdqa ymmword ptr [rdi + rbx + 080h], ymm4
	vmovdqu ymm5, ymmword ptr [rax + 1 + 0a0h]
	vmovdqa ymmword ptr [rdi + rbx + 0a0h], ymm5
	vmovdqu ymm6, ymmword ptr [rax + 1 + 0c0h]
	vmovdqa ymmword ptr [rdi + rbx + 0c0h], ymm6
	vmovdqu ymm7, ymmword ptr [rax + 1 + 0e0h]
	vmovdqa ymmword ptr [rdi + rbx + 0e0h], ymm7

	vmovdqu ymm8, ymmword ptr [rax + 1 + 100h]
	vmovdqa ymmword ptr [rdi + rbx + 100h], ymm8
	vmovdqu ymm9, ymmword ptr [rax + 1 + 120h]
	vmovdqa ymmword ptr [rdi + rbx + 120h], ymm9
	vmovdqu ymm10, ymmword ptr [rax + 1 + 140h]
	vmovdqa ymmword ptr [rdi + rbx + 140h], ymm10
	vmovdqu ymm11, ymmword ptr [rax + 1 + 160h]
	vmovdqa ymmword ptr [rdi + rbx + 160h], ymm11

	vmovdqu ymm12, ymmword ptr [rax + 1 + 180h]
	vmovdqa ymmword ptr [rdi + rbx + 180h], ymm12
	vmovdqu ymm13, ymmword ptr [rax + 1 + 1a0h]
	vmovdqa ymmword ptr [rdi + rbx + 1a0h], ymm13
	vmovdqu ymm14, ymmword ptr [rax + 1 + 1c0h]
	vmovdqa ymmword ptr [rdi + rbx + 1c0h], ymm14
	vmovdqu ymm15, ymmword ptr [rax + 1 + 1e0h]
	vmovdqa ymmword ptr [rdi + rbx + 1e0h], ymm15

	xor rbx, rbx
	mov dword ptr [rdx].state.spi_receivecount, ebx
	mov dword ptr [rdx].state.spi_writeblock, ebx

bad_data:
	mov r13b, SPI_DEFAULT_RESPONSE
	mov byte ptr [rsi+SPI_DATA], r13b
	mov dword ptr [rdx].state.spi_previousvalue, r13d

	spi_log_fromsdcard

	ret	
spi_write_complete endp

spi_read_send_status proc
	spi_log_fromsdcard

	mov dword ptr [rdx].state.spi_sendcount, 0
	mov dword ptr [rdx].state.spi_sendlength, 2

	mov eax, dword ptr [rdx].state.spi_initialised
	test eax, eax
	jz not_initialised

	xor rbx, rbx
	mov rax, qword ptr [rdx].state.spi_outbound_buffer_ptr
	mov byte ptr [rax], bl
	mov byte ptr [rax+1], bl

	ret
not_initialised:
	mov rax, qword ptr [rdx].state.spi_outbound_buffer_ptr
	mov byte ptr [rax], 1fh
	mov byte ptr [rax+1], 0ffh

	ret
spi_read_send_status endp

spi_read_ocr proc
	spi_log_fromsdcard

	mov dword ptr [rdx].state.spi_sendcount, 0
	mov dword ptr [rdx].state.spi_sendlength, 4

	mov rax, qword ptr [rdx].state.spi_outbound_buffer_ptr
	mov byte ptr [rax], 0c0h
	mov byte ptr [rax+1], 0ffh
	mov byte ptr [rax+2], 080h
	mov byte ptr [rax+3], 000h

	ret
spi_read_ocr endp

spi_command_table:
	qword spi_go_idle		; 0
	qword spi_not_known ; 1
	qword spi_not_known ; 2
	qword spi_not_known ; 3
	qword spi_not_known ; 4
	qword spi_not_known ; 5
	qword spi_not_known ; 6
	qword spi_not_known ; 7
	qword spi_cmd8			; 8
	qword spi_not_known ; 9
	qword spi_not_known ; 10
	qword spi_not_known ; 11
	qword spi_not_known ; 12
	qword spi_read_send_status ; 13
	qword spi_not_known ; 14
	qword spi_not_known ; 15
	qword spi_not_known ; 16
	qword spi_read_single_block ; 17
	qword spi_not_known ; 18
	qword spi_not_known ; 19
	qword spi_not_known ; 20
	qword spi_not_known ; 21
	qword spi_not_known ; 22
	qword spi_not_known ; 23
	qword spi_write_single_block ; 24
	qword spi_not_known ; 25
	qword spi_not_known ; 26
	qword spi_not_known ; 27
	qword spi_not_known ; 28
	qword spi_not_known ; 29
	qword spi_not_known ; 30
	qword spi_not_known ; 31
	qword spi_not_known ; 32
	qword spi_not_known ; 33
	qword spi_not_known ; 34
	qword spi_not_known ; 35
	qword spi_not_known ; 36
	qword spi_not_known ; 37
	qword spi_not_known ; 38
	qword spi_not_known ; 39
	qword spi_not_known ; 40
	qword spi_acmd41	 ; 41
	qword spi_not_known ; 42
	qword spi_not_known ; 43
	qword spi_not_known ; 44
	qword spi_not_known ; 45
	qword spi_not_known ; 46
	qword spi_not_known ; 47
	qword spi_not_known ; 48
	qword spi_not_known ; 49
	qword spi_not_known ; 50
	qword spi_not_known ; 51
	qword spi_not_known ; 52
	qword spi_not_known ; 53
	qword spi_not_known ; 54
	qword spi_cmd55		 ; 55
	qword spi_not_known ; 56
	qword spi_not_known ; 57
	qword spi_read_ocr ; 58
	qword spi_not_known ; 59
	qword spi_not_known ; 60
	qword spi_not_known ; 61
	qword spi_not_known ; 62
	qword spi_not_known ; 63
	qword spi_not_known ; 64




vera_update_spi_ctrl proc
	movzx r13, byte ptr [rsi+SPI_CTRL]
	and r13b, SPI_SELECT + SPI_SLOWCLOCK					; can only set slow clock or select
	mov byte ptr [rsi+SPI_CTRL], r13b

	mov ebx, dword ptr [rdx].state.spi_chipselect
	and r13d, 1
	mov dword ptr [rdx].state.spi_chipselect, r13d			; chip select is always set
	xor ebx, r13d
	jz no_change											; we only reset receive count on a change

	mov ebx, dword ptr [rdx].state.spi_receivecount
	xor rax, rax

	bt r13, 0
	cmovc ebx, eax											; clear the receive count if select is 1
	mov dword ptr [rdx].state.spi_receivecount, ebx			
	
no_change:
	; Log ctrl -----------------
	mov eax, dword ptr [rdx].state.spi_position
	mov rbx, qword ptr [rdx].state.spi_history_ptr
	mov byte ptr [rbx + rax+1], 2		; ctrl
	mov byte ptr [rbx + rax], r13b
	adc eax, 2
	and eax, 2048-1
	mov dword ptr [rdx].state.spi_position, eax
	; ------------------

	ret
vera_update_spi_ctrl endp