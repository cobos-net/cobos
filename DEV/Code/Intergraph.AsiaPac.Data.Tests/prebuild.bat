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
echo 1. Pre-process database schemas
echo ==========================================================================

echo 1.1 Refresh the Xslt database variables
echo ------------------------------------
%db2xsd% /schema:eadev /output:%stylesheets%\CadDatabase.xsd AEVEN EVENT EVCOM CD_UNITS UN_HI EV_DISPO DIVERT INCIDENT_TRACKING VEHIC PERSO TOW_VEHIC CONT_NAME PROPT

echo 1.2 Convert the database schema to Xslt variables
echo ------------------------------------
%xsltjs% %stylesheets%\CadDatabase.xsd %stylesheets%\DatabaseTypes.xslt %stylesheets%\CadDatabase.xslt

echo ==========================================================================
echo 2. Create the schemas
echo ==========================================================================

echo --------------------------------------------------------------------------
echo 2.1 Create the Xsd for the datamodels
echo --------------------------------------------------------------------------

echo 2.1.1 Events
echo ------------------------------------
%xsltjs% %eventmodel% %stylesheets%\DataModelSchema.xslt %eventout%\DataModel.xsd

echo 2.1.2 Suplemental Information
echo ------------------------------------
%xsltjs% %suppmodel% %stylesheets%\DataModelSchema.xslt %suppout%\DataModel.xsd

echo 2.1.3 Units
echo ------------------------------------
%xsltjs% %unitmodel% %stylesheets%\DataModelSchema.xslt %unitout%\DataModel.xsd

echo 2.1.4 Hews
echo ------------------------------------
%xsltjs% %hewsmodel% %stylesheets%\DataModelSchema.xslt %hewsout%\DataModel.xsd

echo --------------------------------------------------------------------------
echo 2.2 Create the Xsd for the dataset
echo --------------------------------------------------------------------------

echo 2.2.1 Events
echo ----------------------------------
%xsltjs% %eventmodel% %stylesheets%\DataModelDataset.xslt %eventout%\Dataset.xsd

echo 2.2.2 Supplemental Information
echo ----------------------------------
%xsltjs% %suppmodel% %stylesheets%\DataModelDataset.xslt %suppout%\Dataset.xsd

echo 2.2.3 Units
echo ----------------------------------
%xsltjs% %unitmodel% %stylesheets%\DataModelDataset.xslt %unitout%\Dataset.xsd

echo 2.2.4 Hews
echo ----------------------------------
%xsltjs% %hewsmodel% %stylesheets%\DataModelDataset.xslt %hewsout%\Dataset.xsd

echo ==========================================================================
echo 3. Create strongly typed datasets
echo ==========================================================================

echo 3.1 Events
echo ----------------------------------
%xsd% /dataset /n:Intergraph.AsiaPac.Data.Tests.Events %eventout%\Dataset.xsd /out:%eventout%

echo 3.2 Supplemental Information
echo ----------------------------------
%xsd% /dataset /n:Intergraph.AsiaPac.Data.Tests.SupplementalInformation %suppout%\Dataset.xsd /out:%suppout%

echo 3.3 Units
echo ----------------------------------
%xsd% /dataset /n:Intergraph.AsiaPac.Data.Tests.Units %unitout%\Dataset.xsd /out:%unitout%

echo 3.4 Hews
echo ----------------------------------
%xsd% /dataset /n:Intergraph.AsiaPac.Data.Tests.HospitalDiversion %hewsout%\Dataset.xsd /out:%hewsout%

echo ==========================================================================
echo 4. Create C# boilerplate code
echo ==========================================================================

echo 4.1 Events
echo ----------------------------------
%xsltjs% %eventmodel% %stylesheets%\DataModelClasses.xslt %eventout%\DataModel.cs codeNS="Intergraph.AsiaPac.Data.Tests.Events" contractNS="http://www.intergraph.com/asiapac/cad/interop/dataservice/1.0.0"

echo 4.2 Supplemental Information
echo ----------------------------------
%xsltjs% %suppmodel% %stylesheets%\DataModelClasses.xslt %suppout%\DataModel.cs codeNS="Intergraph.AsiaPac.Data.Tests.SupplementalInformation" contractNS="http://www.intergraph.com/asiapac/cad/interop/dataservice/1.0.0"

echo 4.3 Units
echo ----------------------------------
%xsltjs% %unitmodel% %stylesheets%\DataModelClasses.xslt %unitout%\DataModel.cs codeNS="Intergraph.AsiaPac.Data.Tests.Units" contractNS="http://www.intergraph.com/asiapac/cad/interop/dataservice/1.0.0"

echo 4.4 Hews
echo ----------------------------------
%xsltjs% %hewsmodel% %stylesheets%\DataModelClasses.xslt %hewsout%\DataModel.cs codeNS="Intergraph.AsiaPac.Data.Tests.HospitalDiversion" contractNS="http://www.intergraph.com/asiapac/cad/interop/dataservice/1.0.0"
