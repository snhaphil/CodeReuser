using System.Net.Http;

namespace CodeReuser
{
    public class MsAzureClient
    {
        public string GetRecommendation(string searchText)
        {
            var client = new HttpClient();
            HttpResponseMessage response = client.GetAsync("https://jsonplaceholder.typicode.com/posts/1").Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }

            return string.Empty;
        }
    }
}
