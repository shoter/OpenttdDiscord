using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Tcp
{
    public class TcpOttdClientFactory : ITcpOttdClientFactory
    {
        private readonly ILogger<ITcpOttdClient> logger;
        private readonly ITcpPacketService packetService;
        private readonly IRevisionTranslator revisionTranslator;

        public TcpOttdClientFactory(ILogger<ITcpOttdClient> logger, ITcpPacketService packetService, IRevisionTranslator revisionTranslator)
        {
            this.logger = logger;
            this.packetService = packetService;
            this.revisionTranslator = revisionTranslator;
        }
        public ITcpOttdClient Create() => new TcpOttdClient(packetService, revisionTranslator, logger);
        
    }
}
