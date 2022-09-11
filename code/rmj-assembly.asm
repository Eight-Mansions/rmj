.psx

.open "cd\rmj_1\DATA\SUBTITLES1.DAT",0x80100000
	.importobj "code\rmj\obj\subtitle.obj"
	.importobj "code\rmj\obj\generated_audio.obj"
	.importobj "code\rmj\obj\generated_movie.obj"
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

; Code to load subtitles for both audio and video for disc 1
.org 0x80012f3c
	j LoadSubtitles1

.org 0x8001f1d8
	jal InitVoiceSub
	
;.org 0x8001a1cc ; branch to see if debug text is enabled
;	nop

.org 0x8001f328
	j DisplayVoiceSubs
	
.org 0x8001cc88
	j InitMovieSub
	nop
	
.org 0x8004a1a8
	j DisplayMovieSubs
	
.org 0x8006b168
	j StoreFrameNumber
	
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
		
	InitVoiceSub:
		addiu sp, sp, -8
		sw ra, 0(sp)
		jal InitVoiceSubtitle
		sw a0, 4(sp)
		
		lw ra, 0(sp)
		lw a0, 4(sp)
		addiu sp, sp, 8
		
		li v0, 0x01
		j 0x8001f1e0
		lui at, 0x8009

	; DisplayDebug:
		; addiu a0, r0, 0x01
		; jal DisplayTest
		; nop
		; j 0x8001a224
		; nop

	DisplayVoiceSubs:
		jal DisplaySubtitle
		nop
		li a0, 0x02
		jal 0x8004d6e8
		nop
		jal 0x800503c8
		nop
		j 0x8001f330
		nop
	
	DisplayMovieSubs:
		la a2, SubFont
		la a3, framenum
		jal DrawMovieSubtitle
		lw a3, 0(a3)
		
		j 0x8004a1b0
		nop
		
	InitMovieSub:
		addiu sp, sp, -20
		sw ra, 0(sp)
		sw a0, 4(sp)
		sw a1, 8(sp)
		sw a2, 12(sp)
		jal InitMovieSubtitle
		sw a3, 16(sp)
		
		lw ra, 0(sp)
		lw a0, 4(sp)
		lw a1, 8(sp)
		lw a2, 12(sp)
		lw a3, 16(sp)
		addiu sp, sp, 20
		
		lui v0, 0x8009
		j 0x8001cc90
		lw v0, 0x5da8(v0)
		
		
	StoreFrameNumber:
		;8006b164 : LUI     800b0000 (at), 800b (32779),
		;8006b168 : SW      000000f7 (v0), 84a4 (800b0000 (at)) [800a84a4]
		la a0, framenum
		lui at, 0x800B
		sw v0, 0x84a4(at)
		j 0x8006b16c
		sw v0, 0(a0)
	
framenum:
	.dw 0
		
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