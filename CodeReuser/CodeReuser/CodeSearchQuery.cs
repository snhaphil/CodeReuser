using Newtonsoft.Json;

namespace CodeReuser
{
    /// <summary>
    /// Query request for VSTS Code Search API
    /// </summary>
    public class CodeSearchQuery
    {
        /// <summary>
        /// Initializes a new instance of the CodeSearchQuery class.
        /// Default constructor with default take/pagination.
        /// </summary>
        public CodeSearchQuery()
        {
            this.SkipResults = 0;
            this.TakeResults = 50;
        }

        /// <summary>
        /// Gets or sets primary search text. Equivalent to search bar in VSTS.
        /// </summary>
        [JsonProperty("searchText")]
        public string SearchText { get; set; }

        /// <summary>
        /// Gets or sets number of results to skip before returning. Used for pagination.
        /// </summary>
        [JsonProperty("$skip")]
        public int SkipResults { get; set; }

        /// <summary>
        /// Gets or sets number of results to return.
        /// </summary>
        [JsonProperty("$top")]
        public int TakeResults { get; set; }

        /// <summary>
        /// Gets or sets filter criteria for the search. Ie, specific files types.
        /// </summary>
        [JsonProperty("filters")]
        public CodeSearchFilters QuerySearchFilters { get; set; }
    }
}
