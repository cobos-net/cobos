@echo off
REM =================================================================
REM Filename: prebuild.bat
REM Description: runs xsd.exe to generate C# classes
REM -----------------------------------------------------------------
REM Created by:                 Date:
REM Updated by:                 Date:
REM -----------------------------------------------------------------
REM Notes: Requires 1 parameter to set working directory to project
REM directory so that the relative paths work.
REM
REM =================================================================

cd %1

REM Global build parameters
REM -----------------------------------------------------------------

set xsd="C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe"
set schemas=.\Schemas
set out=.\Generated

REM Log File schema
REM -----------------------------------------------------------------

set logfile=%schemas%\LogFile.xsd

@echo on

%xsd% /classes /n:Cobos.Core.Logger %logfile% /out:%out%