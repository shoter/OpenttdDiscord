using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;

namespace OpenttdDiscord.Infrastructure.Rcon.Runners
{
    internal class RegisterRconChannelRunner : OttdSlashCommandRunnerBase
    {
        private readonly IRegisterRconChannelUseCase registerRconChannelUseCase;

        private readonly IGetServerUseCase getServerByNameUseCase;

        private readonly IGetRconChannelUseCase getRconChannelUseCase;

        public RegisterRconChannelRunner(
            IRegisterRconChannelUseCase registerRconChannelUseCase,
            IGetRconChannelUseCase getRconChannelUseCase,
            IGetServerUseCase getServerByNameUseCase,
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(akkaService, getRoleLevelUseCase)
        {
            this.registerRconChannelUseCase = registerRconChannelUseCase;
            this.getRconChannelUseCase = getRconChannelUseCase;
            this.getServerByNameUseCase = getServerByNameUseCase;
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            OptionsDictionary options)
        {
            ulong guildId = command.GuildId!.Value;
            ulong channelId = command.ChannelId!.Value;

            string serverName = options.GetValueAs<string>("server-name");
            string prefix = options.GetValueAs<string>("prefix");

            return
                from _0 in CheckIfHasCorrectUserLevel(user, UserLevel.Admin).ToAsync()
                from server in getServerByNameUseCase.Execute(
                    serverName,
                    guildId)
                from _1 in ErrorIfRconChannelExists(
                    user,
                    server.Id,
                    channelId)
                from _2 in registerRconChannelUseCase.Execute(
                    user,
                    server.Id,
                    guildId,
                    channelId,
                    prefix)
                select (IInteractionResponse) new TextResponse("Rcon channel registered!");
        }

        private EitherAsyncUnit ErrorIfRconChannelExists(
            User user,
            Guid serverId,
            ulong channelId)
        {
            return getRconChannelUseCase
                .Execute(
                    user,
                    serverId,
                    channelId)
                .Bind(
                    option => option.IsNone
                        ? EitherAsyncUnit.Right(Unit.Default)
                        : EitherAsyncUnit.Left(new HumanReadableError("Rcon channel is already registered!")));
        }
    }
}