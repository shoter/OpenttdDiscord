using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public enum AdminUpdateType
    {
		ADMIN_UPDATE_DATE,            /// Updates about the date of the game.
		ADMIN_UPDATE_CLIENT_INFO,     /// Updates about the information of clients.
		ADMIN_UPDATE_COMPANY_INFO,    /// Updates about the generic information of companies.
		ADMIN_UPDATE_COMPANY_ECONOMY, /// Updates about the economy of companies.
		ADMIN_UPDATE_COMPANY_STATS,   /// Updates about the statistics of companies.
		ADMIN_UPDATE_CHAT,            /// The admin would like to have chat messages.
		ADMIN_UPDATE_CONSOLE,         /// The admin would like to have console messages.
		ADMIN_UPDATE_CMD_NAMES,       /// The admin would like a list of all DoCommand names.
		ADMIN_UPDATE_CMD_LOGGING,     /// The admin would like to have DoCommand information.
		ADMIN_UPDATE_GAMESCRIPT,      /// The admin would like to have gamescript messages.
		ADMIN_UPDATE_END,             /// Must ALWAYS be on the end of this list!! (period)
	}
}
