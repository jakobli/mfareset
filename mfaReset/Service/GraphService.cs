using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using mfaReset.Models;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Security.Claims;
using System.Configuration;
using Microsoft.Graph;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace mfaReset.Service
{

    public class GraphService : IGraphService
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string appKey = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string graphResourceID = "https://graph.microsoft.com";
        private static string tenantID = ConfigurationManager.AppSettings["ida:TenantId"];
        private static GraphServiceClient graphClient;

        public static async Task<GraphServiceClient> GetAuthenticatedGClient()
        {
            try
            {
                graphClient = new GraphServiceClient(
                        new DelegateAuthenticationProvider(
                async (requestMessage) =>
                {
                    string accessToken = await GetTokenForApplication();
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
                }));
            }
            catch (ServiceException e)
            {
                throw new Exception("Could not create Graph client", e);
            }
            return graphClient;


        }
        public async Task<User> GetUser(string upn)
        {
            GraphServiceClient graphClient = await GetAuthenticatedGClient();
            User user = await graphClient.Users[upn].Request().GetAsync();
            return user;
        }

        public async Task<bool> SendMail(string msg)
        {
            List<Recipient> recipients = new List<Recipient>();
            recipients.Add(new Recipient
            {
                EmailAddress = new EmailAddress
                {
                    Address = "your@email.com" //feedback recepient
                }
            });

            // Create the message.
            Message email = new Message
            {
                Body = new ItemBody
                {
                    Content = msg,
                    ContentType = BodyType.Text,
                },
                Subject = "MFA Reset Feedback",
                ToRecipients = recipients
            };
            GraphServiceClient graphClient = await GetAuthenticatedGClient();
            await graphClient.Me.SendMail(email, true).Request().PostAsync();
            return true;
        }
        private static async Task<string> GetTokenForApplication()
        {
            string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            // get a token for the Graph without triggering any user interaction (from the cache, via multi-resource refresh token, etc)
            ClientCredential clientcred = new ClientCredential(clientId, appKey);
            // initialize AuthenticationContext with the token cache of the currently signed in user, as kept in the app's database
            AuthenticationContext authenticationContext = new AuthenticationContext(aadInstance + tenantID, new TableTokenCache(signedInUserID));
            AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenSilentAsync(graphResourceID, clientcred, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));
            return authenticationResult.AccessToken;
        }
    }
}