using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Chatting
{
    public class DiscordMessage
    {
        public string Username { get; set; }
        public ulong ChannelId { get; set; }
        public string Message { get; set; }

    }
}
