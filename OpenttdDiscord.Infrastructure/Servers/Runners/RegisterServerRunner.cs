﻿using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;

namespace OpenttdDiscord.Infrastructure.Servers.Runners
{
    internal class RegisterServerRunner : OttdSlashCommandRunnerBase
    {
        private readonly IRegisterOttdServerUseCase useCase;

        public RegisterServerRunner(
            IRegisterOttdServerUseCase useCase,
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
            this.useCase = useCase;
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            OptionsDictionary options)
        {
            if (command.GuildId == null)
            {
                return new HumanReadableError("GuildId is Null - wtf?");
            }

            string name = options.GetValueAs<string>("name");
            string password = options.GetValueAs<string>("password");
            string ip = options.GetValueAs<string>("ip");
            int port = (int) options.GetValueAs<long>("port");

            var server = new OttdServer(
                Guid.NewGuid(),
                command.GuildId!.Value,
                ip,
                name,
                port,
                password
            );

            return
                from _0 in CheckIfHasCorrectUserLevel(user, UserLevel.Admin).ToAsync()
                from _1 in useCase.Execute(
                    user,
                    server)
                select (IInteractionResponse) new TextResponse($"Created Server {name} - {ip}:{port}");
        }
    }
}