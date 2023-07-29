using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Statuses.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;

namespace OpenttdDiscord.Infrastructure.Statuses.Runners
{
    internal class RegisterStatusMonitorRunner : OttdSlashCommandRunnerBase
    {
        private readonly IOttdServerRepository ottdServerRepository;

        private readonly IRegisterStatusMonitorUseCase registerStatusMonitorUseCase;

        private readonly ICheckIfStatusMonitorExistsUseCase checkIfStatusMonitorExistsUseCase;

        public RegisterStatusMonitorRunner(
            IOttdServerRepository ottdServerRepository,
            IRegisterStatusMonitorUseCase registerStatusMonitorUseCase,
            ICheckIfStatusMonitorExistsUseCase checkIfStatusMonitorExistsUseCase,
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase)
        : base(akkaService, getRoleLevelUseCase)
        {
            this.ottdServerRepository = ottdServerRepository;
            this.registerStatusMonitorUseCase = registerStatusMonitorUseCase;
            this.checkIfStatusMonitorExistsUseCase = checkIfStatusMonitorExistsUseCase;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(SocketSlashCommand command, User user, ExtDictionary<string, object> options)
        {
            string serverName = options.GetValueAs<string>("server-name");
            ulong channelId = command.ChannelId!.Value;
            ulong guildId = command.GuildId!.Value;

            return
            from server in ottdServerRepository.GetServerByName(guildId, serverName)
            from _1 in ReturnErrorIfMonitorExists(user, server.Id, channelId)
            from monitor in registerStatusMonitorUseCase.Execute(user, server, guildId, channelId)
            select (ISlashCommandResponse)new TextCommandResponse("Creating status message in progress");
        }

        private EitherAsyncUnit ReturnErrorIfMonitorExists(User user, Guid serverId, ulong channelId)
        {
            return
            checkIfStatusMonitorExistsUseCase.Execute(user, serverId, channelId)
            .Bind(exists =>
                exists ?
                EitherAsyncUnit.Left(new HumanReadableError("Status monitor already exists for this channel")) :
                EitherAsyncUnit.Right(Unit.Default));
        }
    }
}
