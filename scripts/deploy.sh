#!/bin/bash
cd ..
set -euo pipefail

exec 3>&1

function say() {
  printf "%b\n" "$1" >&3
}

branchName=$1
$dockerImageTagForWebApi=$2
envTag=$3

say "Installing npm packages"
cd ./.aws/cdk
npm ci
npm run build

say "Deploying stack"

npm run cdk deploy -- \
    --verbose \
    --require-approval never \
    --context StackName=bandlab-posts-api-$branchName \
    --context Stage=$branchName \
    --context DockerImageTagForWebApi=$dockerImageTagForWebApi \
    --tags env=$envTag

say "Deployment Done!"