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
}