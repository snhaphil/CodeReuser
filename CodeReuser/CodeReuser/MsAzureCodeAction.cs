using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

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

        public MsAzureCodeAction(SearchItem searchable, string fileName, string link)
        {
            _display = $"Found result in file:{fileName} for {searchable.Type}'s named '{searchable.Name}' in msazure";
            _link = link;
        }

        public Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            var textBlock = new TextBlock();
            textBlock.Padding = new Thickness(3);

            Hyperlink hl = new Hyperlink(new Run(_link));
            hl.Foreground = Brushes.Blue;
            hl.FontSize = 11;
            hl.NavigateUri = new Uri("http://www.google.com"); // This is just a placeholder. The real link is what's executed in Hl_RequestNavigate
            hl.RequestNavigate += Hl_RequestNavigate;

            textBlock.Inlines.Add(hl);

            return Task.FromResult<object>(textBlock);
        }

        private void Hl_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(_link);
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

        private ITrackingSpan _span;
        private string _display;
        private string _link;
        private ITextSnapshot _snapshot;
    }
}
