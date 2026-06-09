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
    Terminal.Warning("Couldn't locate CS:GO (4465480). Using current dircetory...");
}
else
{
    Steam.GamePath = gamePath;
    Terminal.Print($"Found CS:GO (4465480): {gamePath}");
}

try
{
    var manifestResponse = await Api.Launcher.GetManifest();
    await FileManager.ValidateFiles(manifestResponse.Files);
    await FileManager.DownloadMissingFiles();
    await FileManager.DownloadOutdatedFiles();
}
catch (Exception e)
{
    Terminal.Error("An error occurred while validating files.");
    if (Debugger.IsAttached)
        Terminal.Debug(e.InnerException?.Message ?? e.Message);
}