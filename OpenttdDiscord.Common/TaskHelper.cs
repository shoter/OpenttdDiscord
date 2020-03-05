using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Common
{
    public static class TaskHelper
    {
        public static async Task<bool> WaitUntil(Func<bool> condition, TimeSpan delayBetweenChecks, TimeSpan duration)
        {
            var startTime = DateTime.Now;
            while ((DateTime.Now - startTime) < duration)
            {
                if (condition())
                    return true;

                await Task.Delay(delayBetweenChecks);
            }

            return false;
        }

    }
}
