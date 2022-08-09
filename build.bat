@echo off
set working_name=rmj_1

echo Clearing out the old files and creating a clean workspace...
del /s /q cd\%working_name%\* 1>nul
Xcopy /E /q cd\orig\ cd\%working_name%\ 1>nul
echo:

echo Inserting the complex graphics...
del /q graphics\*.TMS 1>nul
tools\rmj_tms_build.exe graphics\TITLE_TMS
del /q cd\%working_name%\TIM\TITLE.TMS
copy graphics\TITLE.TMS cd\%working_name%\TIM\TITLE.TMS
echo:

echo Copying the raw TIM files over...
Xcopy /E /q /Y graphics\tims\ cd\%working_name%\ 1>nul
echo:

REM ::insert asm, building new exe
echo Building new EXE file...
copy /y NUL cd\%working_name%\DATA\SUBTITLES1.DAT >NUL
copy exe\orig\SLPS_010.87 exe\SLPS_010.87
tools\armips.exe code\rmj-assembly.asm
tools\atlas exe\SLPS_005.05 trans\exe_translations.txt >> exe_error.txt
copy exe\SLPS_010.87 cd\%working_name%\SLPS_010.87
echo:

::Build files
echo Building final bin file...
pushd cd
del /q %working_name%_working.bin
..\tools\psximager\psxbuild.exe "%working_name%.cat" "%working_name%_working.bin">> build.log
popd
echo:

echo Build complete!
echo:
pause