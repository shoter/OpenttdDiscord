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
    }
}
