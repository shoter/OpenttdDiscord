using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Discord;
using OpenttdDiscord.Infrastructure.Statuses.UseCases;

namespace OpenttdDiscord.Infrastructure.Statuses.Runners
{
    internal class RegisterStatusMonitorRunner : OttdSlashCommandRunnerBase
    {
        private readonly IOttdServerRepository ottdServerRepository;

        private readonly IRegisterStatusMonitorUseCase registerStatusMonitorUseCase;

        public RegisterStatusMonitorRunner(
            IOttdServerRepository ottdServerRepository, 
            IRegisterStatusMonitorUseCase registerStatusMonitorUseCase)
        {
            this.ottdServerRepository = ottdServerRepository;
            this.registerStatusMonitorUseCase = registerStatusMonitorUseCase;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(SocketSlashCommand command, ExtDictionary<string, object> options)
        {
            // TODO: check rights
            string serverName = options.GetValueAs<string>("server-name");
            ulong channelId = command.ChannelId!.Value;
            ulong guildId = command.GuildId!.Value;

            return
            from server in ottdServerRepository.GetServerByName(guildId, serverName).ToAsync()
            from monitor in registerStatusMonitorUseCase.Execute(new User(command.User), server, guildId, channelId)
            select (ISlashCommandResponse) new TextCommandResponse("Creating status message in progress");
        }
    }
}
