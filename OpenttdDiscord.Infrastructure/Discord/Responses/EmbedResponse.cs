using Discord;

namespace OpenttdDiscord.Infrastructure.Discord.CommandResponses
{
    public class EmbedResponse : InteractionResponseBase
    {
        private readonly Embed embed;

        public EmbedResponse(Embed embed)
        {
            this.embed = embed;
        }

        protected override Task InternalExecute(IDiscordInteraction interaction)
        {
            return interaction.RespondAsync(embed: embed);
        }
    }
}
