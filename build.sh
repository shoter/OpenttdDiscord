
docker build -t "openttd_discord:$IMAGE_VERSION" .
docker tag "openttd_discord:$IMAGE_VERSION" "shoter/openttd_discord:$IMAGE_VERSION"
docker login --username $DOCKER_HUB_USR --password $DOCKER_HUB_PSW
docker push "shoter/openttd_discord:$IMAGE_VERSION"
