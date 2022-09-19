@echo off
set original_disc_1=rmj_1_original.bin
set original_disc_2=rmj_2_original.bin
set working_disc_1=rmj_1_working.bin
set working_disc_2=rmj_2_working.bin

:: Check that the files exist
if not exist cd\%original_disc_1% (
	echo Could not find the original bin
	echo Please verify a file named %original_disc_1% exists in the cd folder
	echo and try again.
	goto :EXIT
)

if not exist cd\%working_disc_1% (
	echo Could not find the translated bin
	echo Please verify a file named %working_disc_1% exists in the cd folder
	echo and try again.
	goto :EXIT
)

:: Create a patch with the two bins
echo Creating patch, please wait...
release\patch_data\xdelta.exe -9 -S none -B 1812725760 -e -vfs cd\%original_disc_1% cd\%working_disc_1% release\patch_data\rmj_disc_1-patch.xdelta
release\patch_data\xdelta.exe -9 -S none -B 1812725760 -e -vfs cd\%original_disc_2% cd\%working_disc_2% release\patch_data\rmj_disc_2-patch.xdelta

echo Patch created successfully in the release\patch_data folder!

:EXIT
echo Press any key to close this window
pause >nul
exit