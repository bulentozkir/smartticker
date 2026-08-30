using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;

namespace SmartTicker.Infrastructure.Networking;

internal sealed class PublicHtmlClient : IDisposable
{
    private const int MaximumResponseBytes = 8 * 1024 * 1024;

    private static readonly string[] HtmlMediaTypes = ["text/html", "application/xhtml+xml"];
    private static readonly string[] JsonMediaTypes = ["application/json", "text/plain", "text/json"];

    private static readonly string TooLargeMessage =
        $"The HTML document exceeds the {MaximumResponseBytes / (1024 * 1024)} MB limit.";
    private readonly HttpClient _httpClient;

    public PublicHtmlClient()
    {
        _httpClient = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
            UseCookies = false,
            Credentials = null,
        })
        {
            Timeout = TimeSpan.FromSeconds(20),
        };
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("SmartTicker/0.1 (+local desktop public HTML reader)");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
    }

    public Task<string> GetStringAsync(Uri pageUri, CancellationToken cancellationToken) =>
        FetchAsync(pageUri, HtmlMediaTypes, "text/html", cancellationToken);

    // Raw file hosts commonly serve JSON as text/plain, so both are accepted.
    public Task<string> GetJsonAsync(Uri fileUri, CancellationToken cancellationToken) =>
        FetchAsync(fileUri, JsonMediaTypes, "application/json", cancellationToken);

    private async Task<string> FetchAsync(
        Uri pageUri,
        string[] allowedMediaTypes,
        string acceptHeader,
        CancellationToken cancellationToken)
    {
        var currentUri = pageUri;
        for (var redirect = 0; redirect <= 3; redirect++)
        {
            await ValidatePublicUriAsync(currentUri, cancellationToken);
            using var request = new HttpRequestMessage(HttpMethod.Get, currentUri);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));
            using var response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (IsRedirect(response.StatusCode))
            {
                if (redirect == 3 || response.Headers.Location is null)
                {
                    throw new HttpRequestException("The source exceeded the safe redirect limit.");
                }

                currentUri = response.Headers.Location.IsAbsoluteUri
                    ? response.Headers.Location
                    : new Uri(currentUri, response.Headers.Location);
                continue;
            }

            response.EnsureSuccessStatusCode();
            if (response.Content.Headers.ContentType?.MediaType is { } mediaType &&
                !allowedMediaTypes.Contains(mediaType, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"The URL returned {mediaType} instead of {string.Join(" or ", allowedMediaTypes)}.");
            }

            return await ReadLimitedHtmlAsync(response.Content, cancellationToken);
        }

        throw new HttpRequestException("The source could not be reached.");
    }

    public void Dispose() => _httpClient.Dispose();

    private static async Task ValidatePublicUriAsync(Uri uri, CancellationToken cancellationToken)
    {
        if (!uri.IsAbsoluteUri || (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp))
        {
            throw new InvalidOperationException("Only HTTP and HTTPS source URLs are supported.");
        }

        if (!string.IsNullOrEmpty(uri.UserInfo) || !uri.IsDefaultPort)
        {
            throw new InvalidOperationException("Credentials and nonstandard ports are not allowed in source URLs.");
        }

        IPAddress[] addresses;
        try
        {
            addresses = IPAddress.TryParse(uri.Host, out var literal)
                ? [literal]
                : await Dns.GetHostAddressesAsync(uri.DnsSafeHost, cancellationToken);
        }
        catch (SocketException exception)
        {
            throw new HttpRequestException("The source host could not be resolved.", exception);
        }

        if (addresses.Length == 0 || addresses.Any(address => !IsPublicAddress(address)))
        {
            throw new InvalidOperationException("Local, private, reserved, and link-local source addresses are blocked.");
        }
    }

    private static bool IsPublicAddress(IPAddress address)
    {
        if (IPAddress.IsLoopback(address) || address.IsIPv6LinkLocal || address.IsIPv6Multicast || address.IsIPv6SiteLocal)
        {
            return false;
        }

        if (address.AddressFamily == AddressFamily.InterNetworkV6)
        {
            var bytes = address.GetAddressBytes();
            return (bytes[0] & 0xFE) != 0xFC;
        }

        var value = address.GetAddressBytes();
        return value[0] is not (0 or 10 or 127) &&
               !(value[0] == 100 && value[1] is >= 64 and <= 127) &&
               !(value[0] == 169 && value[1] == 254) &&
               !(value[0] == 172 && value[1] is >= 16 and <= 31) &&
               !(value[0] == 192 && value[1] == 168) &&
               !(value[0] == 198 && value[1] is 18 or 19) &&
               value[0] < 224;
    }

    private static bool IsRedirect(HttpStatusCode statusCode) =>
        statusCode is HttpStatusCode.Moved or HttpStatusCode.Redirect or HttpStatusCode.RedirectMethod or
            HttpStatusCode.TemporaryRedirect or HttpStatusCode.PermanentRedirect;

    private static async Task<string> ReadLimitedHtmlAsync(HttpContent content, CancellationToken cancellationToken)
    {
        if (content.Headers.ContentLength > MaximumResponseBytes)
        {
            throw new InvalidOperationException(TooLargeMessage);
        }

        await using var input = await content.ReadAsStreamAsync(cancellationToken);
        using var output = new MemoryStream();
        var buffer = new byte[16 * 1024];
        while (true)
        {
            var read = await input.ReadAsync(buffer, cancellationToken);
            if (read == 0)
            {
                break;
            }

            if (output.Length + read > MaximumResponseBytes)
            {
                throw new InvalidOperationException(TooLargeMessage);
            }

            output.Write(buffer, 0, read);
        }

        return Encoding.UTF8.GetString(output.ToArray());
    }
}