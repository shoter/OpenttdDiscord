using Discord;
using OpenttdDiscord.Infrastructure.Chatting.Runners;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Infrastructure.Chatting.Commands
{
    internal class UnregisterChatChannelCommand : OttdSlashCommandBase<UnregisterChatChannelRunner>
    {
        public UnregisterChatChannelCommand(string name) : base(name)
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Removes given server from chat channel completely");
        }
    }
}
