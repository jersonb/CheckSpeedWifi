using System.Net.NetworkInformation;
using System.Text.Json;
using CheckSpeedWifi.Domain;
using Coravel.Invocable;
using ExecuteTerminal;
using Polly;

namespace CheckSpeedWifi.WorkerService
{
    public class Worker(ILogger<Worker> logger) : IInvocable
    {
        private readonly ILogger<Worker> _logger = logger;

        public async Task Invoke()
        {
            var invalidDataPolicy = Policy
                .Handle<InvalidDataException>()
                .WaitAndRetryAsync(retryCount: 3, delay => TimeSpan.FromSeconds(2), (ex, delay, retryCount, _) =>
                {
                    _logger.LogWarning(ex, "Retry {@Delay} {@RetryCount}", delay, retryCount);
                });

            var jsonPolicy = Policy
                .Handle<JsonException>()
                .WaitAndRetryAsync(retryCount: 3, delay => TimeSpan.FromSeconds(2), (ex, delay, retryCount, _) =>
                {
                    _logger.LogWarning(ex, "Retry {@Delay} {@RetryCount}", delay, retryCount);
                });

            var exeptionPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(retryCount: 3, delay => TimeSpan.FromSeconds(10), (ex, delay, retryCount, _) =>
                {
                    _logger.LogWarning(ex, "Retry {@Delay} {@RetryCount}", delay, retryCount);
                });
            var policy = Policy.WrapAsync(invalidDataPolicy, jsonPolicy, exeptionPolicy);

            try
            {
                await policy.ExecuteAsync(Execute);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Problem!");
            }
        }

        private async Task Execute()
        {
            var internetIsAvaliable = await CheckIntertIsAvaliable();

            if (!internetIsAvaliable)
            {
                _logger.LogCritical("Internet is not avaliable.");
                return;
            }

            var command = "speedtest-cli --json";

            var response = await command.ExecuteCommandBashAsync();

            ValidateResponse(response);

            var result = JsonSerializer.Deserialize<SpeedTestCliResult>(response);

            _ = result ?? throw new InvalidCastException($"Error on deserialization: {result}");

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