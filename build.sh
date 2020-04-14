docker build -t "openttd_discord:$VERSION" .
docker tag "openttd_discord:$VERSION "pir.ja.dom/openttd_discord:$VERSION"
docker push "pir.ja.dom/openttd_discord:$VERSION"
