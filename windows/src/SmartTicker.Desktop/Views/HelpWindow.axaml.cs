using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using SmartTicker.Core.Models;
using SmartTicker.Desktop.Controls;
using SmartTicker.Desktop.Localization;
using SmartTicker.Desktop.ViewModels;
using SmartTicker.Infrastructure.Launching;

namespace SmartTicker.Desktop.Views;

public partial class HelpWindow : Window
{
    private const int MaximumHelpBytes = 1_048_576;
    private static readonly HttpClient HelpClient = CreateHelpClient();

    private readonly CancellationTokenSource _lifetimeCancellation = new();
    private readonly Dictionary<string, Control> _headingTargets = new(StringComparer.OrdinalIgnoreCase);
    private MainViewModel? _viewModel;
    private int _loadGeneration;
    private bool _isOpened;

    public HelpWindow()
    {
        InitializeComponent();
        WindowReachability.Attach(this);
        DataContextChanged += (_, _) => ObserveViewModel();
        Opened += (_, _) =>
        {
            _isOpened = true;
            ReloadHelp();
        };
        Closed += (_, _) => ExceptionSafety.Run(() =>
        {
            _isOpened = false;
            if (_viewModel is not null)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }

            _lifetimeCancellation.Cancel();
        });
    }

    public static void Open(Window owner)
    {
        var window = new HelpWindow { DataContext = owner.DataContext };
        window.Show(owner);
        window.Activate();
    }

    internal static Uri HelpUriFor(string? language)
    {
        var code = AppLanguages.Normalize(language);
        var path = code == AppLanguages.Default ? "HELPME.md" : $"help/HELPME.{code}.md";
        return new Uri($"https://raw.githubusercontent.com/bulentozkir/smartticker/refs/heads/main/{path}");
    }

    private void ObserveViewModel()
    {
        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _viewModel = DataContext as MainViewModel;
        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        ApplyChrome(CurrentLanguage);
        if (_isOpened)
        {
            ReloadHelp();
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs change)
    {
        if (change.PropertyName == nameof(MainViewModel.Language))
        {
            ReloadHelp();
        }
    }

    private string CurrentLanguage => AppLanguages.Normalize(_viewModel?.Language);

    private void ReloadHelp()
    {
        var language = CurrentLanguage;
        var generation = Interlocked.Increment(ref _loadGeneration);
        var strings = ApplyChrome(language);
        RenderHelp(ReadEmbeddedHelp(language), language);
        StatusText.Text = strings.CheckingOnline;
        _ = ExceptionSafety.RunAsync(
            () => LoadOnlineHelpAsync(language, generation),
            exception =>
            {
                if (generation == Volatile.Read(ref _loadGeneration) && IsVisible)
                {
                    StatusText.Text = $"{strings.OfflineLoaded} {exception.Message}";
                }
            });
    }

    private async Task LoadOnlineHelpAsync(string language, int generation)
    {
        var strings = HelpLocalization.For(language);

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, HelpUriFor(language));
            request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
            using var response = await HelpClient
                .SendAsync(request, _lifetimeCancellation.Token)
                .ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException($"The help server returned {(int)response.StatusCode}.");
            }

            var contentLength = response.Content.Headers.ContentLength;
            if (contentLength > MaximumHelpBytes)
            {
                throw new InvalidDataException("The online help document is too large.");
            }

            var help = await response.Content
                .ReadAsStringAsync(_lifetimeCancellation.Token)
                .ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(help))
            {
                throw new InvalidDataException("The online help document is empty.");
            }

            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (generation != Volatile.Read(ref _loadGeneration) || !IsVisible)
                {
                    return;
                }

                RenderHelp(help, language);
                StatusText.Text = strings.OnlineLoaded;
            });
        }
        catch (OperationCanceledException) when (_lifetimeCancellation.IsCancellationRequested)
        {
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (generation == Volatile.Read(ref _loadGeneration) && IsVisible)
                {
                    StatusText.Text = strings.OfflineLoaded;
                }
            });
        }
    }

    private HelpStrings ApplyChrome(string language)
    {
        var strings = HelpLocalization.For(language);
        Title = strings.Title;
        HelpTitleText.Text = strings.Title;
        HelpSubtitleText.Text = strings.Subtitle;
        NavigationTitleText.Text = strings.Navigation;
        var direction = language == "ar" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        HelpContentHost.FlowDirection = direction;
        NavigationPanel.FlowDirection = direction;
        return strings;
    }

    private void RenderHelp(string markdown, string language)
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
            if (heading.Level is < 2 or > 3)
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
                Padding = new Avalonia.Thickness(heading.Level == 2 ? 8 : 20, heading.Level == 2 ? 6 : 4),
                Foreground = new SolidColorBrush(Color.Parse("#C9D1D9")),
                FontSize = heading.Level == 2 ? 13 : 11,
                Opacity = heading.Level == 2 ? 1 : 0.82,
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

    internal static string ReadEmbeddedHelp(string? language)
    {
        var assembly = typeof(HelpWindow).Assembly;
        var code = AppLanguages.Normalize(language);
        var suffix = code == AppLanguages.Default ? ".HELPME.md" : $".HELPME.{code}.md";
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) ??
            assembly.GetManifestResourceNames()
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