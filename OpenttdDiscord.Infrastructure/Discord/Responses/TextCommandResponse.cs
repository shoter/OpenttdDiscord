using Discord.WebSocket;

namespace OpenttdDiscord.Infrastructure.Discord.Responses
{
    public class TextCommandResponse : SlashCommandResponseBase
    {
        private readonly string response;

        public TextCommandResponse(string response)
        {
            this.response = response;
        }

        protected override Task InternalExecute(SocketSlashCommand command)
        {
            return command.RespondAsync(response);
        }
    }
}
