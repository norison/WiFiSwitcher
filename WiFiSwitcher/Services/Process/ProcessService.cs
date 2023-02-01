using System.Diagnostics;

namespace WiFiSwitcher.Services.Process;

public class ProcessService : IProcessService
{
    public async Task<IEnumerable<string>> StartProcessAndGetOutputLinesAsync(string file, string arguments)
    {
        var process = new System.Diagnostics.Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = file,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        process.Start();

        var output = new List<string>();

        while (!process.StandardOutput.EndOfStream)
        {
            var line = await process.StandardOutput.ReadLineAsync();

            if (!string.IsNullOrEmpty(line))
            {
                output.Add(line);
            }
        }

        return output;
    }
}
