using System.Text.Json;
using System.Text.Json.Serialization;
using Refit;

namespace Launcher.Helpers;

public class ManifestFile
{
    [JsonPropertyName("path")] public required string Path { get; set; }
    [JsonPropertyName("size")] public required int Size { get; set; }
    [JsonPropertyName("hash")] public required string Hash { get; set; }
}

public class ManifestResponse
{
    [JsonPropertyName("lastUpdated")] public required string LastUpdated { get; set; }
    [JsonPropertyName("files")] public required List<ManifestFile> Files { get; set; }
}

public interface ILauncher
{
    [Headers("User-Agent: Launcher")]
    [Get("/launcher/manifest")]
    Task<ManifestResponse> GetManifest();

    [Headers("User-Agent: Launcher")]
    [Get("/launcher/download")]
    Task<Stream> GetDownload([AliasAs("file_path")] string filePath);
}

public static class Api
{
    public static string Url = "http://localhost:3000/api";

    private static readonly RefitSettings Settings = new()
    {
        ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        })
    };

    public static ILauncher Launcher = RestService.For<ILauncher>(Url, Settings);
}