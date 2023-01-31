namespace WiFiSwitcher.Services.Process;

public interface IProcessService
{
    Task<IEnumerable<string>> StartProcessAndGetOutputLinesAsync(string file, string arguments);
}