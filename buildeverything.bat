@echo off
dotnet build -c Release --property WarningLevel=0
cd RPSX
dotnet build -c Release --property WarningLevel=0

set "BASH_EXE=C:\Program Files\Git\bin\bash.exe"
set "BASE_DIR=%~dp0"
set "SCRIPT_PATH=%BASE_DIR%RPSX/sync.sh"

"%BASH_EXE%" -c "/bin/bash '%SCRIPT_PATH%' -y"
cd ..
