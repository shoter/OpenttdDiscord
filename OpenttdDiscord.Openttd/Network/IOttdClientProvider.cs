﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network
{
    public interface IOttdClientProvider
    {
        IOttdClient Provide(string serverIp, int port);
    }
}
