using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord
{
    public static class DependencyConfig
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        static DependencyConfig()
        {
            var services = new ServiceCollection();

            new ConfigModule().Register(services);

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
