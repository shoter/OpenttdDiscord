using Discord;
using OpenttdDiscord.Backend.Servers;
using OpenttdDiscord.Common;
using OpenttdDiscord.Openttd.Network.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Messaging
{
    public class EmbedFactory : IEmbedFactory
    {
        public Embed Create(PacketUdpServerResponse r, Server server)
        {
            var embed = new EmbedBuilder
            {
                Title = $"{r.ServerName}"
            };

            embed.AddField("Players", $"{r.ClientsOn}/{r.ClientsMax}", true);
            embed.AddField("Map Size", $"{r.MapWidth}x{r.MapHeight}", true);
            embed.AddField("Year", $"{r.GameDate.ToString()}", true);

            embed.AddField("Climate", r.Landscape.Stringify().FirstUpper(), true);
            embed.AddField("Map name", r.MapName, true);
            embed.AddField("Language", r.Language.Stringify().FirstUpper(), true);

            embed.AddField("Server address", $"{server.ServerIp}:{server.ServerPort}", true);
            embed.AddField("Password?", r.HasPassword ? "Yes" : "No", true);

            embed.WithCurrentTimestamp();
            return embed.Build();
        }
    }
}
