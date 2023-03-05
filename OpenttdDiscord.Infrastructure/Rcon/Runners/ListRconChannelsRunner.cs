using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Rcon;
using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;

namespace OpenttdDiscord.Infrastructure.Rcon.Runners
{
    internal class ListRconChannelsRunner : OttdSlashCommandRunnerBase
    {
        private readonly DiscordSocketClient discord;

        private readonly IGetServerUseCase getServerUseCase;

        private readonly IListRconChannelsUseCase listRconChannelsUseCase;

        public ListRconChannelsRunner(
            DiscordSocketClient discord,
            IGetServerUseCase getServerUseCase,
            IListRconChannelsUseCase listRconChannelsUseCase)
        {
            this.discord = discord;
            this.getServerUseCase = getServerUseCase;
            this.listRconChannelsUseCase = listRconChannelsUseCase;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(SocketSlashCommand command, User user, ExtDictionary<string, object> options)
        {
            ulong guildId = command.GuildId!.Value;

            return
                from rconServers in listRconChannelsUseCase.Execute(user, guildId)
                from response in GenerateResponse(rconServers)
                select (ISlashCommandResponse)new TextCommandResponse(response);
        }

        private EitherAsync<IError, string> GenerateResponse(List<RconChannel> channels)
            => TryAsync(async () =>
            {
                StringBuilder sb = new();

                foreach (var rcon in channels)
                {
                    var discordChannel = await discord.GetChannelAsync(rcon.ChannelId);
                    var server = (await getServerUseCase.Execute(User.Master, rcon.ServerId))
                    .ThrowIfError().Right();
                    sb.AppendLine($"{server.Name} - {discordChannel.Name} - prefix: `{rcon.Prefix}`");
                }

                return sb.ToString();
            }).ToEitherAsyncError();
    }
}
