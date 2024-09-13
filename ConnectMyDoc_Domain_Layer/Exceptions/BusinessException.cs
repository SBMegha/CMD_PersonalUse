using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectMyDoc_Domain_Layer.Exceptions
{
    public class BusinessException : ApplicationException
    {
        public BusinessException() { }

        public BusinessException(string message) : base(message) { }

        public BusinessException(string message, Exception exception) : base(message, exception) { }

    }
}
