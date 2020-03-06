using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Tcp
{
    public enum NetworkAction
    {
		NETWORK_ACTION_JOIN,
		NETWORK_ACTION_LEAVE,
		NETWORK_ACTION_SERVER_MESSAGE,
		NETWORK_ACTION_CHAT,
		NETWORK_ACTION_CHAT_COMPANY,
		NETWORK_ACTION_CHAT_CLIENT,
		NETWORK_ACTION_GIVE_MONEY,
		NETWORK_ACTION_NAME_CHANGE,
		NETWORK_ACTION_COMPANY_SPECTATOR,
		NETWORK_ACTION_COMPANY_JOIN,
		NETWORK_ACTION_COMPANY_NEW,
	}
}
