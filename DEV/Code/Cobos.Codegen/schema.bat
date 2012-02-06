@echo off
setlocal
REM ===========================================================================
REM Filename: schema.bat
REM Description: parses an input data model and generates an XSD schema
REM ---------------------------------------------------------------------------
REM Created by: Nicholas Davis			Date: 2010-04-09
REM Updated by:							Date:
REM ---------------------------------------------------------------------------
REM
REM Usage:
REM		schema.bat <data_model> <output_folder> <stylesheets_folder> <xml_namespace> <xslt_parameters...>
REM
REM Description:
REM		data_model:			path to the Xml data model to process.
REM		output_folder:		destination for all generated code.
REM		stylesheets_folder:	source path for the processing stylesheets.
REM		xml_namespace:		the default namespace for the generated schema.
REM		xslt_parameters:	optional list of parameters for the xslt. 
REM 						each parameter is a key=value pair.  it must be enclosed in quotes
REM							otherwise the = is treated as a delimiter.
REM
REM Example:
REM		schema.bat model.xml .\Generated .\Stylesheets "multiplicityMode=optional" "rootNodeName=SomeRootNode"
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
set xml_namespace=%4

pushd "%codegen%"
call var_args 4 %*
popd

REM ---------------------------------------------------------------------------
REM Pre-processing
REM ---------------------------------------------------------------------------

if not exist %output_folder% mkdir %output_folder%

set output_schema="%output_folder%"\DataModel.xsd

if not exist %output_schema% goto START_BUILD

set build_working_dir=%CD%

pushd "%codegen%"
call filetime_cmp %build_working_dir%\%data_model% %build_working_dir%\%output_schema%
popd

if %filetime_cmp% equ 1 (
	echo WARNING: Detected changes to the data model, starting schema generation...
	goto START_BUILD
) else (
	echo WARNING: No changes detected to the data model, skipping schema generation...
	goto BUILD_EVENT_OK
)

:START_BUILD

echo --------------------------------------------------------------------------
echo Creating the data model schema...
echo --------------------------------------------------------------------------

%xslt% %data_model% %stylesheets_folder%\Datamodel\Schema.xslt %output_schema% xmlNamespace=%xml_namespace% %var_args%

if %errorlevel% neq 0 (
	set error_message="Failed to generate the data model schema."
	goto BUILD_EVENT_FAILED
)

REM ==========================================================================
REM Done processing the data model schemas.
REM ==========================================================================

goto BUILD_EVENT_OK

:BUILD_EVENT_FAILED
echo ERROR: %error_message%
exit 1

:BUILD_EVENT_OK
endlocal