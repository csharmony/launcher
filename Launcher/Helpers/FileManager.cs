using System.Diagnostics;
using System.Security.Cryptography;

namespace Launcher.Helpers;

public static class FileManager
{
    private static readonly List<ManifestFile> MissingFiles = [];
    private static readonly List<ManifestFile> OutdatedFiles = [];

    private static async Task<string> GetHashAsync(string filePath)
    {
        using var sha256 = SHA256.Create();
        await using var stream = File.OpenRead(filePath);

        byte[] hashBytes = await sha256.ComputeHashAsync(stream);

        return Convert.ToHexString(hashBytes).ToLower();
    }

    public static async Task ValidateFiles(List<ManifestFile> files)
    {
        foreach (var file in files)
        {
            var fullFilePath = Path.Combine(Steam.GamePath, file.Path);

            if (File.Exists(fullFilePath))
            {
                var hash = await GetHashAsync(fullFilePath);
                if (hash == file.Hash)
                    continue;

                OutdatedFiles.Add(file);
                Terminal.Warning($"Found outdated file: {file.Path}");
            }
            else
            {
                MissingFiles.Add(file);
                Terminal.Warning($"Found missing file: {file.Path}");
            }
        }

        if (MissingFiles.Count == 0 && OutdatedFiles.Count == 0)
            Terminal.Success("All files are up-to-date!");
    }

    public static async Task DownloadMissingFiles()
    {
        if (MissingFiles.Count == 0)
            return;

        Terminal.Print($"Downloading {MissingFiles.Count} missing files...");
        foreach (var file in MissingFiles)
        {
            try
            {
                var fullFilePath = Path.Combine(Steam.GamePath, file.Path);
                var directoryPath = Path.GetDirectoryName(fullFilePath);

                if (!string.IsNullOrEmpty(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                var downloadResponse = await Api.Launcher.GetDownload(file.Path);

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

    public static async Task DownloadOutdatedFiles()
    {
        if (OutdatedFiles.Count == 0)
            return;

        Terminal.Print($"Downloading {OutdatedFiles.Count} outdated files...");
        foreach (var file in OutdatedFiles)
        {
            try
            {
                var fullFilePath = Path.Combine(Steam.GamePath, file.Path);
                var directoryPath = Path.GetDirectoryName(fullFilePath);

                if (!string.IsNullOrEmpty(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                var downloadResponse = await Api.Launcher.GetDownload(file.Path);

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
}