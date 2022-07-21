#include "generated.h"

//EV1A_1M01 | "Hey, open the door!! HEY!"
const u8 partdata_0[] = { 56, 7, 30, 50, 53, 76, 40, 41, 30, 39, 76, 45, 33, 30, 76, 29, 40, 40, 43, 54, 54, 76, 7, 4, 24, 54, 56 };

//EV1A_1M01 | "Help, anyone!!"
const u8 partdata_1[] = { 56, 7, 30, 37, 41, 53, 76, 26, 39, 50, 40, 39, 30, 54, 54, 56 };

//EV1A_1M01 | "Damnit!"
const u8 partdata_2[] = { 56, 3, 26, 38, 39, 34, 45, 54, 56 };

//EV1A_1M01 | "No luck here!"
const u8 partdata_3[] = { 56, 13, 40, 76, 37, 46, 28, 36, 76, 33, 30, 43, 30, 54, 56 };

//EV1A_1M01 | "If there's another explosion, then--"
const u8 partdata_4[] = { 56, 8, 31, 76, 45, 33, 30, 43, 30, 62, 44, 76, 26, 39, 40, 45, 33, 30, 43, 76, 30, 49, 41, 37, 40, 44, 34, 40, 39, 53, 76, 45, 33, 30, 39, 63, 63, 56 };

//EV1A_1M01 | "No need to panic, Aya."
const u8 partdata_5[] = { 56, 13, 40, 76, 39, 30, 30, 29, 76, 45, 40, 76, 41, 26, 39, 34, 28, 53, 76, 0, 50, 26, 52, 56 };

//EV1A_1M01 | "Someone will find us."
const u8 partdata_6[] = { 56, 18, 40, 38, 30, 40, 39, 30, 76, 48, 34, 37, 37, 76, 31, 34, 39, 29, 76, 46, 44, 52, 56 };

//EV1A_1M01 | "What can I do to help?"
const u8 partdata_7[] = { 56, 22, 33, 26, 45, 76, 28, 26, 39, 76, 8, 76, 29, 40, 76, 45, 40, 76, 33, 30, 37, 41, 55, 56 };

MovieSubtitlePart sub0_parts[] = {
	{(const char*)partdata_0, 27, 0, 200, 248},
	{(const char*)partdata_1, 16, 0, 248, 270},
	{(const char*)partdata_2, 9, 0, 287, 298},
	{(const char*)partdata_3, 15, 0, 343, 358},
	{(const char*)partdata_4, 38, 0, 358, 386},
	{(const char*)partdata_5, 24, 0, 399, 434},
	{(const char*)partdata_6, 23, 0, 476, 493},
	{(const char*)partdata_7, 24, 0, 503, 532},
};

const u32 movieSubtitlesCount = 1;
MovieSubtitle movieSubtitles[] = {
	{43613, 8, sub0_parts},
};