using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mfaReset.Service;

namespace mfaResetTests.Fakes
{
    public class FakeFunctionService : IFunctionService
    {
        public Task<bool> ResetUserAsync(string upn, string user)
        {
            return Task.FromResult<bool>(true);
        }
    }
}
