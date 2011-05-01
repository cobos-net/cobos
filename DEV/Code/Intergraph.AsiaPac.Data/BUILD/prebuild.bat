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

REM Build tools & processing stylesheets
REM ===========================================================================

set schema=.\Schemas
set stylesheets=.\Stylesheets

set xsd="C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe"
set xsltjs=%stylesheets%\xslt.js
set db2xsd="\Projects\Intergraph.AsiaPac.DevTools\release build folder\db2xsd\db2xsd.exe"

REM Input data
REM ===========================================================================

set datetimerange=%schema%\DateTimeRange.xsd

REM Output folders
REM ===========================================================================

set utilitiesout=.\Utilities\Generated

IF NOT EXIST %utilitiesout% MKDIR %utilitiesout%

@echo on

echo ==========================================================================
echo 1. Utilities classes
echo ==========================================================================

%xsd% /classes /n:Intergraph.AsiaPac.Data.Utilities %datetimerange% /out:%utilitiesout%
