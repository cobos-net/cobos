echo off
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


set studio=%schemas%\VisualStudioProject.xsd
set manifest=%schemas%\Manifest.xsd

echo on

REM Visual Studio Project schema
REM -----------------------------------------------------------------

%xsd% /classes /n:Intergraph.Oz.Build.Vs %studio% /out:%out%

REM Autobuild manifest schema
REM -----------------------------------------------------------------

%xsd% /classes /n:Intergraph.Oz.Build.Configuration %manifest% /out:%out%
