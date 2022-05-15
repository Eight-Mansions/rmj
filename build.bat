echo Clearing out the old files and create a clean workspace
del /s /q cd\rmj_1\* 1>nul
Xcopy /E /q cd\orig\ cd\rmj_1\ 1>nul
echo:

del /q graphics\*.TMS
tools\rmj_tms_build.exe graphics\TITLE_TMS
del /q cd\rmj_1\TIM\TITLE.TMS
copy graphics\TITLE.TMS cd\rmj_1\TIM\TITLE.TMS

::Copy translation files
::xcopy /e /y trans\* cd\kuro\
::xcopy /e /y fonts\insert\* cd\kuro\

::xcopy /e /y movies\* cd\kuro\
::THE TRANSLATED OP IS CURSED!!!!!!!!!

::insert asm, building new exe
echo Building new EXE file...
copy /y NUL cd\rmj_1\DATA\SUBTITLES1.DAT >NUL
copy exe\orig\SLPS_010.87 exe\SLPS_010.87
tools\armips.exe code\rmj-assembly.asm
::tools\atlas exe\SLPS_005.05 trans\exe_translations.txt >> exe_error.txt
copy exe\SLPS_010.87 cd\rmj_1\SLPS_010.87

::Build files
echo Building final bin file...
pushd cd
del /q rmj_1_working.bin
..\tools\psximager\psxbuild.exe rmj_1.cat rmj_1_working.bin>> build.log
popd
echo:

echo Build complete!