using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace CodeReuser
{
    public class MicrosoftLoginAuthorizationHeaderProvider : IAuthorizationHeaderProvider
    {
        public static MicrosoftLoginAuthorizationHeaderProvider Instance => new MicrosoftLoginAuthorizationHeaderProvider();

        private MicrosoftLoginAuthorizationHeaderProvider()
        {
            var ctx = new AuthenticationContext("https://login.windows.net/common");

            if (ctx.TokenCache.Count > 0)
            {
                string homeTenant = ctx.TokenCache.ReadItems().First().TenantId;
                ctx = new AuthenticationContext("https://login.microsoftonline.com/" + homeTenant);
            }

            _authenticationContext =  ctx;
        }

        /// <inheritdoc />
        public async Task<AuthenticationHeaderValue> GetAsync()
        {
            AuthenticationResult result = null;

            try
            {
                var silentTask = await _authenticationContext.AcquireTokenAsync(CVSTSResourceId, CClientId, SReplyUri, new PlatformParameters(PromptBehavior.Auto));
                return new AuthenticationHeaderValue("Bearer", silentTask.AccessToken);
            }
            catch (AdalException adalException)
            {
                if (adalException.ErrorCode == AdalError.FailedToAcquireTokenSilently
                    || adalException.ErrorCode == AdalError.InteractionRequired)
                {
                    result = await _authenticationContext.AcquireTokenAsync(CVSTSResourceId, CClientId, SReplyUri, new PlatformParameters(PromptBehavior.Auto));
                }
            }

            return result == null ? null : new AuthenticationHeaderValue("Bearer", result.AccessToken);
        }



        private const string CVSTSResourceId = "499b84ac-1321-427f-aa17-267ca6975798";       // Constant value to target VSTS. Do not change  
        private const string CVstsCollectionUrl = "https://msazure.visualstudio.com";        // Change to the URL of your VSTS account; NOTE: This must use HTTPS
        private const string CClientId = "872cd9fa-d31f-45e0-9eab-6e460a02d1f1";             // Change to your app registration's Application ID, unless you are an MSA backed account
        private static readonly Uri SReplyUri = new Uri("urn:ietf:wg:oauth:2.0:oob");        // Change to your app registration's reply URI, unless you are an MSA backed account

        private readonly AuthenticationContext _authenticationContext;


    }
}
