namespace OpenttdDiscord.Domain.Reporting;

public record ReportChannel(Guid ServerId, ulong GuildId, ulong ChannelId);
