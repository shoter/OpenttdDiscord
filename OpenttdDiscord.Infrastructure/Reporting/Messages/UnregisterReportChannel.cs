namespace OpenttdDiscord.Infrastructure.Reporting.Messages;

internal record UnregisterReportChannel(Guid ServerId, ulong GuildId, ulong channelId);
