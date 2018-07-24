using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;

namespace CodeReuser
{
    public class Recommendations
    {
        private Dictionary<FileId, RecommendationItem> _recommendations;
        private List<SearchItem> _items;

        public Recommendations()
        {
            _recommendations = new Dictionary<FileId, RecommendationItem>();
            _items = new List<SearchItem>();
        }

        public void UpdateRecommendations(SearchItem searchItem, CodeSearchResponse searchResponse)
        {
            if (!ShouldUpdate(searchItem))
            {
                return;
            }

            CleanIfNeeded(searchItem);
            if ((searchResponse?.Count ?? 0) > 0)
            {
                foreach (var searchResultValue in searchResponse?.ResultValues ?? Enumerable.Empty<CodeSearchResponse.SearchResultValue>())
                {
                    var fileId = new FileId(searchResultValue.Path, searchResultValue.Repository.Name);
                    if (!_recommendations.ContainsKey(fileId))
                    {
                        _recommendations[fileId] = new RecommendationItem(10, searchResultValue);
                    }
                }

            }
        }

        private void CleanIfNeeded(SearchItem searchItem)
        {
            if (NeedToClean(searchItem))
            {
                _items.Clear();
                _recommendations.Clear();
            }
        }

        private bool NeedToClean(SearchItem searchItem)
        {
            if (_items.Contains(searchItem))
            {
                return false;
            }

            return searchItem.Type == SearchType.Class;
        }

        private bool ShouldUpdate(SearchItem searchItem)
        {
            if (_items.Contains(searchItem))
            {
                return false;
            }
             _items.Add(searchItem);
            return true;
        }

        public IEnumerable<SuggestedActionSet> GetRecommendations()
            => _recommendations.Take(10).ToList().OrderBy(kvp => kvp.Value.Score).Select(kvp => kvp.Value.SuggestedActionSet);
    }

    public class FileId
    {
        public FileId(string path, string repository)
        {
            Path = path;
            Repository = repository;
        }

        public string Path { get; set; }

        public string Repository { get; set; }
    }

    internal class RecommendationItem
    {
        public RecommendationItem(int score, CodeSearchResponse.SearchResultValue searchResultValue)
        {
            Score = score;
            SuggestedActionSet = new SuggestedActionSet(new ISuggestedAction[] { new MsAzureCodeAction(searchResultValue.Repository.Name, searchResultValue.FileName, CreateUri(searchResultValue))});
        }

        public int Score { get; set; }

        public SuggestedActionSet SuggestedActionSet { get; set; }

        private string CreateUri(CodeSearchResponse.SearchResultValue value)
        {
            var path = value.Path.Replace("/", "%2F");
            return $"https://msazure.visualstudio.com/{value.Project.Name}/_git/{value.Repository.Name}?path={path}";
        }
    }
}