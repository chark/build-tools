@echo off

set "scriptPath=%~dp0"
set "scriptPath=%scriptPath:~0,-1%"

"%ProgramFiles%\Unity\Hub\Editor\2022.3.10f1\Editor\Unity.exe" ^
 -batchmode ^
 -quit ^
 -logFile %scriptPath%\test-build.log ^
 -projectPath "%scriptPath%" ^
 -executeMethod CHARK.BuildTools.Editor.BuildScript.Build ^
 -configurationName "BuildConfiguration_BuildAndArchive_All"
