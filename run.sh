docker stop openttd_discord || true && docker rm openttd_discord || true
docker run -d --name="openttd_discord" \
 -e ottd_discord_token="$ottd_discord_token" \
 -e ottd_discord_connectionstring="$ottd_discord_connectionstring" \
 --restart always \
 openttd_discord
docker logs -f openttd_discord
