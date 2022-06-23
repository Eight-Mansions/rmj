.psx

.open "cd\rmj_1\DATA\SUBTITLES1.DAT",0x80100000
	.importobj "code\rmj\obj\subtitle.obj"
	.importobj "code\rmj\obj\generated.obj"
SubFont:
	.incbin "graphics\font\sub_font.bin" ; Font used for subtitles
.close

.open "exe\SLPS_010.87",0x8000F800

.definelabel DisplayFromGraphic16x16, 0x8004c588
.definelabel FntPrint, 0x80060e8c
.definelabel FntFlush, 0x80060b74
.definelabel CdControl, 0x80067798
.definelabel CdGetSector, 0x80067b60
.definelabel CdSearchFile, 0x80069590
.definelabel CdRead, 0x8006a598
.definelabel CdReadSync, 0x8006a69c
.definelabel memcpy, 0x8005ead4
.definelabel VSync, 0x8006b888
.definelabel printf, 0x8005ed78
.definelabel LoadImage, 0x80062394


.org 0x8006c170 ; No clearing out the good stuff in the exe
	nop

.org 0x8001f2a8 ; Swap first variable to the filename to be used for audio
	;li a0, 0x03
	lw a0, 0x0020(fp)

.org 0x8001f2b8
	jal InitSubtitle
	
.org 0x80012f3c
	j LoadSubtitles1
	
;.org 0x8001a1cc ; branch to see if debug text is enabled
;	nop
	
.org 0x8001f320
	j DisplaySubs
	
.org 0x8004a1a8
	j DisplayMovieSubs
	
;.org 0x8001a1cc
;	j DisplayDebug	; take over BNE here that enables debug printing
	
;.org 0x8001a21c
;	nop				; clobber normal debug print

;.org 0x8001a21c
;	jal DisplayTest2

.org 0x80091E00
	.importobj "code\rmj\obj\initsubs.obj"

	LoadSubtitles1:
		addiu sp, sp, -4
		sw ra, 0(sp)
		la a0, 0x80100000
		jal LoadSubtitles
		nop
		lw ra, 0(sp)
		nop
		jr ra ; 0x80091f0c
		addiu sp, sp, 4

	; DisplayDebug:
		; addiu a0, r0, 0x01
		; jal DisplayTest
		; nop
		; j 0x8001a224
		; nop

	DisplaySubs:
		jal DisplaySubtitle
		nop
		li a0, 0x02
		jal 0x8004d6e8
		nop
		jal 0x800501c4
		nop
		j 0x8001f328
		nop
	
	DisplayMovieSubs:
		la a2, SubFont
		jal DrawMovieSubtitle
		nop
		j 0x8004a1b0
; .org 0x8001f320
	; nop

; Below is what gets set for the subtitle
; CPU.PC:    80064130
; DMA.Start: 000A922C
; DMA.Link:  00124988
; ---
; 64808080
; 00D8006E
; 7FF0F100
; 000E0064

; When 0x8001f2b8 is called v0 is set to 0x00D8 006E


;.org 0x8001f2a8
;	addiu a0, r0, 05
; Sets dest x,y of image?
;.org 0x8001F274
;	nop

;Stores the stuff to show the subtitle
;.org 0x8001f2b8
;	nop
	
; Below calls the code to display the subtitle graphic
;.org 0x8001f318
;	nop

.close