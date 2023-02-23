using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Infrastructure.Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Infrastructure.Statuses.Runners
{
    internal class RegisterStatusMonitorRunner : OttdSlashCommandRunnerBase
    {
        private readonly 
        protected override Task<Either<IError, ISlashCommandResponse>> RunInternal(SocketSlashCommand command, ExtDictionary<string, object> options)
        {
        }
    }
}
