#ifndef SUBTITLE_H_
#define SUBTITLE_H_

#include <string.h>
#include "platform.h"
#include "generated.h"
#include "generated.h"


extern "C" {
	extern int DisplayFromGraphic16x16(int imgId, int unk1, int unk2, int unk3, int unk4, int unk5);

	void InitSubtitle(const char* audioname);

	int DisplaySubtitle();

	//extern long FntPrint(long id, const char* format, ...);

	//extern u_long* FntFlush(long id);

	void DisplayTest();

	void DisplayTest2(long id, const char* format, int s, int g, int r);


	void InitMovieSubtitle(const char* videoname);

	void DrawMovieSubtitle(RECT* area, u16* image, u16* font, u32 curFrame);

	extern int LoadImage(RECT* rect, u_long* p);

	static int movieSubIdx = -1;
}
#endif