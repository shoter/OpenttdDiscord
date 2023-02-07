using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord
{
    public class Application
    {
        public Host CreateHost()
        {
            return Host.CreateDefaultBuilder();
        }
    }
}
