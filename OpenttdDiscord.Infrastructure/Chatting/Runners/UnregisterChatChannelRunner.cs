using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;

namespace OpenttdDiscord.Infrastructure.Chatting.Runners
{
    internal class UnregisterChatChannelRunner : OttdSlashCommandRunnerBase
    {
        private readonly IUnregisterChatChannelUseCase unregisterChatChannelUseCase;

        private readonly IGetServerUseCase getServerByNameUseCase;

        public UnregisterChatChannelRunner(
            IUnregisterChatChannelUseCase unregisterChatChannelUseCase,
            IGetServerUseCase getServerByNameUseCase,
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
            this.unregisterChatChannelUseCase = unregisterChatChannelUseCase;
            this.getServerByNameUseCase = getServerByNameUseCase;
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            ExtDictionary<string, object> options)
        {
            string serverName = options.GetValueAs<string>("server-name");
            ulong guildId = command.GuildId!.Value;
            ulong channelId = command.ChannelId!.Value;

            return
                from _0 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Admin)
                    .ToAsync()
                from server in getServerByNameUseCase.Execute(
                    serverName,
                    guildId)
                from _1 in unregisterChatChannelUseCase.Execute(
                    user,
                    server.Id,
                    guildId,
                    channelId)
                select (IInteractionResponse) new TextResponse(
                    $"{server.Name} unregistered from this chat channel");
        }
    }
}