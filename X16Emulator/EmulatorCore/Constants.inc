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

; -- VIA1 ---------------
V_PRB			equ 9f00h
V_PRA			equ 9f01h
V_DDRB			equ 9f02h
V_DDRA			equ 9f03h
V_T1_L			equ 9f04h
V_T1_H			equ 9f05h
V_T1L_L			equ 9f06h
V_T1L_H			equ 9f07h
V_T2_L			equ 9f08h
V_T2_H			equ 9f09h
V_SR			equ 9f0ah
V_ACR			equ 9f0bh
V_PCR			equ 9f0ch
V_IFR			equ 9f0dh
V_IER			equ 9f0eh
V_ORA			equ 9f0fh

; -- VERA ---------------
ADDRx_L			equ 9F20h
ADDRx_M			equ 9F21h
ADDRx_H			equ 9F22h
DATA0			equ 9F23h
DATA1			equ 9F24h
CTRL			equ 9F25h
IEN				equ 9F26h
ISR				equ 9F27h
IRQLINE_L		equ 9F28h
DC_VIDEO		equ 9F29h
DC_HSCALE		equ 9F2Ah
DC_VSCALE		equ 9F2Bh
DC_BORDER		equ 9F2Ch
DC_HSTART		equ 9F29h
DC_HSTOP		equ 9F2Ah
DC_VSTART		equ 9F2Bh
DC_VSTOP		equ 9F2Ch
L0_CONFIG		equ 9F2Dh
L0_MAPBASE		equ 9F2Eh
L0_TILEBASE		equ 9F2Fh
L0_HSCROLL_L	equ 9F30h
L0_HSCROLL_H	equ 9F31h
L0_VSCROLL_L	equ 9F32h
L0_VSCROLL_H	equ 9F33h
L1_CONFIG		equ 9F34h
L1_MAPBASE		equ 9F35h
L1_TILEBASE		equ 9F36h
L1_HSCROLL_L	equ 9F37h
L1_HSCROLL_H	equ 9F38h
L1_VSCROLL_L	equ 9F39h
L1_VSCROLL_H	equ 9F3Ah
AUDIO_CTRL		equ 9F3Bh
AUDIO_RATE		equ 9F3Ch
AUDIO_DATA		equ 9F3Dh
SPI_DATA		equ 9F3Eh
SPI_CTRL		equ 9F3Fh
