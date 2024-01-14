using System.Text.Json;
using System.Text.Json.Serialization;

namespace CheckSpeedWifi.Domain;

public class SpeedTestCliResult
{
    private static readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };

    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; }

    [JsonPropertyName("download")]
    public decimal Download { get; set; }

    [JsonPropertyName("upload")]
    public decimal Upload { get; set; }

    [JsonPropertyName("ping")]
    public decimal Ping { get; set; }

    [JsonPropertyName("bytes_sent")]
    public int BytesSent { get; set; }

    [JsonPropertyName("bytes_received")]
    public int BytesReceived { get; set; }

    [JsonPropertyName("share")]
    public object Share { get; set; } = new();

    [JsonPropertyName("server")]
    public Server Server { get; set; } = new();

    [JsonPropertyName("client")]
    public Client Client { get; set; } = new();

    public override string ToString()
        => JsonSerializer.Serialize(this, jsonSerializerOptions);
}

public class Server
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty!;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty!;

    [JsonPropertyName("lat")]
    public string Lat { get; set; } = string.Empty!;

    [JsonPropertyName("lon")]
    public string Lon { get; set; } = string.Empty!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty!;

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty!;

    [JsonPropertyName("cc")]
    public string Cc { get; set; } = string.Empty!;

    [JsonPropertyName("sponsor")]
    public string Sponsor { get; set; } = string.Empty!;

    [JsonPropertyName("host")]
    public string Host { get; set; } = string.Empty!;

    [JsonPropertyName("d")]
    public decimal D { get; set; }

    [JsonPropertyName("latency")]
    public decimal Latency { get; set; }
}

public class Client
{
    [JsonPropertyName("ip")]
    public string Ip { get; set; } = string.Empty!;

    [JsonPropertyName("lat")]
    public string Lat { get; set; } = string.Empty!;

    [JsonPropertyName("lon")]
    public string Lon { get; set; } = string.Empty!;

    [JsonPropertyName("isp")]
    public string Isp { get; set; } = string.Empty!;

    [JsonPropertyName("isprating")]
    public string Isprating { get; set; } = string.Empty!;

    [JsonPropertyName("rating")]
    public string Rating { get; set; } = string.Empty!;

    [JsonPropertyName("ispdlavg")]
    public string Ispdlavg { get; set; } = string.Empty!;

    [JsonPropertyName("ispulavg")]
    public string Ispulavg { get; set; } = string.Empty!;

    [JsonPropertyName("loggedin")]
    public string Loggedin { get; set; } = string.Empty!;

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty!;
}