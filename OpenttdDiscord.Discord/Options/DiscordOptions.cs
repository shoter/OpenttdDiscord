using System.ComponentModel.DataAnnotations;

namespace OpenttdDiscord.Discord.Options
{
    public class DiscordOptions
    {
        [Required]
        public string Token { get; set; } = default!;
    }
}
