using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileBandSync.OpenTcx
{
    public class OpenTcxException : Exception
    {
        public OpenTcxException(string message) : base(message)
        {

        }

        public OpenTcxException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
