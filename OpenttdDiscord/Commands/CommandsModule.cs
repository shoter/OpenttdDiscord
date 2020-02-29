using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Commands
{
    public class CommandsModule : IModule
    {
        public void Register(in IServiceCollection services)
        {
            services.AddSingleton<CommandHandlingService>();
            services.AddSingleton<ServerCommands>();
        }
    }
}
