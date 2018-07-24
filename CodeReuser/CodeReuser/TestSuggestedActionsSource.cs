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
            var query = new Query();
            if (LineParser.IsSearchable(line))
            {
                _searchItem = LineParser.GetSearchableItem(line);
                if (!_searchItem.IsEmpty())
                {
                    _queryAnswer = await GetRecommendationAsync();
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

            if (_queryAnswer.Length > 0)
            {
                // Copy items
                var searchItem = new SearchItem(_searchItem);
                var queryAnswer = _queryAnswer;

                // Reset
                _queryAnswer = string.Empty;
                _searchItem = SearchItem.EmptySearchItem;

                return new SuggestedActionSet[] { new SuggestedActionSet(new ISuggestedAction[] { new MsAzureCodeAction(searchItem, string.Concat("https://www.", searchItem.Type, searchItem.Name, ".com")) }) };
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

        public async Task<string> GetRecommendationAsync()
        {
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://jsonplaceholder.typicode.com/posts/1");
            if (response.IsSuccessStatusCode)
            {
                return (await response.Content.ReadAsStringAsync()).Substring(0, 10);
            }

            return string.Empty;
        }

        private readonly TestSuggestedActionsSourceProvider m_factory;
        private readonly ITextBuffer m_textBuffer;
        private readonly ITextView m_textView;
        private string _queryAnswer;
        private SearchItem _searchItem = SearchItem.EmptySearchItem;
    }
}
