using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LanguageExt;
using LanguageExt.Common;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Discord;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;
using OpenttdDiscord.Infrastructure.Servers.UseCases;

namespace OpenttdDiscord.Infrastructure.Servers.Runners
{
    internal class ListServerRunner : IOttdSlashCommandRunner
    {
        private readonly IListOttdServersUseCase listServersUseCase;

        public ListServerRunner(IListOttdServersUseCase listOttdServersUseCase)
        {
            listServersUseCase = listOttdServersUseCase;
        }

        public EitherAsync<IError, ISlashCommandResponse> Run(ISlashCommandInteraction command)
        {
            if (command.GuildId.HasValue == false)
            {
                return new HumanReadableError("This command can only be executed within a guild!");
            }

            return
            from servers in listServersUseCase.Execute(new User(command.User), command.GuildId.Value).ToAsync()
            from embed in CreateEmbed(servers).ToAsync()
            select (ISlashCommandResponse)new EmbedCommandResponse(embed);
        }

        private Either<IError, Embed> CreateEmbed(List<OttdServer> servers)
        {
            EmbedBuilder embedBuilder = new();

            embedBuilder
                .WithTitle("List of servers")
                .WithDescription(string.Empty);

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
