using System;
using System.Threading.Tasks;

namespace CodeReuser
{
    class Query
    {
        public async Task<CodeSearchResponse> RunTextQueryWithAstrixIfNotFoundAsync(SearchItem item)
        {
            var result = await RunTextQueryAsync(item);
            if (result.Count == 0)
            {
                result = await RunTextQueryAsync(new SearchItem(item.Type, item.Name + "*"));
            }

            return result;
        }

        public async Task<CodeSearchResponse> RunTextQueryAsync(SearchItem item)
        {
            try
            {
                VisualStudioCodeSearchHelper vsoSearch = new VisualStudioCodeSearchHelper();
                if (item.IsEmpty())
                {
                    return new CodeSearchResponse()
                    {
                        Count = 0,
                        ResultValues = new CodeSearchResponse.SearchResultValue[0]
                    };
                }
                var prefix = item.Type.ToString().ToLower();
                var searchResults = await vsoSearch.RunSearchQueryAsync(
                    new CodeSearchQuery
                    {
                        SearchText = $"{prefix}:{item.Name}",
                        QuerySearchFilters = new CodeSearchFilters
                        {
                            Project = new string[] { "One" },
                        },
                        SkipResults = 0,
                        TakeResults = 100
                    });
                Console.WriteLine(searchResults.Count);
                return searchResults;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new CodeSearchResponse()
                {
                    Count = 0,
                    ResultValues = new CodeSearchResponse.SearchResultValue[0]
                };
            }
        }
    }
}
