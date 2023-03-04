using Discord;

namespace OpenttdDiscord.Validation
{
    public class ValidationErrorEmbedBuilder
    {
        public Embed BuildEmbed(ValidationError validationError)
        {
            EmbedBuilder embedBuilder = new();

            embedBuilder
                .WithTitle("Validation failed")
                .WithDescription("Data you have provided was wrong and could not be parsed");

            foreach (var e in validationError.Errors)
            {
                embedBuilder.AddField(e.PropertyName, e.ErrorMessage);
            }

            return embedBuilder.Build();
        }
    }
}
