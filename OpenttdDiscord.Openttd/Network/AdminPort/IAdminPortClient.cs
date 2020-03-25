using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public interface IAdminPortClient
    {
        AdminConnectionState ConnectionState { get; }

        ConcurrentDictionary<AdminUpdateType, bool> HandledUpdateTypes { get; }

        event EventHandler<IAdminMessage> MessageReceived;

        ServerInfo ServerInfo { get; }

        void SendMessage(IAdminMessage message);

        Task Join();
        Task Disconnect();
        
    }
}
