using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PharmITExchange.Common
{
    public class PharmITException : ApplicationException
    {
        public PharmITException()
        {
        }

        public PharmITException(string message) : base(message)
        {
        }

        public PharmITException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PharmITException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
