@echo off
setlocal
REM ===========================================================================
REM Filename: build.bat
REM Description: 
REM ---------------------------------------------------------------------------
REM Created by:	Nicholas Davis			Date: 2010-04-18
REM Updated by:							Date:
REM ---------------------------------------------------------------------------
REM
REM Usage: 
REM		build.bat <project_folder> <build_config>
REM
REM Description:
REM		project_folder:		The working directory for the project - $(ProjectDir). 
REM		build_config:		The path to the build configuration Xml file.
REM
REM Example:
REM		build.bat c:\demo\project .\BUILD\build.config
REM
REM ===========================================================================

REM ---------------------------------------------------------------------------
REM Build tools
REM ---------------------------------------------------------------------------

set codegen="C:\Program Files\Cobos\CoreSDK\0.1\codegen"
set xslt=%codegen%\xslt.js

REM ---------------------------------------------------------------------------
REM Intialise data
REM ---------------------------------------------------------------------------

set output_folder=%1
set build_config=%2

REM ---------------------------------------------------------------------------
REM Start the build process
REM ---------------------------------------------------------------------------

cd %output_folder%

echo --------------------------------------------------------------------------
echo Processing the build configuration...
echo --------------------------------------------------------------------------

%xslt% %build_config% %codegen%\stylesheets\build\buildconfig.xslt %output_folder%\~build.u.bat targetdir=%output_folder%

REM --------------------------------------------------------------------------
REM Convert generated batch file from Unicode to ANSI...
REM --------------------------------------------------------------------------

cmd.exe /a /c type %output_folder%\~build.u.bat>%output_folder%\~build.bat
del %output_folder%\~build.u.bat

echo --------------------------------------------------------------------------
echo Processing the build configuration...
echo --------------------------------------------------------------------------

call %output_folder%\~build.bat

endlocal