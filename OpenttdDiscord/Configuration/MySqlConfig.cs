using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Configuration
{
    public class MySqlConfig
    {
        public string Host { get; set; }
        public uint Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
