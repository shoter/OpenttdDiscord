
# docker build -t "openttd_discord:$IMAGE_VERSION" .
# docker tag "openttd_discord:$IMAGE_VERSION" "openttd_discord:$IMAGE_VERSION"
docker login --username $DOCKER_HUB_USR --password $DOCKER_HUB_PSW
docker push "openttd_discord:$IMAGE_VERSION"
