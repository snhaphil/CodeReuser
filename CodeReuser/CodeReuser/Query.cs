using System;
using System.Threading.Tasks;

namespace CodeReuser
{
    class Query
    {
        public static string RunQueryAsync(string text)
        {
            try
            {
                VisualStudioCodeSearchHelper vsoSearch = new VisualStudioCodeSearchHelper();
                var searchResults = vsoSearch.RunSearchQueryAsync(
                    new CodeSearchQuery
                    {
                        SearchText = text,
                        QuerySearchFilters = new CodeSearchFilters
                        {
                            Project = new string[] { "One" },
                        },
                        SkipResults = 0,
                        TakeResults = 100
                    }).Result;
                Console.WriteLine(searchResults.Count);
                return searchResults.ResultValues[0]?.FileName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
