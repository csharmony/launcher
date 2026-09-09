using System.Diagnostics;

namespace Launcher.Helpers;

public static class Game
{
    public static async Task Launch()
    {
        if (!File.Exists(Steam.GameExecutable))
        {
            Terminal.Warning($"File doesn't exist: {Steam.GameExecutable}");
            return;
        }

        if (OperatingSystem.IsLinux() && !File.Exists(Steam.LinuxRuntimeExecutable))
        {
            Terminal.Warning($"File doesn't exist: {Steam.LinuxRuntimeExecutable}");
            return;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = OperatingSystem.IsLinux() ? Steam.LinuxRuntimeExecutable : Steam.GameExecutable,
            Arguments = string.Join(" ", Arguments.All),
            WorkingDirectory = Steam.GamePath,
            // disable csgo output in linux terminal
            RedirectStandardOutput = OperatingSystem.IsLinux() && !Debug.IsEnabled,
            RedirectStandardError = OperatingSystem.IsLinux() && !Debug.IsEnabled
        };

        using Process process = new();
        process.StartInfo = startInfo;
        process.Start();

        Terminal.Success("Launched Harmony!");
        if (Arguments.Game.Count > 0)
            Terminal.Print($"Arguments: {string.Join(" ", Arguments.Game)}");

        await process.WaitForExitAsync();
        if (Debug.IsEnabled)
            Terminal.Debug($"Game closed with exit code: {process.ExitCode}");
    }
}
