@echo off
setlocal
REM ===========================================================================
REM Filename: classes.bat
REM Description: parses an input data model and generates all code artefacts
REM ---------------------------------------------------------------------------
REM Created by: Nicholas Davis			Date: 2010-04-09
REM Updated by:							Date:
REM ---------------------------------------------------------------------------
REM
REM Usage:
REM		classes.bat <data_model> <output_folder> <stylesheets_folder> <code_language> <code_namespace> <xml_namespace>
REM
REM Description:
REM		data_model:			path to the Xml data model to process.
REM		output_folder:		destination for all generated code.
REM		stylesheets_folder:	path to the processing stylesheets.
REM		code_language:		code language, e.g. csharp
REM		code_namespace:		the namespace declaration for generated classes.
REM		xml_namespace:		the XML serialization namespace for generated classes.
REM
REM Example:
REM		classes.bat model.xml .\Generated .\Stylesheets CSharp Cobos.Demo.Data http://cobos.co.uk/demo/data/1.0.0
REM
REM ===========================================================================

REM ---------------------------------------------------------------------------
REM Build tools
REM ---------------------------------------------------------------------------

set xsd="C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe"
set codegen="C:\Program Files\Cobos\CoreSDK\0.1\codegen"
set xslt=%codegen%\xslt.js

REM ---------------------------------------------------------------------------
REM Input data
REM ---------------------------------------------------------------------------

set data_model=%1
set output_folder=%2
set stylesheets_folder=%3
set code_language=%4
set code_namespace=%5
set xml_namespace=%6

REM ---------------------------------------------------------------------------
REM Pre-processing
REM ---------------------------------------------------------------------------

set data_model_processed=%output_folder%\DataModel.Processed.xml

if not exist %output_folder% mkdir %output_folder%

if not exist %data_model_processed% goto START_BUILD

set build_working_dir=%CD%

pushd "%codegen%"
call filetime_cmp %build_working_dir%\%data_model% %build_working_dir%\%data_model_processed%
popd

if %filetime_cmp% equ 1 (
	echo WARNING: Detected changes to the data model, starting code generation...
	goto START_BUILD
) else (
	echo WARNING: No changes detected to the data model, skipping code generation...
	goto BUILD_EVENT_OK
)

REM ==========================================================================
REM Create the artefacts
REM ==========================================================================

:START_BUILD

echo --------------------------------------------------------------------------
echo Creating the expanded data model...
echo --------------------------------------------------------------------------

%xslt% %data_model% %stylesheets_folder%\Datamodel\Expand.xslt %data_model_processed%

if %errorlevel% neq 0 (
	set error_message="Failed to expand the data model."
	goto BUILD_EVENT_FAILED
)

echo --------------------------------------------------------------------------
echo Creating the schema for the strongly typed dataset...
echo --------------------------------------------------------------------------

%xslt% %data_model_processed% %stylesheets_folder%\DataModel\Dataset.xslt %output_folder%\Dataset.xsd

if %errorlevel% neq 0 (
	set error_message="Failed to generate the schema for the strongly typed dataset."
	goto BUILD_EVENT_FAILED
)

echo --------------------------------------------------------------------------
echo Creating the strongly typed dataset...
echo --------------------------------------------------------------------------

%xsd% /dataset /n:%code_namespace% %output_folder%\Dataset.xsd /out:%output_folder%

if %errorlevel% neq 0 (
	set error_message="Failed to generate the strongly typed dataset."
	goto BUILD_EVENT_FAILED
)

echo --------------------------------------------------------------------------
echo Creating the C# boilerplate code...
echo --------------------------------------------------------------------------

%xslt% %data_model_processed% %stylesheets_folder%\Classes\%code_language%\Classes.xslt %output_folder%\DataModel.cs codeNamespace="%code_namespace%" xmlNamespace="%xml_namespace%"

if %errorlevel% neq 0 (
	set error_message="Failed to generate C# boilerplate code."
	goto BUILD_EVENT_FAILED
)

REM ==========================================================================
REM Done processing the code artefacts.
REM ==========================================================================

goto BUILD_EVENT_OK

:BUILD_EVENT_FAILED
echo ERROR: %error_message%
exit 1

:BUILD_EVENT_OK
endlocal
