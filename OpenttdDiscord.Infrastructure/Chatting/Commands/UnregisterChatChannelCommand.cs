using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OpenttdDiscord.Infrastructure.Chatting.Runners;
using OpenttdDiscord.Infrastructure.Discord.Commands;

namespace OpenttdDiscord.Infrastructure.Chatting.Commands
{
    internal class UnregisterChatChannelCommand : OttdSlashCommandBase<UnregisterChatChannelRunner>
    {
        public UnregisterChatChannelCommand()
            : base("unregister-chat-channel")
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Removes given server from chat channel completely")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("server-name")
                    .WithRequired(true)
                    .WithDescription("Name of the server which should be removed from this chat channel")
                    .WithType(ApplicationCommandOptionType.String));
        }
    }
}
