#ifndef INITSUBS_H_
#define INITSUBS_H_

#include "platform.h"

extern "C" {
	extern void* memcpy(u_char* param_1, u_char* param_2, int param_3); // I wan't to smack whoever did the SDK and just blanked this out...
	void LoadSubtitles(u_long* subspos);
}

#endif