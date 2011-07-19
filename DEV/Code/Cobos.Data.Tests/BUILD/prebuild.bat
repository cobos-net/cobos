@echo off
REM ===========================================================================
REM Filename: prebuild.bat
REM Description: runs xsd.exe to generate C# classes
REM ---------------------------------------------------------------------------
REM Created by:                 Date:
REM Updated by:                 Date:
REM ---------------------------------------------------------------------------
REM Notes: Requires 1 parameter to set working directory to project
REM directory so that the relative paths work.
REM
REM ===========================================================================

cd %1

REM Build tools & processing stylesheets
REM ===========================================================================

set stylesheets=..\Cobos.Data\Stylesheets
set schemas=.\Schemas

set xsd="C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe"
set xsltjs=%stylesheets%\xslt.js
set db2xsd="\Projects\Cobos.DevTools\RELEASE\Cobos.DatabaseToXsd\bin\Cobos.DatabaseToXsd.exe"

REM Input data
REM ===========================================================================


REM Output folders
REM ===========================================================================

@echo on

echo ==========================================================================
echo 1. Pre-process database schemas
echo ==========================================================================

echo 1.1 Refresh the Xslt database variables
echo ------------------------------------
%db2xsd% /schema:eadev /output:%stylesheets%\CadDatabase.xsd AEVEN EVENT EVCOM CD_UNITS UN_HI DISPASS_EVENT EV_DISPO DIVERT INCIDENT_TRACKING VEHIC PERSO TOW_VEHIC CONT_NAME PROPT

echo 1.2 Convert the database schema to Xslt variables
echo ------------------------------------
%xsltjs% %stylesheets%\CadDatabase.xsd %stylesheets%\DatabaseTypes.xslt %stylesheets%\CadDatabase.xslt
