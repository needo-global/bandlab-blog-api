#!/bin/bash
cd ..
set -euo pipefail

exec 3>&1

function say() {
  printf "%b\n" "[build] $1" >&3
}

set -e # Exit if any step fails

say "Restoring nuget packages"
dotnet restore

say "Build solution in Release"
dotnet build -c Release --no-restore

say "Running tests"
dotnet test -c Release --no-build

dotnet tool install -g Amazon.Lambda.Tools

pushd Posts.DatabaseEventListener > /dev/null

say "Posts.DatabaseEventListener"
dotnet lambda package \
  --configuration release \
  --framework net6.0 \
  --output-package bin/release/net6.0/posts-databaseeventlistener.zip \

popd > /dev/null

say "Build Done!"