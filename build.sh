# docker build -t "openttd-discord:$IMAGE_VERSION" .
docker tag "openttd-discord:$IMAGE_VERSION" "shoter/openttd-discord:$IMAGE_VERSION"
docker login --username $DOCKER_HUB_USR --password $DOCKER_HUB_PSW
docker push "shoter/openttd-discord:$IMAGE_VERSION"
