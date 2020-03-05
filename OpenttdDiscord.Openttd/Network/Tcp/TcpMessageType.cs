using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Tcp
{
    public enum TcpMessageType
    {
        /*
	 * These first three pair of packets (thus six in
	 * total) must remain in this order for backward
	 * and forward compatibility between clients that
	 * are trying to join directly.
	 */

        /* Packets sent by socket accepting code without ever constructing a client socket instance. */
        PACKET_SERVER_FULL,                  /// The server is full and has no place for you.
        PACKET_SERVER_BANNED,                /// The server has banned you.

        /* Packets used by the client to join and an error message when the revision is wrong. */
        PACKET_CLIENT_JOIN,                  /// The client telling the server it wants to join.
        PACKET_SERVER_ERROR,                 /// Server sending an error message to the client.

        /* Packets used for the pre-game lobby. */
        PACKET_CLIENT_COMPANY_INFO,          /// Request information about all companies.
        PACKET_SERVER_COMPANY_INFO,          /// Information about a single company.

        /*
         * Packets after here assume that the client
         * and server are running the same version. As
         * such ordering is unimportant from here on.
         *
         * The following is the remainder of the packets
         * sent as part of authenticating and getting
         * the map and other important data.
         */

        /* After the join step, the first is checking NewGRFs. */
        PACKET_SERVER_CHECK_NEWGRFS,         /// Server sends NewGRF IDs and MD5 checksums for the client to check.
        PACKET_CLIENT_NEWGRFS_CHECKED,       /// Client acknowledges that it has all required NewGRFs.

        /* Checking the game, and then company passwords. */
        PACKET_SERVER_NEED_GAME_PASSWORD,    /// Server requests the (hashed) game password.
        PACKET_CLIENT_GAME_PASSWORD,         /// Clients sends the (hashed) game password.
        PACKET_SERVER_NEED_COMPANY_PASSWORD, /// Server requests the (hashed) company password.
        PACKET_CLIENT_COMPANY_PASSWORD,      /// Client sends the (hashed) company password.

        /* The server welcomes the authenticated client and sends information of other clients. */
        PACKET_SERVER_WELCOME,               /// Server welcomes you and gives you your #ClientID.
        PACKET_SERVER_CLIENT_INFO,           /// Server sends you information about a client.

        /* Getting the savegame/map. */
        PACKET_CLIENT_GETMAP,                /// Client requests the actual map.
        PACKET_SERVER_WAIT,                  /// Server tells the client there are some people waiting for the map as well.
        PACKET_SERVER_MAP_BEGIN,             /// Server tells the client that it is beginning to send the map.
        PACKET_SERVER_MAP_SIZE,              /// Server tells the client what the (compressed) size of the map is.
        PACKET_SERVER_MAP_DATA,              /// Server sends bits of the map to the client.
        PACKET_SERVER_MAP_DONE,              /// Server tells it has just sent the last bits of the map to the client.
        PACKET_CLIENT_MAP_OK,                /// Client tells the server that it received the whole map.

        PACKET_SERVER_JOIN,                  /// Tells clients that a new client has joined.

        /*
         * At this moment the client has the map and
         * the client is fully authenticated. Now the
         * normal communication starts.
         */

        /* Game progress monitoring. */
        PACKET_SERVER_FRAME,                 /// Server tells the client what frame it is in, and thus to where the client may progress.
        PACKET_CLIENT_ACK,                   /// The client tells the server which frame it has executed.
        PACKET_SERVER_SYNC,                  /// Server tells the client what the random state should be.

        /* Sending commands around. */
        PACKET_CLIENT_COMMAND,               /// Client executed a command and sends it to the server.
        PACKET_SERVER_COMMAND,               /// Server distributes a command to (all) the clients.

        /* Human communication! */
        PACKET_CLIENT_CHAT,                  /// Client said something that should be distributed.
        PACKET_SERVER_CHAT,                  /// Server distributing the message of a client (or itself).

        /* Remote console. */
        PACKET_CLIENT_RCON,                  /// Client asks the server to execute some command.
        PACKET_SERVER_RCON,                  /// Response of the executed command on the server.

        /* Moving a client.*/
        PACKET_CLIENT_MOVE,                  /// A client would like to be moved to another company.
        PACKET_SERVER_MOVE,                  /// Server tells everyone that someone is moved to another company.

        /* Configuration updates. */
        PACKET_CLIENT_SET_PASSWORD,          /// A client (re)sets its company's password.
        PACKET_CLIENT_SET_NAME,              /// A client changes its name.
        PACKET_SERVER_COMPANY_UPDATE,        /// Information (password) of a company changed.
        PACKET_SERVER_CONFIG_UPDATE,         /// Some network configuration important to the client changed.

        /* A server quitting this game. */
        PACKET_SERVER_NEWGAME,               /// The server is preparing to start a new game.
        PACKET_SERVER_SHUTDOWN,              /// The server is shutting down.

        /* A client quitting. */
        PACKET_CLIENT_QUIT,                  /// A client tells the server it is going to quit.
        PACKET_SERVER_QUIT,                  /// A server tells that a client has quit.
        PACKET_CLIENT_ERROR,                 /// A client reports an error to the server.
        PACKET_SERVER_ERROR_QUIT,            /// A server tells that a client has hit an error and did quit.

        PACKET_END,                          /// Must ALWAYS be on the end of this list!! (period)


    }
}
