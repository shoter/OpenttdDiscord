using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public enum AdminMessageType
    {
		ADMIN_PACKET_ADMIN_JOIN,             /// The admin announces and authenticates itself to the server.
		ADMIN_PACKET_ADMIN_QUIT,             /// The admin tells the server that it is quitting.
		ADMIN_PACKET_ADMIN_UPDATE_FREQUENCY, /// The admin tells the server the update frequency of a particular piece of information.
		ADMIN_PACKET_ADMIN_POLL,             /// The admin explicitly polls for a piece of information.
		ADMIN_PACKET_ADMIN_CHAT,             /// The admin sends a chat message to be distributed.
		ADMIN_PACKET_ADMIN_RCON,             /// The admin sends a remote console command.
		ADMIN_PACKET_ADMIN_GAMESCRIPT,       /// The admin sends a JSON string for the GameScript.
		ADMIN_PACKET_ADMIN_PING,             /// The admin sends a ping to the server, expecting a ping-reply (PONG) packet.

		ADMIN_PACKET_SERVER_FULL = 100,      /// The server tells the admin it cannot accept the admin.
		ADMIN_PACKET_SERVER_BANNED,          /// The server tells the admin it is banned.
		ADMIN_PACKET_SERVER_ERROR,           /// The server tells the admin an error has occurred.
		ADMIN_PACKET_SERVER_PROTOCOL,        /// The server tells the admin its protocol version.
		ADMIN_PACKET_SERVER_WELCOME,         /// The server welcomes the admin to a game.
		ADMIN_PACKET_SERVER_NEWGAME,         /// The server tells the admin its going to start a new game.
		ADMIN_PACKET_SERVER_SHUTDOWN,        /// The server tells the admin its shutting down.

		ADMIN_PACKET_SERVER_DATE,            /// The server tells the admin what the current game date is.
		ADMIN_PACKET_SERVER_CLIENT_JOIN,     /// The server tells the admin that a client has joined.
		ADMIN_PACKET_SERVER_CLIENT_INFO,     /// The server gives the admin information about a client.
		ADMIN_PACKET_SERVER_CLIENT_UPDATE,   /// The server gives the admin an information update on a client.
		ADMIN_PACKET_SERVER_CLIENT_QUIT,     /// The server tells the admin that a client quit.
		ADMIN_PACKET_SERVER_CLIENT_ERROR,    /// The server tells the admin that a client caused an error.
		ADMIN_PACKET_SERVER_COMPANY_NEW,     /// The server tells the admin that a new company has started.
		ADMIN_PACKET_SERVER_COMPANY_INFO,    /// The server gives the admin information about a company.
		ADMIN_PACKET_SERVER_COMPANY_UPDATE,  /// The server gives the admin an information update on a company.
		ADMIN_PACKET_SERVER_COMPANY_REMOVE,  /// The server tells the admin that a company was removed.
		ADMIN_PACKET_SERVER_COMPANY_ECONOMY, /// The server gives the admin some economy related company information.
		ADMIN_PACKET_SERVER_COMPANY_STATS,   /// The server gives the admin some statistics about a company.
		ADMIN_PACKET_SERVER_CHAT,            /// The server received a chat message and relays it.
		ADMIN_PACKET_SERVER_RCON,            /// The server's reply to a remove console command.
		ADMIN_PACKET_SERVER_CONSOLE,         /// The server gives the admin the data that got printed to its console.
		ADMIN_PACKET_SERVER_CMD_NAMES,       /// The server sends out the names of the DoCommands to the admins.
		ADMIN_PACKET_SERVER_CMD_LOGGING,     /// The server gives the admin copies of incoming command packets.
		ADMIN_PACKET_SERVER_GAMESCRIPT,      /// The server gives the admin information from the GameScript in JSON.
		ADMIN_PACKET_SERVER_RCON_END,        /// The server indicates that the remote console command has completed.
		ADMIN_PACKET_SERVER_PONG,            /// The server replies to a ping request from the admin.

		INVALID_ADMIN_PACKET = 0xFF,         /// An invalid marker for admin packets.
	}
}
