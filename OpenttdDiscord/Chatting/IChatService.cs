using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Chatting
{
    public interface IChatService
    {
        void AddMessage(DiscordMessage message);
        Task Start();
        
    }
}
