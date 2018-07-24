using System;
using System.Threading.Tasks;

namespace CodeReuser
{
    class Program
    {
        static void Main(string[] args)
        {
            RunQueryAsync("Va").Wait();
        }

        private static async Task RunQueryAsync(string text)
        {
            try
            {
                VisualStudioCodeSearchHelper vsoSearch = new VisualStudioCodeSearchHelper();
                var searchResults = await vsoSearch.RunSearchQueryAsync(
                    new CodeSearchQuery
                    {
                        SearchText = text,
                        QuerySearchFilters = new CodeSearchFilters
                        {
                            Project = new string[] { "One" },
                        },
                        SkipResults = 0,
                        TakeResults = 100
                    }).ConfigureAwait(false);
                Console.WriteLine(searchResults.Count);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
