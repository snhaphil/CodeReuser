using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CodeReuser
{
    class Query
    {
        private Lazy<VisualStudioCodeSearchHelper> _vsoSearch;

        public Query()
        {
            _vsoSearch = new Lazy<VisualStudioCodeSearchHelper>(() => new VisualStudioCodeSearchHelper());
        }

        public async Task<CodeSearchResponse> RunTextQueryWithAstrixIfNotFoundAsync(SearchItem item)
        {
            var result = await RunTextQueryAsync(item);
            if (result.Count == 0)
            {
                result = await RunTextQueryAsync(new SearchItem(item.Type, item.Name + "*"));
                item.Accuracy = SearchAccuracy.Astrix;
            }

            return result;
        }

        public async Task<CodeSearchResponse> RunTextQueryAsync(SearchItem item)
        {
            try
            {
                if (item.IsEmpty())
                {
                    return CodeSearchResponse.Empty;
                }
                var prefix = item.Type.ToString().ToLower();
                var searchResults = await _vsoSearch.Value.RunSearchQueryAsync(
                    new CodeSearchQuery
                    {
                        SearchText = $"{prefix}:{item.Name} ext:cs",
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
                return CodeSearchResponse.Empty;
            }
        }
    }
}
