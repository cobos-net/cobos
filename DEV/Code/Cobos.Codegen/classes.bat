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

IF NOT EXIST %output_folder% MKDIR %output_folder%

REM ==========================================================================
REM Create the artefacts
REM ==========================================================================

echo --------------------------------------------------------------------------
echo Creating the expanded data model...
echo --------------------------------------------------------------------------

%xslt% %data_model% %stylesheets_folder%\datamodel\expand.xslt %data_model_processed%

echo --------------------------------------------------------------------------
echo Creating the schema for the strongly typed dataset...
echo --------------------------------------------------------------------------

%xslt% %data_model_processed% %stylesheets_folder%\datamodel\dataset.xslt %output_folder%\Dataset.xsd

echo --------------------------------------------------------------------------
echo Creating the strongly typed dataset...
echo --------------------------------------------------------------------------

%xsd% /dataset /n:%code_namespace% %output_folder%\Dataset.xsd /out:%output_folder%

echo --------------------------------------------------------------------------
echo Creating the C# boilerplate code...
echo --------------------------------------------------------------------------

%xslt% %data_model_processed% %stylesheets_folder%\classes\%code_language%\classes.xslt %output_folder%\DataModel.cs codeNamespace="%code_namespace%" xmlNamespace="%xml_namespace%"

endlocal