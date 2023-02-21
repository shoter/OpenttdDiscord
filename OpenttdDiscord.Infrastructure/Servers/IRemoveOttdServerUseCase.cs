﻿using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Infrastructure.Servers
{
    internal interface IRemoveOttdServerUseCase
    {
        Task<EitherUnit> Execute(User userRights, ulong guildId, string serverName);
    }
}
