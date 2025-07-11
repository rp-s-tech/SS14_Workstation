#!/bin/bash

FOLDER="./RPSX"

# Проверим, есть ли в папке хоть один файл или подкаталог
if [ -z "$(ls -A "$FOLDER")" ]; then
    echo "RPSX is empty. Building public part..."
    dotnet build -c Release SpaceStation14.sln
else
    echo "RPSX is not empty. Building hole part..."
    dotnet build -c Release RPSX.sln
    cd RPSX || exit 1
    ./sync.sh -y
    cd ..
fi

read -p "Press any key to continue..."
