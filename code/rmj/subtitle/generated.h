#ifndef GENERATED_H_
#define GENERATED_H_

#include "platform.h"

struct subtitle {
    const i16 id;
    const char* text;
};

extern const u32 subsCount;
extern const subtitle subs[];

struct movie_subtitle_part {
	const char* text;
	const u8 len;
	u8 textIdx;
	const u16 startFrame;
	const u16 endFrame;
};

struct movie_subtitle {
	const i32 id;
	const u8 partsCount;
	movie_subtitle_part* parts;
};

struct movie_subtitle_displayed {
	const movie_subtitle_part* parts;
	u8 partsCount;
	u8 nextPartIdx;
	u8 ticksTilNext;
};

extern const u32 movieSubsCount;
extern movie_subtitle moviesubs[];


#endif