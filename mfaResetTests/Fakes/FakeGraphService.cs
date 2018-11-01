using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mfaReset.Service;

namespace mfaResetTests.Fakes
{
    public class FakeGraphService : IGraphService
    {
        private List<User> userList;
        public FakeGraphService(List<User> users)
        {
            userList = users;
        }
        public Task<User> GetUser(string upn)
        {
            var result = userList.SingleOrDefault(u => u.UserPrincipalName == upn);
            return Task.FromResult<User>(result);
        }

        public Task<bool> SendMail(string mail)
        {
            return Task.FromResult<bool>(true);
        }
    }
}
