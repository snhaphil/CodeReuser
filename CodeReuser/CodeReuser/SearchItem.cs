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

        protected bool Equals(SearchItem other)
        {
            return Type == other.Type && Accuracy == other.Accuracy && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SearchItem) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Type;
                hashCode = (hashCode * 397) ^ (int) Accuracy;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
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
