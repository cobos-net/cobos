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

set xsd="C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe"
set models=.\Log
set out=.\Log\Generated

REM Log File schema
REM -----------------------------------------------------------------

set logfile=%models%\LogFile.xsd

@echo on

%xsd% /classes /n:Cobos.Core.Log %logfile% /out:%out%