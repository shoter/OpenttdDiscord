using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenttdDiscord.Database.Admins;
using OpenttdDiscord.Database.Reporting;

namespace OpenttdDiscord.Backend
{
    public class DatabaseModule : IModule
    {
        public void Register(in IServiceCollection services)
        {
            new ChattingModule().Register(services);
            new ServersModule().Register(services);
            new AdminModule().Register(services);
            new ReportModule().Register(services);
        }
    }
}
