using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CodeReuser
{
    internal class MsAzureCodeAction : ISuggestedAction
    {
        public bool HasActionSets
        {
            get { return false; }
        }
        public string DisplayText
        {
            get { return _display; }
        }
        public ImageMoniker IconMoniker
        {
            get { return default(ImageMoniker); }
        }
        public string IconAutomationText
        {
            get
            {
                return null;
            }
        }
        public string InputGestureText
        {
            get
            {
                return null;
            }
        }
        public bool HasPreview
        {
            get { return true; }
        }

        public MsAzureCodeAction(SearchItem searchable)
        {
            _display = $"Found a {searchable.Type} named {searchable.Text} in msazure, click to get result";
        }

        public Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            var textBlock = new TextBlock();
            textBlock.Padding = new Thickness(3);
            textBlock.Inlines.Add(new Run() { Text = "Not available until Snir solves his gay issues" });
            return Task.FromResult<object>(textBlock);
        }

        public Task<IEnumerable<SuggestedActionSet>> GetActionSetsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<SuggestedActionSet>>(null);
        }

        public void Invoke(CancellationToken cancellationToken)
        {
            // _span.TextBuffer.Replace(_span.GetSpan(_snapshot), _client.GetRecommendation(_textToSearch));
        }

        public void Dispose()
        {
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            // This is a sample action and doesn't participate in LightBulb telemetry  
            telemetryId = Guid.Empty;
            return false;
        }

        private MsAzureClient _client;
        private ITrackingSpan _span;
        private string _display;
        private string _textToSearch;
        private ITextSnapshot _snapshot;
    }
}
