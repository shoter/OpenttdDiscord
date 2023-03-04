using Discord;
using OpenttdDiscord.Infrastructure.Chatting.Runners;
using OpenttdDiscord.Infrastructure.Discord.Commands;

namespace OpenttdDiscord.Infrastructure.Chatting.Commands
{
    internal class RegisterChatChannelCommand : OttdSlashCommandBase<RegisterChatChannelRunner>
    {
        public RegisterChatChannelCommand()
            : base("register-chat-channel")
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Register channel on which this message will be sent as chat channel for given server.")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("server-name")
                    .WithRequired(true)
                    .WithDescription("Server name")
                    .WithType(ApplicationCommandOptionType.String));
        }
    }
}
