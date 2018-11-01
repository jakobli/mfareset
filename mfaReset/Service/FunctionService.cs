using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using mfaReset.Models;

namespace mfaReset.Service
{
    public class FunctionService: IFunctionService
    {
        static HttpClient client = new HttpClient();
        static TelemetryClient telemetry = new TelemetryClient();
        private static string baseUrl = ConfigurationManager.AppSettings["fun:Endpoint"];
        private static string functionKey = ConfigurationManager.AppSettings["fun:funKey"];
        public async Task<bool> ResetUserAsync(string upn, string user)
        {
            try
            {
                ResetModel model = new ResetModel();
                model.UPN = upn;
                model.Requestor = user;
                HttpResponseMessage response = await client.PostAsJsonAsync(baseUrl + "MFAReset-PS?code="+functionKey, model);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                telemetry.TrackException(e);
                throw new Exception("MFA Reset failed  " + e.Message + e.StackTrace, e);
            }
        }
    }

}