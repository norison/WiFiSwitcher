using System.Diagnostics.CodeAnalysis;

namespace WiFiSwitcher.Services.Http;

public interface IHttpClient : IDisposable
{
    Task<HttpResponseMessage> GetAsync([StringSyntax(StringSyntaxAttribute.Uri)] string? requestUri, CancellationToken cancellationToken);
}