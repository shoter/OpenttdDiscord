using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public static Task WaitMax(this Task task, [CallerMemberName]string callerName = null) => task.WaitMax(TimeSpan.FromSeconds(10), callerName);
        public static async Task WaitMax(this Task task, TimeSpan waitTime, [CallerMemberName]string callerName = null)
        {
            Task delayTask = Task.Delay(waitTime);

            await Task.WhenAny(task, delayTask);

            if(delayTask.IsCompleted)
            {
                throw new TaskWaitException($"Inside {callerName} there was task timeout. Refer to exception to find more details.");
            }
        }

        public static Task<T> WaitMax<T>(this Task<T> task, [CallerMemberName]string callerName = null) => task.WaitMax(TimeSpan.FromSeconds(10), callerName);


        public static async Task<T> WaitMax<T>(this Task<T> task, TimeSpan waitTime, [CallerMemberName]string callerName = null)
        {
            Task delayTask = Task.Delay(waitTime);

            await Task.WhenAny(task, delayTask);

            if (delayTask.IsCompleted)
            {
                throw new TaskWaitException($"Inside {callerName} there was task timeout. Refer to exception to find more details.");
            }

            return await task;
        }

    }
}
