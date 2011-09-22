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
shift
set output_folder=%1
shift
set stylesheets_folder=%1
shift
set xml_namespace=%1
shift

set xslt_args=

:LOOP
	if "%~1"=="" goto :END_LOOP
	set xslt_args=%xslt_args% %~1
	shift
	goto :LOOP
:END_LOOP


REM ---------------------------------------------------------------------------
REM Pre-processing
REM ---------------------------------------------------------------------------

if not exist %output_folder% mkdir %output_folder%

echo --------------------------------------------------------------------------
echo Creating the data model schema...
echo --------------------------------------------------------------------------

%xslt% %data_model% %stylesheets_folder%\datamodel\schema.xslt %output_folder%\DataModel.xsd xmlNamespace=%xml_namespace% %xslt_args%

endlocal