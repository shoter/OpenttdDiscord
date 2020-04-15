docker build -t "openttd_discord:$IMAGE_VERSION" .
docker tag "openttd_discord:$IMAGE_VERSION" "pir.ja.dom:5000/openttd_discord:$IMAGE_VERSION"
docker push "pir.ja.dom:5000/openttd_discord:$IMAGE_VERSION"
