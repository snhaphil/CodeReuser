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

        public Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                if (LineParser.IsSearchable(range.GetText()))
                {
                    return true;
                }

                return false;
            });
        }

        public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            var text = range.GetText();
            if (LineParser.IsSearchable(text))
            {
                var searchableItem = LineParser.GetSearchableItem(text);
                if (!searchableItem.IsEmpty())
                {
                    var reuseAction = new MsAzureCodeAction(searchableItem);
                    return new SuggestedActionSet[] { new SuggestedActionSet(new ISuggestedAction[] { reuseAction }) };
                }
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

        private readonly TestSuggestedActionsSourceProvider m_factory;
        private readonly ITextBuffer m_textBuffer;
        private readonly ITextView m_textView;
    }
}
