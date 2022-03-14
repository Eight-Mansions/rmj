#include "subtitle.h"
#include "generated.h"

//int uv = 0x00120030;

int sdbmHash(const char* text) {
	int hash = 0;
	int i = 0;

	for (; text[i] != 0; i++) {
		hash = text[i] + (hash << 6) + (hash << 16) - hash;
	}
	return hash & 0xFFFF;
}

int GetLetterPos(char letter)
{
	int y = 0;
	int x = 0;

	letter -= 1;
	y = (letter / 26);
	x = letter - (y * 26);
	y *= 16;
	x *= 8;

	x += 208; // Offset to new font
	return (y << 16 | x);
}

int DisplaySubtitle(const char* audioname)
{
	int letterIdx = 0;
	int textId = 0x0A;
	int unk1 = 0; // Apparently always 0?
	int yx = 0x50005c;
	int uv = 0;
	int wh = 0x00100008;
	int unk3 = 0x19; // Is this the "order" on the screen?

	int returnMe = 0; // Do I need to do this?

	int audionameHash = sdbmHash(audioname);
	for (int i = 0; i < subsCount; i++)
	{
		if (audionameHash == subs[i].id)
		{
			letterIdx = 0;
			char letter = subs[i].text[letterIdx];
			letterIdx++;
			while (letter != 0)
			{
				uv = GetLetterPos(letter);
				returnMe = DisplayFromGraphic16x16(textId, unk1, yx, uv, wh, unk3);
				yx += 0x08;
				letter = subs[i].text[letterIdx];
				letterIdx++;
			}
		}
	}

	return returnMe;
}