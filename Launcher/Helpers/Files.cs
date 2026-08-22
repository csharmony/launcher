using System.Security.Cryptography;
using Downloader;
using Spectre.Console;

namespace Launcher.Helpers;

public class FileCategory(string name)
{
    public string Name { get; } = name;
    public List<ManifestFile> List { get; } = [];
}

public static class Files
{
    public static readonly FileCategory Missing = new("missing");
    public static readonly FileCategory Outdated = new("outdated");

    private static readonly List<string> IgnoreWindows = [".so", ".sh", ""];
    private static readonly List<string> IgnoreLinux = [".dll", ".exe"];

    private static readonly DownloadConfiguration DownloaderConfiguration = new()
    {
        ChunkCount = 8,
        ParallelDownload = true,
        ParallelCount = 4,
        MaxTryAgainOnFailure = 5,
        EnableAutoResumeDownload = true,
        CheckDiskSizeBeforeDownload = true,
    };

    private static async Task<string> GetHashAsync(string filePath)
    {
        using var sha256 = SHA256.Create();
        await using var stream = File.OpenRead(filePath);

        byte[] hashBytes = await sha256.ComputeHashAsync(stream);

        return Convert.ToHexString(hashBytes).ToLower();
    }

    public static async Task Validate(List<ManifestFile> files)
    {
        foreach (var file in files)
        {
            var fullFilePath = Path.Combine(Steam.GamePath, file.Path);

            var fileExtension = Path.GetExtension(file.Path);
            var ignoreList = OperatingSystem.IsLinux() ? IgnoreLinux : IgnoreWindows;

            if (ignoreList.Contains(fileExtension))
                continue;

            if (File.Exists(fullFilePath))
            {
                var hash = await GetHashAsync(fullFilePath);
                if (hash == file.Hash)
                    continue;

                Outdated.List.Add(file);
                Terminal.Warning($"Found outdated file: {file.Path}");
            }
            else
            {
                Missing.List.Add(file);
                Terminal.Warning($"Found missing file: {file.Path}");
            }
        }

        if (Missing.List.Count == 0 && Outdated.List.Count == 0)
            Terminal.Success("All files are up-to-date!");
    }

    public static async Task Download(FileCategory category)
    {
        if (category.List.Count == 0)
            return;

        Terminal.Print($"Downloading {category.List.Count} {category.Name} files...");
        foreach (var file in category.List)
        {
            try
            {
                var fullFilePath = Path.Combine(Steam.GamePath, file.Path);
                var directoryPath = Path.GetDirectoryName(fullFilePath);

                if (!string.IsNullOrWhiteSpace(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                // TODO: partially move this to Terminal.cs
                await AnsiConsole.Progress()
                    .StartAsync(async ctx =>
                    {
                        var task = ctx.AddTask(file.Path, new ProgressTaskSettings
                        {
                            MaxValue = 100,
                            AutoStart = false
                        });

                        var downloader = new DownloadService(DownloaderConfiguration);

                        downloader.DownloadStarted += (_, _) => { task.StartTask(); };
                        downloader.DownloadProgressChanged += (_, e) => { task.Value = e.ProgressPercentage; };
                        downloader.DownloadFileCompleted += (_, e) =>
                        {
                            if (e.Cancelled || e.Error != null)
                                Terminal.Error($"Failed to download: {file.Path}");
                            else
                                task.Value = 100;
                            task.StopTask();
                        };

                        await downloader.DownloadFileTaskAsync(
                            Api.Url + $"/launcher/download?game_token={GameToken.Value}&file_path={file.Path}",
                            fullFilePath);
                    });

                if (File.Exists(fullFilePath))
                {
                    var hash = await GetHashAsync(fullFilePath);
                    if (hash != file.Hash)
                        Terminal.Error($"Failed to download: {file.Path}");

                    if (!OperatingSystem.IsLinux())
                        continue;

                    UnixFileMode currentMode = File.GetUnixFileMode(fullFilePath);
                    UnixFileMode newMode = currentMode | UnixFileMode.UserExecute | UnixFileMode.GroupExecute;
                    File.SetUnixFileMode(fullFilePath, newMode);
                }
            }
            catch (Exception e)
            {
                Terminal.Error($"An error occurred while downloading: {file.Path}");
                if (Debug.IsEnabled)
                    Terminal.Debug(e.InnerException?.Message ?? e.Message);
            }
        }
    }
}