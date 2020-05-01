using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Admins
{
    public interface IAdminService
    {
        Task Start();

        Task HandleMessage(ulong channelId, string message);
        
    }
}
