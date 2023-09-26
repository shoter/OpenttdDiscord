using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Rcon;
using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
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
            IListRconChannelsUseCase listRconChannelsUseCase,
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase)
        : base(akkaService, getRoleLevelUseCase)
        {
            this.discord = discord;
            this.getServerUseCase = getServerUseCase;
            this.listRconChannelsUseCase = listRconChannelsUseCase;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(ISlashCommandInteraction command, User user, ExtDictionary<string, object> options)
        {
            ulong guildId = command.GuildId!.Value;

            return
                from _0 in CheckIfHasCorrectUserLevel(user, UserLevel.Moderator).ToAsync()
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
                    var server = (await getServerUseCase.Execute(User.Master, rcon.ServerId))
                    .ThrowIfError().Right();
                    sb.AppendLine($"{server.Name} - {MentionUtils.MentionChannel(rcon.ChannelId)} - prefix: `{rcon.Prefix}`");
                }

                return sb.ToString();
            }).ToEitherAsyncError();
    }
}
