using System.Net;

namespace WiFiSwitcher.Services.Http;

public interface IHttpClientFactory
{
    IHttpClient Create(IPAddress address);
}
