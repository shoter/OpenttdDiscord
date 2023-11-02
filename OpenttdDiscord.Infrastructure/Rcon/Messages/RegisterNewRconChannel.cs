using OpenttdDiscord.Domain.Rcon;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.Rcon.Messages;

internal record RegisterNewRconChannel(
    Guid ServerId,
    RconChannel RconChannel) : IOttdServerMessage;
