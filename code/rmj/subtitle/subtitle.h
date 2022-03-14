#ifndef SUBTITLE_H_
#define SUBTITLE_H_

#include <string.h>
#include "platform.h"

extern "C" {
	extern int DisplayFromGraphic16x16(int imgId, int unk1, int unk2, int unk3, int unk4, int unk5);

	extern int DisplaySubtitle(const char* audioname);
}
#endif