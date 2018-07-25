using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CodeReuser
{
    public class VisualStudioCodeSearchHelper
    {
        /// <summary>
        /// Root, parameterized URL for queries to CodeSearch API.
        /// </summary>
        private readonly Uri DefaultTeamUri = new Uri("https://msazure.almsearch.VisualStudio.com/one/_apis/search/codesearchresults?api-version=4.1-preview.1");

        public HttpClient OneProjectGitReposHttpClient { get; }

        public HttpClient CodeSearchHttpClient { get; private set; }

        public VisualStudioCodeSearchHelper()
        {
            this.CodeSearchHttpClient = VisualStudioHttpClientPool.GetBasicHttpClient(DefaultTeamUri);
            OneProjectGitReposHttpClient = VisualStudioHttpClientPool.GetBasicHttpClient(new Uri("https://msazure.visualstudio.com/one/_apis/git/repositories/"));
        }

        /// <summary>
        /// Executes a search query against VSTS code search service
        /// </summary>
        /// <param name="query">Code search query</param>
        /// <returns>Results from code search query</returns>
        public async Task<CodeSearchResponse> RunSearchQueryAsync(CodeSearchQuery query)
        {
            var requestString = JsonConvert.SerializeObject(query);
            var httpContent = new StringContent(requestString, Encoding.UTF8, "application/json");

            var response = await this.CodeSearchHttpClient.PostAsync(string.Empty, httpContent).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return CodeSearchResponse.Empty;
            }

            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var responseObject = (CodeSearchResponse)JsonConvert.DeserializeObject(responseString, typeof(CodeSearchResponse));
            return responseObject;
        }

        public async Task<SourceFile> DownloadSourceFileAsync(string repoId, string path)
        {
            var response = await this.OneProjectGitReposHttpClient.GetAsync(new Uri($"{repoId}/items?path={path}&includeContent=true&includeContentMetadata=true&api-version=4.1", UriKind.Relative)).ConfigureAwait(false);

            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var responseObject = (SourceFile)JsonConvert.DeserializeObject(responseString, typeof(SourceFile));

            return responseObject;
        }
    }
}
