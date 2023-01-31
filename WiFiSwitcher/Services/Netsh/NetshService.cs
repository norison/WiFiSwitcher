using WiFiSwitcher.Services.Process;

namespace WiFiSwitcher.Services.Netsh;

public class NetshService : INetshService
{
    private readonly IProcessService _processService;

    private const string File = "netsh.exe";
    private const string ShowInterfaceArgument = "interface show interface \"{0}\"";
    private const string SetInterfaceArgument = "interface set interface \"{0}\" {1}";

    public NetshService(IProcessService processService)
    {
        _processService = processService;
    }

    public async Task<InterfaceStatus> GetInterfaceStatusAsync(string name)
    {
        var argument = string.Format(ShowInterfaceArgument, name);
        var outputLines = await _processService.StartProcessAndGetOutputLinesAsync(File, argument);

        var statusLine = outputLines.FirstOrDefault(line => line.Contains("Administrative state", StringComparison.InvariantCultureIgnoreCase));

        if (string.IsNullOrEmpty(statusLine))
        {
            throw new Exception("Interface status line was not found");
        }

        return statusLine.Contains("Enabled", StringComparison.InvariantCultureIgnoreCase) ? InterfaceStatus.Enabled : InterfaceStatus.Disabled;
    }

    public async Task SetInterfaceAsync(string name, string operation)
    {
        var argument = string.Format(SetInterfaceArgument, name, operation);
        await _processService.StartProcessAndGetOutputLinesAsync(File, argument);
    }
}