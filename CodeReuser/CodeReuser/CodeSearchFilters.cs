using Newtonsoft.Json;

namespace CodeReuser
{
    /// <summary>
    /// Filter criteria for a VSTS code search query.
    /// </summary>
    public class CodeSearchFilters
    {
        /// <summary>
        /// Gets or sets list of VSTS projects to scope the search to.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] Project { get; set; }

        /// <summary>
        /// Gets or sets list of repositories to scope the search to.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] Repository { get; set; }

        /// <summary>
        /// Gets or sets list of branches to scope search to.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] Branch { get; set; }

        /// <summary>
        /// Gets or sets list of code elements to scope search to, ie class names.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] CodeElement { get; set; }

        /// <summary>
        /// Gets or sets list of paths to scope search to.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] Path { get; set; }
    }
}
