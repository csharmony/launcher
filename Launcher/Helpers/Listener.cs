using System.Net;
using System.Net.Sockets;
using NetCoreServer;

namespace Launcher.Helpers;

public class ListenerSession(HttpServer server) : HttpSession(server)
{
    protected override void OnReceivedRequest(HttpRequest request)
    {
        if (request.Method != "GET")
        {
            SendResponseAsync(Response.MakeGetResponse("not a GET request"));
            return;
        }

        var path = request.Url;
        if (!path.StartsWith("/set?token="))
        {
            SendResponseAsync();
            return;
        }

        var token = Uri.UnescapeDataString(path);
        token = token.Replace("/set", "", StringComparison.InvariantCultureIgnoreCase);
        token = token.Replace("?token=", "", StringComparison.InvariantCultureIgnoreCase);

        if (token.Length != 128)
        {
            SendResponseAsync(Response.MakeGetResponse("invalid token length"));
            return;
        }

        SendResponseAsync(
            Response.MakeGetResponse("successfully acquired game token, you can now close this page"));

        Token.Value = token;
        Token.GotToken = true;
    }
}

public class ListenerServer(IPAddress address, int port) : HttpServer(address, port)
{
    protected override TcpSession CreateSession()
    {
        return new ListenerSession(this);
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"HTTP session caught an error: {error}");
    }
}