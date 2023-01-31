namespace WiFiSwitcher.Services.Http
{
    public class HttpClientAdapter : HttpClient, IHttpClient
    {
        public HttpClientAdapter()
        {
            
        }

        public HttpClientAdapter(HttpMessageHandler handler) : base(handler)
        {
            
        }
    }
}
