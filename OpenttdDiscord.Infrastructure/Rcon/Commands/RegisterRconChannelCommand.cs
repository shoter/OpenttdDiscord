﻿using Discord;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Rcon.Runners;

namespace OpenttdDiscord.Infrastructure.Rcon.Commands
{
    internal class RegisterRconChannelCommand : OttdSlashCommandBase<RegisterRconChannelRunner>
    {
        public RegisterRconChannelCommand()
            : base("register-admin-channel")
        {
        }

        public override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Registers channel to interact with server through RCON interface. " +
                "DO NOT execute this command on a channel where normal users have access to")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("server-name")
                    .WithRequired(true)
                    .WithDescription("Name of the server")
                    .WithType(ApplicationCommandOptionType.String))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("prefix")
                    .WithRequired(true)
                    .WithDescription("Prefix that you need to use in order to execute rcon commands")
                    .WithType(ApplicationCommandOptionType.String));
        }
    }
}
