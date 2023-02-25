﻿using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Statuses.UseCases;
using OpenttdDiscord.Infrastructure.Discord;

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

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(SocketSlashCommand command, ExtDictionary<string, object> options)
        {
            string serverName = options.GetValueAs<string>("server-name");

            var _ =
            from server in ottdServerRepository.GetServerByName(command.GuildId!.Value, serverName)
            from _2 in removeStatusMonitorUseCase.Execute(new User(command.User), server.Id, command.GuildId!.Value, command.ChannelId!.Value)
            select (ISlashCommandResponse) new TextCommandResponse("Status monitor removed!");
            return _;
        }
    }
}
