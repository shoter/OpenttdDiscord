
docker build -t ottd_discord_dev_database .
$ErrorActionPreference = 'SilentlyContinue'
docker stop ottd_discord_dev_database
docker rm ottd_discord_dev_database
$ErrorActionPreference = 'Continue '
docker run -p 5432:5432 --name ottd_discord_dev_database -d ottd_discord_dev_database
dotnet ef database update --connection "User ID=openttd;Password=secret-pw;Host=localhost;Port=5432;Database=openttd"