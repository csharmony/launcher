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

try
{
    var manifestResponse = await Api.Launcher.GetManifest();
    foreach (var file in manifestResponse.Files)
    {
        try
        {
            var downloadResponse = await Api.Launcher.GetDownload(file.Path);

            var directory = Path.GetDirectoryName(file.Path);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            // assuming launcher is where csgo.exe is
            // TODO: scan for CS:GO folder using registry(?)
            await using var fileStream = File.Create(file.Path);
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