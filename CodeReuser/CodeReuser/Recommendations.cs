using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;

namespace CodeReuser
{
    public class Recommendations
    { 
        public Recommendations()
        {
            _recommendations = new Dictionary<FileId, RecommendationItem>(new FileId.PathRepositoryEqualityComparer());
            _items = new List<SearchItem>();
            _thisLock = new Object();
        }

        public bool HasRecommendation()
            => _recommendations.Any();

        public IEnumerable<SuggestedActionSet> GetRecommendations()
        {
            var a = _recommendations.ToList().OrderBy(kvp => kvp.Value.Score * -1);
            return a.Select(kvp => kvp.Value.SuggestedActionSet).Take(10);
        }

        public void UpdateRecommendations(SearchItem searchItem, CodeSearchResponse searchResponse)
        {
            lock (_thisLock)
            {
                if (!ShouldUpdate(searchItem))
                {
                    return;
                }

                CleanIfNeeded(searchItem);
                _items.Add(searchItem);
                if ((searchResponse?.Count ?? 0) > 0)
                {
                    foreach (var searchResultValue in searchResponse?.ResultValues ?? Enumerable.Empty<CodeSearchResponse.SearchResultValue>())
                    {
                        var fileId = new FileId(searchResultValue.Path, searchResultValue.Repository.Name);
                        if (!_recommendations.TryGetValue(fileId, out var recommendationItem))
                        {
                            _recommendations[fileId] = new RecommendationItem(Score, searchResultValue);
                        }
                        else
                        {
                            recommendationItem.Score += GetScore(searchItem);
                            _recommendations[fileId] = recommendationItem;
                        }
                    }

                }
            }
            
        }

        private int GetScore(SearchItem searchItem)
        {
            var scoreByType = searchItem.Type == SearchType.Method ? Score : 20;
            var scoreByAccuracy = searchItem.Accuracy == SearchAccuracy.Accurate ? scoreByType * 2 : scoreByType;
            return scoreByAccuracy + _items.Count; // prefer the latest results
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

            return searchItem.Type == SearchType.Class || searchItem.Type == SearchType.Interface;
        }

        private bool ShouldUpdate(SearchItem searchItem)
        {
            if (_items.Contains(searchItem))
            {
                return false;
            }

            return true;
        }

        private const int Score = 10;
        private Dictionary<FileId, RecommendationItem> _recommendations;
        private List<SearchItem> _items;
        private object _thisLock;
    }

    public class FileId
    {
        public sealed class PathRepositoryEqualityComparer : IEqualityComparer<FileId>
        {
            public bool Equals(FileId x, FileId y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.Path, y.Path) && string.Equals(x.Repository, y.Repository);
            }

            public int GetHashCode(FileId obj)
            {
                unchecked
                {
                    return ((obj.Path != null ? obj.Path.GetHashCode() : 0) * 397) ^ (obj.Repository != null ? obj.Repository.GetHashCode() : 0);
                }
            }
        }

        public static IEqualityComparer<FileId> PathRepositoryComparer { get; } = new PathRepositoryEqualityComparer();

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
            SuggestedActionSet = new SuggestedActionSet(new ISuggestedAction[] { new MsAzureCodeAction(searchResultValue.Repository.Name, searchResultValue.FileName, searchResultValue.Path, CreateFileUri(searchResultValue), CreateRepoUri(searchResultValue))});
        }

        public int Score { get; set; }

        public SuggestedActionSet SuggestedActionSet { get; set; }

        private string CreateFileUri(CodeSearchResponse.SearchResultValue value)
        {
            var path = value.Path.Replace("/", "%2F");
            return $"{CreateRepoUri(value)}/?path={path}";
        }

        private string CreateRepoUri(CodeSearchResponse.SearchResultValue value)
        {
            return $"https://msazure.visualstudio.com/{value.Project.Name}/_git/{value.Repository.Name}";
        }
    }
}