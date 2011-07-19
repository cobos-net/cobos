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

set stylesheets=..\Intergraph.AsiaPac.Data\Stylesheets
set schemas=.\Schemas

set xsd="C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe"
set xsltjs=%stylesheets%\xslt.js
set db2xsd="\Projects\Intergraph.AsiaPac.DevTools\release build folder\db2xsd\db2xsd.exe"

REM Input data
REM ===========================================================================

set eventmodel=.\Events\DataModel.xml
set suppmodel=.\SupplementalInformation\DataModel.xml
set unitmodel=.\Units\DataModel.xml
set hewsmodel=.\HospitalDiversion\DataModel.xml

REM Output folders
REM ===========================================================================

set eventout=.\Events\Generated
set suppout=.\SupplementalInformation\Generated
set unitout=.\Units\Generated
set hewsout=.\HospitalDiversion\Generated

@echo on

echo ==========================================================================
echo 3. Create strongly typed datasets
echo ==========================================================================

echo 3.1 Events
echo ----------------------------------
%xsd% /dataset /n:Intergraph.AsiaPac.Data.Tests.Events %eventout%\Dataset.xsd /out:%eventout%

