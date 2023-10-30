using System.Text;
using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Rcon;
using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;

namespace OpenttdDiscord.Infrastructure.Rcon.Runners
{
    internal class ListRconChannelsRunner : OttdSlashCommandRunnerBase
    {
        private readonly IGetServerUseCase getServerUseCase;

        private readonly IListRconChannelsUseCase listRconChannelsUseCase;

        public ListRconChannelsRunner(
            IGetServerUseCase getServerUseCase,
            IListRconChannelsUseCase listRconChannelsUseCase,
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
            this.getServerUseCase = getServerUseCase;
            this.listRconChannelsUseCase = listRconChannelsUseCase;
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            ExtDictionary<string, object> options)
        {
            ulong guildId = command.GuildId!.Value;

            return
                from _0 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Moderator)
                    .ToAsync()
                from rconServers in listRconChannelsUseCase.Execute(
                    user,
                    guildId)
                from response in GenerateResponse(rconServers)
                select (IInteractionResponse) new TextResponse(response);
        }

        private EitherAsync<IError, string> GenerateResponse(List<RconChannel> channels) => TryAsync(
                async () =>
                {
                    StringBuilder sb = new();

                    foreach (var rcon in channels)
                    {
                        var server = (await getServerUseCase.Execute(rcon.ServerId))
                            .ThrowIfError()
                            .Right();
                        sb.AppendLine(
                            $"{server.Name} - {MentionUtils.MentionChannel(rcon.ChannelId)} - prefix: `{rcon.Prefix}`");
                    }

                    return sb.ToString();
                })
            .ToEitherAsyncError();
    }
}