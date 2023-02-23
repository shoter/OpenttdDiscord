﻿using Discord;
using Discord.WebSocket;
using LanguageExt;
using LanguageExt.Common;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Infrastructure.Servers
{
    internal class ListServerRunner : IOttdSlashCommandRunner
    {
        private readonly IListOttdServersUseCase listServersUseCase;

        public ListServerRunner(IListOttdServersUseCase listOttdServersUseCase)
        {
            this.listServersUseCase = listOttdServersUseCase;
        }

        public EitherAsync<IError, ISlashCommandResponse> Run(SocketSlashCommand command)
        {
            if (command.GuildId.HasValue == false)
            {
                return new HumanReadableError("This command can only be executed within a guild!");
            }

            return 
            from servers in listServersUseCase.Execute(new User(command.User), command.GuildId.Value).ToAsync()
            from embed in CreateEmbed(servers).ToAsync()
            select (ISlashCommandResponse) new EmbedCommandResponse(embed);

        }

        private Either<IError, Embed> CreateEmbed(List<OttdServer> servers)
        {
            EmbedBuilder embedBuilder = new();

            embedBuilder
                .WithTitle("List of servers")
                .WithDescription("");

            foreach (var server in servers)
            {
                embedBuilder.AddField("Server Name", server.Name);
                embedBuilder.AddField("Server IP", server.Ip, true);
                embedBuilder.AddField("Server Port", server.AdminPort, true);
            }

            return embedBuilder.Build();
        }
    }
}
