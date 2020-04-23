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
        public static Server DefaultServer { get; } = new Server(1, "something", 12345, "something");
    }
}
