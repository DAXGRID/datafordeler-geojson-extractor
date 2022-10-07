namespace DatafordelerGeoJsonExtractor;

internal sealed record FtpSetting
{
    public string Host { get; }
    public string Username { get; }
    public string Password { get; }

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
    public FtpSetting FtpSetting { get; }

    public Setting(FtpSetting ftpSetting)
    {
        FtpSetting = ftpSetting;
    }
}
