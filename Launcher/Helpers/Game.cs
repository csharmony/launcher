using System.Diagnostics;

namespace Launcher.Helpers;

public static class Game
{
    public static async Task Launch(string fileName)
    {
        try
        {
            var filePath = Path.Combine(Steam.GamePath, fileName);
            if (!File.Exists(filePath))
            {
                Terminal.Warning($"File doesn't exist: {fileName}");
                return;
            }

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

            Terminal.Success($"Launched Harmony");
            if (!string.IsNullOrWhiteSpace(arguments))
                Terminal.Print($"Arguments: {arguments}");

            await process.WaitForExitAsync();
            if (Debugger.IsAttached)
                Terminal.Debug($"Game closed with exit code: {process.ExitCode}");
        }
        catch (Exception e)
        {
            Terminal.Error("An error occurred while launching Harmony");
            if (Debugger.IsAttached)
                Terminal.Debug(e.InnerException?.Message ?? e.Message);
        }
    }
}