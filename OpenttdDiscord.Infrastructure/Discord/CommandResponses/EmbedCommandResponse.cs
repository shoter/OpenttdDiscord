using Discord;

namespace OpenttdDiscord.Infrastructure.Discord.CommandResponses
{
    public class EmbedCommandResponse : SlashCommandResponseBase
    {
        private readonly Embed embed;

        public EmbedCommandResponse(Embed embed)
        {
            this.embed = embed;
        }

        protected override Task InternalExecute(ISlashCommandInteraction command)
        {
            return command.RespondAsync(embed: embed);
        }
    }
}
