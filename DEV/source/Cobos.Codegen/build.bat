@echo off
setlocal
REM ===========================================================================
REM Filename: build.bat
REM Description: Bootstrap for the code gen process. Generates a build batch
REM 	file from the supplied build configuration and invokes the build script.
REM ---------------------------------------------------------------------------
REM Created by:	Nicholas Davis			Date: 2010-04-18
REM Updated by:					Date:
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

set COBOS_VERSION=0.2

REM ---------------------------------------------------------------------------
REM Build tools
REM ---------------------------------------------------------------------------

set codegen="C:\Program Files\Cobos\CoreSDK\%COBOS_VERSION%\codegen"
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

echo ==========================================================================
echo Processing the build configuration...
echo ==========================================================================

%xslt% %build_config% %codegen%\Stylesheets\Build\BuildConfig.xslt %output_folder%\~build-unicode.bat targetdir=%output_folder% codegen=%codegen%

if %errorlevel% neq 0 (
	set error_message="Failed to generate the build script from the build configuration."
	goto BUILD_EVENT_FAILED
)

REM --------------------------------------------------------------------------
REM Convert generated batch file from Unicode to ANSI...
REM --------------------------------------------------------------------------

cmd.exe /a /c type %output_folder%\~build-unicode.bat>%output_folder%\~build.bat
del %output_folder%\~build-unicode.bat

if %errorlevel% neq 0 (
	set error_message="Failed to convert the intermediate build script from unicode."
	goto BUILD_EVENT_FAILED
)

echo ==========================================================================
echo Running the build script...
echo ==========================================================================

call %output_folder%\~build.bat

echo ==========================================================================
echo Code generation build script finished.
echo ==========================================================================

goto BUILD_EVENT_OK

:BUILD_EVENT_FAILED
echo ERROR: %error_message%
exit 1

:BUILD_EVENT_OK
endlocal
