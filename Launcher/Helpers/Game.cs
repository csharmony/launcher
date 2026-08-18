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
            Arguments = Arguments.All,
            // disable csgo output in linux terminal
            RedirectStandardOutput = OperatingSystem.IsLinux(),
            RedirectStandardError = OperatingSystem.IsLinux()
        };

        using Process process = new();
        process.StartInfo = startInfo;
        process.Start();

        Terminal.Success("Launched Harmony!");
        if (!string.IsNullOrWhiteSpace(Arguments.Game))
            Terminal.Print($"Arguments: {Arguments.Game}");

        await process.WaitForExitAsync();
        if (Debugger.IsAttached)
            Terminal.Debug($"Game closed with exit code: {process.ExitCode}");
    }
}