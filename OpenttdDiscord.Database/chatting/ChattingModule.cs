using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Chatting
{
    public class ChattingModule : IModule
    {
        public void Register(in IServiceCollection services)
        {
            services.AddSingleton<IChatChannelServerRepository, ChatChannelServerRepository>();
            services.AddSingleton<IChatChannelServerService, ChatChannelServerService>();
        }
    }
}
