using DawaAddress;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using Newtonsoft.Json;

namespace DatafordelerGeoJsonExtractor.Dawa;

internal sealed record GeoJsonFeature
{
    [JsonProperty("type")]
    public string Type { get; }

    [JsonProperty("properties")]
    public Dictionary<string, string?> Properties { get; }

    [JsonProperty("geometry", NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof(GeometryConverter))]
    public Geometry? Geometry { get; }

    public GeoJsonFeature(string type, Dictionary<string, string?> properties)
    {
        Type = type;
        Properties = properties;
    }

    [JsonConstructor]
    public GeoJsonFeature(
        string type,
        Dictionary<string, string?> properties,
        Geometry? geometry)
    {
        Type = type;
        Properties = properties;
        Geometry = geometry;
    }
}

internal sealed class DawaExtract
{
    private readonly ILogger<DawaExtract> _logger;

    public DawaExtract(ILogger<DawaExtract> logger)
    {
        _logger = logger;
    }

    public async Task StartAsync(
        Setting setting,
        CancellationToken cancellationToken = default)
    {
        var datasets = ExtractUtil.GetEnabled(setting.Dawa.Datasets);
        // If none is enabled we just return since there is nothing to process.
        if (!datasets.Any())
        {
            _logger.LogInformation(
                "No datasets enabled for Dawa, so skips extraction.");
            return;
        }

        const string accessAddressOutputName = "adgangsadresse";
        const string unitAddressOutputName = "enhedsadresse";
        const string roadOutputName = "vej";
        const string postCodeOutputName = "postnummer";

        using var httpClient = new HttpClient();
        var client = new DawaClient(httpClient);
        var tId = await client
            .GetLatestTransactionAsync(cancellationToken)
            .ConfigureAwait(false);

        if (datasets.Contains(accessAddressOutputName))
        {
            _logger.LogInformation("Starting processing {Name}", accessAddressOutputName);

            var accessAddressesEnumerable = client
                .GetAllAccessAddresses(tId.Id, cancellationToken);

            await ProcessStream<DawaAccessAddress>(
                setting.OutDirPath,
                accessAddressOutputName,
                accessAddressesEnumerable,
                MapDawa.Map).ConfigureAwait(false);
        }

        if (datasets.Contains(unitAddressOutputName))
        {
            _logger.LogInformation("Starting processing {Name}", unitAddressOutputName);

            var unitAddressEnumerable = client
                .GetAllUnitAddresses(tId.Id, cancellationToken);

            await ProcessStream<DawaUnitAddress>(
                setting.OutDirPath,
                unitAddressOutputName,
                unitAddressEnumerable,
                MapDawa.Map).ConfigureAwait(false);
        }

        if (datasets.Contains(roadOutputName))
        {
            _logger.LogInformation("Starting processing {Name}", roadOutputName);

            var roadEnumerable = client
                .GetAllRoadsAsync(tId.Id, cancellationToken);

            await ProcessStream<DawaRoad>(
                setting.OutDirPath,
                roadOutputName,
                roadEnumerable,
                MapDawa.Map).ConfigureAwait(false);
        }

        if (datasets.Contains(postCodeOutputName))
        {
            _logger.LogInformation("Starting processing {Name}", postCodeOutputName);

            var postCodeEnumerable = client
                .GetAllPostCodesAsync(tId.Id, cancellationToken);

            await ProcessStream<DawaPostCode>(
                setting.OutDirPath,
                postCodeOutputName,
                postCodeEnumerable,
                MapDawa.Map).ConfigureAwait(false);
        }
    }

    private static async Task ProcessStream<T>(
        string outputDirPath,
        string outputName,
        IAsyncEnumerable<T> stream,
        Func<T, GeoJsonFeature> mapFunc)
    {
        var path = Path.Combine(outputDirPath, $"{outputName}.geojson");

        using var lineWriter = new StreamWriter(path);
        await foreach (var line in stream.ConfigureAwait(false))
        {
            var json = JsonConvert.SerializeObject(mapFunc(line));
            await lineWriter.WriteLineAsync(json).ConfigureAwait(false);
        }
    }
}