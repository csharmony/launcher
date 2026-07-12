using System.Diagnostics;
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
    [Get("/launcher/verify")]
    Task<IApiResponse> GetVerify([AliasAs("game_token")] string gameToken);

    [Headers("User-Agent: Launcher")]
    [Get("/launcher/manifest")]
    Task<ManifestResponse> GetManifest([AliasAs("game_token")] string gameToken);

    [Headers("User-Agent: Launcher")]
    [Get("/launcher/download")]
    Task<Stream> GetDownload([AliasAs("game_token")] string gameToken, [AliasAs("file_path")] string filePath);
}

public static class Api
{
    public static string Url = Debugger.IsAttached ? "http://localhost:3000/api" : "https://harmony.heapy.xyz/api";
    private static readonly RefitSettings Settings = new(new NewtonsoftJsonContentSerializer());
    public static ILauncher Launcher = RestService.For<ILauncher>(Url, Settings);
}