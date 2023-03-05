dotnet ./migrator/OpenttdDiscord.Database.Migrator.dll "/app/script.sql"
cd /app/publish/
dotnet OpenttdDiscord.Discord.dll
