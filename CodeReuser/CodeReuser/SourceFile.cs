using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace CodeReuser
{
    public class SourceFile
    {

        public ContentMetadata Metadata { get; set; }
        public string ObjectId { get; set; }

        public string GitObjectType { get; set; }

        public string CommitId { get; set; }

        public string Content { get; set; }

        public string Namespace => _namespace ?? (_namespace = Regex.Match(Content, "\\nnamespace ([a-zA-Z0-9.]+)\\n").Groups[1].Value);

        [JsonProperty("_links")]
        public Links Links { get; set; }

        private string _namespace;
    }

    public class ContentMetadata
    {

        public string FileName { get; set; }

        public string Extension { get; set; }
    }

    public class Links
    {
        public Link Self { get; set; }

        public Link Repository { get; set; }
    }

    public class Link
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }
}
