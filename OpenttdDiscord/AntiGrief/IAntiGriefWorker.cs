using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.AntiGrief
{
    public interface IAntiGriefWorker
    {
        Task Start();
    }
}
