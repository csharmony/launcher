using System.Diagnostics;

namespace Launcher.Helpers;

public static class Game
{
    private static string GetArguments(string arguments)
    {
        if (OperatingSystem.IsLinux())
            return $"-- \"{Steam.GameExecutable}\" -steam --token={GameToken.Value} {arguments}";

        return $"--token={GameToken.Value} {arguments}";
    }

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

        var arguments = string.Join(" ", Environment.GetCommandLineArgs().Skip(1));
        var startInfo = new ProcessStartInfo
        {
            FileName = OperatingSystem.IsLinux() ? Steam.LinuxRuntimeExecutable : Steam.GameExecutable,
            Arguments = GetArguments(arguments),
            // disable csgo output in linux terminal
            RedirectStandardOutput = OperatingSystem.IsLinux(),
            RedirectStandardError = OperatingSystem.IsLinux()
        };

        using Process process = new() { StartInfo = startInfo };
        process.Start();

        Terminal.Success($"Launched Harmony");
        if (!string.IsNullOrWhiteSpace(arguments))
            Terminal.Print($"Arguments: {arguments}");

        await process.WaitForExitAsync();
        if (Debugger.IsAttached)
            Terminal.Debug($"Game closed with exit code: {process.ExitCode}");
    }
}