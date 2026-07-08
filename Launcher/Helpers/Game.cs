using System.Diagnostics;

namespace Launcher.Helpers;

public static class Game
{
    public static async Task Launch(string fileName)
    {
        var arguments = string.Join(" ", Environment.GetCommandLineArgs().Skip(1));
        if (!string.IsNullOrWhiteSpace(arguments))
            Terminal.Print($"Arguments: {arguments}");

        var startInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(Steam.GamePath, fileName),
            Arguments = arguments
        };

        using Process process = new() { StartInfo = startInfo };
        process.Start();

        Terminal.Success($"Launched Harmony successfully!");
        if (!string.IsNullOrWhiteSpace(arguments))
            Terminal.Print($"Arguments: {arguments}");

        await process.WaitForExitAsync();
        if (Debugger.IsAttached)
            Terminal.Debug($"Game closed with exit code: {process.ExitCode}");
    }
}