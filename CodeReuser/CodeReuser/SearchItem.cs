using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReuser
{
    public class SearchItem
    {
        public SearchItem(SearchType type, string name)
        {
            Type = type;
            Name = name;
            Accuracy = SearchAccuracy.Accurate;
        }

        public SearchType Type;
        public SearchAccuracy Accuracy;
        public string Name;

        public static SearchItem EmptySearchItem = new SearchItem(SearchType.None, string.Empty);

        public bool IsEmpty()
        {
            return Type == EmptySearchItem.Type && Name == EmptySearchItem.Name;
        }
    }

    public enum SearchAccuracy
    {
        Accurate,
        Astrix
    }
}
