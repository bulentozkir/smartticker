using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using SmartTicker.Desktop.Controls;
using SmartTicker.Infrastructure.Launching;

namespace SmartTicker.Desktop.Views;

public partial class HelpWindow : Window
{
    private const int MaximumHelpBytes = 1_048_576;
    private static readonly Uri HelpUri =
        new("https://raw.githubusercontent.com/bulentozkir/smartticker/refs/heads/main/HELPME.md");
    private static readonly HttpClient HelpClient = CreateHelpClient();

    private readonly CancellationTokenSource _lifetimeCancellation = new();
    private readonly Dictionary<string, Control> _headingTargets = new(StringComparer.OrdinalIgnoreCase);

    public HelpWindow()
    {
        InitializeComponent();
        WindowReachability.Attach(this);
        Opened += async (_, _) => await ExceptionSafety.RunAsync(
            LoadHelpAsync,
            exception =>
            {
                if (IsVisible)
                {
                    StatusText.Text = $"Help could not be displayed: {exception.Message}";
                }
            });
        Closed += (_, _) => ExceptionSafety.Run(_lifetimeCancellation.Cancel);
    }

    public static void Open(Window owner)
    {
        var window = new HelpWindow();
        window.Show(owner);
        window.Activate();
    }

    private async Task LoadHelpAsync()
    {
        StatusText.Text = "Loading online help…";

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, HelpUri);
            request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
            using var response = await HelpClient.SendAsync(request, _lifetimeCancellation.Token);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException($"The help server returned {(int)response.StatusCode}.");
            }

            var contentLength = response.Content.Headers.ContentLength;
            if (contentLength > MaximumHelpBytes)
            {
                throw new InvalidDataException("The online help document is too large.");
            }

            var help = await response.Content.ReadAsStringAsync(_lifetimeCancellation.Token);
            if (string.IsNullOrWhiteSpace(help))
            {
                throw new InvalidDataException("The online help document is empty.");
            }

            RenderHelp(help);
            StatusText.Text = "Online guide loaded from the SmartTicker repository.";
        }
        catch (OperationCanceledException) when (_lifetimeCancellation.IsCancellationRequested)
        {
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            RenderHelp(ReadEmbeddedHelp());
            StatusText.Text = "Online help is unavailable. Showing the built-in guide.";
        }
    }

    private void RenderHelp(string markdown)
    {
        var document = MarkdownHelpRenderer.Render(
            markdown,
            NavigateToAnchor,
            uri => new DefaultBrowserLinkLauncher().TryOpen(uri));
        HelpContentHost.Content = document.Content;
        HelpScrollViewer.Offset = default;
        _headingTargets.Clear();
        NavigationPanel.Children.Clear();
        foreach (var heading in document.Headings)
        {
            _headingTargets[heading.Anchor] = heading.Target;
            if (heading.Level != 2)
            {
                continue;
            }

            var button = new Button
            {
                Content = new TextBlock
                {
                    Text = heading.Title,
                    TextWrapping = TextWrapping.Wrap,
                },
                Tag = heading.Anchor,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                Background = Brushes.Transparent,
                BorderThickness = new Avalonia.Thickness(0),
                Padding = new Avalonia.Thickness(8, 6),
                Foreground = new SolidColorBrush(Color.Parse("#C9D1D9")),
            };
            button.Click += (_, _) => ExceptionSafety.Run(
                () => NavigateToAnchor((string)button.Tag!));
            NavigationPanel.Children.Add(button);
        }
    }

    private void NavigateToAnchor(string anchor)
    {
        if (_headingTargets.TryGetValue(anchor, out var target))
        {
            target.BringIntoView();
        }
    }

    private static string ReadEmbeddedHelp()
    {
        var assembly = typeof(HelpWindow).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .Single(name => name.EndsWith(".HELPME.md", StringComparison.Ordinal));
        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static HttpClient CreateHelpClient()
    {
        var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15),
            MaxResponseContentBufferSize = MaximumHelpBytes,
        };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("SmartTicker/1.0.3");
        return client;
    }
}