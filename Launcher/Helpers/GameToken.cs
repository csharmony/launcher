using System.Diagnostics;
using System.Net;
using NetCoreServer;

namespace Launcher.Helpers;

class HttpSession : NetCoreServer.HttpSession
{
    public HttpSession(HttpServer server) : base(server) { }

    protected override async void OnReceivedRequest(HttpRequest request)
    {
        if (request.Method != "GET")
            return;

        if (request.Url.StartsWith("/?game_token="))
        {
            GameToken.Value = request.Url.Replace("/?game_token=", "");

            var response = new HttpResponse();
            response.SetBegin(307);
            response.SetHeader("Location", "http://localhost:47123/success");
            response.SetBody();

            SendResponseAsync(response);
        }
        else if (request.Url == "/success")
        {
            SendResponseAsync(Response.MakeGetResponse("Harmony Game Token acquired. You can now close this page."));

            if (!string.IsNullOrWhiteSpace(GameToken.Value))
            {
                // wait until response is sent then stop the server
                await Task.Delay(1000);
                Server.Stop();
            }
        }
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
    private static string _comment = "# DO NOT SHARE THIS FILE TO ANYONE - This is your Harmony Game Token\n# It is used (alongside other things) for authentication with our GC\n# Tip: You can reset your Game Token on our website if you shared it on accident\n";
    private static string _filePath = ".do-not-share";

    public static async Task Acquire()
    {
        if (File.Exists(_filePath))
        {
            var lines = await File.ReadAllLinesAsync(_filePath);
            Value = lines.FirstOrDefault(line => !line.StartsWith('#'))?.Trim();

            if (!string.IsNullOrWhiteSpace(Value))
            {
                var verifyResponse = await Api.Launcher.GetVerify(Value);
                if (!verifyResponse.IsSuccessStatusCode)
                    Value = null;
            }
        }

        if (string.IsNullOrWhiteSpace(Value) && !_server.IsStarted)
        {
            _server.Start();

            while (string.IsNullOrWhiteSpace(Value) && _server.IsStarted)
                await Task.Delay(1000);
        }

        await File.WriteAllTextAsync(_filePath, _comment + Value);
        File.SetAttributes(_filePath, File.GetAttributes(_filePath) | FileAttributes.Hidden);
    }
}
