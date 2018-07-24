using System;
using System.Threading.Tasks;

namespace CodeReuser
{
    class Query
    {
        public async Task<CodeSearchResponse> RunTextQueryAsync(SearchItem item)
        {
            try
            {
                VisualStudioCodeSearchHelper vsoSearch = new VisualStudioCodeSearchHelper();
                var searchResults = await vsoSearch.RunSearchQueryAsync(
                    new CodeSearchQuery
                    {
                        SearchText = item.Name,
                        QuerySearchFilters = new CodeSearchFilters
                        {
                            Project = new string[] { "One" },
                            CodeElement = new string[] { item.Type == SearchType.None ? "Def" : item.Type.ToString()   }
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
                throw;
            }

        }
    }
}
