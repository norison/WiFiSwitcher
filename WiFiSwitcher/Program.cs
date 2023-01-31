using Microsoft.AspNetCore.Builder;
using NLog;
using NLog.Web;
using WiFiSwitcher;
using WiFiSwitcher.Services.Http;
using WiFiSwitcher.Services.Netsh;
using WiFiSwitcher.Services.Network;
using WiFiSwitcher.Services.Process;
using WiFiSwitcher.Settings;

using IHttpClientFactory = WiFiSwitcher.Services.Http.IHttpClientFactory;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var logger = LogManager.Setup().LoadConfigurationFromFile().GetCurrentClassLogger();

try
{
    logger.Info("Starting up the service");

    var builder = WebApplication.CreateBuilder();

    var configuration = builder.Configuration;
    var services = builder.Services;
    var logging = builder.Logging;

    services.Configure<ConnectionSettings>(configuration.GetSection("ConnectionSettings"));
    services.Configure<TimerSettings>(configuration.GetSection("TimerSettings"));
    services.AddSingleton<IHttpClientFactory, HttpClientFactory>();
    services.AddSingleton<INetworkInterfaceService, NetworkInterfaceService>();
    services.AddSingleton<INetshService, NetshService>();
    services.AddSingleton<IProcessService, ProcessService>();
    services.AddHostedService<Worker>();

    logging.ClearProviders();
    logging.SetMinimumLevel(LogLevel.Trace);

    builder.Host.UseWindowsService(options => options.ServiceName = "WiFi Switcher Service");
    builder.Host.UseNLog();

    builder.Build().Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "There was a problem starting the serivce");
}
finally
{
    LogManager.Shutdown();
}