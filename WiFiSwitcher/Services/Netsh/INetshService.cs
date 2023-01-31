namespace WiFiSwitcher.Services.Netsh;

public interface INetshService
{
    Task<InterfaceStatus> GetInterfaceStatusAsync(string name);
    Task SetInterfaceAsync(string name, string operation);
}