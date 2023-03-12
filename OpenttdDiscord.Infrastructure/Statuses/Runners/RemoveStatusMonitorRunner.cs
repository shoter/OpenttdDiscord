using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Statuses.UseCases;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;

namespace OpenttdDiscord.Infrastructure.Statuses.Runners
{
    internal class RemoveStatusMonitorRunner : OttdSlashCommandRunnerBase
    {
        private readonly IRemoveStatusMonitorUseCase removeStatusMonitorUseCase;

        private readonly IOttdServerRepository ottdServerRepository;

        public RemoveStatusMonitorRunner(
            IRemoveStatusMonitorUseCase removeStatusMonitorUseCase,
            IOttdServerRepository ottdServerRepository)
        {
            this.removeStatusMonitorUseCase = removeStatusMonitorUseCase;
            this.ottdServerRepository = ottdServerRepository;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(SocketSlashCommand command, User user, ExtDictionary<string, object> options)
        {
            string serverName = options.GetValueAs<string>("server-name");

            var _ =
            from server in ottdServerRepository.GetServerByName(command.GuildId!.Value, serverName)
            from _2 in removeStatusMonitorUseCase.Execute(user, server.Id, command.GuildId!.Value, command.ChannelId!.Value)
            select (ISlashCommandResponse)new TextCommandResponse("Status monitor removed!");
            return _;
        }
    }
}
