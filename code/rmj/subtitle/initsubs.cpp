#include "initsubs.h"
#include <LIBCD.H>
#include <LIBETC.H>



void LoadSubtitles(u_long* subspos)
{
	const char* filename = "\\DATA\\SUBTITLES1.DAT;1";
	CdlFILE	file;

	printf("Reading file %s... ", filename);

	// Search for the file
	if (!CdSearchFile(&file, (char*)filename))
	{
		// Return value is NULL, file is not found
		printf("Not found!\n");
		return;
	}

	// Set seek target (seek actually happens on CdRead())
	CdControl(CdlSetloc, (u_char*)&file.pos, 0);

	// Read sectors
	CdRead((file.size + 2047) / 2048, subspos, CdlModeSpeed);

	// Wait until read has completed
	CdReadSync(0, 0);

	printf("Done.\n");
}