@echo off

SET WORK_DIR=%~dp0

echo Using %WORK_DIR%

cd %WORK_DIR%

robocopy VirtualTargetingSystem\Output Package\GameData\VirtualTargetingSystem /MIR

cd Package
"%WORK_DIR%Tools\7zip\7za.exe" a -tzip -r ..\VirtualTargetingSystem.zip GameData

pause
