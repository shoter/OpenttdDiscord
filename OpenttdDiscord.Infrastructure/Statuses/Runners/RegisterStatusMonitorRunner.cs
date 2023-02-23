using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Database.Statuses;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord;
using OpenttdDiscord.Infrastructure.Statuses.Messages;

namespace OpenttdDiscord.Infrastructure.Statuses.Runners
{
    internal class RegisterStatusMonitorRunner : OttdSlashCommandRunnerBase
    {
        private readonly IOttdServerRepository ottdServerRepository;

        private readonly IStatusMonitorRepository statusMonitorRepository;

        private readonly IAkkaService akkaService;

        public RegisterStatusMonitorRunner(IOttdServerRepository ottdServerRepository, IStatusMonitorRepository statusMonitorRepository, IAkkaService akkaService)
        {
            this.ottdServerRepository = ottdServerRepository;
            this.statusMonitorRepository = statusMonitorRepository;
            this.akkaService = akkaService;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(SocketSlashCommand command, ExtDictionary<string, object> options)
        {
            // TODO: check rights
            string serverName = options.GetValueAs<string>("server-name");
            ulong channelId = command.ChannelId!.Value;
            ulong guildId = command.GuildId!.Value;

            return
            from server in ottdServerRepository.GetServerByName(guildId, serverName).ToAsync()
            from _1 in RegisterStatusMonitorViaActor(channelId, guildId, server)
            select (ISlashCommandResponse) new TextCommandResponse("Creating status message in progress");
        }

        private EitherAsyncUnit RegisterStatusMonitorViaActor(ulong channelId, ulong guildId, OttdServer server)
        {
            return TryAsync(async () =>
            {
                var msg = new RegisterStatusMonitor(server, guildId, channelId);
                (await akkaService.SelectActor(MainActors.Paths.Guilds)).Tell(msg);
                return Unit.Default;
            }).ToEitherAsyncError();
        }
    }
}
