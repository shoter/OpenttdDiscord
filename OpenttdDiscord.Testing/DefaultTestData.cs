using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Testing
{
    public class DefaultTestData
    {
        public static Server DefaultServer { get; } = new Server(1, 1579, "10.0.0.1", 12345, "something");

        public static Server[] OtherServers { get; } = new Server[]
        {
            new Server(2, 1342, "14.3.2.1", 14312, "anothre s")
        };

        public static Server[] SameGuildServers { get; } = new Server[]
        {
            new Server(3, 3, "11.3.2.1", 1, "sameguild1"),
            new Server(4, 3, "12.3.2.1", 2, "sameguild2"),
            new Server(5, 3, "13.3.2.1", 3, "sameguild3")
        };
    }
}
