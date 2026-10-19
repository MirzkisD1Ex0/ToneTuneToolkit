@echo off
rem Build zcam_ssp: console test (static include) + Unity DLL
rem Decoder: ffmpeg avcodec (in-process). Runtime deps: libssp.dll, avcodec-63.dll, avutil-61.dll
setlocal
set VCVARS=C:\Workflow\Software\Microsoft Visual Studio\18\BuildTools\VC\Auxiliary\Build\vcvars64.bat
set LIBSSP=D:\Workflow\Project\Unity\ZCAM\Materials\libssp-master
set FFSHR=D:\Workflow\Project\Unity\ZCAM\Materials\ffmpeg-shared
rem output dir ends with ~ so Unity ignores it (avoids duplicate plugin import)
set OUTDIR=%~dp0build~

call "%VCVARS%" >nul 2>&1
if errorlevel 1 ( echo [ERR] vcvars failed & exit /b 1 )
if not exist "%OUTDIR%" mkdir "%OUTDIR%"

set INC=/I "%LIBSSP%\include" /I "%FFSHR%\include"
set LNKLNK=/link /LIBPATH:"%LIBSSP%\lib\win_x64_vs2017" /LIBPATH:"%FFSHR%\lib" libssp.lib avcodec.lib avutil.lib

rem --- console test exe (compiles zcam_ssp.cpp directly, no DLL boundary) ---
cl /nologo /W3 /EHsc /O2 /utf-8 %INC% "%~dp0zcam_ssp.cpp" "%~dp0ssp_test.cpp" /Fe:"%OUTDIR%\ssp_test.exe" %LNKLNK% /SUBSYSTEM:CONSOLE
if errorlevel 1 ( echo [ERR] test build failed & exit /b 1 )

rem --- Unity plugin DLL ---
cl /nologo /W3 /EHsc /O2 /utf-8 /LD %INC% "%~dp0zcam_ssp.cpp" /Fe:"%OUTDIR%\zcam_ssp.dll" %LNKLNK%
if errorlevel 1 ( echo [ERR] dll build failed & exit /b 1 )

copy /y "%LIBSSP%\lib\win_x64_vs2017\libssp.dll" "%OUTDIR%\" >nul
copy /y "%FFSHR%\bin\avcodec-63.dll" "%OUTDIR%\" >nul
copy /y "%FFSHR%\bin\avutil-61.dll" "%OUTDIR%\" >nul
copy /y "%FFSHR%\bin\swresample-7.dll" "%OUTDIR%\" >nul
copy /y "%OUTDIR%\zcam_ssp.dll" "%~dp0..\zcam_ssp.dll" >nul
copy /y "%FFSHR%\bin\avcodec-63.dll" "%~dp0..\" >nul
copy /y "%FFSHR%\bin\avutil-61.dll" "%~dp0..\" >nul
copy /y "%FFSHR%\bin\swresample-7.dll" "%~dp0..\" >nul
echo [OK] build complete: %OUTDIR% (dlls deployed to Plugins)
endlocal
