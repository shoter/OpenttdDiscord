using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Backend;
using OpenttdDiscord.Common;
using OpenttdDiscord.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Configuration
{
    public class ConfigModule : IModule
    {
        public void Register(in IServiceCollection services)
        {
            services.AddSingleton(new MySqlConfig());
            services.AddSingleton(new OpenttdDiscordConfig());
        }
    }
}
