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

io_afterwrite proc
	dec r13
	lea rax, io_registers_write
	jmp qword ptr [rax + r13 * 8]
io_afterwrite endp

io_unsupported proc
	ret
io_unsupported endp

io_registers_write:
	; VIA1
	io_9f00 qword io_unsupported
	io_9f01 qword io_unsupported
	io_9f02 qword io_unsupported
	io_9f03 qword io_unsupported
	io_9f04 qword via_timer1_counter_l
	io_9f05 qword via_timer1_counter_h
	io_9f06 qword via_timer1_latch_l
	io_9f07 qword via_timer1_latch_h
	io_9f08 qword via_timer2_latch_l
	io_9f09 qword via_timer2_latch_h
	io_9f0a qword io_unsupported
	io_9f0b qword via_acl
	io_9f0c qword io_unsupported
	io_9f0d qword via_ifr
	io_9f0e qword via_ier
	io_9f0f qword io_unsupported

	; Unused
	io_9f10 qword io_unsupported
	io_9f11 qword io_unsupported
	io_9f12 qword io_unsupported
	io_9f13 qword io_unsupported
	io_9f14 qword io_unsupported
	io_9f15 qword io_unsupported
	io_9f16 qword io_unsupported
	io_9f17 qword io_unsupported
	io_9f18 qword io_unsupported
	io_9f19 qword io_unsupported
	io_9f1a qword io_unsupported
	io_9f1b qword io_unsupported
	io_9f1c qword io_unsupported
	io_9f1d qword io_unsupported
	io_9f1e qword io_unsupported
	io_9f1f qword io_unsupported

	vera_w_9f20 qword vera_update_addrl
	vera_w_9f21 qword vera_update_addrm
	vera_w_9f22 qword vera_update_addrh
	vera_w_9f23 qword vera_update_data
	vera_w_9f24 qword vera_update_data
	vera_w_9f25 qword vera_update_ctrl
	vera_w_9f26 qword vera_update_ien
	vera_w_9f27 qword vera_update_isr
	vera_w_9f28 qword vera_update_irqline_l
	vera_w_9f29 qword vera_update_9f29
	vera_w_9f2a qword vera_update_9f2a
	vera_w_9f2b qword vera_update_9f2b
	vera_w_9f2c qword vera_update_9f2c
	vera_w_9f2d qword vera_update_l0config
	vera_w_9f2e qword vera_update_l0mapbase
	vera_w_9f2f qword vera_update_l0tilebase
	vera_w_9f30 qword vera_update_l0hscroll_l
	vera_w_9f31 qword vera_update_l0hscroll_h
	vera_w_9f32 qword vera_update_l0vscroll_l
	vera_w_9f33 qword vera_update_l0vscroll_h
	vera_w_9f34 qword vera_update_l1config
	vera_w_9f35 qword vera_update_l1mapbase
	vera_w_9f36 qword vera_update_l1tilebase
	vera_w_9f37 qword vera_update_l1hscroll_l
	vera_w_9f38 qword vera_update_l1hscroll_h
	vera_w_9f39 qword vera_update_l1vscroll_l
	vera_w_9f3a qword vera_update_l1vscroll_h
	vera_w_9f3b qword vera_update_notimplemented
	vera_w_9f3c qword vera_update_notimplemented
	vera_w_9f3d qword vera_update_notimplemented
	vera_w_9f3e qword vera_update_notimplemented
	vera_w_9f3f qword vera_update_notimplemented

.code