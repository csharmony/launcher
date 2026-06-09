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
    gamePath = "./";
}
else
{
    Terminal.Print($"Found CS:GO (4465480): {gamePath}");
}

try
{
    var manifestResponse = await Api.Launcher.GetManifest();
    foreach (var file in manifestResponse.Files)
    {
        try
        {
            var downloadResponse = await Api.Launcher.GetDownload(file.Path);
            var fullFilePath = Path.Combine(gamePath, file.Path);
            var directoryPath = Path.GetDirectoryName(fullFilePath);

            if (!string.IsNullOrEmpty(directoryPath))
                Directory.CreateDirectory(directoryPath);

            await using var fileStream = File.Create(fullFilePath);
            await downloadResponse.CopyToAsync(fileStream);

            Terminal.Success($"Successfully downloaded: {file.Path}");
        }
        catch (Exception e)
        {
            Terminal.Error($"An error occurred while downloading: {file.Path}");
            if (Debugger.IsAttached)
                Terminal.Debug(e.InnerException?.Message ?? e.Message);
        }
    }
}
catch (Exception e)
{
    Terminal.Error("An error occurred while verifying files");
    if (Debugger.IsAttached)
        Terminal.Debug(e.InnerException?.Message ?? e.Message);
}