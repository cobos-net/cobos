@echo off
call \QualitySystem\BUILD\MSBUILD\autobuild.bat "%~dp0\package.msbuild" Production "%~dp0\build.state"