@echo off
call \QualitySystem\BUILD\MSBUILD\autobuild.bat "%~dp0\package.msbuild" Integration "%~dp0\build.state"