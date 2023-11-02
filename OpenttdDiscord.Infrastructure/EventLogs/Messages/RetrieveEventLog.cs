using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.EventLogs.Messages;

internal record RetrieveEventLog(
    Guid ServerId,
    ulong GuildId) : IOttdServerMessage;
