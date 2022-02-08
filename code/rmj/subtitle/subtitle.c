#include "subtitle.h" // empty for now but you never know...

int DisplaySubtitle()
{
	/*8001f2b8 d3 2e 01 0c     jal        FUN_8004bb4c
	8001f2bc 00 00 00 00     _nop
	8001f2c0 10 00 c2 af     sw         v0, local_10(s8)*/

	//local_10 = FUN_8004bb4c(3,0,0xd8006e,0x19);
	int imgId = 0x0A;
	int unk1 = 0x00;
	int xy = 0x00600095; // x / y
	int unk3 = 0x00;
	int wh = 0x00100010;  // width and height, 16x16 hard coded due to font
	int y = 0xFF;
	return DisplayFromGraphic16x16(imgId, unk1, xy, unk3, wh, y);
}