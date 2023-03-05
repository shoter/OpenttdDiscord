using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenttdDiscord.Domain.Statuses;

namespace OpenttdDiscord.Infrastructure.Statuses.Messages
{
    public record UpdateStatusMonitor(StatusMonitor UpdatedMonitor);
}
