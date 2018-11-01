using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mfaReset.Service
{
    public interface ILogService
    {
        bool LogRequest(string requestor, string user, DateTime requestTime);
    }
}
