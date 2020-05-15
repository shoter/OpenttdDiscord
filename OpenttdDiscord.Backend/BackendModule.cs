using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Backend.Admins;
using OpenttdDiscord.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Backend
{
    public class BackendModule : IModule
    {
        public void Register(in IServiceCollection services)
        {
            new AdminsModule().Register(services);
        }
    }
}
