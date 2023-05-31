using static OpenttdDiscord.Infrastructure.Akkas.MainActors;

namespace OpenttdDiscord.Infrastructure.Akkas
{
    public static class MainActors
    {
        public static class Names
        {
            public const string Guilds = "guilds";

            public const string ChatChannelManager = "chat-channels";

            /// <summary>
            /// Guild-<b>{guildId}</b>
            /// </summary>
            public static string Guild(ulong guildId) => $"Guild-{guildId}";

            public const string HealthCheck = "health-check";
        }

        public static class Paths
        {
            public const string Guilds = "/user/" + Names.Guilds;

            public const string HealthCheck = "/user/" + Names.HealthCheck;

            public const string ChatChannelManager = "/user/" + Names.ChatChannelManager;
        }
    }
}
