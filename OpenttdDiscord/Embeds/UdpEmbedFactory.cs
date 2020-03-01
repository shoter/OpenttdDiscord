using Discord;
using OpenttdDiscord.Backend.Servers;
using OpenttdDiscord.Common;
using OpenttdDiscord.Openttd.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Embeds
{
    public class UdpEmbedFactory : IUdpEmbedFactory
    {
        public Task<Embed> Create(IUdpMessage message, Server server)
        {
            switch(message)
            {
                case PacketUdpServerResponse r:
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
                        return Task.FromResult(embed.Build());
                    }
            }

            return null;
            
        }
    }
}
