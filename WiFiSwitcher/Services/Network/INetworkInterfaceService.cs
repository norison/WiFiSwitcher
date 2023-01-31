using System.Net;

namespace WiFiSwitcher.Services.Network;

public interface INetworkInterfaceService
{
    IPAddress GetIpAddress();
    Task EnableWiFiAdapter();
    Task DisableWiFiAdapter();
}