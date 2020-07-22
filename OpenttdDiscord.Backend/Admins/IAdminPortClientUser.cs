using OpenTTDAdminPort.Events;
using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Backend.Admins
{
    public interface IAdminPortClientUser
    {
        void ParseServerEvent(Server server, IAdminEvent adminEvent);
    }
}
