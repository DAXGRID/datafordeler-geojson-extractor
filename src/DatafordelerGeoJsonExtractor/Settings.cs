using System.Text.Json.Serialization;

namespace DatafordelerGeoJsonExtractor;

internal sealed record MatrikelSetting
{
    [JsonPropertyName("datasets")]
    public Dictionary<string, bool> Datasets { get; init; }

    [JsonConstructor]
    public MatrikelSetting(Dictionary<string, bool> datasets)
    {
        Datasets = datasets;
    }
}

internal sealed record GeoDanmarkSetting
{
    [JsonPropertyName("datasets")]
    public Dictionary<string, bool> Datasets { get; init; }

    [JsonConstructor]
    public GeoDanmarkSetting(Dictionary<string, bool> datasets)
    {
        Datasets = datasets;
    }
}

internal sealed record FtpSetting
{
    [JsonPropertyName("host")]
    public string Host { get; init; }

    [JsonPropertyName("username")]
    public string Username { get; init; }

    [JsonPropertyName("password")]
    public string Password { get; init; }

    [JsonConstructor]
    public FtpSetting(string host, string username, string password)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            throw new ArgumentException(
                "Cannot be null, empty or whitespace.",
                nameof(host));
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException(
                "Cannot be null, empty or whitespace.",
                nameof(username));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException(
                "Cannot be null, empty or whitespace.",
                nameof(password));
        }

        Host = host;
        Username = username;
        Password = password;
    }
}

internal sealed record Setting
{
    [JsonPropertyName("ftpSetting")]
    public FtpSetting FtpSetting { get; init; }

    [JsonPropertyName("outDirPath")]
    public string OutDirPath { get; init; }

    [JsonPropertyName("matrikel")]
    public MatrikelSetting Matrikel { get; init; }

    [JsonPropertyName("geoDanmark")]
    public GeoDanmarkSetting GeoDanmark { get; init; }

    [JsonConstructor]
    public Setting(
        FtpSetting ftpSetting,
        string outDirPath,
        MatrikelSetting matrikel,
        GeoDanmarkSetting geoDanmark)
    {
        FtpSetting = ftpSetting;
        OutDirPath = outDirPath;
        Matrikel = matrikel;
        GeoDanmark = geoDanmark;
    }
}
