#!/bin/sh
set -e
cd `dirname $0`

dotnet test --no-build --logger junit --configuration Release --framework netcoreapp3.1 UnitTests/UnitTests.csproj
