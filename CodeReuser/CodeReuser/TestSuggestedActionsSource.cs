using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeReuser
{
    internal class TestSuggestedActionsSource : ISuggestedActionsSource
    {
        public event EventHandler<EventArgs> SuggestedActionsChanged;

        public TestSuggestedActionsSource(TestSuggestedActionsSourceProvider testSuggestedActionsSourceProvider, ITextView textView, ITextBuffer textBuffer)
        {
            m_factory = testSuggestedActionsSourceProvider;
            m_textBuffer = textBuffer;
            m_textView = textView;
        }

        public async Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            var line = range.GetText();
            if (LineParser.IsSearchable(line))
            {
                _searchItem = LineParser.GetSearchableItem(line);
                if (!_searchItem.IsEmpty())
                {
                    _queryAnswer = await GetRecommendationAsync(_searchItem);
                    return true;
                }
            }

          return false;
        }

        public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            /*
            var line = range.GetText();
            var query = new Query();
            if (LineParser.IsSearchable(line))
            {
                var searchableItem = LineParser.GetSearchableItem(line);
                if (!searchableItem.IsEmpty())
                {
                    var response = query.RunTextQuery(searchableItem);
                    var reuseAction = new MsAzureCodeAction(searchableItem, "test");
                    return new SuggestedActionSet[] { new SuggestedActionSet(new ISuggestedAction[] { reuseAction }) };
                }
            }
            */

            if ((_queryAnswer?.Count ?? 0) > 0)
            {
                // Copy items
                var searchItem = new SearchItem(_searchItem);
                var queryAnswer = _queryAnswer;

                // Reset
                _queryAnswer = null;
                _searchItem = SearchItem.EmptySearchItem;

                var actions = new List<SuggestedActionSet>();
                foreach (var searchResultValue in (queryAnswer?.ResultValues.Take(10) ?? Enumerable.Empty<CodeSearchResponse.SearchResultValue>()))
                {
                    actions.Add(new SuggestedActionSet(new ISuggestedAction[] { new MsAzureCodeAction(searchItem, searchResultValue.FileName, CreateUri(searchResultValue)) }));
                }

                return actions;
            }
            
            return Enumerable.Empty<SuggestedActionSet>();
        }

        public void Dispose()
        {
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            // This is a sample provider and doesn't participate in LightBulb telemetry  
            telemetryId = Guid.Empty;
            return false;
        }

        public async Task<CodeSearchResponse> GetRecommendationAsync(SearchItem searchItem)
        {
            var query = new Query();
            return await query.RunTextQueryAsync(searchItem);
        }

        private string CreateUri(CodeSearchResponse.SearchResultValue value)
        {
            var path = value.Path.Replace("/", "%2F");
            return $"https://msazure.visualstudio.com/{value.Project.Name}/_git/{value.Repository.Name}?path={path}";
        }

        private readonly TestSuggestedActionsSourceProvider m_factory;
        private readonly ITextBuffer m_textBuffer;
        private readonly ITextView m_textView;
        private CodeSearchResponse _queryAnswer;
        private SearchItem _searchItem = SearchItem.EmptySearchItem;
    }
}
