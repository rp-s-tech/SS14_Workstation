#!/bin/bash

dotnet build -c Release --property WarningLevel=0
cd RPSX || exit
dotnet build -c Release --property WarningLevel=0

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

bash "$SCRIPT_DIR/sync.sh" -y

cd ..
