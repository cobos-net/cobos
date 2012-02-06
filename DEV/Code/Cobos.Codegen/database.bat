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
REM		table_args:			optional list of parameters for processing Database/../Schema.xslt.
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
set db_platform=%2
set db_connection=%3
set db_schema=%4

pushd "%codegen%"
call var_args 4 %*
popd

REM ==========================================================================
REM Process the database schemas
REM ==========================================================================

echo --------------------------------------------------------------------------
echo Refreshing the Xsd database schema...
echo --------------------------------------------------------------------------

%db2xsd% /connection:%db_connection% /schema:%db_schema% /output:%stylesheets_folder%\Database\Schema.xsd %var_args%

if %errorlevel% neq 0 (
	set error_message="Failed to generate the Database schema.  Check the database connection and parameters."
	goto BUILD_EVENT_FAILED
)

echo --------------------------------------------------------------------------
echo Converting the database schema to Xslt variables...
echo --------------------------------------------------------------------------

%xslt% %stylesheets_folder%\database\schema.xsd %stylesheets_folder%\Database\%db_platform%\Merge.xslt %stylesheets_folder%\Database\Schema.xslt

if %errorlevel% neq 0 (
	set error_message="Failed to generate the Database schema variables stylesheet."
	goto BUILD_EVENT_FAILED
)

REM ==========================================================================
REM Done processing the Database schemas.
REM ==========================================================================

goto BUILD_EVENT_OK

:BUILD_EVENT_FAILED
echo ERROR: %error_message%
exit 1

:BUILD_EVENT_OK
endlocal
