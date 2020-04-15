#docker build -t "openttd_discord:$IMAGE_VERSION" .
#docker tag "openttd_discord:$IMAGE_VERSION" "pir.ja.dom/openttd_discord:$IMAGE_VERSION"
docker push "pir.ja.dom/openttd_discord:$IMAGE_VERSION"
