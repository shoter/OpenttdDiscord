﻿using static OpenttdDiscord.Infrastructure.Akkas.MainActors;

namespace OpenttdDiscord.Infrastructure.Akkas
{
    public static class MainActors
    {
        public static class Names
        {
            public const string Guilds = "guilds";

            /// <summary>
            /// Guild-<b>{guildId}</b>
            /// </summary>
            public static string Guild(ulong guildId) => $"Guild-{guildId}";
        }

        public static class Paths
        {
            public const string Guilds = "/user/" + Names.Guilds;
        }
    }
}
