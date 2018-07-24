using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CodeReuser
{
    public static class LineParser
    {
        public static bool IsSearchable(string line)
        {
            // Dont use Regex here because string interpolation is faster than Regex
            if (line.Contains(" class ") || line.Contains(" interface ") || (line.Contains("(") && line.Contains(")")))
            {
                return true;
            }

            return false;
        }

        public static SearchItem GetSearchableItem(string line)
        {
            if (line.Contains(" class "))
            {
                var match = Regex.Match(line, CIdentifiers[SearchType.Class]);
                if (match.Success)
                {
                    return new SearchItem(SearchType.Class, match.Groups[1].Value);
                }
            }

            if (line.Contains(" interface "))
            {
                var match = Regex.Match(line, CIdentifiers[SearchType.Interface]);
                if (match.Success)
                {
                    return new SearchItem(SearchType.Interface, match.Groups[1].Value);
                }
            }

            if (line.Contains("(") && line.Contains(")"))
            {
                var match = Regex.Match(line, CIdentifiers[SearchType.Method]);
                if (match.Success)
                {
                    return new SearchItem(SearchType.Method, match.Groups[1].Value);
                }
            }

            return SearchItem.EmptySearchItem;
        }

        public static Dictionary<SearchType, string> CIdentifiers = new Dictionary<SearchType, string>()
        {
            { SearchType.Method, @"\s+([A-Za-z]+)\(([A-Za-z\s\,*]*)\)" },
            { SearchType.Class, @"class\s+([A-Za-z]+)" },
            { SearchType.Interface, @"interface\s+([A-Za-z]+)" },
        };
    }
}
