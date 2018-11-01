using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mfaReset.Service;

namespace mfaResetTests.Fakes
{
    public class FakeLogService : ILogService
    {
        public bool LogRequest(string requestor, string user, DateTime requestTime)
        {
            return true;
        }
    }
}
