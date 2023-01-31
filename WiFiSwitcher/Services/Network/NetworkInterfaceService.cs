using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Options;
using WiFiSwitcher.Services.Netsh;
using WiFiSwitcher.Settings;

namespace WiFiSwitcher.Services.Network;

public class NetworkInterfaceService : INetworkInterfaceService
{
    private readonly ILogger<NetworkInterfaceService> _logger;
    private readonly INetshService _netshService;
    private readonly ConnectionSettings _connectionSettings;

    private const string WiFiAdapterName = "Wi-Fi";
    private const string EnableOperation = "enable";
    private const string DisableOperation = "disable";

    public NetworkInterfaceService(
        ILogger<NetworkInterfaceService> logger,
        INetshService netshService,
        IOptions<ConnectionSettings> connectionSettingsOptions)
    {
        _logger = logger;
        _netshService = netshService;
        _connectionSettings = connectionSettingsOptions.Value;
    }

    public IPAddress GetIpAddress()
    {
        foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            foreach (var unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (unicastAddress.IPv4Mask.ToString() == _connectionSettings.Ipv4Mask)
                {
                    return unicastAddress.Address;
                }
            }
        }

        throw new Exception($"IP address by IPv4Mask was not found. IPv4Mask: {_connectionSettings.Ipv4Mask}");
    }

    public async Task EnableWiFiAdapter()
    {
        try
        {
            var status = await _netshService.GetInterfaceStatusAsync(WiFiAdapterName);

            if (status == InterfaceStatus.Enabled)
            {
                _logger.LogInformation("The interface status is enabled. The operation will be skipped");
                return;
            }

            await _netshService.SetInterfaceAsync(WiFiAdapterName, EnableOperation);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
        }
    }

    public async Task DisableWiFiAdapter()
    {
        try
        {
            var status = await _netshService.GetInterfaceStatusAsync(WiFiAdapterName);

            if (status == InterfaceStatus.Disabled)
            {
                _logger.LogInformation("The interface status is disabled. The operation will be skipped");
                return;
            }

            await _netshService.SetInterfaceAsync(WiFiAdapterName, DisableOperation);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
        }
    }
}