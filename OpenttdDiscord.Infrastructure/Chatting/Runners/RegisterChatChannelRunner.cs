using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Chatting;
using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;

namespace OpenttdDiscord.Infrastructure.Chatting.Runners
{
    internal class RegisterChatChannelRunner : OttdSlashCommandRunnerBase
    {
        private readonly IGetServerByNameUseCase getServerByNameUseCase;

        private readonly IRegisterChatChannelUseCase registerChatChannelUseCase;

        private readonly IGetChatChannelUseCase getChatChannelUseCase;

        public RegisterChatChannelRunner(
            IGetServerByNameUseCase getServerByNameUseCase, 
            IRegisterChatChannelUseCase registerChatChannelUseCase,
            IGetChatChannelUseCase getChatChannelUseCase)
        {
            this.getServerByNameUseCase = getServerByNameUseCase;
            this.registerChatChannelUseCase = registerChatChannelUseCase;
            this.getChatChannelUseCase = getChatChannelUseCase;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(SocketSlashCommand command, ExtDictionary<string, object> options)
        {
            var user = new User(command.User);
            string serverName = options.GetValueAs<string>("server-name");

            return
                from guildId in CheckIfGuildCommand(command).ToAsync()
                from channelId in CheckIfChannelCommand(command).ToAsync()
                from server in getServerByNameUseCase.Execute(user, serverName, guildId)
                from _1 in ReturnErrorIfChatChannelExists(user, server.Id, channelId)
                from _2 in registerChatChannelUseCase.Execute(user, new ChatChannel(server.Id, guildId, channelId))
                select (ISlashCommandResponse)new TextCommandResponse("Chat channel registered!");
        }

        private EitherAsyncUnit ReturnErrorIfChatChannelExists(User user, Guid serverId, ulong channelId)
        {
            var result = getChatChannelUseCase.Execute(user, serverId, channelId);
            return result.Bind(chatChannel =>
                chatChannel.IsNone ?
                EitherAsyncUnit.Left(new HumanReadableError("Chat channel already exists!")) :
                EitherAsyncUnit.Right(Unit.Default));
        }
    }
}
