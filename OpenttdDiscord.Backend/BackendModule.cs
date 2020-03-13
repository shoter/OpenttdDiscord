﻿using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Backend.Chatting;
using OpenttdDiscord.Backend.Servers;
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
            new ChattingModule().Register(services);
            new ServersModule().Register(services);
        }
    }
}
