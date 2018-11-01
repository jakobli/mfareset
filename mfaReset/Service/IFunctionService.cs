using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mfaReset.Service
{
    public interface IFunctionService
    {
        Task<bool> ResetUserAsync(string upn, string user);
    }
}
