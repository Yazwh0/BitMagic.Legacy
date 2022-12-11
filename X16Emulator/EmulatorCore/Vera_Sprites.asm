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

SPRITE_LEN equ 32

SPRITE_SEARCHING equ 0
SPRITE_RENDERING equ 1
SPRITE_DONE equ 2

sprite struct
	address dword ?
	palette_offset dword ?
	collision_mask dword ?
	YHeight dword ?
	x dword ?
	y dword ?
	mode dword ?	; mode, height, width, vflip, hflip : 7 bits.
	depth dword ?
sprite ends

; r12 actual line (w/ v_scroll)
sprites_render proc
	mov eax, dword ptr [rdx].state.sprite_render_mode
	test rax, rax
	jz sprites_render_find

	ret
sprites_render endp

; check current sprite to see if its to be rendered
sprites_render_find proc

	mov eax, dword ptr [rdx].state.sprite_position
	shl rax, 5	; * 32
	mov rsi, [rdx].state.sprite_ptr

	; check if sprite is on line
	mov ebx, dword ptr [rsi + rax].sprite.y
	cmp rbx, r12
	jl not_on_line

	add ebx, dword ptr [rsi + rax].sprite.yheight
	jg not_on_line

	; found a sprite!
	mov dword ptr [rdx].state.sprite_render_mode, SPRITE_RENDERING 
	ret
	
not_on_line:
	shr rax, 5		; / 32, or back to index.
	add eax, 1
	cmp eax, 127
	je all_done
	mov dword ptr [rdx].state.sprite_position, eax

	ret

all_done:
	mov dword ptr [rdx].state.sprite_render_mode, SPRITE_DONE  ; all done
	ret
sprites_render_find endp

; Inptus:
; r13b value
; rdi vram address
sprite_update_registers proc

	; keep sprite data in line with vram
	mov rsi, [rdx].state.sprite_ptr
	
	; sprites are 8bytes, find sprite index
	mov rax, rdi
	sub rax, 1fc00h
	and rax, 001111111000b	; mask off index (128 sprites)
	shl rax, 5-3			; adust to actual offset

	; set rbx to the index of what was changed
	mov rbx, rdi
	and rbx, 07h 

	lea r12, sprite_jump_table
	jmp qword ptr [r12 + rbx * 8]

sprite_jump_table:
	dq sprite_byte_0
	dq sprite_byte_1
	dq sprite_byte_2
	dq sprite_byte_3
	dq sprite_byte_4
	dq sprite_byte_5
	dq sprite_byte_6
	dq sprite_byte_7

; Address Low
sprite_byte_0:
	shl r13, 5
	mov r12d, dword ptr [rsi + rax].sprite.address
	and r12, 1e000h ; bits 16-13
	or r12, r13
	mov dword ptr [rsi + rax].sprite.address, r12d

	ret

; Mode + Address H
sprite_byte_1:
	mov r12, r13
	and r12, 10000000b	; islate mode
	shr r12, 1			; move to destination
	mov ebx, dword ptr [rsi + rax].sprite.mode
	and rbx, 00111111b	; mask
	or rbx, r12
	mov dword ptr [rsi + rax].sprite.mode, ebx

	and r13, 01111b
	shl r13, 13
	mov r12d, dword ptr [rsi + rax].sprite.address
	and r12, 01111111100000b ; bits 12-5
	or r12, r13
	mov dword ptr [rsi + rax].sprite.address, r12d

	ret

; X Low
sprite_byte_2:
	mov r12d, dword ptr [rsi + rax].sprite.x
	and r12d, 0300h
	or r12, r13
	mov dword ptr [rsi + rax].sprite.x, r12d

	ret

; X High
sprite_byte_3:
	and r13, 03h
	shl r13, 8
	mov r12d, dword ptr [rsi + rax].sprite.x
	and r12d, 00ffh
	or r12, r13
	mov dword ptr [rsi + rax].sprite.x, r12d
	
	ret

; Y Low
sprite_byte_4:
	mov r12d, dword ptr [rsi + rax].sprite.y
	and r12d, 0300h
	or r12, r13
	mov dword ptr [rsi + rax].sprite.y, r12d

	ret

; Y High
sprite_byte_5:
	and r13, 03h
	shl r13, 8
	mov r12d, dword ptr [rsi + rax].sprite.y
	and r12d, 00ffh
	or r12, r13
	mov dword ptr [rsi + rax].sprite.y, r12d

	ret

sprite_byte_6:
	; store depth
	mov r12, r13
	and r12, 1100b
	shr r12, 2
	mov dword ptr [rsi + rax].sprite.depth, r12d

	; store collision mask
	mov r12, r13
	and r12, 11110000b
	shr r12, 4
	mov dword ptr [rsi + rax].sprite.collision_mask, r12d

	mov r12, r13
	and r12, 00000011b
	mov ebx, dword ptr [rsi + rax].sprite.mode
	and rbx, 11111100b	; mask
	or rbx, r12

	mov dword ptr [rsi + rax].sprite.mode, ebx

	ret

sprite_byte_7:
	; store palette offset
	mov r12, r13
	and r12, 1111b
	mov dword ptr [rsi + rax].sprite.palette_offset, r12d

	push rcx
	mov rcx, r13
	and rcx, 11000000b
	shr rcx, 6
	mov rbx, 8
	shl rbx, cl
	mov dword ptr [rsi + rax].sprite.yheight, ebx
	pop rcx

	; store height change for render mode
	and r13, 11110000b
	shr r13, 2
	mov ebx, dword ptr [rsi + rax].sprite.mode
	and rbx, 01000011b	; mask
	or rbx, r13
	mov dword ptr [rsi + rax].sprite.mode, ebx

	ret

exit:
	ret

sprite_update_registers endp

; render a sprite to our cached data so rendering is quicker every frame
; inputs:
; rax sprite number to update
;sprite_update_cache proc
;	mov rdi, [rdx].state.sprite_cache_ptr

;	mov r13, rax
;	shl r13, 12		; * 64*64
;	add rdi, r13	; rdi now points to the memory location of the cached sprite image data

;	mov ebx, dword ptr [rsi + rax].sprite.mode
;	lea r13, sprite_definition_jump
;	jmp qword ptr [r13 + rbx * 8]

;sprite_update_cache endp

; render a sprite to the sprite image cache
; expects 
; rax : sprite number

render_sprite macro bpp, height, width, vflip, hflip
	


	ret
endm

sprite_definition_proc macro _sprite_bpp, _sprite_height, _sprite_width, _vflip, _hflip, _sprite_definition_count
sprite_definition_&_sprite_definition_count& proc
	render_sprite _sprite_bpp, _sprite_height, _sprite_width, _vflip, _hflip
sprite_definition_&_sprite_definition_count& endp
endm

sprite_definition_proc 0, 0, 0, 0, 0, 0
sprite_definition_proc 0, 0, 0, 0, 1, 1
sprite_definition_proc 0, 0, 0, 1, 0, 2
sprite_definition_proc 0, 0, 0, 1, 1, 3
sprite_definition_proc 0, 0, 1, 0, 0, 4
sprite_definition_proc 0, 0, 1, 0, 1, 5
sprite_definition_proc 0, 0, 1, 1, 0, 6
sprite_definition_proc 0, 0, 1, 1, 1, 7
sprite_definition_proc 0, 0, 2, 0, 0, 8
sprite_definition_proc 0, 0, 2, 0, 1, 9
sprite_definition_proc 0, 0, 2, 1, 0, 10
sprite_definition_proc 0, 0, 2, 1, 1, 11
sprite_definition_proc 0, 0, 3, 0, 0, 12
sprite_definition_proc 0, 0, 3, 0, 1, 13
sprite_definition_proc 0, 0, 3, 1, 0, 14
sprite_definition_proc 0, 0, 3, 1, 1, 15
sprite_definition_proc 0, 1, 0, 0, 0, 16
sprite_definition_proc 0, 1, 0, 0, 1, 17
sprite_definition_proc 0, 1, 0, 1, 0, 18
sprite_definition_proc 0, 1, 0, 1, 1, 19
sprite_definition_proc 0, 1, 1, 0, 0, 20
sprite_definition_proc 0, 1, 1, 0, 1, 21
sprite_definition_proc 0, 1, 1, 1, 0, 22
sprite_definition_proc 0, 1, 1, 1, 1, 23
sprite_definition_proc 0, 1, 2, 0, 0, 24
sprite_definition_proc 0, 1, 2, 0, 1, 25
sprite_definition_proc 0, 1, 2, 1, 0, 26
sprite_definition_proc 0, 1, 2, 1, 1, 27
sprite_definition_proc 0, 1, 3, 0, 0, 28
sprite_definition_proc 0, 1, 3, 0, 1, 29
sprite_definition_proc 0, 1, 3, 1, 0, 30
sprite_definition_proc 0, 1, 3, 1, 1, 31
sprite_definition_proc 0, 2, 0, 0, 0, 32
sprite_definition_proc 0, 2, 0, 0, 1, 33
sprite_definition_proc 0, 2, 0, 1, 0, 34
sprite_definition_proc 0, 2, 0, 1, 1, 35
sprite_definition_proc 0, 2, 1, 0, 0, 36
sprite_definition_proc 0, 2, 1, 0, 1, 37
sprite_definition_proc 0, 2, 1, 1, 0, 38
sprite_definition_proc 0, 2, 1, 1, 1, 39
sprite_definition_proc 0, 2, 2, 0, 0, 40
sprite_definition_proc 0, 2, 2, 0, 1, 41
sprite_definition_proc 0, 2, 2, 1, 0, 42
sprite_definition_proc 0, 2, 2, 1, 1, 43
sprite_definition_proc 0, 2, 3, 0, 0, 44
sprite_definition_proc 0, 2, 3, 0, 1, 45
sprite_definition_proc 0, 2, 3, 1, 0, 46
sprite_definition_proc 0, 2, 3, 1, 1, 47
sprite_definition_proc 0, 3, 0, 0, 0, 48
sprite_definition_proc 0, 3, 0, 0, 1, 49
sprite_definition_proc 0, 3, 0, 1, 0, 50
sprite_definition_proc 0, 3, 0, 1, 1, 51
sprite_definition_proc 0, 3, 1, 0, 0, 52
sprite_definition_proc 0, 3, 1, 0, 1, 53
sprite_definition_proc 0, 3, 1, 1, 0, 54
sprite_definition_proc 0, 3, 1, 1, 1, 55
sprite_definition_proc 0, 3, 2, 0, 0, 56
sprite_definition_proc 0, 3, 2, 0, 1, 57
sprite_definition_proc 0, 3, 2, 1, 0, 58
sprite_definition_proc 0, 3, 2, 1, 1, 59
sprite_definition_proc 0, 3, 3, 0, 0, 60
sprite_definition_proc 0, 3, 3, 0, 1, 61
sprite_definition_proc 0, 3, 3, 1, 0, 62
sprite_definition_proc 0, 3, 3, 1, 1, 63
sprite_definition_proc 1, 0, 0, 0, 0, 64
sprite_definition_proc 1, 0, 0, 0, 1, 65
sprite_definition_proc 1, 0, 0, 1, 0, 66
sprite_definition_proc 1, 0, 0, 1, 1, 67
sprite_definition_proc 1, 0, 1, 0, 0, 68
sprite_definition_proc 1, 0, 1, 0, 1, 69
sprite_definition_proc 1, 0, 1, 1, 0, 70
sprite_definition_proc 1, 0, 1, 1, 1, 71
sprite_definition_proc 1, 0, 2, 0, 0, 72
sprite_definition_proc 1, 0, 2, 0, 1, 73
sprite_definition_proc 1, 0, 2, 1, 0, 74
sprite_definition_proc 1, 0, 2, 1, 1, 75
sprite_definition_proc 1, 0, 3, 0, 0, 76
sprite_definition_proc 1, 0, 3, 0, 1, 77
sprite_definition_proc 1, 0, 3, 1, 0, 78
sprite_definition_proc 1, 0, 3, 1, 1, 79
sprite_definition_proc 1, 1, 0, 0, 0, 80
sprite_definition_proc 1, 1, 0, 0, 1, 81
sprite_definition_proc 1, 1, 0, 1, 0, 82
sprite_definition_proc 1, 1, 0, 1, 1, 83
sprite_definition_proc 1, 1, 1, 0, 0, 84
sprite_definition_proc 1, 1, 1, 0, 1, 85
sprite_definition_proc 1, 1, 1, 1, 0, 86
sprite_definition_proc 1, 1, 1, 1, 1, 87
sprite_definition_proc 1, 1, 2, 0, 0, 88
sprite_definition_proc 1, 1, 2, 0, 1, 89
sprite_definition_proc 1, 1, 2, 1, 0, 90
sprite_definition_proc 1, 1, 2, 1, 1, 91
sprite_definition_proc 1, 1, 3, 0, 0, 92
sprite_definition_proc 1, 1, 3, 0, 1, 93
sprite_definition_proc 1, 1, 3, 1, 0, 94
sprite_definition_proc 1, 1, 3, 1, 1, 95
sprite_definition_proc 1, 2, 0, 0, 0, 96
sprite_definition_proc 1, 2, 0, 0, 1, 97
sprite_definition_proc 1, 2, 0, 1, 0, 98
sprite_definition_proc 1, 2, 0, 1, 1, 99
sprite_definition_proc 1, 2, 1, 0, 0, 100
sprite_definition_proc 1, 2, 1, 0, 1, 101
sprite_definition_proc 1, 2, 1, 1, 0, 102
sprite_definition_proc 1, 2, 1, 1, 1, 103
sprite_definition_proc 1, 2, 2, 0, 0, 104
sprite_definition_proc 1, 2, 2, 0, 1, 105
sprite_definition_proc 1, 2, 2, 1, 0, 106
sprite_definition_proc 1, 2, 2, 1, 1, 107
sprite_definition_proc 1, 2, 3, 0, 0, 108
sprite_definition_proc 1, 2, 3, 0, 1, 109
sprite_definition_proc 1, 2, 3, 1, 0, 110
sprite_definition_proc 1, 2, 3, 1, 1, 111
sprite_definition_proc 1, 3, 0, 0, 0, 112
sprite_definition_proc 1, 3, 0, 0, 1, 113
sprite_definition_proc 1, 3, 0, 1, 0, 114
sprite_definition_proc 1, 3, 0, 1, 1, 115
sprite_definition_proc 1, 3, 1, 0, 0, 116
sprite_definition_proc 1, 3, 1, 0, 1, 117
sprite_definition_proc 1, 3, 1, 1, 0, 118
sprite_definition_proc 1, 3, 1, 1, 1, 119
sprite_definition_proc 1, 3, 2, 0, 0, 120
sprite_definition_proc 1, 3, 2, 0, 1, 121
sprite_definition_proc 1, 3, 2, 1, 0, 122
sprite_definition_proc 1, 3, 2, 1, 1, 123
sprite_definition_proc 1, 3, 3, 0, 0, 124
sprite_definition_proc 1, 3, 3, 0, 1, 125
sprite_definition_proc 1, 3, 3, 1, 0, 126
sprite_definition_proc 1, 3, 3, 1, 1, 127

sprite_definition_jump:
	qword sprite_definition_0
	qword sprite_definition_1
	qword sprite_definition_2
	qword sprite_definition_3
	qword sprite_definition_4
	qword sprite_definition_5
	qword sprite_definition_6
	qword sprite_definition_7
	qword sprite_definition_8
	qword sprite_definition_9
	qword sprite_definition_10
	qword sprite_definition_11
	qword sprite_definition_12
	qword sprite_definition_13
	qword sprite_definition_14
	qword sprite_definition_15
	qword sprite_definition_16
	qword sprite_definition_17
	qword sprite_definition_18
	qword sprite_definition_19
	qword sprite_definition_20
	qword sprite_definition_21
	qword sprite_definition_22
	qword sprite_definition_23
	qword sprite_definition_24
	qword sprite_definition_25
	qword sprite_definition_26
	qword sprite_definition_27
	qword sprite_definition_28
	qword sprite_definition_29
	qword sprite_definition_30
	qword sprite_definition_31
	qword sprite_definition_32
	qword sprite_definition_33
	qword sprite_definition_34
	qword sprite_definition_35
	qword sprite_definition_36
	qword sprite_definition_37
	qword sprite_definition_38
	qword sprite_definition_39
	qword sprite_definition_40
	qword sprite_definition_41
	qword sprite_definition_42
	qword sprite_definition_43
	qword sprite_definition_44
	qword sprite_definition_45
	qword sprite_definition_46
	qword sprite_definition_47
	qword sprite_definition_48
	qword sprite_definition_49
	qword sprite_definition_50
	qword sprite_definition_51
	qword sprite_definition_52
	qword sprite_definition_53
	qword sprite_definition_54
	qword sprite_definition_55
	qword sprite_definition_56
	qword sprite_definition_57
	qword sprite_definition_58
	qword sprite_definition_59
	qword sprite_definition_60
	qword sprite_definition_61
	qword sprite_definition_62
	qword sprite_definition_63
	qword sprite_definition_64
	qword sprite_definition_65
	qword sprite_definition_66
	qword sprite_definition_67
	qword sprite_definition_68
	qword sprite_definition_69
	qword sprite_definition_70
	qword sprite_definition_71
	qword sprite_definition_72
	qword sprite_definition_73
	qword sprite_definition_74
	qword sprite_definition_75
	qword sprite_definition_76
	qword sprite_definition_77
	qword sprite_definition_78
	qword sprite_definition_79
	qword sprite_definition_80
	qword sprite_definition_81
	qword sprite_definition_82
	qword sprite_definition_83
	qword sprite_definition_84
	qword sprite_definition_85
	qword sprite_definition_86
	qword sprite_definition_87
	qword sprite_definition_88
	qword sprite_definition_89
	qword sprite_definition_90
	qword sprite_definition_91
	qword sprite_definition_92
	qword sprite_definition_93
	qword sprite_definition_94
	qword sprite_definition_95
	qword sprite_definition_96
	qword sprite_definition_97
	qword sprite_definition_98
	qword sprite_definition_99
	qword sprite_definition_100
	qword sprite_definition_101
	qword sprite_definition_102
	qword sprite_definition_103
	qword sprite_definition_104
	qword sprite_definition_105
	qword sprite_definition_106
	qword sprite_definition_107
	qword sprite_definition_108
	qword sprite_definition_109
	qword sprite_definition_110
	qword sprite_definition_111
	qword sprite_definition_112
	qword sprite_definition_113
	qword sprite_definition_114
	qword sprite_definition_115
	qword sprite_definition_116
	qword sprite_definition_117
	qword sprite_definition_118
	qword sprite_definition_119
	qword sprite_definition_120
	qword sprite_definition_121
	qword sprite_definition_122
	qword sprite_definition_123
	qword sprite_definition_124
	qword sprite_definition_125
	qword sprite_definition_126
	qword sprite_definition_127
