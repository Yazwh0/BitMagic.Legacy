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

switch_rambank proc
	mov rax, [rdx].state.memory_ptr
	add rax, 0a000h						; source
	
	mov rbx, current_rambank			; memory address of current rambank, this is our dest
	call copy_8k						; call copy
switch_rambank endp						; fall through to rambank copy

copy_rambank_to_memory proc
	mov rbx, [rdx].state.rambank_ptr	
	movzx rax, byte ptr [rsi]
	shl rax, 13							; bank * 8k
	add rax, rbx						; source
	mov current_rambank, rax			; store memory address of the new rambank

	mov rbx, [rdx].state.memory_ptr
	add rbx, 0a000h						; destination
	jmp copy_8k							; this proc is call'd, so jmp as copy_8k as a ret.
copy_rambank_to_memory endp

preserve_current_rambank proc
	mov rax, [rdx].state.memory_ptr
	add rax, 0a000h						; source
	
	mov rbx, current_rambank			; memory address of current rambank, this is our dest
	jmp copy_8k							; call copy
preserve_current_rambank endp

copy_rombank_to_memory proc
	mov rbx, [rdx].state.rom_ptr
	movzx rax, byte ptr [rsi+1]
	shl rax, 14							; bank * 8k
	add rax, rbx						; source

	mov rbx, rsi
	add rbx, 0c000h					; destination
	call copy_8k
	add rbx, 02000h					; next dest and source
	add rax, 02000h
	jmp copy_8k
copy_rombank_to_memory endp

; copys 8k from rax to rbx
copy_8k proc
	vmovdqa ymm0, ymmword ptr [rax + 00h]
	vmovdqa ymmword ptr [rbx + 00h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 020h]
	vmovdqa ymmword ptr [rbx + 020h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 040h]
	vmovdqa ymmword ptr [rbx + 040h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 060h]
	vmovdqa ymmword ptr [rbx + 060h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 080h]
	vmovdqa ymmword ptr [rbx + 080h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0A0h]
	vmovdqa ymmword ptr [rbx + 0A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0C0h]
	vmovdqa ymmword ptr [rbx + 0C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0E0h]
	vmovdqa ymmword ptr [rbx + 0E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0100h]
	vmovdqa ymmword ptr [rbx + 0100h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0120h]
	vmovdqa ymmword ptr [rbx + 0120h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0140h]
	vmovdqa ymmword ptr [rbx + 0140h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0160h]
	vmovdqa ymmword ptr [rbx + 0160h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0180h]
	vmovdqa ymmword ptr [rbx + 0180h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01A0h]
	vmovdqa ymmword ptr [rbx + 01A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01C0h]
	vmovdqa ymmword ptr [rbx + 01C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01E0h]
	vmovdqa ymmword ptr [rbx + 01E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0200h]
	vmovdqa ymmword ptr [rbx + 0200h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0220h]
	vmovdqa ymmword ptr [rbx + 0220h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0240h]
	vmovdqa ymmword ptr [rbx + 0240h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0260h]
	vmovdqa ymmword ptr [rbx + 0260h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0280h]
	vmovdqa ymmword ptr [rbx + 0280h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 02A0h]
	vmovdqa ymmword ptr [rbx + 02A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 02C0h]
	vmovdqa ymmword ptr [rbx + 02C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 02E0h]
	vmovdqa ymmword ptr [rbx + 02E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0300h]
	vmovdqa ymmword ptr [rbx + 0300h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0320h]
	vmovdqa ymmword ptr [rbx + 0320h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0340h]
	vmovdqa ymmword ptr [rbx + 0340h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0360h]
	vmovdqa ymmword ptr [rbx + 0360h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0380h]
	vmovdqa ymmword ptr [rbx + 0380h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 03A0h]
	vmovdqa ymmword ptr [rbx + 03A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 03C0h]
	vmovdqa ymmword ptr [rbx + 03C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 03E0h]
	vmovdqa ymmword ptr [rbx + 03E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0400h]
	vmovdqa ymmword ptr [rbx + 0400h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0420h]
	vmovdqa ymmword ptr [rbx + 0420h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0440h]
	vmovdqa ymmword ptr [rbx + 0440h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0460h]
	vmovdqa ymmword ptr [rbx + 0460h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0480h]
	vmovdqa ymmword ptr [rbx + 0480h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 04A0h]
	vmovdqa ymmword ptr [rbx + 04A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 04C0h]
	vmovdqa ymmword ptr [rbx + 04C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 04E0h]
	vmovdqa ymmword ptr [rbx + 04E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0500h]
	vmovdqa ymmword ptr [rbx + 0500h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0520h]
	vmovdqa ymmword ptr [rbx + 0520h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0540h]
	vmovdqa ymmword ptr [rbx + 0540h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0560h]
	vmovdqa ymmword ptr [rbx + 0560h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0580h]
	vmovdqa ymmword ptr [rbx + 0580h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 05A0h]
	vmovdqa ymmword ptr [rbx + 05A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 05C0h]
	vmovdqa ymmword ptr [rbx + 05C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 05E0h]
	vmovdqa ymmword ptr [rbx + 05E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0600h]
	vmovdqa ymmword ptr [rbx + 0600h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0620h]
	vmovdqa ymmword ptr [rbx + 0620h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0640h]
	vmovdqa ymmword ptr [rbx + 0640h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0660h]
	vmovdqa ymmword ptr [rbx + 0660h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0680h]
	vmovdqa ymmword ptr [rbx + 0680h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 06A0h]
	vmovdqa ymmword ptr [rbx + 06A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 06C0h]
	vmovdqa ymmword ptr [rbx + 06C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 06E0h]
	vmovdqa ymmword ptr [rbx + 06E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0700h]
	vmovdqa ymmword ptr [rbx + 0700h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0720h]
	vmovdqa ymmword ptr [rbx + 0720h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0740h]
	vmovdqa ymmword ptr [rbx + 0740h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0760h]
	vmovdqa ymmword ptr [rbx + 0760h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0780h]
	vmovdqa ymmword ptr [rbx + 0780h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 07A0h]
	vmovdqa ymmword ptr [rbx + 07A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 07C0h]
	vmovdqa ymmword ptr [rbx + 07C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 07E0h]
	vmovdqa ymmword ptr [rbx + 07E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0800h]
	vmovdqa ymmword ptr [rbx + 0800h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0820h]
	vmovdqa ymmword ptr [rbx + 0820h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0840h]
	vmovdqa ymmword ptr [rbx + 0840h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0860h]
	vmovdqa ymmword ptr [rbx + 0860h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0880h]
	vmovdqa ymmword ptr [rbx + 0880h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 08A0h]
	vmovdqa ymmword ptr [rbx + 08A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 08C0h]
	vmovdqa ymmword ptr [rbx + 08C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 08E0h]
	vmovdqa ymmword ptr [rbx + 08E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0900h]
	vmovdqa ymmword ptr [rbx + 0900h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0920h]
	vmovdqa ymmword ptr [rbx + 0920h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0940h]
	vmovdqa ymmword ptr [rbx + 0940h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0960h]
	vmovdqa ymmword ptr [rbx + 0960h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0980h]
	vmovdqa ymmword ptr [rbx + 0980h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 09A0h]
	vmovdqa ymmword ptr [rbx + 09A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 09C0h]
	vmovdqa ymmword ptr [rbx + 09C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 09E0h]
	vmovdqa ymmword ptr [rbx + 09E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0A00h]
	vmovdqa ymmword ptr [rbx + 0A00h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0A20h]
	vmovdqa ymmword ptr [rbx + 0A20h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0A40h]
	vmovdqa ymmword ptr [rbx + 0A40h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0A60h]
	vmovdqa ymmword ptr [rbx + 0A60h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0A80h]
	vmovdqa ymmword ptr [rbx + 0A80h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0AA0h]
	vmovdqa ymmword ptr [rbx + 0AA0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0AC0h]
	vmovdqa ymmword ptr [rbx + 0AC0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0AE0h]
	vmovdqa ymmword ptr [rbx + 0AE0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0B00h]
	vmovdqa ymmword ptr [rbx + 0B00h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0B20h]
	vmovdqa ymmword ptr [rbx + 0B20h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0B40h]
	vmovdqa ymmword ptr [rbx + 0B40h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0B60h]
	vmovdqa ymmword ptr [rbx + 0B60h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0B80h]
	vmovdqa ymmword ptr [rbx + 0B80h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0BA0h]
	vmovdqa ymmword ptr [rbx + 0BA0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0BC0h]
	vmovdqa ymmword ptr [rbx + 0BC0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0BE0h]
	vmovdqa ymmword ptr [rbx + 0BE0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0C00h]
	vmovdqa ymmword ptr [rbx + 0C00h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0C20h]
	vmovdqa ymmword ptr [rbx + 0C20h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0C40h]
	vmovdqa ymmword ptr [rbx + 0C40h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0C60h]
	vmovdqa ymmword ptr [rbx + 0C60h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0C80h]
	vmovdqa ymmword ptr [rbx + 0C80h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0CA0h]
	vmovdqa ymmword ptr [rbx + 0CA0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0CC0h]
	vmovdqa ymmword ptr [rbx + 0CC0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0CE0h]
	vmovdqa ymmword ptr [rbx + 0CE0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0D00h]
	vmovdqa ymmword ptr [rbx + 0D00h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0D20h]
	vmovdqa ymmword ptr [rbx + 0D20h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0D40h]
	vmovdqa ymmword ptr [rbx + 0D40h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0D60h]
	vmovdqa ymmword ptr [rbx + 0D60h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0D80h]
	vmovdqa ymmword ptr [rbx + 0D80h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0DA0h]
	vmovdqa ymmword ptr [rbx + 0DA0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0DC0h]
	vmovdqa ymmword ptr [rbx + 0DC0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0DE0h]
	vmovdqa ymmword ptr [rbx + 0DE0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0E00h]
	vmovdqa ymmword ptr [rbx + 0E00h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0E20h]
	vmovdqa ymmword ptr [rbx + 0E20h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0E40h]
	vmovdqa ymmword ptr [rbx + 0E40h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0E60h]
	vmovdqa ymmword ptr [rbx + 0E60h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0E80h]
	vmovdqa ymmword ptr [rbx + 0E80h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0EA0h]
	vmovdqa ymmword ptr [rbx + 0EA0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0EC0h]
	vmovdqa ymmword ptr [rbx + 0EC0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0EE0h]
	vmovdqa ymmword ptr [rbx + 0EE0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0F00h]
	vmovdqa ymmword ptr [rbx + 0F00h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0F20h]
	vmovdqa ymmword ptr [rbx + 0F20h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0F40h]
	vmovdqa ymmword ptr [rbx + 0F40h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0F60h]
	vmovdqa ymmword ptr [rbx + 0F60h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0F80h]
	vmovdqa ymmword ptr [rbx + 0F80h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0FA0h]
	vmovdqa ymmword ptr [rbx + 0FA0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0FC0h]
	vmovdqa ymmword ptr [rbx + 0FC0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 0FE0h]
	vmovdqa ymmword ptr [rbx + 0FE0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01000h]
	vmovdqa ymmword ptr [rbx + 01000h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01020h]
	vmovdqa ymmword ptr [rbx + 01020h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01040h]
	vmovdqa ymmword ptr [rbx + 01040h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01060h]
	vmovdqa ymmword ptr [rbx + 01060h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01080h]
	vmovdqa ymmword ptr [rbx + 01080h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 010A0h]
	vmovdqa ymmword ptr [rbx + 010A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 010C0h]
	vmovdqa ymmword ptr [rbx + 010C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 010E0h]
	vmovdqa ymmword ptr [rbx + 010E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01100h]
	vmovdqa ymmword ptr [rbx + 01100h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01120h]
	vmovdqa ymmword ptr [rbx + 01120h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01140h]
	vmovdqa ymmword ptr [rbx + 01140h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01160h]
	vmovdqa ymmword ptr [rbx + 01160h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01180h]
	vmovdqa ymmword ptr [rbx + 01180h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 011A0h]
	vmovdqa ymmword ptr [rbx + 011A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 011C0h]
	vmovdqa ymmword ptr [rbx + 011C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 011E0h]
	vmovdqa ymmword ptr [rbx + 011E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01200h]
	vmovdqa ymmword ptr [rbx + 01200h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01220h]
	vmovdqa ymmword ptr [rbx + 01220h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01240h]
	vmovdqa ymmword ptr [rbx + 01240h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01260h]
	vmovdqa ymmword ptr [rbx + 01260h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01280h]
	vmovdqa ymmword ptr [rbx + 01280h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 012A0h]
	vmovdqa ymmword ptr [rbx + 012A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 012C0h]
	vmovdqa ymmword ptr [rbx + 012C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 012E0h]
	vmovdqa ymmword ptr [rbx + 012E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01300h]
	vmovdqa ymmword ptr [rbx + 01300h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01320h]
	vmovdqa ymmword ptr [rbx + 01320h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01340h]
	vmovdqa ymmword ptr [rbx + 01340h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01360h]
	vmovdqa ymmword ptr [rbx + 01360h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01380h]
	vmovdqa ymmword ptr [rbx + 01380h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 013A0h]
	vmovdqa ymmword ptr [rbx + 013A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 013C0h]
	vmovdqa ymmword ptr [rbx + 013C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 013E0h]
	vmovdqa ymmword ptr [rbx + 013E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01400h]
	vmovdqa ymmword ptr [rbx + 01400h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01420h]
	vmovdqa ymmword ptr [rbx + 01420h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01440h]
	vmovdqa ymmword ptr [rbx + 01440h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01460h]
	vmovdqa ymmword ptr [rbx + 01460h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01480h]
	vmovdqa ymmword ptr [rbx + 01480h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 014A0h]
	vmovdqa ymmword ptr [rbx + 014A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 014C0h]
	vmovdqa ymmword ptr [rbx + 014C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 014E0h]
	vmovdqa ymmword ptr [rbx + 014E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01500h]
	vmovdqa ymmword ptr [rbx + 01500h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01520h]
	vmovdqa ymmword ptr [rbx + 01520h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01540h]
	vmovdqa ymmword ptr [rbx + 01540h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01560h]
	vmovdqa ymmword ptr [rbx + 01560h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01580h]
	vmovdqa ymmword ptr [rbx + 01580h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 015A0h]
	vmovdqa ymmword ptr [rbx + 015A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 015C0h]
	vmovdqa ymmword ptr [rbx + 015C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 015E0h]
	vmovdqa ymmword ptr [rbx + 015E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01600h]
	vmovdqa ymmword ptr [rbx + 01600h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01620h]
	vmovdqa ymmword ptr [rbx + 01620h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01640h]
	vmovdqa ymmword ptr [rbx + 01640h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01660h]
	vmovdqa ymmword ptr [rbx + 01660h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01680h]
	vmovdqa ymmword ptr [rbx + 01680h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 016A0h]
	vmovdqa ymmword ptr [rbx + 016A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 016C0h]
	vmovdqa ymmword ptr [rbx + 016C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 016E0h]
	vmovdqa ymmword ptr [rbx + 016E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01700h]
	vmovdqa ymmword ptr [rbx + 01700h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01720h]
	vmovdqa ymmword ptr [rbx + 01720h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01740h]
	vmovdqa ymmword ptr [rbx + 01740h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01760h]
	vmovdqa ymmword ptr [rbx + 01760h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01780h]
	vmovdqa ymmword ptr [rbx + 01780h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 017A0h]
	vmovdqa ymmword ptr [rbx + 017A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 017C0h]
	vmovdqa ymmword ptr [rbx + 017C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 017E0h]
	vmovdqa ymmword ptr [rbx + 017E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01800h]
	vmovdqa ymmword ptr [rbx + 01800h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01820h]
	vmovdqa ymmword ptr [rbx + 01820h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01840h]
	vmovdqa ymmword ptr [rbx + 01840h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01860h]
	vmovdqa ymmword ptr [rbx + 01860h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01880h]
	vmovdqa ymmword ptr [rbx + 01880h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 018A0h]
	vmovdqa ymmword ptr [rbx + 018A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 018C0h]
	vmovdqa ymmword ptr [rbx + 018C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 018E0h]
	vmovdqa ymmword ptr [rbx + 018E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01900h]
	vmovdqa ymmword ptr [rbx + 01900h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01920h]
	vmovdqa ymmword ptr [rbx + 01920h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01940h]
	vmovdqa ymmword ptr [rbx + 01940h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01960h]
	vmovdqa ymmword ptr [rbx + 01960h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01980h]
	vmovdqa ymmword ptr [rbx + 01980h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 019A0h]
	vmovdqa ymmword ptr [rbx + 019A0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 019C0h]
	vmovdqa ymmword ptr [rbx + 019C0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 019E0h]
	vmovdqa ymmword ptr [rbx + 019E0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01A00h]
	vmovdqa ymmword ptr [rbx + 01A00h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01A20h]
	vmovdqa ymmword ptr [rbx + 01A20h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01A40h]
	vmovdqa ymmword ptr [rbx + 01A40h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01A60h]
	vmovdqa ymmword ptr [rbx + 01A60h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01A80h]
	vmovdqa ymmword ptr [rbx + 01A80h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01AA0h]
	vmovdqa ymmword ptr [rbx + 01AA0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01AC0h]
	vmovdqa ymmword ptr [rbx + 01AC0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01AE0h]
	vmovdqa ymmword ptr [rbx + 01AE0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01B00h]
	vmovdqa ymmword ptr [rbx + 01B00h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01B20h]
	vmovdqa ymmword ptr [rbx + 01B20h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01B40h]
	vmovdqa ymmword ptr [rbx + 01B40h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01B60h]
	vmovdqa ymmword ptr [rbx + 01B60h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01B80h]
	vmovdqa ymmword ptr [rbx + 01B80h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01BA0h]
	vmovdqa ymmword ptr [rbx + 01BA0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01BC0h]
	vmovdqa ymmword ptr [rbx + 01BC0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01BE0h]
	vmovdqa ymmword ptr [rbx + 01BE0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01C00h]
	vmovdqa ymmword ptr [rbx + 01C00h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01C20h]
	vmovdqa ymmword ptr [rbx + 01C20h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01C40h]
	vmovdqa ymmword ptr [rbx + 01C40h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01C60h]
	vmovdqa ymmword ptr [rbx + 01C60h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01C80h]
	vmovdqa ymmword ptr [rbx + 01C80h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01CA0h]
	vmovdqa ymmword ptr [rbx + 01CA0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01CC0h]
	vmovdqa ymmword ptr [rbx + 01CC0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01CE0h]
	vmovdqa ymmword ptr [rbx + 01CE0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01D00h]
	vmovdqa ymmword ptr [rbx + 01D00h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01D20h]
	vmovdqa ymmword ptr [rbx + 01D20h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01D40h]
	vmovdqa ymmword ptr [rbx + 01D40h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01D60h]
	vmovdqa ymmword ptr [rbx + 01D60h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01D80h]
	vmovdqa ymmword ptr [rbx + 01D80h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01DA0h]
	vmovdqa ymmword ptr [rbx + 01DA0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01DC0h]
	vmovdqa ymmword ptr [rbx + 01DC0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01DE0h]
	vmovdqa ymmword ptr [rbx + 01DE0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01E00h]
	vmovdqa ymmword ptr [rbx + 01E00h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01E20h]
	vmovdqa ymmword ptr [rbx + 01E20h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01E40h]
	vmovdqa ymmword ptr [rbx + 01E40h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01E60h]
	vmovdqa ymmword ptr [rbx + 01E60h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01E80h]
	vmovdqa ymmword ptr [rbx + 01E80h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01EA0h]
	vmovdqa ymmword ptr [rbx + 01EA0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01EC0h]
	vmovdqa ymmword ptr [rbx + 01EC0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01EE0h]
	vmovdqa ymmword ptr [rbx + 01EE0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01F00h]
	vmovdqa ymmword ptr [rbx + 01F00h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01F20h]
	vmovdqa ymmword ptr [rbx + 01F20h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01F40h]
	vmovdqa ymmword ptr [rbx + 01F40h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01F60h]
	vmovdqa ymmword ptr [rbx + 01F60h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01F80h]
	vmovdqa ymmword ptr [rbx + 01F80h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01FA0h]
	vmovdqa ymmword ptr [rbx + 01FA0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01FC0h]
	vmovdqa ymmword ptr [rbx + 01FC0h], ymm0
	vmovdqa ymm0, ymmword ptr [rax + 01FE0h]
	vmovdqa ymmword ptr [rbx + 01FE0h], ymm0


	ret
copy_8k endp

.data

	current_rambank qword 0

.code