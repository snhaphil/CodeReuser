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
using ICSharpCode.AvalonEdit.Search;

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

        public MsAzureCodeAction(string repo, string fileName, string path, string fileLink, string repoLink)
        {
            _display = $"Found result in file:{fileName} under repo:{repo}";
            _path = path;
            _repo = repo;
            _fileLink = fileLink;
            _repoLink = repoLink;
        }

        public async Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            ScrollViewer scroll = new ScrollViewer()
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            var stackPanel = new StackPanel();

            var sourceFile = await new VisualStudioCodeSearchHelper().DownloadSourceFileAsync(_repo, _path);

            // Code text editor
            var textEditor = new ICSharpCode.AvalonEdit.TextEditor()
            {
                IsReadOnly = true,
                Text = sourceFile.Content,
                BorderThickness = new Thickness(5),
                BorderBrush = Brushes.Black,
                SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("C#"),
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden
            };

            textEditor.MouseLeave += (sender, args) => ((ICSharpCode.AvalonEdit.TextEditor)sender).Copy();
            SearchPanel.Install(textEditor);

            // Repository text block
            var repoTextBlock = new TextBlock();
            var repoText = new TextBlock();
            repoText.Padding = new Thickness(3);
            repoText.Text = $"Repository: ";

            Hyperlink hl = new Hyperlink(new Run(_repo));
            hl.Foreground = Brushes.Blue;
            hl.FontSize = 11;
            hl.NavigateUri = new Uri("http://www.google.com"); // This is just a placeholder. The real link is what's executed in Hl_RequestNavigate
            hl.RequestNavigate += (s, e) => Process.Start(_repoLink);

            repoTextBlock.Inlines.Add(repoText);
            repoTextBlock.Inlines.Add(hl);

            // Namespace text block 
            var namespaceTextBlock = new TextBlock();
            namespaceTextBlock.Padding = new Thickness(3);
            namespaceTextBlock.Text = $"Namespace: {sourceFile.Namespace}";

            // Link text block
            var linkTextBlock = new TextBlock();
            linkTextBlock.Padding = new Thickness(3);

            var linkText = new TextBlock();
            linkText.Padding = new Thickness(3);
            linkText.Text = "File: ";

            hl = new Hyperlink(new Run(sourceFile.ContentMetadata.FileName));
            hl.Foreground = Brushes.Blue;
            hl.FontSize = 11;
            hl.NavigateUri = new Uri("http://www.google.com"); // This is just a placeholder. The real link is what's executed in Hl_RequestNavigate
            hl.RequestNavigate += (s ,e) => Process.Start(_fileLink);

            linkTextBlock.Inlines.Add(linkText);
            linkTextBlock.Inlines.Add(hl);

            stackPanel.Children.Add(repoTextBlock);
            stackPanel.Children.Add(linkTextBlock);
            stackPanel.Children.Add(namespaceTextBlock);
            stackPanel.Children.Add(textEditor);
            scroll.Content = stackPanel;
            return scroll;
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
        private string _repo;
        private string _path;
        private string _fileLink;
        private string _repoLink;
        private ITextSnapshot _snapshot;
    }
}
