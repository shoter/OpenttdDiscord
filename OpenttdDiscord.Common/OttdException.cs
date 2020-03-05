using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Common
{
    public class OttdException : Exception
    {
        public OttdException()
        {
        }

        public OttdException(string message) : base(message)
        {
        }

        public OttdException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OttdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
