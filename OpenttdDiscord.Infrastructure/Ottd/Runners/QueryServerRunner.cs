using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.Ottd.Runners
{
    internal class QueryServerRunner : OttdSlashCommandRunnerBase
    {
        private readonly IOttdServerRepository ottdServerRepository;

        public QueryServerRunner(
            IOttdServerRepository ottdServerRepository,
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
            this.ottdServerRepository = ottdServerRepository;
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            OptionsDictionary options)
        {
            if (!command.ChannelId.HasValue)
            {
                return new HumanReadableError("This command can be only run on channel!");
            }

            string serverName = options.GetValueAs<string>("server-name");
            ulong channelId = command.ChannelId.Value;

            return
                from _0 in CheckIfHasCorrectUserLevel(user, UserLevel.Moderator).ToAsync()
                from server in ottdServerRepository.GetServerByName(
                    command.GuildId!.Value,
                    serverName)
                from actor in AkkaService.SelectActor(MainActors.Paths.Guilds)
                from _1 in actor.TellExt(
                        new QueryServer(
                            server.Id,
                            command.GuildId!.Value,
                            channelId))
                    .ToAsync()
                select (IInteractionResponse) new TextResponse("Executing command");
        }
    }
}