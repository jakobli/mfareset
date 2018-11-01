using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using mfaReset.Models;

namespace mfaReset.Service
{
    public class LogService : ILogService
    {
        private static string _connectionString = ConfigurationManager.AppSettings["sta:endpoint"];
        private static MfaResetRepository MfaLogs;

        public bool LogRequest(string requestor, string user, DateTime requestTime)
        {
            try
            {
                MfaLogs = new MfaResetRepository(_connectionString);
                MfaLogEntity newLog = new MfaLogEntity(requestor, user, requestTime);
                MfaLogs.InsertOrReplace(newLog);
                return true;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}