using System.Diagnostics;
using System.Net;
using NetCoreServer;

namespace Launcher.Helpers;

class HttpSession(HttpServer server) : NetCoreServer.HttpSession(server)
{
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
            SendResponseAsync(
                Response.MakeGetResponse("Harmony Game Token acquired. You can now close this page."));

            if (!string.IsNullOrWhiteSpace(GameToken.Value))
            {
                // wait until response is sent then stop the server
                await Task.Delay(1000);
                Server.Stop();
            }
        }
    }
}

class HttpServer(IPAddress address, int port) : NetCoreServer.HttpServer(address, port)
{
    protected override TcpSession CreateSession()
    {
        return new HttpSession(this);
    }

    protected override void OnStarted()
    {
        base.OnStarted();

        // disable browser output in linux terminal
        if (OperatingSystem.IsLinux())
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "xdg-open",
                Arguments = Api.Url + "/launcher/auth",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
        }
        else
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Api.Url + "/launcher/auth",
                UseShellExecute = true
            });
        }
    }
}

public static class GameToken
{
    public static string? Value;
    private static readonly HttpServer Server = new(IPAddress.Loopback, 47123);

    private static readonly string Comment =
        "# DO NOT SHARE THIS FILE TO ANYONE - This is your Harmony Game Token\n# It is used (alongside other things) for authentication with our GC\n# Tip: You can reset your Game Token on our website if you shared it on accident\n";

    private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".do-not-share");

    public static async Task Acquire()
    {
        if (File.Exists(FilePath))
        {
            var lines = await File.ReadAllLinesAsync(FilePath);
            Value = lines.FirstOrDefault(line => !line.StartsWith('#'))?.Trim();

            if (!string.IsNullOrWhiteSpace(Value))
            {
                try
                {
                    var verifyResponse = await Api.Launcher.GetVerify(Value);
                    if (!verifyResponse.IsSuccessStatusCode)
                        Value = null;
                }
                catch (Exception e)
                {
                    Terminal.Error(
                        "An error occurred while verifying your Game Token. Are you connected to the Internet?");

                    if (Debug.IsEnabled)
                        Terminal.Debug(e.InnerException?.Message ?? e.Message);
                }
            }
        }

        if (string.IsNullOrWhiteSpace(Value) && !Server.IsStarted)
        {
            Server.Start();

            while (string.IsNullOrWhiteSpace(Value) && Server.IsStarted)
                await Task.Delay(1000);
        }

        if (File.Exists(FilePath))
            File.SetAttributes(FilePath, File.GetAttributes(FilePath) & ~FileAttributes.Hidden);

        await File.WriteAllTextAsync(FilePath, Comment + Value);
        File.SetAttributes(FilePath, File.GetAttributes(FilePath) | FileAttributes.Hidden);
    }
}
