.psx
.open "exe\SLPS_010.87",0x8000F800

.definelabel DisplayFromGraphic16x16, 0x8004c588

.org 0x8006c170 ; No clearing out the good stuff in the exe
	nop

.org 0x80091E00
	.importobj "code\rmj\obj\subtitle.obj"


.org 0x8001f2b8
	jal DisplaySubtitle

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