using System.Text.Json;
using CheckSpeedWifi.Domain;
using Coravel.Invocable;
using ExecuteTerminal;

namespace CheckSpeedWifi.WorkerService
{
    public class Worker(ILogger<Worker> logger) : IInvocable
    {
        private readonly ILogger<Worker> _logger = logger;

        public async Task Invoke()
        {
            try
            {
                var command = "speedtest-cli --json";

                var response = await command.ExecuteCommandBashAsync();

                ValidateResponse(response);

                var result = JsonSerializer.Deserialize<SpeedTestCliResult>(response);

                _ = result ?? throw new InvalidCastException($"Error on deserialization: {result}");

                _logger.LogInformation("Download: {@Download} Upload: {@Upload} Ping: {@Ping}", result.Download / 1000000, result.Upload / 1000000, result.Ping);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Problem!");
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