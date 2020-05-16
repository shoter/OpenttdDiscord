using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Common.Networking
{
    public class NetworkPort
    {
        public static bool IsCorrect(int port) => port >= 0 && port <= 65535;
    }
}
