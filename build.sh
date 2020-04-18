#!/bin/bash
docker build -t "openttd-discord:$IMAGE_VERSION" .
docker tag "openttd-discord:$IMAGE_VERSION" "shoter/openttd-discord:$IMAGE_VERSION"
docker login --username $DOCKER_HUB_USR --password $DOCKER_HUB_PSW
echo "shoter/openttd-discord:$IMAGE_VERSION"
docker push "shoter/openttd-discord:$IMAGE_VERSION"
docker tag "openttd-discord:$IMAGE_VERSION" "shoter/openttd-discord:$BUILD_NUMBER"
echo "shoter/openttd-discord:$BUILD_NUMBER"
docker push "shoter/openttd-discord:$BUILD_NUMBER"
 