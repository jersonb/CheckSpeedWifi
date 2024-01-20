using System.Net.NetworkInformation;
using System.Text.Json;
using CheckSpeedWifi.Data;
using CheckSpeedWifi.Domain;
using Coravel.Invocable;
using ExecuteTerminal;
using Polly;

namespace CheckSpeedWifi.WorkerService
{
    public class Worker(IDataService dataService, ILogger<Worker> logger) : IInvocable
    {
        private readonly ILogger<Worker> _logger = logger;
        private readonly IDataService _dataService = dataService;

        private readonly IAsyncPolicy _invalidDataPolicy = Policy
                .Handle<InvalidDataException>()
                .WaitAndRetryAsync(retryCount: 3, delay => TimeSpan.FromSeconds(2), (ex, delay, retryCount, _) =>
                {
                    logger.LogWarning(ex, "Retry {@Delay} {@RetryCount}", delay, retryCount);
                });

        private readonly IAsyncPolicy _jsonPolicy = Policy
            .Handle<JsonException>()
            .WaitAndRetryAsync(retryCount: 3, delay => TimeSpan.FromSeconds(2), (ex, delay, retryCount, _) =>
            {
                logger.LogWarning(ex, "Retry {@Delay} {@RetryCount}", delay, retryCount);
            });

        private readonly IAsyncPolicy _exeptionPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(retryCount: 3, delay => TimeSpan.FromSeconds(10), (ex, delay, retryCount, _) =>
            {
                logger.LogWarning(ex, "Retry {@Delay} {@RetryCount}", delay, retryCount);
            });

        public async Task Invoke()
        {
            var policy = Policy.WrapAsync(_invalidDataPolicy, _jsonPolicy, _exeptionPolicy);

            try
            {
                await policy.ExecuteAsync(Execute);
            }
            catch (Exception ex)
            {
                await _dataService.Error(ex.Message);
                _logger.LogError(ex, "Problem!");
            }
        }

        private async Task Execute()
        {
            var internetIsAvaliable = await CheckIntertIsAvaliable();

            if (!internetIsAvaliable)
            {
                var message = "Internet is not avaliable.";
                await _dataService.Error(message);
                _logger.LogCritical(message);
                return;
            }

            var command = "speedtest-cli --json";

            var response = await command.ExecuteCommandBashAsync();

            ValidateResponse(response);

            var result = JsonSerializer.Deserialize<SpeedTestCliResult>(response);

            _ = result ?? throw new InvalidCastException($"Error on deserialization: {result}");

            await _dataService.Insert(result.Download, result.Upload, result.Ping, result);
            _logger.LogInformation("Download: {@Download} Upload: {@Upload} Ping: {@Ping}", result.Download / 1000000, result.Upload / 1000000, result.Ping);
        }

        private async Task<bool> CheckIntertIsAvaliable()
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync("www.google.com");
                return reply?.Status == IPStatus.Success;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Problem to check ping");
                return false;
            }
        }

        private void ValidateResponse(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new InvalidDataException("No data, internet is Ok?");
            }

            try
            {
                JsonDocument.Parse(source);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Error: {@source}", source);
                throw;
            }
        }
    }
}