REM ===========================================================================
REM Filename: filetime_cmp.bat
REM Description: Compares two files by filetime.
REM ---------------------------------------------------------------------------
REM Created by: Nicholas Davis			Date: 2010-04-09
REM Updated by:							Date:
REM ---------------------------------------------------------------------------
REM
REM Usage:
REM		filetime_cmp.bat <file1> <file2>
REM
REM Description:
REM		file1:	The file to compare against.
REM		file2:	The file to compare with.
REM
REM Example:
REM		This is designed to be called from another batch file when you want
REM		to compare two files by filetime to compare the times the files
REM		were last modified.
REM
REM		The result is stored in the environment variable 'filetime_cmp':
REM
REM		-1: file1 is older than file2
REM		0: 	file1 is the same age as file2
REM		1:	file1 is newer than file2
REM 	
REM		The batch file example.bat contains the following commands:
REM 
REM		call filetime_cmp c:\older.txt c:\newer.txt
REM
REM		if %filetime_cmp% equ -1 (
REM			echo older.txt is older than newer.txt 
REM		) else if %filetime_cmp% equ 0 (
REM			echo older.txt is the same age as newer.txt
REM		) else if %filetime_cmp% equ 1 (
REM			echo older.txt is newer than newer.txt
REM		)
REM
REM		The resulting output is:
REM
REM 	> older.txt is older than newer.txt
REM ===========================================================================

@setlocal & set filetime_cmp=

call filetime_iso8601 %1
set lhs=%filetime_iso8601%

call filetime_iso8601 %2
set rhs=%filetime_iso8601%

if "%lhs%" lss "%rhs%" (
set cmp=-1
) else if "%lhs%" gtr "%rhs%" (
set cmp=1
) else (
set cmp=0
)

::return value
@endlocal & set filetime_cmp=%cmp%