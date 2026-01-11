using Refit;

namespace Launcher.Helpers;

public class VerifyTokenBody
{
    [AliasAs("token")] public string Token { get; set; }
}

public interface ILauncher
{
    [Headers("User-Agent: Launcher")]
    [Get("/launcher/status")]
    Task<ApiResponse<string>> GetStatus();

    [Headers("User-Agent: Launcher")]
    [Post("/launcher/token/verify")]
    Task<ApiResponse<string>> VerifyToken([Body(BodySerializationMethod.UrlEncoded)] VerifyTokenBody body);
}

public static class Api
{
    public static string Url = "http://localhost:3000/api";
    private static readonly RefitSettings Settings = new(new NewtonsoftJsonContentSerializer());
    public static ILauncher Launcher = RestService.For<ILauncher>(Url, Settings);
}