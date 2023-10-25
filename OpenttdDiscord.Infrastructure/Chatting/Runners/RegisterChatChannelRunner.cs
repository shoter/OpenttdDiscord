using Discord;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Chatting;
using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;

namespace OpenttdDiscord.Infrastructure.Chatting.Runners
{
    internal class RegisterChatChannelRunner : OttdSlashCommandRunnerBase
    {
        private readonly IGetServerUseCase getServerByNameUseCase;

        private readonly IRegisterChatChannelUseCase registerChatChannelUseCase;

        private readonly IGetChatChannelUseCase getChatChannelUseCase;

        public RegisterChatChannelRunner(
            IGetServerUseCase getServerByNameUseCase,
            IRegisterChatChannelUseCase registerChatChannelUseCase,
            IGetChatChannelUseCase getChatChannelUseCase,
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
            this.getServerByNameUseCase = getServerByNameUseCase;
            this.registerChatChannelUseCase = registerChatChannelUseCase;
            this.getChatChannelUseCase = getChatChannelUseCase;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            ExtDictionary<string, object> options)
        {
            string serverName = options.GetValueAs<string>("server-name");

            return
                from guildId in EnsureItIsGuildCommand(command)
                    .ToAsync()
                from channelId in EnsureItIsChannelCommand(command)
                    .ToAsync()
                from _0 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Admin)
                    .ToAsync()
                from server in getServerByNameUseCase.Execute(
                    user,
                    serverName,
                    guildId)
                from _1 in ReturnErrorIfChatChannelExists(
                    user,
                    server.Id,
                    channelId)
                from _2 in registerChatChannelUseCase.Execute(
                    user,
                    new ChatChannel(
                        server.Id,
                        guildId,
                        channelId))
                select (ISlashCommandResponse) new TextCommandResponse("Chat channel registered!");
        }

        private EitherAsyncUnit ReturnErrorIfChatChannelExists(
            User user,
            Guid serverId,
            ulong channelId)
        {
            var result = getChatChannelUseCase.Execute(
                user,
                serverId,
                channelId);
            return result.Bind(
                chatChannel =>
                    chatChannel.IsNone
                        ? EitherAsyncUnit.Right(Unit.Default)
                        : EitherAsyncUnit.Left(new HumanReadableError("Chat channel already exists!")));
        }
    }
}