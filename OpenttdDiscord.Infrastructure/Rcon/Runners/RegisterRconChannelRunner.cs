using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;

namespace OpenttdDiscord.Infrastructure.Rcon.Runners
{
    internal class RegisterRconChannelRunner : OttdSlashCommandRunnerBase
    {
        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(SocketSlashCommand command, User user, ExtDictionary<string, object> options)
        {
            throw new NotImplementedException();
        }
    }
}
