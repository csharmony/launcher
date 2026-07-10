using System.Diagnostics;
using System.Net;
using NetCoreServer;

namespace Launcher.Helpers;

class HttpSession : NetCoreServer.HttpSession
{
    public HttpSession(HttpServer server) : base(server) { }

    protected override void OnReceivedRequest(HttpRequest request)
    {
        if (request.Method != "GET" || !request.Url.StartsWith("/?game_token="))
            return;

        GameToken.Value = request.Url.Replace("/?game_token=", "");
        SendResponseAsync(Response.MakeGetResponse("Harmony Game Token acquired. You can now close this page."));
    }
}

class HttpServer : NetCoreServer.HttpServer
{
    public HttpServer(IPAddress address, int port) : base(address, port) { }

    protected override TcpSession CreateSession() { return new HttpSession(this); }

    protected override void OnStarted()
    {
        base.OnStarted();

        Process.Start(new ProcessStartInfo
        {
            FileName = Api.Url + "/launcher/auth",
            UseShellExecute = true
        });
    }
}

public static class GameToken
{
    public static string? Value;
    private static HttpServer _server = new HttpServer(IPAddress.Loopback, 47123);

    public static async Task Acquire()
    {
        if (!_server.IsStarted)
            _server.Start();

        while (string.IsNullOrEmpty(Value))
            await Task.Delay(1000);
    }
}
