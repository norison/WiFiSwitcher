using System.Net;
using System.Net.Sockets;

namespace WiFiSwitcher.Services.Http;

public class HttpClientFactory: IHttpClientFactory
{
    public IHttpClient Create(IPAddress address)
    {
        if (IPAddress.Any.Equals(address))
        {
            return new HttpClientAdapter();
        }

        var handler = new SocketsHttpHandler
        {
            ConnectCallback = async (context, cancellationToken) =>
            {
                var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

                socket.Bind(new IPEndPoint(address, 0));

                socket.NoDelay = true;

                try
                {
                    await socket.ConnectAsync(context.DnsEndPoint, cancellationToken).ConfigureAwait(false);

                    return new NetworkStream(socket, true);
                }
                catch
                {
                    socket.Dispose();

                    throw;
                }
            }
        };

        return new HttpClientAdapter(handler);
    }
}
