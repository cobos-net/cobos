@echo off
setlocal
REM ===========================================================================
REM Filename: database.bat
REM Description: processes database tables and constraints into XSD and XSLT.
REM The resulting schema and XSLT variables are fed into the toolchain as
REM pre-requisities for processing data models into code artefacts.
REM ---------------------------------------------------------------------------
REM Created by: Nicholas Davis			Date: 2010-04-09
REM Updated by:							Date:
REM ---------------------------------------------------------------------------
REM
REM Usage:
REM		database.bat <stylesheets_folder> <platform> "<db_connection>" <db_schema> <table_args...>
REM
REM Description:
REM		stylesheets_folder:	source & destination path for the processing stylesheets.
REM 	platform:			the database platform e.g. Oracle
REM		db_connection:		the database connection string (enclosed in quotes)
REM		db_schema:			the database schema containing the tables for processing.
REM		table_args:			optional list of parameters for processing database/../schema.xslt.
REM
REM Example:
REM		database.bat .\Stylesheets Oracle myschema TABLE_1 TABLE_2 TABLE_3
REM
REM ===========================================================================

REM ---------------------------------------------------------------------------
REM Build tools & processing stylesheets
REM ---------------------------------------------------------------------------

set db2xsd="C:\Program Files\Cobos\CoreSDK\0.1\bin\Cobos.DatabaseToXsd.exe"
set codegen="C:\Program Files\Cobos\CoreSDK\0.1\codegen"
set xslt=%codegen%\xslt.js

REM ---------------------------------------------------------------------------
REM Input data
REM ---------------------------------------------------------------------------

set stylesheets_folder=%1
shift
set db_platform=%1
shift
set db_connection=%1
shift
set db_schema=%1
shift

set table_args=

:LOOP
	if "%1"=="" goto :END_LOOP
	set table_args=%table_args% %1
	shift
	goto :LOOP
:END_LOOP

REM ==========================================================================
REM Process the database schemas
REM ==========================================================================

echo --------------------------------------------------------------------------
echo Refreshing the Xsd database schema...
echo --------------------------------------------------------------------------

%db2xsd% /connection:%db_connection% /schema:%db_schema% /output:%stylesheets_folder%\database\schema.xsd %table_args%

echo --------------------------------------------------------------------------
echo Converting the database schema to Xslt variables...
echo --------------------------------------------------------------------------

%xslt% %stylesheets_folder%\database\schema.xsd %stylesheets_folder%\database\%db_platform%\merge.xslt %stylesheets_folder%\database\schema.xslt

endlocal
