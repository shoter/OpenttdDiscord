using OpenttdDiscord.Domain.Rcon;

namespace OpenttdDiscord.Infrastructure.Rcon.Messages;

internal record RegisterNewRconChannel(Guid ServerId, RconChannel RconChannel);
