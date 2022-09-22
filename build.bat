@echo off
:: Parameters are passed via "build_disc_1.bat" or "build_disc_2.bat"
set working_name=%1
set disc=%2
set exe=%3

echo Building disc %disc%
echo:

echo Clearing out the old files and creating a clean workspace...
del /s /q cd\%working_name%_%disc%\* 1>nul
Xcopy /E /q cd\orig_%disc%\ cd\%working_name%_%disc%\ 1>nul
echo:

echo Inserting the complex graphics...
del /q graphics\*.TMS 1>nul
tools\rmj_tms_build.exe graphics\TITLE_TMS
del /q cd\%working_name%_%disc%\TIM\TITLE.TMS
copy graphics\TITLE.TMS cd\%working_name%_%disc%\TIM\TITLE.TMS
echo:

echo Copying the raw TIM files over...
Xcopy /E /q /Y graphics\tims\ cd\%working_name%_%disc%\ 1>nul
Xcopy /E /q /Y graphics\tims_disc_%disc%\ cd\%working_name%_%disc%\ 1>nul
echo:

echo Generate audio subtitles for disc 1
tools\rmj_generate_audio_subtitles.exe trans\audio_subs\audio_translation.txt tools\font_mapping.txt disc1

echo Generate audio subtitles for disc 2
tools\rmj_generate_audio_subtitles.exe trans\audio_subs\audio_translation.txt tools\font_mapping.txt disc2

echo Generate movie subtitles
tools\rmj_generate_movie_subtitles.exe trans\movie_subs tools\movie_mapping.txt

echo Building RMJ PSX code for disc 1
pushd code\rmj
pmake -e RELMODE=DEBUG clean
mkdir Debug
pmake -e RELMODE=DEBUG -e OUTFILE=main -e OPTIMIZE=2 -f disc1

pmake -e RELMODE=DEBUG clean
mkdir Debug
pmake -e RELMODE=DEBUG -e OUTFILE=main -e OPTIMIZE=2 -f disc2
popd

REM ::insert asm, building new exe
echo Building new EXE file...
copy /y NUL cd\%working_name%_%disc%\DATA\SUBTITLES1.DAT >NUL
copy exe\orig_%disc%\%exe% exe\%exe%
tools\armips.exe code\rmj-assembly_%disc%.asm
::tools\atlas exe\SLPS_005.05 trans\exe_translations.txt >> exe_error.txt
copy exe\%exe% cd\%working_name%_%disc%\%exe%
echo:

::Build files
echo Building final bin file...
pushd cd
del /q %working_name%_%disc%_working.bin
..\tools\psximager\psxbuild.exe "%working_name%_%disc%.cat" "%working_name%_%disc%_working.bin">> build.log
popd
echo:

echo Build complete!
echo: