using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReuser
{
    public class SearchItem
    {
        public SearchItem(SearchType type, string text)
        {
            Type = type;
            Text = text;
        }

        public SearchType Type;
        public string Text;

        public static SearchItem EmptySearchItem = new SearchItem(SearchType.None, string.Empty);

        public bool IsEmpty()
        {
            return Type == EmptySearchItem.Type && Text == EmptySearchItem.Text;
        }
    }
}
