using System.Diagnostics;
using Launcher.Helpers;

Terminal.PrintWelcome();

if (Debugger.IsAttached)
{
    Terminal.Print("This is an info message");
    Terminal.Success("This is a success message");
    Terminal.Warning("This is a warning message");
    Terminal.Error("This is an error message");
    Terminal.Debug("This is a debug message");
}

var gamePath = Steam.GetGamePath(4465480);
if (string.IsNullOrEmpty(gamePath))
{
    Terminal.Warning("Couldn't locate CS:GO (4465480). Using current directory...");
}
else
{
    Steam.GamePath = gamePath;
    Terminal.Print($"Found CS:GO (4465480): {gamePath}");
}

await GameToken.Acquire();

try
{
    var manifestResponse = await Api.Launcher.GetManifest(GameToken.Value!);
    await Files.Validate(manifestResponse.Files);
    await Files.Download(Files.Missing);
    await Files.Download(Files.Outdated);
}
catch (Exception e)
{
    Terminal.Error("An error occurred while validating files");
    if (Debugger.IsAttached)
        Terminal.Debug(e.InnerException?.Message ?? e.Message);
}

await Game.Launch("harmony_csgo.exe");

Terminal.Print("Closing launcher in 5 seconds...");
await Task.Delay(5000);