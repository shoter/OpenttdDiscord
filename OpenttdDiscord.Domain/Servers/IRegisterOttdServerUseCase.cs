﻿using LanguageExt;
using LanguageExt.Common;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Servers
{
    public interface IRegisterOttdServerUseCase : IUseCase
    {
        Task<EitherUnit> Execute(User userRights, OttdServer server);
    }
}
