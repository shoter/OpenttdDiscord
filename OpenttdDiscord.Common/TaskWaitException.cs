using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Common
{
    public class TaskWaitException : Exception
    {
        public TaskWaitException()
        {
        }

        public TaskWaitException(string message) : base(message)
        {
        }

        public TaskWaitException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TaskWaitException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
