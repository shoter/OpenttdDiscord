using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Security;
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

        public async Task<EitherString> Run(SocketSlashCommand command)
        {
            if(command.GuildId.HasValue == false)
            {
                return HumanReadableError.EitherString("This command can only be executed within a guild!");
            }

            var servers = await listServersUseCase.Execute(new UserRights(UserLevel.Admin), command.GuildId.Value);

            servers.Bind<string>(server =>
            {

                command.respon
            });
        }
    }
}
