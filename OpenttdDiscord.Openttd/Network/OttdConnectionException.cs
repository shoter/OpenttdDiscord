using OpenttdDiscord.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network
{
    public class OttdConnectionException : OttdException
    {
        public OttdConnectionException()
        {
        }

        public OttdConnectionException(string message) : base(message)
        {
        }

        public OttdConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OttdConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
