@echo off

set "FOLDER=.\RPSX"

rem Проверим, есть ли в папке хоть один файл или подкаталог
dir /b "%FOLDER%" | findstr . >nul

if errorlevel 1 (
    echo RPSX is empty. Building public part...
    dotnet build -c Release SpaceStation14.sln
) else (
    echo RPSX is not empty. Building hole part...
    dotnet build -c Release RPSX.sln
    cd RPSX
    .\sync.sh -y
)

cd ..
pause