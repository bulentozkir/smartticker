using System.Net;
using SmartTicker.Core.Services;
using SmartTicker.Infrastructure.Networking;

namespace SmartTicker.Infrastructure.Tests;

public sealed class PublicHtmlClientTests
{
    [Fact]
    public async Task GetStringAsync_BlocksCrossHostRedirect_WhenSettingIsDisabled()
    {
        var cookieFreeHandler = new RedirectHandler();
        var cookieHandler = new RejectingHandler();
        using var client = new PublicHtmlClient(new WebsiteAccessPolicy(), cookieFreeHandler, cookieHandler);

        var exception = await Assert.ThrowsAsync<HttpRequestException>(() =>
            client.GetStringAsync(new Uri("https://1.1.1.1/start"), CancellationToken.None));

        Assert.Contains("different website", exception.Message);
        Assert.Single(cookieFreeHandler.Requests);
        Assert.Empty(cookieHandler.Requests);
    }

    [Fact]
    public async Task GetStringAsync_FollowsCrossHostRedirect_WhenSettingIsEnabled()
    {
        var cookieFreeHandler = new RejectingHandler();
        var cookieHandler = new RedirectHandler();
        var policy = new WebsiteAccessPolicy { AllowCookiesAndCrossHostRedirects = true };
        using var client = new PublicHtmlClient(policy, cookieFreeHandler, cookieHandler);

        var html = await client.GetStringAsync(new Uri("https://1.1.1.1/start"), CancellationToken.None);

        Assert.Equal("<html>complete</html>", html);
        Assert.Empty(cookieFreeHandler.Requests);
        Assert.Equal(2, cookieHandler.Requests.Count);
        Assert.Equal("8.8.8.8", cookieHandler.Requests[1].Host);
    }

    [Fact]
    public void CreateHandler_UsesCookiesOnlyForEnabledMode()
    {
        using var cookieFreeHandler = PublicHtmlClient.CreateHandler(useCookies: false);
        using var cookieHandler = PublicHtmlClient.CreateHandler(useCookies: true);

        Assert.False(cookieFreeHandler.UseCookies);
        Assert.True(cookieHandler.UseCookies);
        Assert.NotNull(cookieHandler.CookieContainer);
    }

    [Fact]
    public async Task GetStringAsync_DoesNotPostNetworkContinuationsToTheCallerContext()
    {
        var context = new RecordingSynchronizationContext();
        var originalContext = SynchronizationContext.Current;
        using var client = new PublicHtmlClient(
            new WebsiteAccessPolicy(),
            new DelayedHtmlHandler(),
            new RejectingHandler());
        Task<string> fetch;
        try
        {
            SynchronizationContext.SetSynchronizationContext(context);
            fetch = client.GetStringAsync(new Uri("https://1.1.1.1/quote"), CancellationToken.None);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(originalContext);
        }

        var html = await fetch;

        Assert.Equal("<html>complete</html>", html);
        Assert.Equal(0, context.PostCount);
    }

    [Theory]
    [InlineData(WebsiteConsentDecision.Accept, "csrfToken=abc123&agree=agree", "reject=reject")]
    [InlineData(WebsiteConsentDecision.Reject, "csrfToken=abc123&reject=reject", "agree=agree")]
    public async Task GetStringAsync_SubmitsOnlyThePrivacyChoiceSelectedByTheUser(
        WebsiteConsentDecision decision,
        string expectedBody,
        string excludedField)
    {
        var cookieFreeHandler = new RejectingHandler();
        var consentHandler = new ConsentHandler();
        WebsiteConsentRequest? prompt = null;
        var policy = new WebsiteAccessPolicy
        {
            AllowCookiesAndCrossHostRedirects = true,
            ConsentPrompt = (request, _) =>
            {
                prompt = request;
                return Task.FromResult(decision);
            },
        };
        using var client = new PublicHtmlClient(policy, cookieFreeHandler, consentHandler);

        var html = await client.GetStringAsync(new Uri("https://1.1.1.1/start"), CancellationToken.None);

        Assert.Equal("<html>complete</html>", html);
        Assert.NotNull(prompt);
        Assert.Equal("Privacy choices", prompt.Title);
        Assert.Equal("Accept all", prompt.AcceptLabel);
        Assert.Equal("Reject all", prompt.RejectLabel);
        Assert.Equal(expectedBody, consentHandler.PostedBody);
        Assert.DoesNotContain(excludedField, consentHandler.PostedBody);
    }

    [Fact]
    public void WebsiteConsentForm_DoesNotTreatALoginFormAsConsent()
    {
        const string html = """
            <html><title>Privacy login</title><form method="post">
            <p>Accept cookies after signing in.</p>
            <input type="password" name="password">
            <button name="agree" value="agree">Accept</button>
            <button name="reject" value="reject">Reject</button>
            </form></html>
            """;

        Assert.False(WebsiteConsentForm.TryParse(new Uri("https://1.1.1.1/login"), html, out _));
    }

    private sealed class RedirectHandler : HttpMessageHandler
    {
        public List<Uri> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Requests.Add(request.RequestUri!);
            if (Requests.Count == 1)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.TemporaryRedirect)
                {
                    Headers = { Location = new Uri("https://8.8.8.8/complete") },
                });
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("<html>complete</html>", null, "text/html"),
            });
        }
    }

    private sealed class DelayedHtmlHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) => Task.Run(() =>
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("<html>complete</html>", null, "text/html"),
                },
                cancellationToken);
    }

    private sealed class RecordingSynchronizationContext : SynchronizationContext
    {
        private int _postCount;

        public int PostCount => Volatile.Read(ref _postCount);

        public override void Post(SendOrPostCallback callback, object? state)
        {
            Interlocked.Increment(ref _postCount);
            ThreadPool.QueueUserWorkItem(_ => callback(state));
        }
    }

    private sealed class RejectingHandler : HttpMessageHandler
    {
        public List<Uri> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Requests.Add(request.RequestUri!);
            throw new InvalidOperationException("The wrong HTTP mode was selected.");
        }
    }

    private sealed class ConsentHandler : HttpMessageHandler
    {
        public string PostedBody { get; private set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Get && request.RequestUri!.AbsolutePath == "/start")
            {
                return Redirect("https://8.8.8.8/consent");
            }

            if (request.Method == HttpMethod.Get && request.RequestUri!.AbsolutePath == "/consent")
            {
                return Html("""
                    <html><title>Privacy choices</title><form method="post" action="/choice">
                    <p>Your privacy choices. This website uses cookies.</p>
                    <input type="hidden" name="csrfToken" value="abc123">
                    <button type="submit" name="agree" value="agree">Accept all</button>
                    <button type="submit" name="reject" value="reject">Reject all</button>
                    </form></html>
                    """);
            }

            if (request.Method == HttpMethod.Post && request.RequestUri!.AbsolutePath == "/choice")
            {
                PostedBody = await request.Content!.ReadAsStringAsync(cancellationToken);
                return Redirect("https://1.1.1.1/complete");
            }

            if (request.Method == HttpMethod.Get && request.RequestUri!.AbsolutePath == "/complete")
            {
                return Html("<html>complete</html>");
            }

            throw new InvalidOperationException($"Unexpected request: {request.Method} {request.RequestUri}");
        }

        private static HttpResponseMessage Redirect(string location) => new(HttpStatusCode.Redirect)
        {
            Headers = { Location = new Uri(location) },
        };

        private static HttpResponseMessage Html(string html) => new(HttpStatusCode.OK)
        {
            Content = new StringContent(html, null, "text/html"),
        };
    }
}