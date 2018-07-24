using System;
using System.Threading.Tasks;

namespace CodeReuser
{
    class Query : IQuery
    {
        public CodeSearchResponse RunCodeElementQuery(SearchItem item)
        {
            throw new NotImplementedException();
        }

        public CodeSearchResponse RunTextQuery(SearchItem item)
        {
            try
            {
                return new CodeSearchResponse();
                /*
                VisualStudioCodeSearchHelper vsoSearch = new VisualStudioCodeSearchHelper();
                var searchResults = vsoSearch.RunSearchQueryAsync(
                    new CodeSearchQuery
                    {
                        SearchText = item.Text,
                        QuerySearchFilters = new CodeSearchFilters
                        {
                            Project = new string[] { "One" },
                        },
                        SkipResults = 0,
                        TakeResults = 100
                    }).Result;
                Console.WriteLine(searchResults.Count);
                return searchResults;*/
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
