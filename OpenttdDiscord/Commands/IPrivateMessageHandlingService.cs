using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Commands
{
    public interface IPrivateMessageHandlingService
    {
        Task HandleMessage(SocketUserMessage message, SocketDMChannel channel);
    }
}
