#include "subtitle.h"

//int uv = 0x00120030;
int subidx = 0;

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

	x += 256; // Offset to new font
	return (y << 16 | x);
}

void InitSubtitle(const char* audioname)
{
	printf("%s\n", audioname);
	int audionameHash = sdbmHash(audioname);
	for (int i = 0; i < subsCount; i++)
	{
		if (audionameHash == subs[i].id)
		{
			printf("%d\n", i);
			subidx = i;
			break;
		}
	}
}

int DisplaySubtitle()
{
	printf("%d\n", subidx);
	if (subidx != -1)
	{

		int letterIdx = 0;
		int textId = 0x0A;
		int unk1 = 0; // Apparently always 0?
		int yx = 0x50005c;
		int uv = 0;
		int wh = 0x00100008;
		int unk3 = 0x19; // Is this the "order" on the screen?

		int returnMe = 0; // Do I need to do this?


		letterIdx = 0;
		char letter = subs[subidx].text[letterIdx];
		letterIdx++;
		while (letter != 0)
		{
			printf("%c\n", letter);
			uv = GetLetterPos(letter);
			returnMe = DisplayFromGraphic16x16(textId, unk1, yx, uv, wh, unk3);
			yx += 0x08;
			letter = subs[subidx].text[letterIdx];
			letterIdx++;
		}

	printf("\n");

		subidx = -1;

		return returnMe;
	}
}

void DisplayTest(long id)
{
	//printf("Hi Hi\n");
	FntPrint("%d", 1);
	//FntFlush(id);
}

void DisplayTest2(long id, const char* format, int s, int g, int r)
{
	//FntPrint(id, format, s, g, r);
	FntPrint(id, "Hello");
}

u32 text[] = { 0, 1, 2 };
u32 textIdx = 0;
u32 textLen = 2;
u32 textX = 332;
u32 curDrawX = 0;

u32 overflow[0x80];
u32 overflowedW = 0;

void DrawMovieSubtitle(RECT* area, u16* image, u16* font)
{
	u32 sliceW = area->w;
	u32 sliceX = area->x;

	if (sliceX <= textX && textX <= sliceX + sliceW)
	{
		textIdx = 0;
		curDrawX = textX - sliceX;
	}

	u32 desPixelPos = 0;
	while (textIdx < textLen)
	{
		u32 srcPixelPos = text[textIdx] * 0x80;
		

		for (u32 x = 0; x < 8; x++)
		{
			for (u32 y = 0; y < 16; y++)
			{
				image[desPixelPos + (y * 16) + x] = font[srcPixelPos++];
			}
		}

		textIdx++;
		desPixelPos += 8;

		/*u32 overflowIdx = 0;
		for (u32 y = 0; y < 16; y++)
		{
			u32 destPixelPos = y * 16;
			for (u32 x = curDrawX; x < (curDrawX + 8); x++, srcPixelPos++)
			{
				u16 sp = font[srcPixelPos];
				if (x < sliceW)
				{
					if (sp != 0x8000)
					{
						image[destPixelPos + x] = sp;
					}
				}
				else
				{
					overflow[overflowIdx + (x - sliceW)] = sp;
				}
			}

			overflowIdx += 8;
		}
		textIdx++;
		curDrawX += 8;

		if (curDrawX >= sliceX + sliceW)
		{
			overflowedW = curDrawX - (sliceX + sliceW);
			curDrawX = 0;
			break;
		}*/
	}

	LoadImage(area, (u_long*)image);
}