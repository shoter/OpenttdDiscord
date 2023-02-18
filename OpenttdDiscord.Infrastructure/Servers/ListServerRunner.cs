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

        public async Task<Either<IError, ISlashCommandResponse>> Run(SocketSlashCommand command)
        {
            if(command.GuildId.HasValue == false)
            {
                return new HumanReadableError("This command can only be executed within a guild!");
            }

            var servers = await listServersUseCase.Execute(new UserRights(UserLevel.Admin), command.GuildId.Value);

            throw new Exception();
        }
    }
}
