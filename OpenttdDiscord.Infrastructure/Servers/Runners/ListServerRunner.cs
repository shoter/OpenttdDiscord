﻿using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;

namespace OpenttdDiscord.Infrastructure.Servers.Runners
{
    internal class ListServerRunner : OttdSlashCommandRunnerBase
    {
        private readonly IOttdServerRepository ottdServerRepository;

        public ListServerRunner(
            IOttdServerRepository ottdServerRepository,
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
            this.ottdServerRepository = ottdServerRepository;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            ExtDictionary<string, object> options)
        {
            return
                from _0 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Moderator)
                    .ToAsync()
                from guildId in EnsureItIsGuildCommand(command)
                    .ToAsync()
                from servers in ottdServerRepository.GetServersForGuild(guildId)
                from embed in CreateEmbed(servers)
                    .ToAsync()
                select (ISlashCommandResponse) new EmbedCommandResponse(embed);
        }

        private Either<IError, Embed> CreateEmbed(List<OttdServer> servers)
        {
            EmbedBuilder embedBuilder = new();

            embedBuilder
                .WithTitle("List of servers")
                .WithDescription(string.Empty);

            foreach (var server in servers)
            {
                embedBuilder.AddField(
                    "Server Name",
                    server.Name);
                embedBuilder.AddField(
                    "Server IP",
                    server.Ip,
                    true);
                embedBuilder.AddField(
                    "Server Port",
                    server.AdminPort,
                    true);
            }

            return embedBuilder.Build();
        }
    }
}