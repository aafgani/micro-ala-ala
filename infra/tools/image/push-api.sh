# how to run:
#   bash infra/tools/image/push-api.sh

# This script builds and pushes a Docker image for the API Todo application to Azure Container Registry (ACR).
# It generates a version based on the current date and time, tags the image, and pushes it to ACR.
# Ensure you have Docker and Azure CLI installed and configured before running this script
# Make sure to replace `aafgani` with your actual Azure Container Registry name.
# Ensure you have the necessary permissions to push images to the specified ACR.
# This script assumes you are in the root directory of the project where the Dockerfile is located
# and that the Dockerfile is located at `src/apps/App.Api.Todo/dockerfile`.


#!/bin/bash

set -e

# === [1] Generate version ===
BASE_VERSION="2.1.0"
BUILD_REVISION="$(date +%H%M)"  # or even just a 2- or 3-digit incremental build number
VERSION="${BASE_VERSION}.${BUILD_REVISION}"  

echo "📝 Writing version to .version: $VERSION"
echo "$VERSION" > .version

DOCKER_TAG="${VERSION//+/-}" # Replace '+' with '-' for Docker tag compatibility

# === [2] Prepare image tagging ===
IMAGE_NAME="api.todo"
ACR_NAME="aafgani"  # Replace with your Azure Container Registry name
ACR_IMAGE="$ACR_NAME.azurecr.io/$IMAGE_NAME:$DOCKER_TAG"

echo "📦 Tagging image as: $ACR_IMAGE"

# === [3] Build & Push ===
docker build --build-arg APP_VERSION="$DOCKER_TAG" -t "$ACR_IMAGE" -f src/apps/App.Api.Todo/dockerfile . --no-cache
az acr login --name "$ACR_NAME"
docker push "$ACR_IMAGE"
