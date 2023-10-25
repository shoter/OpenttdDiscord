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

        protected override Task InternalExecute(ISlashCommandInteraction command)
        {
            return command.RespondAsync(embed: embed);
        }
    }
}
