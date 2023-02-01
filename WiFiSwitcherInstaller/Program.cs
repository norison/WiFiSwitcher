using System.Diagnostics;

try
{
    const string exeName = "WiFiSwitcher.exe";

    var currentDirectory = Directory.GetCurrentDirectory();

    var path = Path.Combine(currentDirectory, exeName);

    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "sc.exe",
            Arguments = $"create \"WiFi Switcher Service\" binpath= {path}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        }
    };

    process.Start();

    while (!process.StandardOutput.EndOfStream)
    {
        Console.WriteLine(await process.StandardOutput.ReadLineAsync());
    }
}
catch (Exception exception)
{
    Console.WriteLine(exception.Message);
}

Console.WriteLine();
Console.WriteLine("Press any key to continue...");
Console.ReadKey();