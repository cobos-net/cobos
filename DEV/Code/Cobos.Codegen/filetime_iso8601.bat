REM ===========================================================================
REM Filename: filetime_iso8601.bat
REM Description: Converts a DOS filetime to ISO-8601 format.
REM ---------------------------------------------------------------------------
REM Created by: Nicholas Davis			Date: 2010-04-09
REM Updated by:							Date:
REM ---------------------------------------------------------------------------
REM
REM Usage:
REM		filetime_iso8601.bat <path>
REM
REM Description:
REM		path:		Path to any file.
REM
REM Example:
REM		This is designed to be called from another batch file when you want
REM		to get the filetime in ISO-8601 format.  This is useful for comparing
REM		filetimes for processing and sorting.
REM
REM		The result is stored in the environment variable 'filetime_iso8601'
REM 	
REM		The batch file example.bat contains the following commands:
REM 
REM		dir c:\example.txt
REM		call filetime_iso8601 c:\example.txt
REM		echo %filetime_iso8601%
REM
REM		The resulting output (edited for clarity) is:
REM
REM 	> ...
REM 	> 				06/02/2012  10:56 AM                 0 example.txt
REM		> ...
REM		> 2012-02-06T10:56
REM ===========================================================================

@setlocal & set filetime_iso8601=

if not exist %1 (
	echo ERROR: Invalid file path: %1
	exit 1
)

for %%f in (%1) do ( set filetime=%%~tf )

::filetime format is: 12/01/2012 10:17 AM

set year=%filetime:~6,4%
set month=%filetime:~3,2%
set day=%filetime:~0,2%
set hour=%filetime:~11,2%
set min=%filetime:~14,2%
set period=%filetime:~17,2%

if "%period%" == "PM" (
	if "%hour%" lss "12" (
		set /a hour=%hour% + 12
	)
)

::return value
@endlocal & set filetime_iso8601=%year%-%month%-%day%T%hour%:%min%