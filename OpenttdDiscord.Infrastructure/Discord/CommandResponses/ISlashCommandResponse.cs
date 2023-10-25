using Discord;

namespace OpenttdDiscord.Infrastructure.Discord.CommandResponses
{
    public interface ISlashCommandResponse
    {
        public EitherAsyncUnit Execute(ISlashCommandInteraction command);
    }
}
