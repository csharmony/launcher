using System.Diagnostics;
using System.Net;

namespace Launcher.Helpers;

public static class Token
{
    public static bool GotToken;
    public static string Value = string.Empty;

    public static void Get()
    {
        var server = new ListenerServer(IPAddress.Loopback, 8888);
        server.Start();

        var serverAddress = $"http://{server.Address}:{server.Port}/set?token=";
        ProcessStartInfo process = new($"{Api.Url}/launcher/token/get?redirectUrl={serverAddress}")
        {
            UseShellExecute = true,
            Verb = "open"
        };
        Process.Start(process);

        while (!GotToken)
        {
        }

        server.Stop();
        Terminal.Success("Successfully acquired game token!");
    }

    public static void Verify()
    {
        // TO-DO: finish this method
        // if verification fails (through API), call Get()
    }

    public static bool TokenFileExists()
    {
        // TO-DO: finish this method
        // if true, call Verify() instead of Get()
        return false;
    }
}