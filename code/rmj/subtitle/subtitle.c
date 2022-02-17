#include "subtitle.h" // empty for now but you never know...

//int uv = 0x00120030;

int GetLetterPos(char letter)
{
	int y = 0;
	int x = 0;

	letter -= 0x41;
	y = (letter / 13);
	x = letter - (y * 13);
	y *= 16;
	x *= 8;

	x += 208; // Offset to new font
	return (y << 16 | x);
}

int DisplaySubtitle()
{
	/*8001f2b8 d3 2e 01 0c     jal        FUN_8004bb4c
	8001f2bc 00 00 00 00     _nop
	8001f2c0 10 00 c2 af     sw         v0, local_10(s8)*/

	char covid[5] = { 'C', 'O', 'V', 'I', 'D' };
	//char covid[5] = { 'A', 'A', 'A', 'A', 'A' };
	int textId = 0x0A;
	int unk1 = 0; // Apparently always 0?
	int yx = 0x50005c;
	int uv = 0;
	int wh = 0x00100008;
	int unk3 = 0x19; // Is this the "order" on the screen?

	int returnMe = 0; // Do I need to do this?

	int i = 0;
	for (; i < 5; i++)
	{
		uv = GetLetterPos(covid[i]);//0x00120030;
		returnMe = DisplayFromGraphic16x16(textId, unk1, yx, uv, wh, unk3);
		yx += 0x10;
	}	

	//local_10 = FUN_8004bb4c(3,0,0xd8006e,0x19);
	

	return returnMe;
}