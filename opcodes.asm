'69	ADC	#$44
'65	ADC	$44
'75	ADC	$44, X
'6D	ADC	$4433
'7D	ADC	$4433, X
'79	ADC	$4433, Y
'61	ADC	($44, X)
'71	ADC	($44), Y
'72	ADC	($44)
'29	AND	#$44
'25	AND	$44
'35	AND	$44, X
'2D	AND	$4433
'3D	AND	$4433, X
'39	AND	$4433, Y
'21	AND	($44, X)
'31	AND	($44), Y
'32	AND	($44)
'0A	ASL	
'06	ASL	$44
'16	ASL	$44, X
'0E	ASL	$4433
'1E	ASL	$4433, X
'90	BCC	reldest_0
'B0	BCS	reldest_0
'F0	BEQ	reldest_0
'24	BIT	$44
'2C	BIT	$4433
'30	BMI	reldest_0
'D0	BNE	reldest_0
'10	BPL	reldest_0
'80	BRA	reldest_0
'00	BRK	
'50	BVC	reldest_0
'70	BVS	reldest_0
'18	CLC	
'D8	CLD	
'58	CLI	
'B8	CLV	
'C9	CMP	#$44
'C5	CMP	$44
'D5	CMP	$44, X
'CD	CMP	$4433
'DD	CMP	$4433, X
'D9	CMP	$4433, Y
'C1	CMP	($44, X)
'D1	CMP	($44), Y
'D2	CMP	($44)
'E0	CPX	#$44
'E4	CPX	$44
'EC	CPX	$4433
'C0	CPY	#$44
'C4	CPY	$44
'CC	CPY	$4433
'3A	DEC	
'C6	DEC	$44
'D6	DEC	$44, X
'CE	DEC	$4433
'DE	DEC	$4433, X
'CA	DEX	
'88	DEY	
'49	EOR	#$44
'45	EOR	$44
'55	EOR	$44, X
'4D	EOR	$4433
'5D	EOR	$4433, X
'59	EOR	$4433, Y
'41	EOR	($44, X)
'51	EOR	($44), Y
'52	EOR	($44)
'1A	INC	
'E6	INC	$44
'F6	INC	$44, X
'EE	INC	$4433
'FE	INC	$4433, X
'E8	INX	
'C8	INY	
'4C	JMP	$4433
'6C	JMP	($4433)
'7C	JMP	($4433, X)
'20	JSR	$4433
'A9	LDA	#$44
'A5	LDA	$44
'B5	LDA	$44, X
'AD	LDA	$4433
'BD	LDA	$4433, X
'B9	LDA	$4433, Y
'A1	LDA	($44, X)
'B1	LDA	($44), Y
'B2	LDA	($44)
'A2	LDX	#$44
'A6	LDX	$44
'B6	LDX	$44, Y
'AE	LDX	$4433
'BE	LDX	$4433, Y
'A0	LDY	#$44
'A4	LDY	$44
'B4	LDY	$44, X
'AC	LDY	$4433
'BC	LDY	$4433, X
'4A	LSR	
'46	LSR	$44
'56	LSR	$44, X
'4E	LSR	$4433
'5E	LSR	$4433, X
'EA	NOP	
'09	ORA	#$44
'05	ORA	$44
'15	ORA	$44, X
'0D	ORA	$4433
'1D	ORA	$4433, X
'19	ORA	$4433, Y
'01	ORA	($44, X)
'11	ORA	($44), Y
'12	ORA	($44)
'48	PHA	
'08	PHP	
'DA	PHX	
'5A	PHY	
'68	PLA	
'7A	PLY	
'FA	PLX	
'28	PLP	
'2A	ROL	
'26	ROL	$44
'36	ROL	$44, X
'2E	ROL	$4433
'3E	ROL	$4433, X
'6A	ROR	
'66	ROR	$44
'76	ROR	$44, X
'6E	ROR	$4433
'7E	ROR	$4433, X
'40	RTI	
'60	RTS	
'E9	SBC	#$44
'E5	SBC	$44
'F5	SBC	$44, X
'ED	SBC	$4433
'FD	SBC	$4433, X
'F9	SBC	$4433, Y
'E1	SBC	($44, X)
'F1	SBC	($44), Y
'F2	SBC	($44)
'38	SEC	
'F8	SED	
'78	SEI	
'85	STA	$44
'95	STA	$44, X
'8D	STA	$4433
'9D	STA	$4433, X
'99	STA	$4433, Y
'81	STA	($44, X)
'91	STA	($44), Y
'92	STA	($44)
'64	STZ	$44
'74	STZ	$44, X
'9C	STZ	$4433
'9E	STZ	$4433, X
'86	STX	$44
'96	STX	$44, Y
'8E	STX	$4433
'84	STY	$44
'94	STY	$44, X
'8C	STY	$4433
'DB	STP	
'AA	TAX	
'A8	TAY	
'BA	TSX	
'8A	TXA	
'9A	TXS	
'98	TYA	