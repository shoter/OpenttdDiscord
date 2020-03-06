using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network
{
    public class ReceivedChatMessage
    {
        public uint Recipient { get; }

        public string Message { get; }

        public ChatDestination ChatDestination { get; }

        public ReceivedChatMessage(string msg, uint recipient, ChatDestination chatDestination)
        {
            this.Recipient = recipient;
            this.Message = msg;
            this.ChatDestination = chatDestination;
        }


    }
}
