using System;
using System.Collections.Concurrent;
using System.Net.Http;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.WebApi;

namespace CodeReuser
{
    public class VisualStudioHttpClientPool
    {
        private static ConcurrentDictionary<string, HttpClient> basicHttpClientCache = new ConcurrentDictionary<string, HttpClient>();
        private static ConcurrentDictionary<string, VssHttpClientBase> vssHttpClientCache = new ConcurrentDictionary<string, VssHttpClientBase>();

        private static ConcurrentDictionary<string, VssConnection> vssConnectionCache = new ConcurrentDictionary<string, VssConnection>();


        public static VssConnection GetConnection(Uri uri)
        {
            var connection = new VssConnection(uri, new VssClientCredentials());
            return connection;
        }

        public static HttpClient GetBasicHttpClient(Uri uri)
        {
            string cacheKey = uri.OriginalString;

            if (!basicHttpClientCache.ContainsKey(cacheKey))
            {
                var httpClient = new HttpClient();

                var header = MicrosoftLoginAuthorizationHeaderProvider.Instance.GetAsync().Result;
                httpClient.BaseAddress = uri;
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = header;

                basicHttpClientCache.TryAdd(uri.OriginalString, httpClient);
            }

            return basicHttpClientCache[uri.OriginalString];
        }

        public static T GetVssHttpClient<T>(Uri uri) where T : VssHttpClientBase
        {
            string cacheKey = uri.OriginalString;

            if (!vssHttpClientCache.ContainsKey(cacheKey))
            {
                VssConnection vssConnection = GetConnection(uri);

                //var vssConnection = new VssConnection(uri, new VssAadCredential());
                //var clientCredentials = new VssClientCredentials(new VssBasicCredential("<UserName>","<Password-FromKeyVault>"));
                //vssConnection = new VssConnection(this.TeamUri, clientCredentials);

                vssHttpClientCache.TryAdd(uri.OriginalString, vssConnection.GetClient<T>());
            }

            return vssHttpClientCache[uri.OriginalString] as T;
        }
    }
}
