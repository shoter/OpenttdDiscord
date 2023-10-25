using Discord;

namespace OpenttdDiscord.Infrastructure.Discord.CommandResponses
{
    public interface IInteractionResponse
    {
        public EitherAsyncUnit Execute(IDiscordInteraction interaction);
    }
}
