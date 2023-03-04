using Discord;
using LanguageExt.Pipes;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Game;

namespace OpenttdDiscord.Infrastructure.Statuses
{
    public class ServerStatusEmbedBuilder
    {
        public Embed CreateServerStatusEmbed(IAdminPortClient client, ServerStatus serverStatus, AdminServerInfo info, string serverName)
        {
            string mapName = string.IsNullOrEmpty(info.MapName) ? "Random map" : info.MapName;

            EmbedBuilder embedBuilder = new();
            embedBuilder.WithTitle($"{info.ServerName} Status");

            embedBuilder.AddField("Players", serverStatus.Players.Count, true);
            embedBuilder.AddField("Map Size", $"{info.MapWidth}x{info.MapHeight}", true);
            embedBuilder.AddField("Year", info.Date, true);

            embedBuilder.AddField("Map Name", mapName, true);
            embedBuilder.AddField("Climate", info.Landscape.ToHumanReadable(), true);
            embedBuilder.AddField("Server address", $"{client.ServerInfo.ServerIp}", true);

            string players = string.Join('\n', serverStatus.Players.Values.Select(StringifyPlayer));
            if (!string.IsNullOrEmpty(players))
            {
                embedBuilder.AddField("Players", players, false);
            }

            var embed = embedBuilder.Build();
            return embed;
        }

        private string StringifyPlayer(Player player)
        {
            return player.ClientId == 1 ?
                $"{player.Name} [Server]" :
                player.Name;
        }
    }
}
