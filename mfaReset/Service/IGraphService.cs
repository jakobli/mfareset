using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mfaReset.Models;

namespace mfaReset.Service
{
    public interface IGraphService
    {
        Task<User> GetUser(string upn);
        Task<bool> SendMail(string mail);

    }
}
