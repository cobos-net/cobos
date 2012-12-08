REM ===========================================================================
REM Filename: var_args.bat
REM Description: retrieves remaining batch file arguments starting from the nth parameter.
REM ---------------------------------------------------------------------------
REM Created by: Nicholas Davis			Date: 2010-04-09
REM Updated by:							Date:
REM ---------------------------------------------------------------------------
REM
REM Usage:
REM		var_args.bat <nth> <...>
REM
REM Description:
REM		nth:	The nth parameter after which to start retrieving the remaining arguments.
REM		...:	Variable length list of arguments.
REM
REM Example:
REM		This is designed to be called from another batch file when you want
REM		to extract the remaing arguments.  Usually you will want to do this
REM		when you pass some of the arguments to another batch file.
REM 	
REM		The batch file example.bat contains the following commands:
REM 
REM		set arg1=%1
REM		set arg2=%2
REM
REM		call var_args 2 %*
REM		echo %var_args%
REM
REM		The batch file example.bat is called with the following arguments:
REM
REM		> example.bat first second third fourth "fifth=5"
REM
REM		The resulting output is:
REM
REM 	> third fourth fifth=5
REM
REM		The result is all remaining arguments after the 2nd argument.
REM		Note the use of quotes around the fifth argument.  If those are 
REM		omitted then the command interpreter treats 'fifth' and '5' as seperate
REM		arguments.
REM ===========================================================================

@setlocal & set var_args=

::strip of first n arguments to be ignored
set nth=%1
shift

for /L %%i in (1,1,%nth%) do shift

::get all remaining arguments passed into the batch file
set var_args=

:LOOP
	if "%~1"=="" goto :END_LOOP
	set var_args=%var_args% %~1
	shift
	goto :LOOP
:END_LOOP

::return value
@endlocal & set var_args=%var_args%