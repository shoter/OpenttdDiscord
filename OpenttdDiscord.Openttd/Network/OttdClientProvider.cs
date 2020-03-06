using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network
{
    public class OttdClientProvider : IOttdClientProvider
    {
        private List<IOttdClient> clients = new List<IOttdClient>();
        private readonly IOttdClientFactory ottdClientFactory;

        public OttdClientProvider(IOttdClientFactory ottdClientFactory)
        {
            this.ottdClientFactory = ottdClientFactory;
        }

        public IOttdClient Provide(string serverIp, int port)
        {
            IOttdClient client;
            if((client = clients.FirstOrDefault(c => c.ServerInfo.ServerIp == serverIp && c.ServerInfo.ServerPort == port)) != null)
                return client;

            clients.Add(client = ottdClientFactory.Create(new ServerInfo(serverIp, port)));

            return client;
        }
    }
}
