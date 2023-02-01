using Microsoft.Extensions.Options;
using WiFiSwitcher.Services.Network;
using WiFiSwitcher.Settings;

using IHttpClientFactory = WiFiSwitcher.Services.Http.IHttpClientFactory;

namespace WiFiSwitcher;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly INetworkInterfaceService _networkInterfaceService;
    private readonly ConnectionSettings _connectionSettings;
    private readonly TimerSettings _timerSettings;

    public Worker(
        ILogger<Worker> logger,
        IHttpClientFactory httpClientFactory,
        INetworkInterfaceService networkInterfaceService,
        IOptions<ConnectionSettings> connectionSettingsOptions,
        IOptions<TimerSettings> timerSettingsOptions)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _networkInterfaceService = networkInterfaceService;
        _connectionSettings = connectionSettingsOptions.Value;
        _timerSettings = timerSettingsOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Operation started");

            await ExecuteOperationAsync(stoppingToken);

            _logger.LogInformation("Operation ended");

            await Task.Delay(_timerSettings.Delay, stoppingToken);
        }
    }

    private async Task ExecuteOperationAsync(CancellationToken stoppingToken)
    {
        try
        {
            var response = await SendRequestAsync(stoppingToken);

            if (response.IsSuccessStatusCode)
            {
                await _networkInterfaceService.DisableWiFiAdapter();
            }
            else
            {
                await _networkInterfaceService.EnableWiFiAdapter();
            }
        }
        catch (TaskCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
            await _networkInterfaceService.DisableWiFiAdapter();
        }
    }

    private async Task<HttpResponseMessage> SendRequestAsync(CancellationToken stoppingToken)
    {
        var ipAddress = _networkInterfaceService.GetIpAddress();

        _logger.LogInformation("Resolved IP address: {address}", ipAddress);

        using var httpClient = _httpClientFactory.Create(ipAddress);

        var response = await httpClient.GetAsync(_connectionSettings.TargetAddress, stoppingToken);

        _logger.LogInformation("Operation status code: {statusCode}", response.StatusCode);

        return response;
    }
}