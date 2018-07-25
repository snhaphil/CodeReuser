using System.Collections.Generic;
using Newtonsoft.Json;

namespace CodeReuser
{
    /// <summary>
    /// Root response from the VSTS Code Search service.
    /// </summary>
    public class CodeSearchResponse
    {
        public static CodeSearchResponse Empty => new CodeSearchResponse
        {
            Count = 0,
            ResultValues = new SearchResultValue[0]
        };

        /// <summary>
        /// Gets or sets count of results.
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets list of results from VSTS Code Search query.
        /// </summary>
        [JsonProperty("results")]
        public SearchResultValue[] ResultValues { get; set; }

        /// <summary>
        /// Value of the search result from VSTS code search service.
        /// </summary>
        public class SearchResultValue
        {
            [JsonProperty("collection")]
            public Collection Collection { get; set; }

            /// <summary>
            /// Gets or sets the content ID for the file/resource where his is located.
            /// </summary>
            [JsonProperty("contentId")]
            public string ContentId { get; set; }

            /// <summary>
            /// Gets or sets file name where match occurred.
            /// </summary>
            [JsonProperty("fileName")]
            public string FileName { get; set; }

            public Dictionary<string, Match[]> Matches { get; set; }

            /// <summary>
            /// Gets or sets path where search result was found.
            /// </summary>
            [JsonProperty("path")]
            public string Path { get; set; }

            /// <summary>
            /// Gets or sets VSTS Project where hit is located.
            /// </summary>
            [JsonProperty("project")]
            public Project Project { get; set; }

            [JsonProperty("repository")]
            public Repository Repository { get; set; }

            public Version[] Versions { get; set; }

        }
    }

    public class Match
    {
        public int CharOffset { get; set; }

        public int Length { get; set; }

    }

    public class Repository
    {
        public string Name { get; set; }

        public string Id { get; set; }

    }

    public class VersionControlType
    {
        public string Custom { get; set; }

        public string Git { get; set; }

        public string Tfvc { get; set; }

    }

    public class Project
    {
        public string Name { get; set; }

        public string Id { get; set; }

    }

    public class Collection
    {
        public string Name { get; set; }
    }

    public class Version
    {
        public string branchName { get; set; }

        public string changeId { get; set; }
    }
}