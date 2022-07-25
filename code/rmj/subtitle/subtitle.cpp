#include "subtitle.h"

const u8 widths[] =
{
		0x09, // A
		0x08, // B
		0x07, // C
		0x08, // D
		0x07, // E
		0x07, // F
		0x08, // G
		0x08, // H
		0x03, // I
		0x07, // J
		0x08, // K
		0x07, // L
		0x08, // M
		0x08, // N
		0x08, // O
		0x08, // P
		0x08, // Q
		0x08, // R
		0x07, // S
		0x08, // T
		0x07, // U
		0x08, // V
		0x09, // W
		0x07, // X
		0x07, // Y
		0x08, // Z
		0x07, // a
		0x07, // b
		0x07, // c
		0x07, // d
		0x07, // e
		0x05, // f
		0x07, // g
		0x07, // h
		0x03, // i
		0x04, // j
		0x07, // k
		0x04, // l
		0x07, // m
		0x07, // n
		0x07, // o
		0x07, // p
		0x07, // q
		0x06, // r
		0x07, // s
		0x05, // t
		0x07, // u
		0x07, // v
		0x07, // w
		0x07, // x
		0x07, // y
		0x07, // z
		0x04, // .
		0x04, // ,
		0x03, // !
		0x07, // ?
		0x06, // "
		0x04, // (
		0x05, // )
		0x04, // :
		0x04, // ;
		0x07, // ~
		0x03, // '
		0x06, // -
		0x07, // 0
		0x05, // 1
		0x08, // 2
		0x07, // 3
		0x08, // 4
		0x07, // 5
		0x07, // 6
		0x07, // 7
		0x08, // 8
		0x07, // 9
		0x04, //  
};

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

void InitVoiceSubtitle(const char* audioname)
{
	audioSubIdx = -1;
	int audionameHash = sdbmHash(audioname);
	for (int i = 0; i < subsCount; i++)
	{
		if (audionameHash == subs[i].id)
		{
			audioSubIdx = i;
			break;
		}
	}
}

int DisplaySubtitle()
{
	if (audioSubIdx != -1)
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
		char letter = subs[audioSubIdx].text[letterIdx];
		letterIdx++;
		while (letter != 0)
		{
			uv = GetLetterPos(letter);
			returnMe = DisplayFromGraphic16x16(textId, unk1, yx, uv, wh, unk3);
			yx += 0x08;
			letter = subs[audioSubIdx].text[letterIdx];
			letterIdx++;
		}

		audioSubIdx = -1;

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

u32 textX = 320;
u32 textY = 176;
u32 curDrawX = 0;
u32 curDrawY = 0;

void InitMovieSubtitle(const char* videoname)
{
	movieSubIdx = -1;
	int videonameHash = sdbmHash(videoname);
	for (int i = 0; i < movieSubtitlesCount; i++)
	{
		if (videonameHash == movieSubtitles[i].id)
		{
			movieSubIdx = i;
			break;
		}
	}
}

void DrawMovieSubtitle(RECT* area, u16* image, u16* font, u32 curFrame)
{
	u32 sliceW = area->w;
	u32 sliceX = area->x;

	if (movieSubIdx >= 0)
	{
		MovieSubtitle subs = movieSubtitles[movieSubIdx];

		for (int i = 0; i < subs.partsCount; i++)
		{

			if (subs.parts[i].startFrame <= curFrame && curFrame < subs.parts[i].endFrame)
			{
				const char* text = subs.parts[i].text;


				if (sliceX <= subs.parts[i].x && subs.parts[i].x < sliceX + sliceW)
				{
					subs.parts[i].textIdx = 0;
					subs.parts[i].curX = subs.parts[i].x - sliceX;
					subs.parts[i].curY = subs.parts[i].y * 16; // 16 comes from max width of a character = 8 * 2 (16bpp = 2 bytes)
				}

				u16 curX = subs.parts[i].curX;
				u16 curY = subs.parts[i].curY;
				while (subs.parts[i].textIdx < subs.parts[i].len)
				{
					u32 srcPixelPos = text[subs.parts[i].textIdx] * 0x80; // 0x80 is half the width of our letters.  The entire byte count is (w * 2 (16bpp) * h).  We're using shorts or 2 bytes at a time so half.

					bool overflowed = false;
					for (u32 x = 0; x < 8; x++) // 8 is our max letter width... soon will be width of letter
					{
						for (u32 y = 0; y < 256;) // += 16 comes from max width of a character = 8 * 2 (16bpp = 2 bytes)  ----- 256 = may height times the 16 we get from the previous equation
						{
							u32 imgPos = curX + curY + y;

							// 0x8000 is the pixel color of the black background
							u16 sp = font[srcPixelPos++];
							if (sp != 0x8000) image[imgPos] = sp;

							sp = font[srcPixelPos++];
							if (sp != 0x8000) image[imgPos + 16] = sp;

							y += 32;
						}

						curX++;
					}

					subs.parts[i].textIdx++;

					if (curX >= sliceW)
					{
						subs.parts[i].curX = 0;
						break;
					}
				}
			}
		}
	}

	LoadImage(area, (u_long*)image);
}