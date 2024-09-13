using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectMyDoc_Domain_Layer.Services
{
    public interface IMessageService
    {
        string GetMessage(string key);
    }
}
