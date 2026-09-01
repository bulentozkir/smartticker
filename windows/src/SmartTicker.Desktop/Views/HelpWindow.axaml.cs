using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SmartTicker.Infrastructure.Launching;

namespace SmartTicker.Desktop.Views;

public partial class HelpWindow : Window
{
    private const int MaximumHelpBytes = 1_048_576;
    private static readonly Uri HelpUri =
        new("https://raw.githubusercontent.com/bulentozkir/smartticker/refs/heads/main/HELPME.md");
    private static readonly HttpClient HelpClient = CreateHelpClient();

    private readonly CancellationTokenSource _lifetimeCancellation = new();

    public HelpWindow()
    {
        InitializeComponent();
        Opened += async (_, _) => await LoadHelpAsync();
        Closed += (_, _) => _lifetimeCancellation.Cancel();
    }

    public static void Open(Window owner) => new HelpWindow().Show(owner);

    private async Task LoadHelpAsync()
    {
        ReloadButton.IsEnabled = false;
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

            HelpText.Text = help;
            StatusText.Text = "Online guide loaded from the SmartTicker repository.";
        }
        catch (OperationCanceledException) when (_lifetimeCancellation.IsCancellationRequested)
        {
        }
        catch (Exception)
        {
            HelpText.Text = ReadEmbeddedHelp();
            StatusText.Text = "Online help is unavailable. Showing the built-in guide.";
        }
        finally
        {
            if (!_lifetimeCancellation.IsCancellationRequested)
            {
                ReloadButton.IsEnabled = true;
            }
        }
    }

    private async void ReloadHelp(object? sender, RoutedEventArgs e) => await LoadHelpAsync();

    private void OpenOnlineHelp(object? sender, RoutedEventArgs e) =>
        new DefaultBrowserLinkLauncher().TryOpen(HelpUri);

    private void CloseWindow(object? sender, RoutedEventArgs e) => Close();

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
        client.DefaultRequestHeaders.UserAgent.ParseAdd("SmartTicker/1.0.1");
        return client;
    }
}